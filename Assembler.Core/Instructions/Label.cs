
using Assembler.Core.Models;

namespace Assembler.Core.Instructions
{
    public class Label : X86Instruction
    {
        public string Text { get; set; }

        public Label(string text)
        {
            Text = text;
        }

        public override string Emit()
        {
            return $"{Text}:";
        }
    }
}
