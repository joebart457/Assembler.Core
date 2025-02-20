using Assembler.Core.Constants;
using Assembler.Core.Models;

namespace Language.Experimental.Compiler.Instructions
{
    public class Pop_Register : X86Instruction
    {
        public X86Register Destination { get; set; }

        public Pop_Register(X86Register destination)
        {
            Destination = destination;
        }

        public override string Emit()
        {
            return $"pop {Destination}";
        }
    }
}
