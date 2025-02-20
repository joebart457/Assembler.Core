
using Assembler.Core.Models;

namespace Language.Experimental.Compiler.Instructions
{
    public class Neg_Offset : X86Instruction
    {
        public RegisterOffset Operand { get; set; }

        public Neg_Offset(RegisterOffset operand)
        {
            Operand = operand;
        }

        public override string Emit()
        {
            return $"neg {Operand}";
        }
    }
}
