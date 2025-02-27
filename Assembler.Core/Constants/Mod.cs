
namespace Assembler.Core.Constants;

public static class Mod
{
    public const byte MemoryModeNoDisplacement = 0b00_000_000; // No displacement, except special case of [ebp] which is always treated as needing disp32
    // A Mod value of 00 (2 bits) means that the R/M field directly specifies a base register for memory addressing
    // No displacement is included, except for the following case:
    //      - if R/M = 101 (EBP), a 32 bit displacement is required because [EBP] is ambiguous with Mod = 00
    //      - this is because R/M 101 always means absolute memory addressing [disp32]
    // Therefore, Mod = 00 -> Memory mode, no displacement
    // R/M selects the base register, except a value of 101 means absolute [disp32]
    // 
    // 
    // Example: mov eax, [ecx]
    //      8B -> Opcode (MOV r32, r/m32)
    //      01 -> ModR/M byte
    //              Mod = 00
    //              Reg = 000 (EAX destination)
    //              R/M = 001 (ECX base register)
    //     
    // Example: mov eax, [ebp] ; (special case)
    //      8B -> Opcode (MOV r32, r/m32)
    //      01 -> ModR/M byte
    //              Mod = 00
    //              Reg = 000 (EAX destination)
    //              R/M = 101 (Special case: [disp32] mode NOT [EBP])
    //     00 00 00 00 -> 32 bit displacement


    public const byte MemoryModeWith8BitDisplacement = 0b01_000_000;
    // A Mod value of 01 means that the instruction uses memory addressing with an 8 bit signed displacement [disp8]
    // The R/M field determines the base register
    // An 8 bit displacement [disp8] follows the ModR/M byte
    // The effective memory address is computed as [Base Register] + disp8
    //
    // Example: mov eax, [ebx + 4]
    //      8B -> Opcode (MOV r32, r/m32)
    //      43 -> ModR/M byte
    //              Mod = 01
    //              Reg = 000 (EAX destination)
    //              R/M = 011 (EBX base register)
    //      04 -> 8 bit displacement [disp8] of 0x04
    //
    // Example: mov edx, [esp + 16]
    //      8B -> Opcode (MOV r32, r/m32)
    //      54 -> ModR/M byte
    //              Mod = 01
    //              Reg = 010 (EAX destination)
    //              R/M = 100 (ESP triggers SIB byte!) *** See note on SIB at the bottom 
    //      24 -> SIB byte
    //              Scale = 00 (1x)
    //              Index = 100 (ignored because 100 == esp)
    //              Base = 100 (ESP)
    //      10 -> 8 bit displacement [disp8] of decimal 16

    public const byte MemoryModeWith32BitDisplacement = 0b10_000_000;
    // A Mod value of 10 (2 bits) means that a register is used as a base and a 32 bit  displacement (disp32) follows the encoding
    // The R/M field selects the base register
    // The displacement (disp32) is a signed 4 byte offset added to the base register
    // [OpCode] [ModR/M] [disp32]
    // Example: mov eax, [ebx+0x12345678]
    //      8B -> Opcode (MOV r32, r/m32)
    //      83 -> ModR/M byte
    //              Mod = 10
    //              Reg = 000 (EAX destination)
    //              R/M = 011 (EBX as base register)
    //      78 56 34 12 -> 32 bit displacement, little endian
    
    public const byte RegisterDirect = 0b11_000_000;
    // A Mod value of 11 (2 bits) means that the instruction uses register-direct mode instead of accessing memory
    // The R/M field directly specifies a register 
    // No displacement, SIB, or memory addressing is used
    // The instruction operates purely on registers
    //
    // Example: mov eax, ecx
    //      8B -> Opcode (MOV r32, r/m32)
    //      C8 -> ModR/M byte
    //              Mod = 11
    //              Reg = 001 (ECX source)        Note: the d bit in the instruction opcode determines if reg is the source or destination operand. d = 0 means reg is the source. 0b_000000_d_s
    //              R/M = 000 (EAX destination)
    //      
    // Example: xor ebx, ebx
    //      33 -> Opcode (MOV r32, r/m32)
    //      D8 -> ModR/M byte
    //              Mod = 11
    //              Reg = 011 (EAX destination) Note: the d bit in the instruction opcode determines if reg is the source or destination operand. d = 1 means reg is the destination. 0b_000000_d_s
    //              R/M = 011 (EBX source)

}


// *** Note on SIB
//  Scale-Index-Base
//      Scale (2 bits):
//          The Scale field determines how much an index register is multiplied by when calculating the effective address
//          Values:
//              00 -> multiplies by factor of 1
//              01 -> multiplies by factor of 2
//              10 -> multiplies by factor of 4
//              11 -> multiplies by factor of 8
//      Index (3 bits):
//          Specifies the register used as an index
//          Values:
//              000 -> EAX
//              001 -> ECX
//              010 -> EDX
//              011 -> EBX
//              100 -> Forbidden. Normally, an addressing mode without an index would simply use a bare ModR/M byte without a SIB byte at all, but this is necessary to encode an ESP-relative address ([ESP+disp0/8/32])
//              101 -> EBP (or EBP + disp32)
//              110 -> ESI
//              111 -> ESI
//      Base (3 bits):
//          The Base field specifies the register used as the base address in the calculation
//          Values:
//              000 -> EAX
//              001 -> ECX
//              010 -> EDX
//              011 -> EBX
//              100 -> No Base register (used with disp32)
//              101 -> EBP (or EBP + disp32)
//              110 -> ESI
//              111 -> ESI
//
// Example: mov eax, [eax + ecx * 4]
//      8B -> Opcode (MOV r32, r/m32)
//      04 -> ModR/M byte
//              Mod = 00
//              Reg = 000 (EAX destination)
//              R/M = 100 (ESP -> triggers SIB)
//     88 -> SIB Byte
//              Scale = 10 (4x multiplier)
//              Index = 001 (ECX)
//              Base = 000 (EAX)