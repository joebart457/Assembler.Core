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
            return $"[{repr}]";
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
                var disp8 = (byte)Convert.ToSByte(Offset);
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
                var disp8 = (byte)Convert.ToSByte(Offset);
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
                var disp8 = (byte)Convert.ToSByte(Offset);
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
