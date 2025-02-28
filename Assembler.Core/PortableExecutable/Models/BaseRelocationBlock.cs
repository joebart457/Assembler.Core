using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Core.PortableExecutable.Models;

public class BaseRelocationBlock
{
    public uint PageRVA { get; set; }
    public uint BlockSize => 8 + (uint)Entries.Count * 2; // size of block header + block entries
    public List<BaseRelocationEntry> Entries { get; set; } = new();

    public List<byte> GetBytes()
    {
        var result = new List<byte>();
        result.AddRange(BitConverter.GetBytes(PageRVA));
        result.AddRange(BitConverter.GetBytes(BlockSize));
        foreach (var entry in Entries)
        {
            result.AddRange(entry.GetByteRepresentation());
        }
        return result;
    }

    public void AddEntry(ushort offset)
    {
        Entries.Add(new BaseRelocationEntry { Offset = offset });
    }

    public void AddEntry(int offset)
    {
        Entries.Add(new BaseRelocationEntry { Offset = (ushort)offset });
    }
}