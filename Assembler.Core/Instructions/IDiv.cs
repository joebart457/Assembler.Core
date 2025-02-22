using Assembler.Core.Constants;
using Assembler.Core.Models;

namespace Assembler.Core.Instructions
{
    public class IDiv_Offset : X86Instruction
    {
        public RegisterOffset Divisor { get; set; }

        public IDiv_Offset(RegisterOffset divisor)
        {
            Divisor = divisor;
        }

        public override string Emit()
        {
            return $"idiv {Divisor}";
        }
    }
}
