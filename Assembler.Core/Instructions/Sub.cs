using Assembler.Core.Constants;
using Assembler.Core.Models;

namespace Assembler.Core.Instructions
{
    public class Sub_Register_Immediate : X86Instruction
    {
        public X86Register Destination { get; set; }
        public int ValueToSubtract { get; set; }
        public Sub_Register_Immediate(X86Register destination, int valueToSubtract)
        {
            Destination = destination;
            ValueToSubtract = valueToSubtract;
        }

        public override string Emit()
        {
            return $"sub {Destination}, {ValueToSubtract}";
        }
    }

    public class Sub_Register_Register : X86Instruction
    {
        public X86Register Destination { get; set; }
        public X86Register Source { get; set; }

        public Sub_Register_Register(X86Register destination, X86Register source)
        {
            Destination = destination;
            Source = source;
        }

        public override string Emit()
        {
            return $"sub {Destination}, {Source}";
        }
    }
}
