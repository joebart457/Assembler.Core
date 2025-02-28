

using Assembler.Core.Constants;
using Assembler.Core.Extensions;
using Assembler.Core.Interfaces;
using Assembler.Core.Models;
using Assembler.Core.PortableExecutable;
using Assembler.Core.PortableExecutable.Models;

namespace Assembler.Core.Instructions
{
    public class Not_RegisterOffset : X86Instruction, IRegisterOffset_Destination
    {
        public RegisterOffset Destination { get; set; }

        public Not_RegisterOffset(RegisterOffset destination)
        {
            Destination = destination;
        }

        public override string Emit()
        {
            return $"not {Destination}";
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            byte opCode = 0xF7;
            // Here edx is 010 which is opcode extension 2

            return opCode.Encode(Destination.EncodeAsRM(X86Register.edx));
        }

        public override uint GetVirtualSize() => 1 + (uint)Destination.EncodeAsRM(X86Register.edx).Length;
        public override uint GetSizeOnDisk() => 1 + (uint)Destination.EncodeAsRM(X86Register.edx).Length;
    }

    public class Not_Register : X86Instruction, IRegister_Destination
    {
        public X86Register Destination { get; set; }

        public Not_Register(X86Register destination)
        {
            Destination = destination;
        }

        public override string Emit()
        {
            return $"not {Destination}";
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            byte opCode = 0xF7;
            // Here edx is 010 which is opcode extension 2
            var modRM = Mod.RegisterDirect.ApplyOperand1(X86Register.edx).ApplyOperand2(Destination);
            return [opCode, modRM];
        }

        public override uint GetVirtualSize() => 2;
        public override uint GetSizeOnDisk() => 2;
    }
}
