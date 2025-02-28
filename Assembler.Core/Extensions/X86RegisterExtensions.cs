using Assembler.Core.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Core.Extensions;

public static  class X86RegisterExtensions
{

    public static byte ToOperand1(this X86Register x86Register)
    {
        if (x86Register == X86Register.eax) return 0b00_000_000;
        else if (x86Register == X86Register.ecx) return 0b00_001_000;
        else if (x86Register == X86Register.edx) return 0b00_010_000;
        else if (x86Register == X86Register.ebx) return 0b00_011_000;
        else if (x86Register == X86Register.esp) return 0b00_100_000;
        else if (x86Register == X86Register.ebp) return 0b00_101_000;
        else if (x86Register == X86Register.esi) return 0b00_110_000;
        else if (x86Register == X86Register.edi) return 0b00_111_000;
        else throw new NotImplementedException($"invalid register {x86Register}");
    }

    public static byte ToOperand2(this X86Register x86Register)
    {
        if (x86Register == X86Register.eax) return 0b00_000_000;
        else if (x86Register == X86Register.ecx) return 0b00_000_001;
        else if (x86Register == X86Register.edx) return 0b00_000_010;
        else if (x86Register == X86Register.ebx) return 0b00_000_011;
        else if (x86Register == X86Register.esp) return 0b00_000_100;
        else if (x86Register == X86Register.ebp) return 0b00_000_101;
        else if (x86Register == X86Register.esi) return 0b00_000_110;
        else if (x86Register == X86Register.edi) return 0b00_000_111;
        else throw new NotImplementedException($"invalid register {x86Register}");
    }

    public static byte ApplyOperand1(this byte b, X86Register x86Register)
    {
        b |= x86Register.ToOperand1();
        return b;
    }

    public static byte ApplyOperand2(this byte b, X86Register x86Register)
    {
        b |= x86Register.ToOperand2();
        return b;
    }


    // Some instructions use the lower 3 bits to encode the operand
    public static byte ApplyRegister(this byte b, X86Register x86Register)
    {
        b |= x86Register.ToOperand2();
        return b;
    }
}