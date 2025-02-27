using Assembler.Core.Constants;

namespace Assembler.Core.Models
{
    public class Offset
    {
        public static RegisterOffset Create(X86Register register, int offset) => new RegisterOffset(register, offset);
        public static SymbolOffset CreateSymbolOffset(string symbol, int offset) => new SymbolOffset(symbol, offset);
    }

    public class Rva
    {
        public static Rva Create(string symbol) => new Rva(symbol);
        public string Symbol { get; set; }

        public Rva(string symbol)
        {
            Symbol = symbol;
        }

        public override string ToString()
        {
            return $"RVA {Symbol}";
        }
    }
}