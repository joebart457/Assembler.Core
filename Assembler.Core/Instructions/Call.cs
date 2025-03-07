﻿using Assembler.Core.Constants;
using Assembler.Core.Extensions;
using Assembler.Core.Interfaces;
using Assembler.Core.Models;
using Assembler.Core.PortableExecutable;
using Assembler.Core.PortableExecutable.Models;

namespace Assembler.Core.Instructions
{
    public class Call : X86Instruction, ICall
    {
        public string Callee { get; set; }
        public bool IsIndirect { get; set; }
        public Call(string callee, bool isIndirect)
        {
            Callee = callee;
            IsIndirect = isIndirect;
        }

        public override string Emit()
        {
            if (IsIndirect) return $"call dword [{Callee}]";
            return $"call {Callee}";
        }

        public override uint GetSizeOnDisk() => IsIndirect? (uint)6 : (uint)5;

        public override uint GetVirtualSize() => IsIndirect ? (uint)6 : (uint)5;

        public override void AddRelocationEntry(BaseRelocationBlock baseRelocationBlock, ushort currentVirtualOffsetFromSectionStart)
        {
            if (!IsIndirect) return;
            baseRelocationBlock.AddEntry(currentVirtualOffsetFromSectionStart + 2); // since indirect far calls absolute address
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            var address = GetAddressOrThrow(resolvedLabels, Callee);
            if (IsIndirect)
            {
                byte opCode = 0xFF;
                // in this case edx is 010 which is the opcode extension 2
                byte modRM = Mod.MemoryModeNoDisplacement.ApplyOperand1(X86Register.edx).ApplyOperand2(X86Register.ebp);
                List<byte> bytes = [opCode, modRM];                
                bytes.AddRange(BitConverter.GetBytes(address.VirtualAddress));
                return bytes.ToArray();
            }
            else
            {
                byte callInstruction = 0xE8;
                var offset = address.VirtualAddress - (absoluteInstructionPointer + GetVirtualSize());
                return callInstruction.Encode(offset.ToBytes());
            }
        }

    }

    public class Call_RegisterOffset : X86Instruction, IRegisterOffset_Source, ICall
    {
        public RegisterOffset Source { get; set; }
        public Call_RegisterOffset(RegisterOffset callee)
        {
            Source = callee;
        }

        public override string Emit()
        {
            return $"call {Source}";
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            byte opCode = 0xFF;
            // ECX is encoded as 0b00_010_000 which in this case is used not as a reg but as the instruction opcode extension
            return opCode.Encode(Source.EncodeAsRM(X86Register.edx));
        }

        public override uint GetSizeOnDisk() => 1 + (uint)Source.EncodeAsRM(X86Register.edx).Length;
        public override uint GetVirtualSize() => 1 + (uint)Source.EncodeAsRM(X86Register.edx).Length;
    }

    public class Call_Register : X86Instruction, IRegister_Source, ICall
    {
        public X86Register Source { get; set; }
        public Call_Register(X86Register callee)
        {
            Source = callee;
        }

        public override string Emit()
        {
            return $"call {Source}";
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            byte opCode = 0xFF;
            // ECX is encoded as 0b00_010_000 which in this case is used not as a reg but as the instruction opcode extension
            var modRM = Mod.RegisterDirect.ApplyOperand1(X86Register.edx).ApplyOperand2(Source);
            return [opCode, modRM];
        }

        public override uint GetSizeOnDisk() => 2;
        public override uint GetVirtualSize() => 2;
    }
}
