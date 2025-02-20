namespace Assembler.Core.Models
{
    public abstract class X86Instruction
    {
        public abstract string Emit();
        public override string ToString()
        {
            return Emit();
        }
    }
}
