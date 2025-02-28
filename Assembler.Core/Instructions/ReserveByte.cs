using Assembler.Core.Models;
using Assembler.Core.PortableExecutable;
using Assembler.Core.PortableExecutable.Models;

namespace Assembler.Core.Instructions;


/// <summary>
/// Used for reserving virtual space in the .bss section
/// </summary>
public class ReserveByte : X86Instruction
{
    public int Count { get; set; }

    public ReserveByte(int count)
    {
        Count = count;
    }

    public override string Emit()
    {
        return $"rb {Count}";
    }

    public override uint GetVirtualSize() => (uint)Count;
    public override uint GetSizeOnDisk() => 0;
    public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
    {
        return [];
    }

}