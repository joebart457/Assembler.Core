using Assembler.Core.Constants;
using Assembler.Core.Extensions;
using Assembler.Core.Interfaces;
using Assembler.Core.Models;
using Assembler.Core.PortableExecutable;
using Assembler.Core.PortableExecutable.Models;

namespace Assembler.Core.Instructions
{
    public class Push_Register : X86Instruction, IRegister_Source, IPush
    {
        public X86Register Source { get; set; }
        public Push_Register(X86Register source)
        {
            Source = source;
        }

        public override string Emit()
        {
            return $"push {Source}";
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            byte opCode = 0x50;
            return [opCode.ApplyRegister(Source)];
        }

        public override uint GetSizeOnDisk() => 1;
        public override uint GetVirtualSize() => 1;
    }

    public class Push_RegisterOffset : X86Instruction, IRegisterOffset_Source, IPush
    {
        public RegisterOffset Source { get; set; }
        public Push_RegisterOffset(RegisterOffset source)
        {
            Source = source;
        }

        public override string Emit()
        {
            return $"push {Source}";
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            byte opCode = 0xFF;
            // here esi is 110 opcode extension 6
            return opCode.Encode(Source.EncodeAsRM(X86Register.esi));
        }

        public override uint GetSizeOnDisk() => 1 + (uint)Source.EncodeAsRM(X86Register.esi).Length;
        public override uint GetVirtualSize() => 1 + (uint)Source.EncodeAsRM(X86Register.esi).Length;
    }

    public class Push_SymbolOffset : X86Instruction, ISymbolOffset_Source, IPush
    {
        public SymbolOffset Source { get; set; }
        public Push_SymbolOffset(SymbolOffset source)
        {
            Source = source;
        }

        public override string Emit()
        {
            return $"push {Source}";
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            var address = GetAddressOrThrow(resolvedLabels, Source.Symbol);
            byte opCode = 0xFF;
            // here esi is 110 opcode extension 6
            return opCode.Encode(Source.EncodeAsRM(X86Register.esi, address));
        }

        public override void AddRelocationEntry(BaseRelocationBlock baseRelocationBlock, ushort currentVirtualOffsetFromSectionStart)
        {
            baseRelocationBlock.AddEntry(currentVirtualOffsetFromSectionStart + ((ushort)GetVirtualSize() - 4)); // symbol address is placed at last 4 bytes of instruction encoding
        }

        public override uint GetSizeOnDisk() => 6;
        public override uint GetVirtualSize() => 6;
    }

    public class Push_Address : X86Instruction, IPush
    {
        public string Address { get; set; }
        public Push_Address(string address)
        {
            Address = address;
        }

        public override string Emit()
        {
            return $"push {Address}";
        }

        public override uint GetVirtualSize() => 5;
        public override uint GetSizeOnDisk() => 5;

        public override void AddRelocationEntry(BaseRelocationBlock baseRelocationBlock, ushort currentVirtualOffsetFromSectionStart)
        {
            baseRelocationBlock.AddEntry(currentVirtualOffsetFromSectionStart + 1);
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            List<byte> resultBytes = [0x68];
            var address = GetAddressOrThrow(resolvedLabels, Address);
            resultBytes.AddRange(BitConverter.GetBytes(address.VirtualAddress));
            return resultBytes.ToArray();
        }
    }

    public class Push_Immediate : X86Instruction, IPush
    {
        public int Immediate { get; set; }

        public Push_Immediate(int immediate)
        {
            Immediate = immediate;
        }

        public override string Emit()
        {
            return $"push {Immediate}";
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            byte opCode = 0x68;
            // here esi is 110 opcode extension 6
            return opCode.Encode(Immediate.ToBytes());
        }

        public override uint GetSizeOnDisk() => 5;
        public override uint GetVirtualSize() => 5;
    }
}
