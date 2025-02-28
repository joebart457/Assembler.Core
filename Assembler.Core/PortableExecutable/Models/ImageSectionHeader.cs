using Assembler.Core.Extensions;
using System.Runtime.InteropServices;

public class ImageSectionHeader
{
    public static uint Size => 40;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
    public byte[] Name;
    public UInt32 VirtualSize;
    public UInt32 VirtualAddress;
    public UInt32 SizeOfRawData;
    public UInt32 PointerToRawData;
    public UInt32 PointerToRelocations;
    public UInt32 PointerToLinenumbers;
    public UInt16 NumberOfRelocations;
    public UInt16 NumberOfLinenumbers;
    public UInt32 Characteristics;

    public List<byte> GetBytes()
    {
        var result = Name.ToList();
        result.AddRange(VirtualSize.ToBytes());
        result.AddRange(VirtualAddress.ToBytes());
        result.AddRange(SizeOfRawData.ToBytes());
        result.AddRange(PointerToRawData.ToBytes());
        result.AddRange(PointerToRelocations.ToBytes());
        result.AddRange(PointerToLinenumbers.ToBytes());
        result.AddRange(NumberOfRelocations.ToBytes());
        result.AddRange(NumberOfLinenumbers.ToBytes());
        result.AddRange(Characteristics.ToBytes());
        return result;
    }
}
