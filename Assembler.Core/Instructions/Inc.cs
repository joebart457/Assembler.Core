using Assembler.Core.Constants;
using Assembler.Core.Extensions;
using Assembler.Core.Models;
using Assembler.Core.PortableExecutable;
using Assembler.Core.PortableExecutable.Models;

namespace Assembler.Core.Instructions
{
    public class Inc_Register : X86Instruction
    {
        public X86Register Destination { get; set; }

        public Inc_Register(X86Register destination)
        {
            Destination = destination;
        }

        public override string Emit()
        {
            return $"inc {Destination}";
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            byte opCode = 0x40;
            return [opCode.ApplyRegister(Destination)];
        }

        public override uint GetVirtualSize() => 1;
        public override uint GetSizeOnDisk() => 1;
    }
    public class Dec_Register : X86Instruction
    {
        public X86Register Destination { get; set; }

        public Dec_Register(X86Register destination)
        {
            Destination = destination;
        }

        public override string Emit()
        {
            return $"dec {Destination}";
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            byte opCode = 0x48;
            return [opCode.ApplyRegister(Destination)];
        }

        public override uint GetVirtualSize() => 1;
        public override uint GetSizeOnDisk() => 1;
    }

    public class Inc_RegisterOffset : X86Instruction
    {
        public RegisterOffset Destination { get; set; }

        public Inc_RegisterOffset(RegisterOffset destination)
        {
            Destination = destination;
        }

        public override string Emit()
        {
            return $"inc {Destination}";
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            byte opCode = 0xFF;
            // Here eax is 000 opcode extension
           
            return opCode.Encode(Destination.EncodeAsRM(X86Register.eax));
        }

        public override uint GetVirtualSize() => 2;
        public override uint GetSizeOnDisk() => 2;
    }

    public class Dec_RegisterOffset : X86Instruction
    {
        public RegisterOffset Destination { get; set; }

        public Dec_RegisterOffset(RegisterOffset destination)
        {
            Destination = destination;
        }

        public override string Emit()
        {
            return $"dec {Destination}";
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            byte opCode = 0xFF;
            // Here ecx is 001 opcode extension

            return opCode.Encode(Destination.EncodeAsRM(X86Register.ecx));
        }

        public override uint GetVirtualSize() => 2;
        public override uint GetSizeOnDisk() => 2;
    }
}
