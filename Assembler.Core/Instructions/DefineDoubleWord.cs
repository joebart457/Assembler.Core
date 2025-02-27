using Assembler.Core.Models;
using Assembler.Core.PortableExecutable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Core.Instructions;


public class DefineDoubleWord : X86Instruction
{
    public int[] DoubleWords { get; set; }


    public DefineDoubleWord(int doubleWord)
    {
        DoubleWords = [doubleWord];
    }

    public DefineDoubleWord(int[] doubleWords)
    {
        DoubleWords = doubleWords;
    }

    public override uint GetSizeOnDisk() => (uint)DoubleWords.Length * 4;
    public override uint GetVirtualSize() => (uint)DoubleWords.Length * 4;

    public override string Emit()
    {
        throw new NotImplementedException();
    }

    public override byte[] Assemble(Section section, Dictionary<string, Address> resolvedLabels)
    {
        var result = new List<byte>();  
        foreach(var dw in DoubleWords)
        {
            result.AddRange(BitConverter.GetBytes(dw));
        }
        return result.ToArray();
    }
}

public class DefineDoubleWord_Rva : X86Instruction
{
    public Rva Rva { get; set; }

    public DefineDoubleWord_Rva(Rva rva)
    {
        Rva = rva;
    }

    public override uint GetSizeOnDisk() => 4;
    public override uint GetVirtualSize() => 4;

    public override string Emit()
    {
        throw new NotImplementedException();
    }

    public override byte[] Assemble(Section section, Dictionary<string, Address> resolvedLabels)
    {
        var address = GetAddressOrThrow(resolvedLabels, Rva.Symbol);
        return BitConverter.GetBytes(address.RelativeVirtualAddress);
    }
}

public class DefineDoubleWord_Address : X86Instruction
{
    public string Symbol { get; set; }

    public DefineDoubleWord_Address(string symbol)
    {
        Symbol = symbol;
    }

    public override uint GetSizeOnDisk() => 4;
    public override uint GetVirtualSize() => 4;

    public override string Emit()
    {
        throw new NotImplementedException();
    }

    public override byte[] Assemble(Section section, Dictionary<string, Address> resolvedLabels)
    {
        var address = GetAddressOrThrow(resolvedLabels, Symbol);
        return BitConverter.GetBytes(address.VirtualAddress);
    }
}