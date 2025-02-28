using Assembler.Core.Constants;


namespace Assembler.Core.Extensions;

public static class X86ByteRegisterExtensions
{
    public static X86Register ToFullRegister(this X86ByteRegister byteRegister)
    {
        if (byteRegister == X86ByteRegister.al) return X86Register.eax;
        if (byteRegister == X86ByteRegister.cl) return X86Register.ecx;
        if (byteRegister == X86ByteRegister.dl) return X86Register.edx;
        if (byteRegister == X86ByteRegister.bl) return X86Register.ebx;
        if (byteRegister == X86ByteRegister.ah) return X86Register.eax;
        if (byteRegister == X86ByteRegister.ch) return X86Register.ecx;
        if (byteRegister == X86ByteRegister.dh) return X86Register.edx;
        if (byteRegister == X86ByteRegister.bh) return X86Register.ebx;
        throw new InvalidOperationException();
    }

    // Byte registers
    public static byte ToOperand1(this X86ByteRegister x86Register)
    {
        if (x86Register == X86ByteRegister.al) return 0b00_000_000;
        else if (x86Register == X86ByteRegister.cl) return 0b00_001_000;
        else if (x86Register == X86ByteRegister.dl) return 0b00_010_000;
        else if (x86Register == X86ByteRegister.bl) return 0b00_011_000;
        else if (x86Register == X86ByteRegister.ah) return 0b00_100_000;
        else if (x86Register == X86ByteRegister.ch) return 0b00_101_000;
        else if (x86Register == X86ByteRegister.bh) return 0b00_110_000;
        else if (x86Register == X86ByteRegister.dh) return 0b00_111_000;
        else throw new NotImplementedException($"invalid register {x86Register}");
    }

    public static byte ToOperand2(this X86ByteRegister x86Register)
    {
        if (x86Register == X86ByteRegister.al) return 0b00_000_000;
        else if (x86Register == X86ByteRegister.cl) return 0b00_000_001;
        else if (x86Register == X86ByteRegister.dl) return 0b00_000_010;
        else if (x86Register == X86ByteRegister.bl) return 0b00_000_011;
        else if (x86Register == X86ByteRegister.ah) return 0b00_000_100;
        else if (x86Register == X86ByteRegister.ch) return 0b00_000_101;
        else if (x86Register == X86ByteRegister.bh) return 0b00_000_110;
        else if (x86Register == X86ByteRegister.dh) return 0b00_000_111;
        else throw new NotImplementedException($"invalid register {x86Register}");
    }

    public static byte ApplyOperand1(this byte b, X86ByteRegister x86Register)
    {
        b |= x86Register.ToOperand1();
        return b;
    }

    public static byte ApplyOperand2(this byte b, X86ByteRegister x86Register)
    {
        b |= x86Register.ToOperand2();
        return b;
    }

    // Some instructions use the lower 3 bits to encode the operand
    public static byte ApplyRegister(this byte b, X86ByteRegister x86Register)
    {
        b |= x86Register.ToOperand2();
        return b;
    }
}