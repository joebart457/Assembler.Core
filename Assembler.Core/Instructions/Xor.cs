﻿using Assembler.Core.Constants;
using Assembler.Core.Extensions;
using Assembler.Core.Models;
using Assembler.Core.PortableExecutable;

namespace Assembler.Core.Instructions
{
    public class Xor_Register_Register : X86Instruction
    {
        public X86Register Destination { get; set; }
        public X86Register Source { get; set; }

        public Xor_Register_Register(X86Register destination, X86Register source)
        {
            Destination = destination;
            Source = source;
        }

        public override string Emit()
        {
            return $"xor {Destination}, {Source}";
        }

        public override byte[] Assemble(Section section, Dictionary<string, Address> resolvedLabels)
        {
            byte opCode = 0x31;
            var modRM = Mod.RegisterDirect.ApplyOperand1(Source).ApplyOperand2(Destination);
            return [opCode, modRM];
        }

        public override uint GetSizeOnDisk() => 2;
        public override uint GetVirtualSize() => 2;
    }

    public class Xor_Register_Register__Byte : X86Instruction
    {
        public X86ByteRegister Destination { get; set; }
        public X86ByteRegister Source { get; set; }

        public Xor_Register_Register__Byte(X86ByteRegister destination, X86ByteRegister source)
        {
            Destination = destination;
            Source = source;
        }

        public override string Emit()
        {
            return $"xor {Destination}, {Source}";
        }

        public override byte[] Assemble(Section section, Dictionary<string, Address> resolvedLabels)
        {
            byte opCode = 0x30;
            var modRM = Mod.RegisterDirect.ApplyOperand1(Source).ApplyOperand2(Destination);
            return [opCode, modRM];
        }

        public override uint GetSizeOnDisk() => 2;
        public override uint GetVirtualSize() => 2;
    }
}
