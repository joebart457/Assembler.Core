using Assembler.Core.Constants;
using Assembler.Core.Models;

namespace Assembler.Core.Instructions;
public static class X86Instructions
{
    public static Cdq Cdq() => new Cdq();

    public static Push_Register Push(X86Register register) => new Push_Register(register);
    public static Push_RegisterOffset Push(RegisterOffset offset) => new Push_RegisterOffset(offset);
    public static Push_Address Push(string address) => new Push_Address(address);
    public static Push_Immediate Push(int immediateValue) => new Push_Immediate(immediateValue);
    public static Push_SymbolOffset Push(SymbolOffset offset) => new Push_SymbolOffset(offset);

    public static X86Instruction Lea(X86Register destination, IOffset source)
    {
        if (source is RegisterOffset registerOffset) return Lea(destination, registerOffset);
        else if (source is SymbolOffset symbolOffset) return Lea(destination, symbolOffset);
        else throw new InvalidOperationException();
    }
    public static Lea_Register_RegisterOffset Lea(X86Register destination, RegisterOffset source) => new Lea_Register_RegisterOffset(destination, source);
    public static Lea_Register_SymbolOffset Lea(X86Register destination, SymbolOffset source) => new Lea_Register_SymbolOffset(destination, source);

    public static Mov_Register_RegisterOffset Mov(X86Register destination, RegisterOffset source) => new Mov_Register_RegisterOffset(destination, source);
    public static Mov_RegisterOffset_Register Mov(RegisterOffset destination, X86Register source) => new Mov_RegisterOffset_Register(destination, source);
    public static Mov_RegisterOffset_Immediate Mov(RegisterOffset destination, int immediate) => new Mov_RegisterOffset_Immediate(destination, immediate);
    public static Mov_Register_Register Mov(X86Register destination, X86Register source) => new Mov_Register_Register(destination, source);
    public static Mov_Register_Immediate Mov(X86Register destination, int immediate) => new Mov_Register_Immediate(destination, immediate);

    public static Mov_SymbolOffset_Register Mov(SymbolOffset destination, X86Register source) => new Mov_SymbolOffset_Register(destination, source);
    public static Mov_SymbolOffset_Immediate Mov(SymbolOffset destination, int immediateValue) => new Mov_SymbolOffset_Immediate(destination, immediateValue);

    public static Mov_SymbolOffset_ByteRegister Mov(SymbolOffset destination, X86ByteRegister source) => new Mov_SymbolOffset_ByteRegister(destination, source);
    public static Mov_RegisterOffset_ByteRegister Mov(RegisterOffset destination, X86ByteRegister source) => new Mov_RegisterOffset_ByteRegister(destination, source);


    public static Movsx_Register_RegisterOffset_Byte Movsx(X86Register destination, RegisterOffset source) => new Movsx_Register_RegisterOffset_Byte(destination, source);
    public static Movsx_Register_SymbolOffset_Byte Movsx(X86Register destination, SymbolOffset source) => new Movsx_Register_SymbolOffset_Byte(destination, source);

    public static Sub_Register_Immediate Sub(X86Register destination, int valueToSubtract) => new Sub_Register_Immediate(destination, valueToSubtract);
    public static Sub_Register_Register Sub(X86Register destination, X86Register source) => new Sub_Register_Register(destination, source);

    public static Add_Register_Immediate Add(X86Register destination, int value) => new Add_Register_Immediate(destination, value);
    public static Add_Register_Register Add(X86Register destination, X86Register source) => new Add_Register_Register(destination, source);


    public static And_Register_Register And(X86Register destination, X86Register source) => new And_Register_Register(destination, source);
    public static Or_Register_Register Or(X86Register destination, X86Register source) => new Or_Register_Register(destination, source);
    public static Xor_Register_Register Xor(X86Register destination, X86Register source) => new Xor_Register_Register(destination, source);


    public static Pop_Register Pop(X86Register destination) => new Pop_Register(destination);

    public static Neg_RegisterOffset Neg(RegisterOffset destination) => new Neg_RegisterOffset(destination);
    public static Not_RegisterOffset Not(RegisterOffset destination) => new Not_RegisterOffset(destination);
    public static Neg_Register Neg(X86Register destination) => new Neg_Register(destination);
    public static Not_Register Not(X86Register destination) => new Not_Register(destination);

    public static Inc_Register Inc(X86Register destination) => new Inc_Register(destination);
    public static Dec_Register Dec(X86Register destination) => new Dec_Register(destination);
    public static Inc_RegisterOffset Inc(RegisterOffset destination) => new Inc_RegisterOffset(destination);
    public static Dec_RegisterOffset Dec(RegisterOffset destination) => new Dec_RegisterOffset(destination);

