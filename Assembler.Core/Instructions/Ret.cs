

using Assembler.Core.Models;

namespace Assembler.Core.Instructions
{
    public class Ret : X86Instruction
    {
        public override string Emit()
        {
            return $"ret";
        }
    }

    public class Ret_Immediate : X86Instruction
    {
        public int ImmediateValue { get; set; }

        public Ret_Immediate(int immediateValue)
        {
            ImmediateValue = immediateValue;
        }

        public override string Emit()
        {
            return $"ret {ImmediateValue}";
        }
    }
}
