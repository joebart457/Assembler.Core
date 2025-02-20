
using Assembler.Core.Models;

namespace Language.Experimental.Compiler.Instructions
{
    public class Cdq : X86Instruction
    {
        public override string Emit()
        {
            return $"cdq";
        }
    }
}
