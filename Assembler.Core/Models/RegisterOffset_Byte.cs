using Assembler.Core.Constants;

namespace Assembler.Core.Models
{
    public class RegisterOffset_Byte : IOffset
    {
        public X86Register Register { get; set; }
        public int Offset { get; set; }
        public RegisterOffset_Byte(X86Register register, int offset)
        {
            Register = register;
            Offset = offset;
        }


        public override string ToString()
        {
            var repr = Offset == 0 ? Register.ToString() : $"{Register} {(Offset > 0 ? "+" : "-")} {Math.Abs(Offset)}";
            return $"byte [{repr}]";
        }

        public override bool Equals(object? obj)
        {
            if (obj is RegisterOffset_Byte offset)
            {
                return Offset == offset.Offset && Register == offset.Register;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Register.GetHashCode();
        }

        public IOffset ToByteOffset() => this;
    }
}
