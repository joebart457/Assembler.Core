using Assembler.Core.Constants;
using Assembler.Core.Extensions;
using Assembler.Core.Models;
using Assembler.Core.PortableExecutable;
using Assembler.Core.PortableExecutable.Models;

namespace Assembler.Core.Instructions
{
    public class IDiv_RegisterOffset : X86Instruction
    {
        public RegisterOffset Divisor { get; set; }

        public IDiv_RegisterOffset(RegisterOffset divisor)
        {
            Divisor = divisor;
        }

        public override string Emit()
        {
            return $"idiv {Divisor}";
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            byte opCode = 0xF7;
            //Here edi is 111 which is opcode extension 7
            return opCode.Encode(Divisor.EncodeAsRM(X86Register.edi));
        }

        public override uint GetVirtualSize() => 1 + (uint)Divisor.EncodeAsRM(X86Register.edi).Length;
        public override uint GetSizeOnDisk() => 1 + (uint)Divisor.EncodeAsRM(X86Register.edi).Length;
    }
}
