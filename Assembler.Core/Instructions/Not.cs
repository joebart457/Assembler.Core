﻿

using Assembler.Core.Models;

namespace Assembler.Core.Instructions
{
    public class Not_Offset : X86Instruction
    {
        public RegisterOffset Operand { get; set; }

        public Not_Offset(RegisterOffset operand)
        {
            Operand = operand;
        }

        public override string Emit()
        {
            return $"not {Operand}";
        }
    }
}
