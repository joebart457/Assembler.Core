using Assembler.Core.Constants;
using Assembler.Core.Models;

namespace Assembler.Core.Instructions
{
    public class IMul_Register_Register : X86Instruction
    {
        public X86Register Destination { get; set; }
        public X86Register Source { get; set; }

        public IMul_Register_Register(X86Register destination, X86Register source)
        {
            Destination = destination;
            Source = source;
        }

        public override string Emit()
        {
            return $"imul {Destination}, {Source}";
        }
    }

    public class IMul_Register_Immediate : X86Instruction
    {
        public X86Register Destination { get; set; }
        public int Immediate { get; set; }

        public IMul_Register_Immediate(X86Register destination, int immediate)
        {
            Destination = destination;
            Immediate = immediate;
        }

        public override string Emit()
        {
            return $"imul {Destination}, {Immediate}";
        }
    }
}
