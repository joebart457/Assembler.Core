using Assembler.Core.Models;
using Assembler.Core.PortableExecutable;
using System.Text;

namespace Assembler.Core.Instructions;


public class DefineByte: X86Instruction
{
    public byte[] DefinedBytes { get; set; }

    public DefineByte(byte value)
    {
        DefinedBytes = [value];
    }
    public DefineByte(byte[] definedBytes)
    {
        DefinedBytes = definedBytes;
    }

    public DefineByte(string str)
    {
        var bytes = Encoding.UTF8.GetBytes(str).ToList();
        bytes.Add(0x00);
        DefinedBytes = bytes.ToArray();
    }

    public override string Emit()
    {
        return $"db {BytesAsHex(DefinedBytes)}";
    }

    private static string BytesAsHex(byte[] bytes)
    {
        return $"0x{BitConverter.ToString(bytes).Replace("-", ",0x")},0x00";
    }

    public override uint GetVirtualSize()
    {
        return (uint)DefinedBytes.Length;
    }

    public override uint GetSizeOnDisk()
    {
        return (uint)DefinedBytes.Length;
    }
    public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
    {
        return DefinedBytes;
    }

}
