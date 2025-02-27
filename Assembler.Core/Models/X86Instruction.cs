using Assembler.Core.Instructions;
using Assembler.Core.PortableExecutable;

namespace Assembler.Core.Models
{
    public abstract class X86Instruction
    {
        public abstract string Emit();
        public override string ToString()
        {
            return Emit();
        }

        public virtual uint GetVirtualSize()
        {
            throw new NotImplementedException();
        }
        public virtual uint GetSizeOnDisk()
        {
            throw new NotImplementedException();
        }
        public virtual void AddRelocationEntry(BaseRelocationBlock baseRelocationBlock, ushort currentVirtualOffsetFromSectionStart)
        {
        }

        public abstract byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels);


        protected Address GetAddressOrThrow(Dictionary<string, Address> resolvedLabels, string symbol)
        {
            if (!resolvedLabels.TryGetValue(symbol, out var address)) throw new InvalidOperationException($"unable to determine address of symbol {symbol}");
            return address;
        }
    }
}
