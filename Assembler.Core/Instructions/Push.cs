using Assembler.Core.Constants;
using Assembler.Core.Extensions;
using Assembler.Core.Models;
using Assembler.Core.PortableExecutable;
using Assembler.Core.PortableExecutable.Models;

namespace Assembler.Core.Instructions
{
    public class Push_Register : X86Instruction
    {
        public X86Register Register { get; set; }
        public Push_Register(X86Register register)
        {
            Register = register;
        }

        public override string Emit()
        {
            return $"push {Register}";
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            byte opCode = 0x50;
            return [opCode.ApplyRegister(Register)];
        }

        public override uint GetSizeOnDisk() => 1;
        public override uint GetVirtualSize() => 1;
    }

    public class Push_RegisterOffset : X86Instruction
    {
        public RegisterOffset Offset { get; set; }
        public Push_RegisterOffset(RegisterOffset offset)
        {
            Offset = offset;
        }

        public override string Emit()
        {
            return $"push {Offset}";
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            byte opCode = 0xFF;
            // here esi is 110 opcode extension 6
            return opCode.Encode(Offset.EncodeAsRM(X86Register.esi));
        }

        public override uint GetSizeOnDisk() => 1 + (uint)Offset.EncodeAsRM(X86Register.esi).Length;
        public override uint GetVirtualSize() => 1 + (uint)Offset.EncodeAsRM(X86Register.esi).Length;
    }

    public class Push_SymbolOffset : X86Instruction
    {
        public SymbolOffset Offset { get; set; }
        public Push_SymbolOffset(SymbolOffset offset)
        {
            Offset = offset;
        }

        public override string Emit()
        {
            return $"push {Offset}";
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            var address = GetAddressOrThrow(resolvedLabels, Offset.Symbol);
            byte opCode = 0xFF;
            // here esi is 110 opcode extension 6
            return opCode.Encode(Offset.EncodeAsRM(X86Register.esi, address));
        }

        public override uint GetSizeOnDisk() => 6;
        public override uint GetVirtualSize() => 6;
    }

    public class Push_Address : X86Instruction
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

        public override void AddRelocationEntry(BaseRelocationBlock baseRelocationBlock, ushort currentRVA)
        {
            baseRelocationBlock.AddEntry(currentRVA + 1);
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            List<byte> resultBytes = [0x68];
            var address = GetAddressOrThrow(resolvedLabels, Address);
            resultBytes.AddRange(BitConverter.GetBytes(address.VirtualAddress));
            return resultBytes.ToArray();
        }
    }

    public class Push_Immediate : X86Instruction
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
