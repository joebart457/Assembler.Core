using Assembler.Core.Constants;
using Assembler.Core.Extensions;
using Assembler.Core.Models;
using Assembler.Core.PortableExecutable;

namespace Assembler.Core.Instructions
{
    public class Fstp_Offset : X86Instruction
    {
        public RegisterOffset Destination { get; set; }

        public Fstp_Offset(RegisterOffset destination)
        {
            Destination = destination;
        }

        public override string Emit()
        {
            return $"fstp {Destination}";
        }

        public override byte[] Assemble(Section section, Dictionary<string, Address> resolvedLabels)
        {
            byte opCode = 0xD9;
            //Here ebx is 011 which is opcode extension 3
            return opCode.Encode(Destination.EncodeAsRM(X86Register.ebx));
        }

        public override uint GetVirtualSize() => 1 + (uint)Destination.EncodeAsRM(X86Register.ebx).Length;
        public override uint GetSizeOnDisk() => 1 + (uint)Destination.EncodeAsRM(X86Register.ebx).Length;
    }  

    public class Fld_Offset : X86Instruction
    {
        public RegisterOffset Source { get; set; }

        public Fld_Offset(RegisterOffset source)
        {
            Source = source;
        }

        public override string Emit()
        {
            return $"fld {Source}";
        }

        public override byte[] Assemble(Section section, Dictionary<string, Address> resolvedLabels)
        {
            byte opCode = 0xD9;
            //Here eax is 000 which is opcode extension 0
            return opCode.Encode(Source.EncodeAsRM(X86Register.eax));
        }

        public override uint GetVirtualSize() => 1 + (uint)Source.EncodeAsRM(X86Register.eax).Length;
        public override uint GetSizeOnDisk() => 1 + (uint)Source.EncodeAsRM(X86Register.eax).Length;
    }

    public class Fild : X86Instruction
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

        public override byte[] Assemble(Section section, Dictionary<string, Address> resolvedLabels)
        {
            byte opCode = 0xDB;
            //Here eax is 000 which is opcode extension 0
            return opCode.Encode(Source.EncodeAsRM(X86Register.eax));
        }

        public override uint GetVirtualSize() => 1 + (uint)Source.EncodeAsRM(X86Register.eax).Length;
        public override uint GetSizeOnDisk() => 1 + (uint)Source.EncodeAsRM(X86Register.eax).Length;
    }

    public class Fistp : X86Instruction
    {
        public RegisterOffset Source { get; set; }

        public Fistp(RegisterOffset source)
        {
            Source = source;
        }

        public override string Emit()
        {
            return $"fistp {Source}";
        }

        public override byte[] Assemble(Section section, Dictionary<string, Address> resolvedLabels)
        {
            byte opCode = 0xDB;
            //Here ebx is 011 which is opcode extension 3
            return opCode.Encode(Source.EncodeAsRM(X86Register.ebx));
        }

        public override uint GetVirtualSize() => 1 + (uint)Source.EncodeAsRM(X86Register.ebx).Length;
        public override uint GetSizeOnDisk() => 1 + (uint)Source.EncodeAsRM(X86Register.ebx).Length;
    }

    public class FAdd : X86Instruction
    {
        public RegisterOffset Source { get; set; }

        public FAdd(RegisterOffset source)
        {
            Source = source;
        }

        public override string Emit()
        {
            return $"fadd {Source}";
        }
    }

    public class FiAdd : X86Instruction
    {
        public RegisterOffset Source { get; set; }

        public FiAdd(RegisterOffset source)
        {
            Source = source;
        }

        public override string Emit()
        {
            return $"fiadd {Source}";
        }
    }

    public class FSub : X86Instruction
    {
        public RegisterOffset Source { get; set; }

        public FSub(RegisterOffset source)
        {
            Source = source;
        }

        public override string Emit()
        {
            return $"fsub {Source}";
        }
    }

    public class FiSub : X86Instruction
    {
        public RegisterOffset Source { get; set; }

        public FiSub(RegisterOffset source)
        {
            Source = source;
        }

        public override string Emit()
        {
            return $"fisub {Source}";
        }
    }

    public class FMul : X86Instruction
    {
        public RegisterOffset Source { get; set; }

        public FMul(RegisterOffset source)
        {
            Source = source;
        }

        public override string Emit()
        {
            return $"fmul {Source}";
        }
    }

    public class FiMul : X86Instruction
    {
        public RegisterOffset Source { get; set; }

        public FiMul(RegisterOffset source)
        {
            Source = source;
        }

        public override string Emit()
        {
            return $"fimul {Source}";
        }
    }

    public class FDiv : X86Instruction
    {
        public RegisterOffset Source { get; set; }

        public FDiv(RegisterOffset source)
        {
            Source = source;
        }

        public override string Emit()
        {
            return $"fdiv {Source}";
        }
    }
    public class FiDiv : X86Instruction
    {
        public RegisterOffset Source { get; set; }

        public FiDiv(RegisterOffset source)
        {
            Source = source;
        }

        public override string Emit()
        {
            return $"fidiv {Source}";
        }
    }

    public class FAddp : X86Instruction
    {

        public override string Emit()
        {
            return $"faddp";
        }
    }

    public class FiAddp : X86Instruction
    {
        public override string Emit()
        {
            return $"fiaddp";
        }
    }

    public class FSubp : X86Instruction
    {

        public override string Emit()
        {
            return $"fsubp";
        }
    }

    public class FiSubp : X86Instruction
    {

        public override string Emit()
        {
            return $"fisubp";
        }
    }

    public class FMulp : X86Instruction
    {
        public override string Emit()
        {
            return $"fmulp";
        }
    }

    public class FiMulp : X86Instruction
    {

        public override string Emit()
        {
            return $"fimulp";
        }
    }

    public class FDivp : X86Instruction
    {

        public override string Emit()
        {
            return $"fdivp";
        }
    }
    public class FiDivp : X86Instruction
    {
        public override string Emit()
        {
            return $"fidivp";
        }
    }

    public class FComip : X86Instruction
    {
        public X87Register Operand { get; set; }
        public FComip(X87Register operand)
        {
            Operand = operand;
        }

        public override string Emit()
        {
            return $"fcomip {Operand}";
        }
    }

}
