using Assembler.Core.Extensions;
using Assembler.Core.PortableExecutable.Constants;
using Assembler.Core.PortableExecutable.Models;
using System.Text;

namespace Assembler.Core.PortableExecutable;

public class RelocationsSection : Section
{
    public override byte[] Name => ".reloc".GetBytes(8);
    public List<BaseRelocationBlock> Blocks { get; set; } = new();

    public override uint VirtualSize => (uint)Blocks.Sum(x => x.BlockSize) + (uint)DataInstructions.Sum(x => x.GetVirtualSize());
    public override uint Characteristics => SectionCharacteristics.ContainsInitializedData | SectionCharacteristics.MemDiscardable | SectionCharacteristics.MemRead;

    public override uint RawInstructionSize => (uint)Blocks.Sum(x => x.BlockSize);

    public override List<byte> Assemble(Dictionary<string, Address> resolvedLabels)
    {
        var result = new List<byte>();
        foreach (var block in Blocks)
        {
            result.AddRange(block.GetBytes());
        }
        return result;
    }

}