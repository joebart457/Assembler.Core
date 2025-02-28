using Assembler.Core.Extensions;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public class ImageDataDirectory
{
    public UInt32 VirtualAddress;
    public UInt32 Size;
    public ImageDataDirectory(uint virtualAddress, uint size)
    {
        VirtualAddress = virtualAddress;
        Size = size;
    }

    public static ImageDataDirectory Zero => new(0, 0);
    public List<byte> GetBytes()
    {
        var result = new List<byte>();

        result.AddRange(VirtualAddress.ToBytes());
        result.AddRange(Size.ToBytes());

        return result;
    }
}
