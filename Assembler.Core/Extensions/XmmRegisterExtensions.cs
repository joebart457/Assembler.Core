using Assembler.Core.Constants;


namespace Assembler.Core.Extensions;
public static class XmmRegisterExtensions
{
    // XMM Registers
    public static byte ToOperand1(this XmmRegister xmmRegister)
    {
        if (xmmRegister == XmmRegister.xmm0) return 0b00_000_000;
        else if (xmmRegister == XmmRegister.xmm1) return 0b00_001_000;
        else if (xmmRegister == XmmRegister.xmm2) return 0b00_010_000;
        else if (xmmRegister == XmmRegister.xmm3) return 0b00_011_000;
        else if (xmmRegister == XmmRegister.xmm4) return 0b00_100_000;
        else if (xmmRegister == XmmRegister.xmm5) return 0b00_101_000;
        else if (xmmRegister == XmmRegister.xmm6) return 0b00_110_000;
        else if (xmmRegister == XmmRegister.xmm7) return 0b00_111_000;
        else throw new NotImplementedException($"invalid register {xmmRegister}");
    }

    public static byte ToOperand2(this XmmRegister xmmRegister)
    {
        if (xmmRegister == XmmRegister.xmm0) return 0b00_000_000;
        else if (xmmRegister == XmmRegister.xmm1) return 0b00_000_001;
        else if (xmmRegister == XmmRegister.xmm2) return 0b00_000_010;
        else if (xmmRegister == XmmRegister.xmm3) return 0b00_000_011;
        else if (xmmRegister == XmmRegister.xmm4) return 0b00_000_100;
        else if (xmmRegister == XmmRegister.xmm5) return 0b00_000_101;
        else if (xmmRegister == XmmRegister.xmm6) return 0b00_000_110;
        else if (xmmRegister == XmmRegister.xmm7) return 0b00_000_111;
        else throw new NotImplementedException($"invalid register {xmmRegister}");
    }

    public static byte ApplyOperand1(this byte b, XmmRegister xmmRegister)
    {
        b |= xmmRegister.ToOperand1();
        return b;
    }

    public static byte ApplyOperand2(this byte b, XmmRegister xmmRegister)
    {
        b |= xmmRegister.ToOperand2();
        return b;
    }
}