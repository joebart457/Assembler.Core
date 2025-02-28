using Assembler.Core.Models;
using Assembler.Core.PortableExecutable;
using Assembler.Core.PortableExecutable.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Core.Instructions;


public class DefineWord : X86Instruction
{
    public short[] Words { get; set; }


    public DefineWord(short word)
    {
        Words = [word];
    }

    public DefineWord(short[] words)
    {
        Words = words;
    }

    public override uint GetSizeOnDisk() => (uint)Words.Length * 2;
    public override uint GetVirtualSize() => (uint)Words.Length * 2;

    public override string Emit()
    {
        return $"dw {string.Join(", ", Words.Select(x => x.ToString()))}";
    }

    public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
    {
        var result = new List<byte>();
        foreach (var w in Words)
        {
            result.AddRange(BitConverter.GetBytes(w));
        }
        return result.ToArray();
    }
}