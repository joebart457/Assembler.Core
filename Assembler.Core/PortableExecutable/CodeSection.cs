using Assembler.Core.Extensions;
using Assembler.Core.Instructions;
using System.Text;

namespace Assembler.Core.PortableExecutable;

public class CodeSection: Section
{
    public override byte[] Name => ".text".GetBytes(8);
    public override UInt32 PointerToRelocations => 0;
    public override UInt32 PointerToLineNumbers => 0;
    public override UInt16 NumberOfRelocations => 0;
    public override UInt16 NumberOfLineNumbers => 0;
    public override UInt32 Characteristics => SectionCharacteristics.ContainsCode | SectionCharacteristics.MemExecute | SectionCharacteristics.MemRead;


}