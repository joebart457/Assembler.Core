using Assembler.Core.Constants;

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

        public IOffset ToByteOffset() => new RegisterOffset_Byte(Register, Offset);
    }
}
