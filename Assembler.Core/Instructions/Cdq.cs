using Assembler.Core.Models;
using Assembler.Core.PortableExecutable;

namespace Assembler.Core.Instructions
{
    public class Cdq : X86Instruction
    {
        public override string Emit()
        {
            return $"cdq";
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            byte opCode = 0x99;
            return [opCode];
        }

        public override uint GetSizeOnDisk() => 1;
        public override uint GetVirtualSize() => 1;
    }
}
