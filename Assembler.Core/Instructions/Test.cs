using Assembler.Core.Constants;
using Assembler.Core.Extensions;
using Assembler.Core.Interfaces;
using Assembler.Core.Models;
using Assembler.Core.PortableExecutable;
using Assembler.Core.PortableExecutable.Models;

namespace Assembler.Core.Instructions
{
    public class Test_Register_Register : X86Instruction, INonAltering_Register_Register
    {
        public X86Register Destination { get; set; }
        public X86Register Source { get; set; }
        public Test_Register_Register(X86Register destination, X86Register source)
        {
            Destination = destination;
            Source = source;
        }

        public override string Emit()
        {
            return $"test {Destination}, {Source}";
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            byte opCode = 0x85;
            var modRM = Mod.RegisterDirect.ApplyOperand1(Source).ApplyOperand2(Destination);
            return [opCode, modRM];
        }

        public override uint GetSizeOnDisk() => 2;
        public override uint GetVirtualSize() => 2;
    }

    public class Test_Register_RegisterOffset : X86Instruction, INonAltering_Register_RegisterOffset
    {
        public X86Register Destination { get; set; }
        public RegisterOffset Source { get; set; }
        public Test_Register_RegisterOffset(X86Register operand1, RegisterOffset operand2)
        {
            Destination = operand1;
            Source = operand2;
        }

        public override string Emit()
        {
            return $"test {Destination}, {Source}";
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            byte opCode = 0x85;
            return opCode.Encode(Source.EncodeAsRM(Destination));
        }

        public override uint GetSizeOnDisk() => 2;
        public override uint GetVirtualSize() => 2;
    }
}
