
namespace Assembler.Core.PortableExecutable.Constants;
public static class SectionCharacteristics
{
    public const uint TypeReg = 0x00000000; // Reserved
    public const uint TypeDSect = 0x00000001; // Reserved
    public const uint TypeNoLoad = 0x00000002; // Reserved
    public const uint TypeGroup = 0x00000004; // Reserved
    public const uint TypeNoPad = 0x00000008; // Reserved
    public const uint TypeCopy = 0x00000010; // Reserved
    public const uint ContainsCode = 0x00000020; // Section contains code
    public const uint ContainsInitializedData = 0x00000040; // Section contains initialized data
    public const uint ContainsUninitializedData = 0x00000080; // Section contains uninitialized data
    public const uint LinkerOther = 0x00000100; // Reserved
    public const uint LinkerInfo = 0x00000200; // Section contains comments or other information
    public const uint LinkerRemove = 0x00000800; // Section contents will not become part of image
    public const uint LinkerComdat = 0x00001000; // Section contents comdat
    public const uint NoDeferSpecExc = 0x00004000; // Reset speculative exceptions handling bits in the TLB entries for this section
    public const uint GPREl = 0x00008000; // Section content can be accessed relative to GP
    public const uint MemFardata = 0x00008000; // Obsolete
    public const uint MemPurgeable = 0x00020000; // Reserved
    public const uint Mem16Bit = 0x00020000; // Reserved
    public const uint MemLocked = 0x00040000; // Reserved
    public const uint MemPreload = 0x00080000; // Reserved
    public const uint Align1Bytes = 0x00100000; // Align data on a 1-byte boundary
    public const uint Align2Bytes = 0x00200000; // Align data on a 2-byte boundary
    public const uint Align4Bytes = 0x00300000; // Align data on a 4-byte boundary
    public const uint Align8Bytes = 0x00400000; // Align data on an 8-byte boundary
    public const uint Align16Bytes = 0x00500000; // Align data on a 16-byte boundary
    public const uint Align32Bytes = 0x00600000; // Align data on a 32-byte boundary
    public const uint Align64Bytes = 0x00700000; // Align data on a 64-byte boundary
    public const uint Align128Bytes = 0x00800000; // Align data on a 128-byte boundary
    public const uint Align256Bytes = 0x00900000; // Align data on a 256-byte boundary
    public const uint Align512Bytes = 0x00A00000; // Align data on a 512-byte boundary
    public const uint Align1024Bytes = 0x00B00000; // Align data on a 1024-byte boundary
    public const uint Align2048Bytes = 0x00C00000; // Align data on a 2048-byte boundary
    public const uint Align4096Bytes = 0x00D00000; // Align data on a 4096-byte boundary
    public const uint Align8192Bytes = 0x00E00000; // Align data on an 8192-byte boundary
    public const uint LinkerNRelocOvfl = 0x01000000; // Section contains extended relocations
    public const uint MemDiscardable = 0x02000000; // Section can be discarded
    public const uint MemNotCached = 0x04000000; // Section is not cacheable
    public const uint MemNotPaged = 0x08000000; // Section is not pageable
    public const uint MemShared = 0x10000000; // Section is shareable
    public const uint MemExecute = 0x20000000; // Section is executable
    public const uint MemRead = 0x40000000; // Section is readable
    public const uint MemWrite = 0x80000000; // Section is writeable
}