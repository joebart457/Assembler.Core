﻿using Assembler.Core.Extensions;
using Assembler.Core.Instructions;
using Assembler.Core.Models;
using Assembler.Core.PortableExecutable.Constants;
using Assembler.Core.PortableExecutable.Models;
using System.Text;

namespace Assembler.Core.PortableExecutable
{
    public abstract class Section
    {
        private PEFile? _peFile;
        public PEFile PEFile { get
            {
                return _peFile ?? throw new ArgumentNullException(nameof(PEFile));
            }
            set
            {
                _peFile = value;
            }
        }

        protected List<X86Instruction> DataInstructions = new();
        public virtual uint HeaderSize => 40;
        public abstract byte[] Name { get; }
        public virtual uint VirtualSize  => (uint)DataInstructions.Sum(x => x.GetVirtualSize());
        public uint RelativeVirtualAddress { get; set; }
        public virtual uint SizeOfRawData => RawInstructionSize.RoundUpToNearestMultipleOfFactor(PEFile.OptionalHeader32.FileAlignment);
        public virtual UInt32 PointerToRawData { get; set; }
        public virtual UInt32 PointerToRelocations { get; }
        public virtual UInt32 PointerToLineNumbers { get; }
        public virtual UInt16 NumberOfRelocations { get; }
        public virtual UInt16 NumberOfLineNumbers { get; }
        public virtual UInt32 Characteristics { get; }


        public virtual uint AbsoluteVirtualAddress => RelativeVirtualAddress + PEFile.OptionalHeader32.ImageBase;
        public virtual uint RawInstructionSize => (uint)DataInstructions.Sum(x => x.GetSizeOnDisk());
        public virtual uint TotalVirtualSize => VirtualSize.RoundUpToNearestMultipleOfFactor(PEFile.OptionalHeader32.SectionAlignment);
        public ImageSectionHeader ImageSectionHeader => new ImageSectionHeader()
        {
            Name = Name,
            VirtualSize = VirtualSize,
            VirtualAddress = RelativeVirtualAddress,
            SizeOfRawData = SizeOfRawData,
            PointerToRawData = PointerToRawData,
            PointerToRelocations = PointerToRelocations,
            PointerToLinenumbers = PointerToLineNumbers,
            NumberOfRelocations = NumberOfRelocations,
            NumberOfLinenumbers = NumberOfLineNumbers,
            Characteristics = Characteristics,
        };

        public ImageDataDirectory ToDataDirectory() => new(RelativeVirtualAddress, RawInstructionSize);

        public Section AddInstruction(X86Instruction instruction)
        {
            DataInstructions.Add(instruction);
            return this;
        }

        public virtual List<byte> Assemble(Dictionary<string, Address> resolvedLabels)
        {
            var result = new List<byte>();
            var absoluteInstructionPointer = AbsoluteVirtualAddress;
            foreach (var instruction in DataInstructions)
            {
                result.AddRange(instruction.Assemble(this, absoluteInstructionPointer, resolvedLabels));
                absoluteInstructionPointer += instruction.GetVirtualSize();
            }
            return result;
        }

        public virtual List<byte> AssembleAndAlign(Dictionary<string, Address> resolvedLabels)
        {
            var result = Assemble(resolvedLabels);
            result.PadToAlignment(PEFile.OptionalHeader32.FileAlignment);

            return result;
        }

        public virtual List<BaseRelocationBlock> GetRelocations(PEFile peFile)
        {
            uint currentVirtualOffsetFromSectionStart = 0;
            uint pageSize = Defaults.PageSize;
            if (peFile.OptionalHeader32.SectionAlignment != pageSize) throw new InvalidOperationException("section alignment must equal page size");
            if (pageSize >= ushort.MaxValue) throw new InvalidOperationException("page size must be less than ushort.MaxValue");

            var currentBlock = new BaseRelocationBlock()
            {
                PageRVA = RelativeVirtualAddress
            };
            var relocationBlocks = new List<BaseRelocationBlock>();
            foreach (var instruction in DataInstructions)
            {
                instruction.AddRelocationEntry(currentBlock, (ushort)currentVirtualOffsetFromSectionStart); // this cast should not truncate data since pageSize cannot be greater than ushort max

                currentVirtualOffsetFromSectionStart += (ushort)instruction.GetVirtualSize();
                if (currentVirtualOffsetFromSectionStart > peFile.OptionalHeader32.SectionAlignment)
                {
                    currentVirtualOffsetFromSectionStart -= peFile.OptionalHeader32.SectionAlignment; // I am still unsure if instructions can cross page boundaries or not. This may need to be reworked if not
                    if (currentBlock.Entries.Count != 0) relocationBlocks.Add(currentBlock);
                    currentBlock = new BaseRelocationBlock()
                    {
                        PageRVA = currentBlock.PageRVA + pageSize,
                    };
                }
            }
            if (relocationBlocks.LastOrDefault() != currentBlock && currentBlock.Entries.Count != 0) relocationBlocks.Add(currentBlock);
            return relocationBlocks;
        }

        public virtual void ExtractLabelAddresses(PEFile peFile, Dictionary<string, Address> labelsWithAddresses)
        {
            uint currentVirtualAddress = AbsoluteVirtualAddress;
            foreach (var instruction in DataInstructions)
            {
                if (instruction is Label label)
                {
                    if (labelsWithAddresses.ContainsKey(label.Text))
                        throw new InvalidOperationException($"duplicate label encountered: {label.Text}");
                    labelsWithAddresses[label.Text] = new Address { VirtualAddress = currentVirtualAddress, RelativeVirtualAddress = currentVirtualAddress - peFile.OptionalHeader32.ImageBase };
                }
                currentVirtualAddress += instruction.GetVirtualSize();
            }
        }

        public virtual string Emit()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"section {Encoding.UTF8.GetString(Name)}");
            bool encounteredLabel = false;
            foreach (var instruction in DataInstructions)
            {

                if (instruction is Label label)
                {
                    encounteredLabel = true;
                    sb.AppendLine(instruction.Emit().Indent(1));
                }
                else if (encounteredLabel) sb.AppendLine(instruction.Emit().Indent(2));
                else sb.AppendLine(instruction.Emit().Indent(1));
            }
            return sb.ToString();
        }
        
    }
}
