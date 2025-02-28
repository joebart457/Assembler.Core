using Assembler.Core.Constants;
using Assembler.Core.Extensions;
using Assembler.Core.Instructions;
using Assembler.Core.PortableExecutable;
using Assembler.Core.PortableExecutable.Models;

namespace Assembler.Core.Models;
public abstract class X86Instruction
{
    public abstract string Emit();
    public override string ToString()
    {
        return Emit();
    }

    public abstract uint GetVirtualSize();
    public abstract uint GetSizeOnDisk();
    public virtual void AddRelocationEntry(BaseRelocationBlock baseRelocationBlock, ushort currentVirtualOffsetFromSectionStart)
    {
        // A lot of instructions will not require relocations, only instructions that reference symbols
    }

    public abstract byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels);


    protected Address GetAddressOrThrow(Dictionary<string, Address> resolvedLabels, string symbol)
    {
        if (!resolvedLabels.TryGetValue(symbol, out var address)) throw new InvalidOperationException($"unable to determine address of symbol {symbol}");
        return address;
    }
}
