using Assembler.Core.Constants;
using Assembler.Core.Extensions;
using Assembler.Core.Interfaces;
using Assembler.Core.Models;
using Assembler.Core.PortableExecutable;
using Assembler.Core.PortableExecutable.Models;

namespace Assembler.Core.Instructions
{
    public class Fstp_RegisterOffset : X86Instruction, IRegisterOffset_Destination
    {
        public RegisterOffset Destination { get; set; }

        public Fstp_RegisterOffset(RegisterOffset destination)
        {
            Destination = destination;
        }

        public override string Emit()
        {
            return $"fstp {Destination}";
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            byte opCode = 0xD9;
            //Here ebx is 011 which is opcode extension 3
            return opCode.Encode(Destination.EncodeAsRM(X86Register.ebx));
        }

        public override uint GetVirtualSize() => 1 + (uint)Destination.EncodeAsRM(X86Register.ebx).Length;
        public override uint GetSizeOnDisk() => 1 + (uint)Destination.EncodeAsRM(X86Register.ebx).Length;
    }  

    public class Fld_RegisterOffset : X86Instruction, IRegisterOffset_Source
    {
        public RegisterOffset Source { get; set; }

        public Fld_RegisterOffset(RegisterOffset source)
        {
            Source = source;
        }

        public override string Emit()
        {
            return $"fld {Source}";
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            byte opCode = 0xD9;
            //Here eax is 000 which is opcode extension 0
            return opCode.Encode(Source.EncodeAsRM(X86Register.eax));
        }

        public override uint GetVirtualSize() => 1 + (uint)Source.EncodeAsRM(X86Register.eax).Length;
        public override uint GetSizeOnDisk() => 1 + (uint)Source.EncodeAsRM(X86Register.eax).Length;
    }

    public class Fild : X86Instruction, IRegisterOffset_Source
    {
        public RegisterOffset Source { get; set; }

        public Fild(RegisterOffset source)
        {
            Source = source;
        }

        public override string Emit()
        {
            return $"fild {Source}";
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            byte opCode = 0xDB;
            //Here eax is 000 which is opcode extension 0
            return opCode.Encode(Source.EncodeAsRM(X86Register.eax));
        }

        public override uint GetVirtualSize() => 1 + (uint)Source.EncodeAsRM(X86Register.eax).Length;
        public override uint GetSizeOnDisk() => 1 + (uint)Source.EncodeAsRM(X86Register.eax).Length;
    }

    public class Fistp : X86Instruction, IRegisterOffset_Destination
    {
        public RegisterOffset Destination { get; set; }

        public Fistp(RegisterOffset destination)
        {
            Destination = destination;
        }

        public override string Emit()
        {
            return $"fistp {Destination}";
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            byte opCode = 0xDB;
            //Here ebx is 011 which is opcode extension 3
            return opCode.Encode(Destination.EncodeAsRM(X86Register.ebx));
        }

        public override uint GetVirtualSize() => 1 + (uint)Destination.EncodeAsRM(X86Register.ebx).Length;
        public override uint GetSizeOnDisk() => 1 + (uint)Destination.EncodeAsRM(X86Register.ebx).Length;
    }

}
