namespace Assembler.Core.Models
{
    public class SymbolOffset_Byte : IOffset
    {
        public string Symbol { get; set; }
        public int Offset { get; set; }
        public SymbolOffset_Byte(string symbol, int offset)
        {
            Symbol = symbol;
            Offset = offset;
        }

        public override string ToString()
        {
            var repr = Offset == 0 ? Symbol : $"{Symbol} {(Offset > 0 ? "+" : "-")} {Math.Abs(Offset)}";
            return $"byte [{repr}]";
        }

        public override bool Equals(object? obj)
        {
            if (obj is SymbolOffset offset)
            {
                return Offset == offset.Offset && Symbol == offset.Symbol;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Symbol.GetHashCode();
        }

        public IOffset ToByteOffset() => this;
    }
}
