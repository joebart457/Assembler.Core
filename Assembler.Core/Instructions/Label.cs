﻿
using Assembler.Core.Models;
using Assembler.Core.PortableExecutable;

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

        public override uint GetSizeOnDisk() => 0;
        public override uint GetVirtualSize() => 0;

        public override byte[] Assemble(Section section, Dictionary<string, Address> resolvedLabels)
        {
            return [];
        }

    }
}
