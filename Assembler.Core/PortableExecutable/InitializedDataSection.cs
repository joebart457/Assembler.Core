using Assembler.Core.Extensions;
using Assembler.Core.Instructions;
using Assembler.Core.Models;

namespace Assembler.Core.PortableExecutable
{
    public class InitializedDataSection: Section
    {
        public override byte[] Name => ".data".GetBytes(8);
        public override uint VirtualSize => (uint)DataInstructions.Sum(x => x.GetVirtualSize());
        public override UInt32 PointerToRelocations => 0;
        public override UInt32 PointerToLineNumbers => 0;
        public override UInt16 NumberOfRelocations => 0;
        public override UInt16 NumberOfLineNumbers => 0;
        public override UInt32 Characteristics => SectionCharacteristics.ContainsInitializedData | SectionCharacteristics.MemRead | SectionCharacteristics.MemWrite;

    }
}
