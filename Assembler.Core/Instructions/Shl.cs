using Assembler.Core.Constants;
using Assembler.Core.Extensions;
using Assembler.Core.Models;
using Assembler.Core.PortableExecutable;
using Assembler.Core.PortableExecutable.Models;

namespace Assembler.Core.Instructions
{
    public class Shl_Register_Immediate : X86Instruction
    {
        public X86Register Destination { get; set; }
        public byte ImmediateValue { get; set; }

        public Shl_Register_Immediate(X86Register destination, byte immediateValue)
        {
            Destination = destination;
            ImmediateValue = immediateValue;
        }

        public override string Emit()
        {
            return $"shl {Destination}, {ImmediateValue}";
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            byte opCode = 0xC1;
            // here esp is 100 which is opcode extension 4
            var modRM = Mod.RegisterDirect.ApplyOperand1(X86Register.esp).ApplyOperand2(Destination);
            return [opCode, modRM, ImmediateValue];
        }

        public override uint GetSizeOnDisk() => 3;
        public override uint GetVirtualSize() => 3;
    }

    public class Shl_RegisterOffset_Immediate : X86Instruction
    {
        public RegisterOffset Destination { get; set; }
        public byte ImmediateValue { get; set; }

        public Shl_RegisterOffset_Immediate(RegisterOffset destination, byte immediateValue)
        {
            Destination = destination;
            ImmediateValue = immediateValue;
        }

        public override string Emit()
        {
            return $"shl {Destination}, {ImmediateValue}";
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            byte opCode = 0xC1;
            // here esp is 100 which is opcode extension 4
            return opCode.Encode(Destination.EncodeAsRM(X86Register.esp));
        }

        public override uint GetSizeOnDisk() => 1 + (uint)Destination.EncodeAsRM(X86Register.esp).Length;
        public override uint GetVirtualSize() => 1 + (uint)Destination.EncodeAsRM(X86Register.esp).Length;
    }
}
