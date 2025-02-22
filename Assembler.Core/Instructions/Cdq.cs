
using Assembler.Core.Models;

namespace Assembler.Core.Instructions
{
    public class Cdq : X86Instruction
    {
        public override string Emit()
        {
            return $"cdq";
        }
    }
}
