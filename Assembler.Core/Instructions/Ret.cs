

using Assembler.Core.Extensions;
using Assembler.Core.Interfaces;
using Assembler.Core.Models;
using Assembler.Core.PortableExecutable;
using Assembler.Core.PortableExecutable.Models;

namespace Assembler.Core.Instructions
{
    public class Ret : X86Instruction, IRet
    {
        public override string Emit()
        {
            return $"ret";
        }

        public override uint GetSizeOnDisk() => 1;
        public override uint GetVirtualSize() => 1;

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            return [0xC3];
        }
    }

    public class Ret_Immediate : X86Instruction, IRet
    {
        public ushort ImmediateValue { get; set; }

        public Ret_Immediate(ushort immediateValue)
        {
            ImmediateValue = immediateValue;
        }

        public override string Emit()
        {
            return $"ret {ImmediateValue}";
        }

        public override uint GetSizeOnDisk() => 3;
        public override uint GetVirtualSize() => 3;

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            byte opCode = 0xC2;
            return opCode.Encode(ImmediateValue.ToBytes());
        }
    }
}
