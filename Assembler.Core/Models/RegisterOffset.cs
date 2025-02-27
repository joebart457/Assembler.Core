using Assembler.Core.Constants;
using Assembler.Core.Extensions;

namespace Assembler.Core.Models
{
    public class RegisterOffset : IOffset
    {
        public X86Register Register { get; set; }
        public int Offset { get; set; }
        public RegisterOffset(X86Register register, int offset)
        {
            Register = register;
            Offset = offset;
        }

        public override string ToString()
        {
            var repr = Offset == 0 ? Register.ToString() : $"{Register} {(Offset > 0 ? "+" : "-")} {Math.Abs(Offset)}";
            return $"dword [{repr}]";
        }

        public override bool Equals(object? obj)
        {
            if (obj is RegisterOffset offset)
            {
                return Offset == offset.Offset && Register == offset.Register;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Register.GetHashCode();
        }


        public byte[] EncodeAsRM(X86Register reg)
        {
            // This assumes R/M is always base
            if (Offset == 0)
            {
                if (Register == X86Register.ebp)
                {
                    return Mod.MemoryModeNoDisplacement.ApplyOperand1(reg).ApplyOperand2(X86Register.ebp).Encode(Offset.ToBytes());
                }
                else if (Register == X86Register.esp)
                {
                    return Mod.MemoryModeNoDisplacement
                        .ApplyOperand1(reg)
                        .ApplyOperand2(X86Register.esp)
                        .AddEspSIBByte()
                        .Concat(Offset.ToBytes()).ToArray();
                }
                return [Mod.MemoryModeNoDisplacement.ApplyOperand1(reg).ApplyOperand2(Register)];
            }
            else if (sbyte.MinValue <= Offset && Offset <= sbyte.MaxValue)
            {
                var disp8 = Convert.ToByte(Offset);
                if (Register == X86Register.ebp)
                {
                    return [Mod.MemoryModeWith8BitDisplacement.ApplyOperand1(reg).ApplyOperand2(X86Register.ebp), disp8];
                }
                else if (Register == X86Register.esp)
                {
                    return Mod.MemoryModeWith8BitDisplacement
                        .ApplyOperand1(reg)
                        .ApplyOperand2(X86Register.esp)
                        .AddEspSIBByte()
                        .Append(disp8).ToArray();
                }
                return [Mod.MemoryModeWith8BitDisplacement.ApplyOperand1(reg).ApplyOperand2(Register), disp8];
            }
            else
            {
                var disp32 = Offset.ToBytes();
                if (Register == X86Register.ebp)
                {
                    return Mod.MemoryModeWith32BitDisplacement.ApplyOperand1(reg).ApplyOperand2(X86Register.ebp).Encode(disp32);
                }
                else if (Register == X86Register.esp)
                {
                    return Mod.MemoryModeWith32BitDisplacement
                        .ApplyOperand1(reg)
                        .ApplyOperand2(X86Register.esp)
                        .AddEspSIBByte()
                        .Concat(disp32).ToArray();
                }
                return Mod.MemoryModeWith8BitDisplacement.ApplyOperand1(reg).ApplyOperand2(Register).Encode(disp32);
            }       
        }

        public byte[] EncodeAsRM(byte extension)
        {
            if ((extension & 0b00_000_000) == extension) return EncodeAsRM(X86Register.eax);
            if ((extension & 0b00_001_000) == extension) return EncodeAsRM(X86Register.ecx);
            if ((extension & 0b00_010_000) == extension) return EncodeAsRM(X86Register.edx);
            if ((extension & 0b00_011_000) == extension) return EncodeAsRM(X86Register.ebx);
            if ((extension & 0b00_100_000) == extension) return EncodeAsRM(X86Register.esp);
            if ((extension & 0b00_101_000) == extension) return EncodeAsRM(X86Register.ebp);
            if ((extension & 0b00_110_000) == extension) return EncodeAsRM(X86Register.esi);
            if ((extension & 0b00_111_000) == extension) return EncodeAsRM(X86Register.edi);
            throw new InvalidOperationException($"invalid ModR/M extension: {extension}");
        }

        public byte[] EncodeAsRM(X86ByteRegister reg)
        {
            // This assumes R/M is always base
            if (Offset == 0)
            {
                if (Register == X86Register.ebp)
                {
                    return Mod.MemoryModeNoDisplacement.ApplyOperand1(reg).ApplyOperand2(X86Register.ebp).Encode(Offset.ToBytes());
                }
                else if (Register == X86Register.esp)
                {
                    return Mod.MemoryModeNoDisplacement
                        .ApplyOperand1(reg)
                        .ApplyOperand2(X86Register.esp)
                        .AddEspSIBByte()
                        .Concat(Offset.ToBytes()).ToArray();
                }
                return [Mod.MemoryModeNoDisplacement.ApplyOperand1(reg).ApplyOperand2(Register)];
            }
            else if (sbyte.MinValue <= Offset && Offset <= sbyte.MaxValue)
            {
                var disp8 = Convert.ToByte(Offset);
                if (Register == X86Register.ebp)
                {
                    return [Mod.MemoryModeWith8BitDisplacement.ApplyOperand1(reg).ApplyOperand2(X86Register.ebp), disp8];
                }
                else if (Register == X86Register.esp)
                {
                    return Mod.MemoryModeWith8BitDisplacement
                        .ApplyOperand1(reg)
                        .ApplyOperand2(X86Register.esp)
                        .AddEspSIBByte()
                        .Append(disp8).ToArray();
                }
                return [Mod.MemoryModeWith8BitDisplacement.ApplyOperand1(reg).ApplyOperand2(Register), disp8];
            }
            else
            {
                var disp32 = Offset.ToBytes();
                if (Register == X86Register.ebp)
                {
                    return Mod.MemoryModeWith32BitDisplacement.ApplyOperand1(reg).ApplyOperand2(X86Register.ebp).Encode(disp32);
                }
                else if (Register == X86Register.esp)
                {
                    return Mod.MemoryModeWith32BitDisplacement
                        .ApplyOperand1(reg)
                        .ApplyOperand2(X86Register.esp)
                        .AddEspSIBByte()
                        .Concat(disp32).ToArray();
                }
                return Mod.MemoryModeWith8BitDisplacement.ApplyOperand1(reg).ApplyOperand2(Register).Encode(disp32);
            }
        }

        public byte[] EncodeAsRM(XmmRegister reg)
        {
            // This assumes R/M is always base
            if (Offset == 0)
            {
                if (Register == X86Register.ebp)
                {
                    return Mod.MemoryModeNoDisplacement.ApplyOperand1(reg).ApplyOperand2(X86Register.ebp).Encode(Offset.ToBytes());
                }
                else if (Register == X86Register.esp)
                {
                    return Mod.MemoryModeNoDisplacement
                        .ApplyOperand1(reg)
                        .ApplyOperand2(X86Register.esp)
                        .AddEspSIBByte()
                        .Concat(Offset.ToBytes()).ToArray();
                }
                return [Mod.MemoryModeNoDisplacement.ApplyOperand1(reg).ApplyOperand2(Register)];
            }
            else if (sbyte.MinValue <= Offset && Offset <= sbyte.MaxValue)
            {
                var disp8 = Convert.ToByte(Offset);
                if (Register == X86Register.ebp)
                {
                    return [Mod.MemoryModeWith8BitDisplacement.ApplyOperand1(reg).ApplyOperand2(X86Register.ebp), disp8];
                }
                else if (Register == X86Register.esp)
                {
                    return Mod.MemoryModeWith8BitDisplacement
                        .ApplyOperand1(reg)
                        .ApplyOperand2(X86Register.esp)
                        .AddEspSIBByte()
                        .Append(disp8).ToArray();
                }
                return [Mod.MemoryModeWith8BitDisplacement.ApplyOperand1(reg).ApplyOperand2(Register), disp8];
            }
            else
            {
                var disp32 = Offset.ToBytes();
                if (Register == X86Register.ebp)
                {
                    return Mod.MemoryModeWith32BitDisplacement.ApplyOperand1(reg).ApplyOperand2(X86Register.ebp).Encode(disp32);
                }
                else if (Register == X86Register.esp)
                {
                    return Mod.MemoryModeWith32BitDisplacement
                        .ApplyOperand1(reg)
                        .ApplyOperand2(X86Register.esp)
                        .AddEspSIBByte()
                        .Concat(disp32).ToArray();
                }
                return Mod.MemoryModeWith8BitDisplacement.ApplyOperand1(reg).ApplyOperand2(Register).Encode(disp32);
            }
        }
    }
}
