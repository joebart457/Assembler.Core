using Assembler.Core.Models;
using Assembler.Core.PortableExecutable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        throw new NotImplementedException();
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


/// <summary>
/// Internal use only.
/// </summary>
internal class PadByte : X86Instruction
{
    public byte[] DefinedBytes { get; set; }

    public PadByte(byte value)
    {
        DefinedBytes = [value];
    }
    public PadByte(byte[] definedBytes)
    {
        DefinedBytes = definedBytes;
    }

    public override string Emit()
    {
        throw new NotImplementedException();
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