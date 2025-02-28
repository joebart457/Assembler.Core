using Assembler.Core.Extensions;
using Assembler.Core.PortableExecutable.Constants;
using System.Text;

namespace Assembler.Core.PortableExecutable;


public class ImportsSection: Section
{
    public override byte[] Name => ".idata".GetBytes(8);
    public override UInt32 PointerToRelocations => 0;
    public override UInt32 PointerToLineNumbers => 0;
    public override UInt16 NumberOfRelocations => 0;
    public override UInt16 NumberOfLineNumbers => 0;
    public override UInt32 Characteristics => SectionCharacteristics.ContainsInitializedData | SectionCharacteristics.MemRead | SectionCharacteristics.MemWrite;

}