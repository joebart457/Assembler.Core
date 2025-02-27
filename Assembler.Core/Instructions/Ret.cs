

using Assembler.Core.Extensions;
using Assembler.Core.Models;
using Assembler.Core.PortableExecutable;

namespace Assembler.Core.Instructions
{
    public class Ret : X86Instruction
    {
        public override string Emit()
        {
            return $"ret";
        }

        public override uint GetSizeOnDisk() => 1;
        public override uint GetVirtualSize() => 1;

        public override byte[] Assemble(Section section, Dictionary<string, Address> resolvedLabels)
        {
            return [0xC3];
        }
    }

    public class Ret_Immediate : X86Instruction
    {
        public int ImmediateValue { get; set; }

        public Ret_Immediate(int immediateValue)
        {
            ImmediateValue = immediateValue;
        }

        public override string Emit()
        {
            return $"ret {ImmediateValue}";
        }

        public override uint GetSizeOnDisk() => 1;
        public override uint GetVirtualSize() => 1;

        public override byte[] Assemble(Section section, Dictionary<string, Address> resolvedLabels)
        {
            byte opCode = 0xC2;
            return opCode.Encode(ImmediateValue.ToBytes());
        }
    }
}
