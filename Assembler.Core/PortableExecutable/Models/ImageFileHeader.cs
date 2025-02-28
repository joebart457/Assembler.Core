using Assembler.Core.Extensions;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct ImageFileHeader
{
    public UInt32 Signature;
    public UInt16 Machine;
    public UInt16 NumberOfSections;
    public UInt32 TimeDateStamp;
    public UInt32 PointerToSymbolTable;
    public UInt32 NumberOfSymbols;
    public UInt16 SizeOfOptionalHeader;
    public UInt16 Characteristics;

    public List<byte> GetBytes()
    {
        var result = new List<byte>();

        result.AddRange(Signature.ToBytes());
        result.AddRange(Machine.ToBytes());
        result.AddRange(NumberOfSections.ToBytes());
        result.AddRange(TimeDateStamp.ToBytes());
        result.AddRange(PointerToSymbolTable.ToBytes());
        result.AddRange(NumberOfSymbols.ToBytes());
        result.AddRange(SizeOfOptionalHeader.ToBytes());
        result.AddRange(Characteristics.ToBytes());

        return result;
    }
}
