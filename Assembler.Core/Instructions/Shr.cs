using Assembler.Core.Constants;
using Assembler.Core.Extensions;
using Assembler.Core.Interfaces;
using Assembler.Core.Models;
using Assembler.Core.PortableExecutable;
using Assembler.Core.PortableExecutable.Models;


namespace Assembler.Core.Instructions
{
    public class Shr_Register_Immediate : X86Instruction, IRegister_ByteImmediate
    {
        public X86Register Destination { get; set; }
        public byte ImmediateValue { get; set; }

        public Shr_Register_Immediate(X86Register destination, byte immediateValue)
        {
            Destination = destination;
            ImmediateValue = immediateValue;
        }

        public override string Emit()
        {
            return $"shr {Destination}, {ImmediateValue}";
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            byte opCode = 0xC1;
            // here ebp is 101 which is opcode extension 5
            var modRM = Mod.RegisterDirect.ApplyOperand1(X86Register.ebp).ApplyOperand2(Destination);
            return [opCode, modRM, ImmediateValue];
        }

        public override uint GetSizeOnDisk() => 3;
        public override uint GetVirtualSize() => 3;
    }

    public class Shr_RegisterOffset_Immediate : X86Instruction, IRegisterOffset_ByteImmediate
    {
        public RegisterOffset Destination { get; set; }
        public byte ImmediateValue { get; set; }

        public Shr_RegisterOffset_Immediate(RegisterOffset destination, byte immediateValue)
        {
            Destination = destination;
            ImmediateValue = immediateValue;
        }

        public override string Emit()
        {
            return $"shr {Destination}, {ImmediateValue}";
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            byte opCode = 0xC1;
            // here ebp is 101 which is opcode extension 5
            return opCode.Encode(Destination.EncodeAsRM(X86Register.ebp));
        }

        public override uint GetSizeOnDisk() => 1 + (uint)Destination.EncodeAsRM(X86Register.ebp).Length;
        public override uint GetVirtualSize() => 1 + (uint)Destination.EncodeAsRM(X86Register.ebp).Length;
    }
}
