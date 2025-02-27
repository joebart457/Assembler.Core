using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Core.PortableExecutable
{
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

        public void AddEntry(UInt16 offset)
        {
            Entries.Add(new BaseRelocationEntry { Offset = offset });
        }

        public void AddEntry(int offset)
        {
            Entries.Add(new BaseRelocationEntry { Offset = (ushort)offset });
        }
    }

    public class BaseRelocationEntry
    {
        /// <summary>
        /// Type is stored in high 4 bits of word. Super strange and horrible, I know.
        /// </summary>
        public UInt16 RelocationType => 0x0003; // IMAGE_REL_BASED_HIGHLOW (All 32 bits applied to relocation) 
        /// <summary>
        /// Stored in the remaining 12 bits of the WORD, an offset from the starting address that was specified in the Page RVA field for the block. This offset specifies where the base relocation is to be applied.
        /// </summary>
        public UInt16 Offset { get; set; }
        public UInt16 GetInt16Representation()
        {
            UInt16 type = (UInt16)(RelocationType << 12); // shift to high 4 bits of word
            return (UInt16)(type + Offset); // apply offset to remaining 12 bits (and pray offset does not exceed 12 bits of representation)
        }

        public byte[] GetByteRepresentation()
        {
            return BitConverter.GetBytes(GetInt16Representation()); 
        }
    }
}
