
using Assembler.Core.Constants;
using Assembler.Core.Extensions;
using Assembler.Core.Models;
using Assembler.Core.PortableExecutable;

namespace Assembler.Core.Instructions
{
    public class Neg_Offset : X86Instruction
    {
        public RegisterOffset Operand { get; set; }

        public Neg_Offset(RegisterOffset operand)
        {
            Operand = operand;
        }

        public override string Emit()
        {
            return $"neg {Operand}";
        }

        public override byte[] Assemble(Section section, Dictionary<string, Address> resolvedLabels)
        {
            byte opCode = 0xF7;
            // Here ebx is 011 which is opcode extension 3
            
            return opCode.Encode(Operand.EncodeAsRM(X86Register.ebx));
        }

        public override uint GetVirtualSize() => 1 + (uint)Operand.EncodeAsRM(X86Register.ebx).Length;
        public override uint GetSizeOnDisk() => 1 + (uint)Operand.EncodeAsRM(X86Register.ebx).Length;
    }

    public class Neg_Register : X86Instruction
    {
        public X86Register Operand { get; set; }

        public Neg_Register(X86Register operand)
        {
            Operand = operand;
        }

        public override string Emit()
        {
            return $"neg {Operand}";
        }

        public override byte[] Assemble(Section section, Dictionary<string, Address> resolvedLabels)
        {
            byte opCode = 0xF7;
            // Here ebx is 011 which is opcode extension 3
            var modRM = Mod.RegisterDirect.ApplyOperand1(X86Register.ebx).ApplyOperand2(Operand);
            return [opCode, modRM];
        }

        public override uint GetVirtualSize() => 2;
        public override uint GetSizeOnDisk() => 2;
    }
}
