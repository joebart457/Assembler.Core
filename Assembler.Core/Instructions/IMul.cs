﻿using Assembler.Core.Constants;
using Assembler.Core.Extensions;
using Assembler.Core.Models;
using Assembler.Core.PortableExecutable;

namespace Assembler.Core.Instructions
{
    public class IMul_Register_Register : X86Instruction
    {
        public X86Register Destination { get; set; }
        public X86Register Source { get; set; }

        public IMul_Register_Register(X86Register destination, X86Register source)
        {
            Destination = destination;
            Source = source;
        }

        public override string Emit()
        {
            return $"imul {Destination}, {Source}";
        }


        public override byte[] Assemble(Section section, Dictionary<string, Address> resolvedLabels)
        {
            var opCode = new List<byte>() { 0x0F, 0xAF };
            var modRM = Mod.RegisterDirect.ApplyOperand1(Source).ApplyOperand2(Destination);
            return [0x0F, 0xAF, modRM];
        }

        public override uint GetVirtualSize() => 3;
        public override uint GetSizeOnDisk() => 3;
    }

    public class IMul_Register_Immediate : X86Instruction
    {
        public X86Register Destination { get; set; }
        public int Immediate { get; set; }

        public IMul_Register_Immediate(X86Register destination, int immediate)
        {
            Destination = destination;
            Immediate = immediate;
        }

        public override string Emit()
        {
            return $"imul {Destination}, {Immediate}";
        }

        public override byte[] Assemble(Section section, Dictionary<string, Address> resolvedLabels)
        {
            byte opCode = 0x69;
            var modRM = Mod.RegisterDirect.ApplyOperand1(Destination).ApplyOperand2(Destination);
            return new List<byte>() { opCode, modRM }.Concat(Immediate.ToBytes()).ToArray();
        }

        public override uint GetVirtualSize() => 3;
        public override uint GetSizeOnDisk() => 3;
    }
}
