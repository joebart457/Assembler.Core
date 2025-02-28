using Assembler.Core.Constants;
using Assembler.Core.Extensions;
using Assembler.Core.Interfaces;
using Assembler.Core.Models;
using Assembler.Core.PortableExecutable;
using Assembler.Core.PortableExecutable.Models;

namespace Assembler.Core.Instructions
{
    public class Add_Register_Immediate : X86Instruction, IRegister_Immediate
    {
        public X86Register Destination { get; set; }
        public int ImmediateValue { get; set; }

        public Add_Register_Immediate(X86Register destination, int immediateValue)
        {
            Destination = destination;
            ImmediateValue = immediateValue;
        }

        public override string Emit()
        {
            return $"add {Destination}, {ImmediateValue}";
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            byte opCode = 0x81;
            var modRM = Mod.RegisterDirect.ApplyOperand2(Destination);
            return new byte[] { opCode, modRM }.Concat(ImmediateValue.ToBytes()).ToArray();
        }

        public override uint GetSizeOnDisk() => 6;
        public override uint GetVirtualSize() => 6;
    }

    public class Add_Register_Register: X86Instruction, IRegister_Register
    {
        public X86Register Destination { get; set; }
        public X86Register Source { get; set; }

        public Add_Register_Register(X86Register destination, X86Register source)
        {
            Destination = destination;
            Source = source;
        }

        public override string Emit()
        {
            return $"add {Destination}, {Source}";
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            byte opCode = 0x01;
            var modRM = Mod.RegisterDirect.ApplyOperand1(Source).ApplyOperand2(Destination);
            return [opCode, modRM];
        }

        public override uint GetSizeOnDisk() => 2;
        public override uint GetVirtualSize() => 2;
    }

    public class Add_Register_RegisterOffset : X86Instruction, IRegister_RegisterOffset
    {
        public X86Register Destination { get; set; }
        public RegisterOffset Source { get; set; }

        public Add_Register_RegisterOffset(X86Register destination, RegisterOffset source)
        {
            Destination = destination;
            Source = source;
        }

        public override string Emit()
        {
            return $"add {Destination}, {Source}";
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            byte opCode = 0x03;
            return opCode.Encode(Source.EncodeAsRM(Destination));
        }

        public override uint GetSizeOnDisk() => 1 + (uint)Source.EncodeAsRM(Destination).Length;
        public override uint GetVirtualSize() => 1 + (uint)Source.EncodeAsRM(Destination).Length;
    }
}
