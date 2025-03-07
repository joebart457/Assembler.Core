﻿using Assembler.Core.Constants;
using Assembler.Core.Extensions;
using Assembler.Core.Interfaces;
using Assembler.Core.Models;
using Assembler.Core.PortableExecutable;
using Assembler.Core.PortableExecutable.Models;

namespace Assembler.Core.Instructions
{
    public class Sub_Register_Immediate : X86Instruction, IRegister_Immediate
    {
        public X86Register Destination { get; set; }
        public int ImmediateValue { get; set; }
        public Sub_Register_Immediate(X86Register destination, int valueToSubtract)
        {
            Destination = destination;
            ImmediateValue = valueToSubtract;
        }

        public override string Emit()
        {
            return $"sub {Destination}, {ImmediateValue}";
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            byte opCode = 0x81;
            // here ebp is 101 which is opcode extension 7
            var modRM = Mod.RegisterDirect.ApplyOperand1(X86Register.ebp).ApplyOperand2(Destination);
            return new byte[] { opCode, modRM }.Concat(ImmediateValue.ToBytes()).ToArray();
        }

        public override uint GetSizeOnDisk() => 6;
        public override uint GetVirtualSize() => 6;
    }

    public class Sub_Register_Register : X86Instruction, IRegister_Register
    {
        public X86Register Destination { get; set; }
        public X86Register Source { get; set; }

        public Sub_Register_Register(X86Register destination, X86Register source)
        {
            Destination = destination;
            Source = source;
        }

        public override string Emit()
        {
            return $"sub {Destination}, {Source}";
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            byte opCode = 0x29;
            var modRM = Mod.RegisterDirect.ApplyOperand1(Source).ApplyOperand2(Destination);
            return [opCode, modRM];
        }

        public override uint GetSizeOnDisk() => 2;
        public override uint GetVirtualSize() => 2;
    }
}
