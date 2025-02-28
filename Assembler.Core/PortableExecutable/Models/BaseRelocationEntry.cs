namespace Assembler.Core.PortableExecutable.Models;

public class BaseRelocationEntry
{
    /// <summary>
    /// Type is stored in high 4 bits of word. Super strange and horrible, I know.
    /// </summary>
    public ushort RelocationType => 0x0003; // IMAGE_REL_BASED_HIGHLOW (All 32 bits applied to relocation) 
    /// <summary>
    /// Stored in the remaining 12 bits of the WORD, an offset from the starting address that was specified in the Page RVA field for the block. This offset specifies where the base relocation is to be applied.
    /// </summary>
    public ushort Offset { get; set; }
    public ushort GetInt16Representation()
    {
        ushort type = (ushort)(RelocationType << 12); // shift to high 4 bits of word
        return (ushort)(type + Offset); // apply offset to remaining 12 bits (and pray offset does not exceed 12 bits of representation)
    }

    public byte[] GetByteRepresentation()
    {
        return BitConverter.GetBytes(GetInt16Representation());
    }
}