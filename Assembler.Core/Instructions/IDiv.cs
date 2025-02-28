using Assembler.Core.Constants;
using Assembler.Core.Extensions;
using Assembler.Core.Interfaces;
using Assembler.Core.Models;
using Assembler.Core.PortableExecutable;
using Assembler.Core.PortableExecutable.Models;

namespace Assembler.Core.Instructions
{
    public class IDiv_RegisterOffset : X86Instruction, IRegisterOffset_Source
    {
        public RegisterOffset Source { get; set; }

        public IDiv_RegisterOffset(RegisterOffset divisor)
        {
            Source = divisor;
        }

        public override string Emit()
        {
            return $"idiv {Source}";
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            byte opCode = 0xF7;
            //Here edi is 111 which is opcode extension 7
            return opCode.Encode(Source.EncodeAsRM(X86Register.edi));
        }

        public override uint GetVirtualSize() => 1 + (uint)Source.EncodeAsRM(X86Register.edi).Length;
        public override uint GetSizeOnDisk() => 1 + (uint)Source.EncodeAsRM(X86Register.edi).Length;
    }
}
