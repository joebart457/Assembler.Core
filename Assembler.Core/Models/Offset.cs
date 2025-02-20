using Assembler.Core.Constants;

namespace Assembler.Core.Models
{
    public class Offset
    {
        public static RegisterOffset Create(X86Register register, int offset) => new RegisterOffset(register, offset);
        public static RegisterOffset_Byte CreateByteOffset(X86Register register, int offset) => new RegisterOffset_Byte(register, offset);
        public static SymbolOffset CreateSymbolOffset(string symbol, int offset) => new SymbolOffset(symbol, offset);
        public static SymbolOffset_Byte CreateSymbolByteOffset(string symbol, int offset) => new SymbolOffset_Byte(symbol, offset);
    }
}