    public static IDiv_RegisterOffset IDiv(RegisterOffset divisor) => new IDiv_RegisterOffset(divisor);
    public static IMul_Register_Register IMul(X86Register destination, X86Register source) => new IMul_Register_Register(destination, source);
    public static IMul_Register_Immediate IMul(X86Register destination, int immediate) => new IMul_Register_Immediate(destination, immediate);
    public static Add_Register_RegisterOffset Add(X86Register destination, RegisterOffset source) => new Add_Register_RegisterOffset(destination, source);


    public static Jmp Jmp(string label) => new Jmp(label);
    public static JmpGt JmpGt(string label) => new JmpGt(label);
    public static JmpGte JmpGte(string label) => new JmpGte(label);
    public static JmpLt JmpLt(string label) => new JmpLt(label);
    public static JmpLte JmpLte(string label) => new JmpLte(label);
    public static JmpEq JmpEq(string label) => new JmpEq(label);
    public static JmpNeq JmpNeq(string label) => new JmpNeq(label);
    public static Jz Jz(string label) => new Jz(label);
    public static Jnz Jnz(string label) => new Jnz(label);
    public static Js Js(string label) => new Js(label);
    public static Jns Jns(string label) => new Jns(label);
    public static Ja Ja(string label) => new Ja(label);
    public static Jae Jae(string label) => new Jae(label);
    public static Jb Jb(string label) => new Jb(label);
    public static Jbe Jbe(string label) => new Jbe(label);

    public static Test_Register_Register Test(X86Register operand1, X86Register operand2) => new Test_Register_Register(operand1, operand2);
    public static Test_Register_RegisterOffset Test(X86Register operand1, RegisterOffset operand2) => new Test_Register_RegisterOffset(operand1, operand2);
    public static Cmp_Register_Register Cmp(X86Register operand1, X86Register operand2) => new Cmp_Register_Register(operand1, operand2);
    public static Cmp_Register_Immediate Cmp(X86Register operand1, int operand2) => new Cmp_Register_Immediate(operand1, operand2);
    public static Cmp_Byte_Byte Cmp(X86ByteRegister operand1, X86ByteRegister operand2) => new Cmp_Byte_Byte(operand1, operand2);

    public static Call Call(string callee, bool isIndirect) => new Call(callee, isIndirect);
    public static Call_RegisterOffset Call(RegisterOffset offset) => new Call_RegisterOffset(offset);
    public static Call_Register Call(X86Register register) => new Call_Register(register);
    public static Label Label(string text) => new Label(text);
    public static Ret Ret() => new Ret();
    public static Ret_Immediate Ret(ushort immediate) => new Ret_Immediate(immediate);

    public static Fstp_RegisterOffset Fstp(RegisterOffset destination) => new Fstp_RegisterOffset(destination);
    public static Fld_RegisterOffset Fld(RegisterOffset source) => new Fld_RegisterOffset(source);

    public static Movss_RegisterOffset_Register Movss(RegisterOffset destination, XmmRegister source) => new Movss_RegisterOffset_Register(destination, source);
    public static Movss_Register_RegisterOffset Movss(XmmRegister destination, RegisterOffset source) => new Movss_Register_RegisterOffset(destination, source);
    public static Movss_Register_Register Movss(XmmRegister destination, XmmRegister source) => new Movss_Register_Register(destination, source);
    public static Comiss_Register_RegisterOffset Comiss(XmmRegister destination, RegisterOffset source) => new Comiss_Register_RegisterOffset(destination, source);
    public static Comiss_Register_Register Comiss(XmmRegister destination, XmmRegister source) => new Comiss_Register_Register(destination, source);
    public static Ucomiss_Register_Register Ucomiss(XmmRegister destination, XmmRegister source) => new Ucomiss_Register_Register(destination, source);
    public static Addss_Register_RegisterOffset Addss(XmmRegister destination, RegisterOffset source) => new Addss_Register_RegisterOffset(destination, source);
    public static Subss_Register_RegisterOffset Subss(XmmRegister destination, RegisterOffset source) => new Subss_Register_RegisterOffset(destination, source);
    public static Mulss_Register_RegisterOffset Mulss(XmmRegister destination, RegisterOffset source) => new Mulss_Register_RegisterOffset(destination, source);
    public static Divss_Register_RegisterOffset Divss(XmmRegister destination, RegisterOffset source) => new Divss_Register_RegisterOffset(destination, source);
    public static Cvtsi2ss_Register_RegisterOffset Cvtsi2ss(XmmRegister destination, RegisterOffset source) => new Cvtsi2ss_Register_RegisterOffset(destination, source);
    public static Cvtss2si_Register_RegisterOffset Cvtss2si(X86Register destination, RegisterOffset source) => new Cvtss2si_Register_RegisterOffset(destination, source);
}