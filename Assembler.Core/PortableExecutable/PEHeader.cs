using Assembler.Core.Extensions;
using Assembler.Core.PortableExecutable;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Xml.Linq;

public class IMAGE_DOS_HEADER
{      // DOS .EXE header
    public UInt16 e_magic;              // Magic number
    public UInt16 e_cblp = 0x08;               // Bytes on last page of file
    public UInt16 e_cp = 0x01;                 // Pages in file
    public UInt16 e_crlc;               // Relocations
    public UInt16 e_cparhdr = 0x04;            // Size of header in paragraphs
    public UInt16 e_minalloc = 0x01;           // Minimum extra paragraphs needed
    public UInt16 e_maxalloc = 0xFFFF;           // Maximum extra paragraphs needed
    public UInt16 e_ss = 0x0104;                 // Initial (relative) SS value
    public UInt16 e_sp;                 // Initial SP value
    public UInt16 e_csum;               // Checksum
    public UInt16 e_ip;                 // Initial IP value
    public UInt16 e_cs= 0x04;                 // Initial (relative) CS value
    public UInt16 e_lfarlc;             // File address of relocation table
    public UInt16 e_ovno;               // Overlay number
    public UInt16 e_res_0;              // Reserved words
    public UInt16 e_res_1;              // Reserved words
    public UInt16 e_res_2;              // Reserved words
    public UInt16 e_res_3;              // Reserved words
    public UInt16 e_oemid;              // OEM identifier (for e_oeminfo)
    public UInt16 e_oeminfo;            // OEM information; e_oemid specific
    public UInt16 e_res2_0;             // Reserved words
    public UInt16 e_res2_1;             // Reserved words
    public UInt16 e_res2_2;             // Reserved words
    public UInt16 e_res2_3;             // Reserved words
    public UInt16 e_res2_4;             // Reserved words
    public UInt16 e_res2_5;             // Reserved words
    public UInt16 e_res2_6;             // Reserved words
    public UInt16 e_res2_7;             // Reserved words
    public UInt16 e_res2_8;             // Reserved words
    public UInt16 e_res2_9;             // Reserved words
    public UInt32 e_lfanew;             // File address of new exe header from beginning of file on disk

    public static uint Size => 64;
    public List<byte> GetBytes()
    {
        var result = new List<byte>();
        result.AddRange(e_magic.ToBytes());
        result.AddRange(e_cblp.ToBytes());
        result.AddRange(e_cp.ToBytes());
        result.AddRange(e_crlc.ToBytes());
        result.AddRange(e_cparhdr.ToBytes());
        result.AddRange(e_minalloc.ToBytes());
        result.AddRange(e_maxalloc.ToBytes());
        result.AddRange(e_ss.ToBytes());
        result.AddRange(e_sp.ToBytes());
        result.AddRange(e_csum.ToBytes());
        result.AddRange(e_ip.ToBytes());
        result.AddRange(e_cs.ToBytes());
        result.AddRange(e_lfarlc.ToBytes());
        result.AddRange(e_ovno.ToBytes());
        result.AddRange(e_res_0.ToBytes());
        result.AddRange(e_res_1.ToBytes());
        result.AddRange(e_res_2.ToBytes());
        result.AddRange(e_res_3.ToBytes());
        result.AddRange(e_oemid.ToBytes());
        result.AddRange(e_oeminfo.ToBytes());
        result.AddRange(e_res2_0.ToBytes());
        result.AddRange(e_res2_1.ToBytes());
        result.AddRange(e_res2_2.ToBytes());
        result.AddRange(e_res2_3.ToBytes());
        result.AddRange(e_res2_4.ToBytes());
        result.AddRange(e_res2_5.ToBytes());
        result.AddRange(e_res2_6.ToBytes());
        result.AddRange(e_res2_7.ToBytes());
        result.AddRange(e_res2_8.ToBytes());
        result.AddRange(e_res2_9.ToBytes());
        result.AddRange(e_lfanew.ToBytes());
        return result;
    }

}




[StructLayout(LayoutKind.Sequential)]
public class IMAGE_DATA_DIRECTORY
{
    public UInt32 VirtualAddress;
    public UInt32 Size;
    public IMAGE_DATA_DIRECTORY(uint virtualAddress, uint size)
    {
        VirtualAddress = virtualAddress;
        Size = size;
    }

    public static IMAGE_DATA_DIRECTORY Zero => new(0, 0);
    public List<byte> GetBytes()
    {
        var result = new List<byte>();

        result.AddRange(VirtualAddress.ToBytes());
        result.AddRange(Size.ToBytes());

        return result;
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct IMAGE_FILE_HEADER
{
    public UInt32 Signature;
    public UInt16 Machine;
    public UInt16 NumberOfSections;
    public UInt32 TimeDateStamp;
    public UInt32 PointerToSymbolTable;
    public UInt32 NumberOfSymbols;
    public UInt16 SizeOfOptionalHeader;
    public UInt16 Characteristics;

    public List<byte> GetBytes()
    {
        var result = new List<byte>();

        result.AddRange(Signature.ToBytes());
        result.AddRange(Machine.ToBytes());
        result.AddRange(NumberOfSections.ToBytes());
        result.AddRange(TimeDateStamp.ToBytes());
        result.AddRange(PointerToSymbolTable.ToBytes());
        result.AddRange(NumberOfSymbols.ToBytes());
        result.AddRange(SizeOfOptionalHeader.ToBytes());
        result.AddRange(Characteristics.ToBytes());

        return result;
    }
}

public class IMAGE_SECTION_HEADER
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


public class PEFile {

    public IMAGE_DOS_HEADER DosHeader = new();

    public byte[] DosStub =
    [
        0x0e,0x1f,0xba,0x0e,0x00,0xb4,0x09,0xcd,0x21,0xb8,0x01,0x4c,0xcd,0x21,0x54,0x68,0x69,0x73,0x20,0x70,0x72,0x6f,0x67,0x72,0x61,0x6d,0x20,0x63,0x61,0x6e,0x6e,0x6f,0x74,0x20,0x62,0x65,0x20,0x72,0x75,0x6e,0x20,0x69,0x6e,0x20,0x44,0x4f,0x53,0x20,0x6d,0x6f,0x64,0x65,0x2e,0x0d,0x0a,0x24,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00
    ];

    /// The file header

    public IMAGE_FILE_HEADER FileHeader;

    /// Optional 32 bit file header 

    public IMAGE_OPTIONAL_HEADER32 OptionalHeader32 = new();

    /// Image Section headers. Number of sections is in the file header.
    public uint GetPEHeaderSize() => 512;

    public uint GetImageSize()
    {
        var headerBytes = AssembleHeaders();
        var size = ((uint)headerBytes.Length).RoundUpToNearestMultipleOfFactor(OptionalHeader32.SectionAlignment);
        size += InitializedDataSection.TotalVirtualSize;
        size += CodeSection.TotalVirtualSize;
        size += ImportsSection.TotalVirtualSize;
        size += RelocationSection?.TotalVirtualSize ?? 0;
        return size;
    }

    public uint GetHeadersSize()
    {
        var size = GetPEHeaderSize();
        size += InitializedDataSection.HeaderSize;
        size += CodeSection.HeaderSize;
        size += ImportsSection.HeaderSize;
        size += RelocationSection?.HeaderSize ?? 0;
        return size;
    }

    private void CreateDosHeader()
    {
        DosHeader.e_magic = Defaults.DOSMagic;
        //DosHeader.e_cblp = 0;
        //DosHeader.e_cblp = 0;
        //DosHeader.e_cp = 0;
        //DosHeader.e_crlc = 0;
        //DosHeader.e_cparhdr = 0;
        //DosHeader.e_minalloc = 0;
        //DosHeader.e_maxalloc = 0;
        //DosHeader.e_ss = 0;
        //DosHeader.e_sp = 0;
        //DosHeader.e_csum = 0;
        //DosHeader.e_ip = 0;
        //DosHeader.e_cs = 0;
        //DosHeader.e_lfarlc = 0;
        //DosHeader.e_ovno = 0;
        //DosHeader.e_res_0 = 0;
        //DosHeader.e_res_1 = 0;
        //DosHeader.e_res_2 = 0;
        //DosHeader.e_res_3 = 0;
        //DosHeader.e_oemid = 0;
        //DosHeader.e_oeminfo = 0;
        //DosHeader.e_res2_0 = 0;
        //DosHeader.e_res2_1 = 0;
        //DosHeader.e_res2_2 = 0;
        //DosHeader.e_res2_3 = 0;
        //DosHeader.e_res2_4 = 0;
        //DosHeader.e_res2_5 = 0;
        //DosHeader.e_res2_6 = 0;
        //DosHeader.e_res2_7 = 0;
        //DosHeader.e_res2_8 = 0;
        //DosHeader.e_res2_9 = 0;
        DosHeader.e_lfanew = IMAGE_DOS_HEADER.Size + (uint)DosStub.Length;
    }

    private void CreateOptionalHeader32()
    {
        OptionalHeader32.Magic = Defaults.PE32Magic;
        OptionalHeader32.MajorLinkerVersion = 0;
        OptionalHeader32.MinorLinkerVersion = 1;
        OptionalHeader32.SizeOfCode = 0; // Must be set at assemble time
        OptionalHeader32.SizeOfInitializedData = 0; // Must be set at assemble time
        OptionalHeader32.AddressOfEntryPoint = 0; // must be set at assemble time
        OptionalHeader32.BaseOfCode = 0; // Must be set at assemble time
        OptionalHeader32.BaseOfData = 0; // Must be set at assemble time
        OptionalHeader32.ImageBase = Defaults.ImageBase;
        OptionalHeader32.SectionAlignment = Defaults.SectionAlignment;
        OptionalHeader32.FileAlignment = Defaults.FileAlignment;
        OptionalHeader32.MajorOperatingSystemVersion = 0x06; // vista
        OptionalHeader32.MinorOperatingSystemVersion = 0x00;
        OptionalHeader32.MajorImageVersion = 0x00;
        OptionalHeader32.MinorImageVersion = 0x00;
        OptionalHeader32.MajorSubsystemVersion = 0x0003; // 3
        OptionalHeader32.MinorSubsystemVersion = 0x000A; // .10
        OptionalHeader32.Win32VersionValue = 0;
        OptionalHeader32.SizeOfImage = 0; // Must be set at assemble time
        OptionalHeader32.SizeOfHeaders = 0; // Must be set at assemble time
        OptionalHeader32.CheckSum = 0;
        OptionalHeader32.Subsystem = Defaults.Subsystem;
        OptionalHeader32.DllCharacteristics = 0; // TODO
        OptionalHeader32.SizeOfStackReserve = Defaults.SizeOfStackReserve;
        OptionalHeader32.SizeOfStackCommit = Defaults.SizeOfStackCommit;
        OptionalHeader32.SizeOfHeapReserve = Defaults.SizeOfHeapReserve;
        OptionalHeader32.SizeOfHeapCommit = Defaults.SizeOfHeapCommit;
        OptionalHeader32.LoaderFlags = 0; // Obsolete
        OptionalHeader32.NumberOfDataDirectories = 0x00000010; // 16 data directories

        // Define Data Directories
        OptionalHeader32.ExportTable = IMAGE_DATA_DIRECTORY.Zero;
        OptionalHeader32.ImportTable = IMAGE_DATA_DIRECTORY.Zero; // Must be set at assemble time
        OptionalHeader32.ResourceTable = IMAGE_DATA_DIRECTORY.Zero;
        OptionalHeader32.ExceptionTable = IMAGE_DATA_DIRECTORY.Zero;
        OptionalHeader32.CertificateTable = IMAGE_DATA_DIRECTORY.Zero;
        OptionalHeader32.BaseRelocationTable = IMAGE_DATA_DIRECTORY.Zero; // Must be set at assemble time
        OptionalHeader32.Debug = IMAGE_DATA_DIRECTORY.Zero;
        OptionalHeader32.Architecture = IMAGE_DATA_DIRECTORY.Zero;
        OptionalHeader32.GlobalPtr = IMAGE_DATA_DIRECTORY.Zero;
        OptionalHeader32.TLSTable = IMAGE_DATA_DIRECTORY.Zero;
        OptionalHeader32.LoadConfigTable = IMAGE_DATA_DIRECTORY.Zero;
        OptionalHeader32.BoundImport = IMAGE_DATA_DIRECTORY.Zero;
        OptionalHeader32.IAT = IMAGE_DATA_DIRECTORY.Zero;
        OptionalHeader32.DelayImportDescriptor = IMAGE_DATA_DIRECTORY.Zero;
        OptionalHeader32.CLRRuntimeHeader = IMAGE_DATA_DIRECTORY.Zero;
        OptionalHeader32.Reserved = IMAGE_DATA_DIRECTORY.Zero;
    }

    private void CreateNTHeader()
    {
        FileHeader.Signature = Defaults.PEMagic;
        FileHeader.NumberOfSections = 3;
        FileHeader.Machine = Defaults.Machine;
        FileHeader.TimeDateStamp = (uint)DateTime.Now.Ticks;
        FileHeader.PointerToSymbolTable = 0;
        FileHeader.NumberOfSymbols = 0;
        FileHeader.SizeOfOptionalHeader = OptionalHeader32.Size;
        FileHeader.Characteristics = PE32Characteristics.ExecutableImage | PE32Characteristics.LineNumsStripped | PE32Characteristics.LocalSymsStripped | PE32Characteristics.Bit32Machine;
        if (RelocationSection == null) FileHeader.Characteristics |= PE32Characteristics.RelocsStripped;
    }

    private byte[] AssembleHeaders()
    {
        var assembledBytes = new List<byte>();
        assembledBytes.AddRange(DosHeader.GetBytes());
        assembledBytes.AddRange(DosStub);
        assembledBytes.AddRange(FileHeader.GetBytes());
        assembledBytes.AddRange(OptionalHeader32.GetBytes());
        assembledBytes.AddRange(InitializedDataSection.ImageSectionHeader.GetBytes());
        assembledBytes.AddRange(CodeSection.ImageSectionHeader.GetBytes());
        assembledBytes.AddRange(ImportsSection.ImageSectionHeader.GetBytes());
        if (RelocationSection != null) assembledBytes.AddRange(RelocationSection.ImageSectionHeader.GetBytes());
        PadToFileAlignment(assembledBytes);
        return assembledBytes.ToArray();
    }

    public byte[] AssembleProgram(string entryPoint)
    {
        CodeSection.PEFile = this;
        InitializedDataSection.PEFile = this;
        ImportsSection.PEFile = this;

        CreateDosHeader();
        CreateNTHeader();
        CreateOptionalHeader32();

        //var relocations = new List<BaseRelocationBlock>();
        //relocations.AddRange(InitializedDataSection.GetRelocations(this));
        //relocations.AddRange(CodeSection.GetRelocations(this));
        //relocations.AddRange(ImportsSection.GetRelocations(this));
        //RelocationSection = new RelocationSection(relocations);



        if (OptionalHeader32.SectionAlignment < GetPEHeaderSize()) throw new InvalidOperationException();

        var headerBytes = AssembleHeaders();

        var currentBytesOnDisk = (uint)headerBytes.Length;
        SetPointerToRawData(InitializedDataSection, ref currentBytesOnDisk);
        SetPointerToRawData(CodeSection, ref currentBytesOnDisk);
        SetPointerToRawData(ImportsSection, ref currentBytesOnDisk);
        SetPointerToRawData(RelocationSection, ref currentBytesOnDisk);

        var currentVirtualAddress = ((uint)headerBytes.Length).RoundUpToNearestMultipleOfFactor(OptionalHeader32.SectionAlignment);
        SetVirtualAddress(InitializedDataSection, ref currentVirtualAddress);
        SetVirtualAddress(CodeSection, ref currentVirtualAddress);
        SetVirtualAddress(ImportsSection, ref currentVirtualAddress);
        SetVirtualAddress(RelocationSection, ref currentVirtualAddress);

        var labelsWithAddresses = new Dictionary<string, Address>();
        InitializedDataSection.ExtractLabelAddresses(this, labelsWithAddresses);
        CodeSection.ExtractLabelAddresses(this, labelsWithAddresses);
        ImportsSection.ExtractLabelAddresses(this, labelsWithAddresses);


        FileHeader.Characteristics = PE32Characteristics.ExecutableImage | PE32Characteristics.LineNumsStripped | PE32Characteristics.LocalSymsStripped | PE32Characteristics.Bit32Machine;
        if (RelocationSection == null) FileHeader.Characteristics |= PE32Characteristics.RelocsStripped;

        if (!labelsWithAddresses.TryGetValue(entryPoint, out var addressOfEntryPoint)) throw new InvalidOperationException($"unable to locate entry point {entryPoint}");

        OptionalHeader32.AddressOfEntryPoint = addressOfEntryPoint.RelativeVirtualAddress;
        OptionalHeader32.SizeOfCode = CodeSection.SizeOfRawData;
        OptionalHeader32.SizeOfInitializedData = InitializedDataSection.SizeOfRawData + ImportsSection.SizeOfRawData + (RelocationSection?.SizeOfRawData ?? 0);
        OptionalHeader32.BaseOfData = InitializedDataSection.RelativeVirtualAddress;
        OptionalHeader32.BaseOfCode = CodeSection.RelativeVirtualAddress;
        OptionalHeader32.SizeOfImage = GetImageSize();
        OptionalHeader32.SizeOfHeaders = InitializedDataSection.PointerToRawData;
        // Define Data Directories
        OptionalHeader32.ImportTable = new(ImportsSection.RelativeVirtualAddress, ImportsSection.RawInstructionSize);
        OptionalHeader32.BaseRelocationTable = RelocationSection == null? IMAGE_DATA_DIRECTORY.Zero : new(RelocationSection.RelativeVirtualAddress, RelocationSection.RawInstructionSize);


        var assembledBytes = AssembleHeaders().ToList();
        var checksumOffset = 0xD8;

        assembledBytes.AddRange(InitializedDataSection.AssembleAndAlign(labelsWithAddresses));    
        assembledBytes.AddRange(CodeSection.AssembleAndAlign(labelsWithAddresses));
        assembledBytes.AddRange(ImportsSection.AssembleAndAlign(labelsWithAddresses));
        if (RelocationSection != null) assembledBytes.AddRange(RelocationSection.AssembleAndAlign(labelsWithAddresses));

        OptionalHeader32.CheckSum = CalculateChecksum(assembledBytes.ToArray(), checksumOffset);
        var checksumBytes = BitConverter.GetBytes(OptionalHeader32.CheckSum);
        for(int i = 0; i < checksumBytes.Length; i++)
        {
            assembledBytes[checksumOffset + i] = checksumBytes[i]; 
        }

        return assembledBytes.ToArray();
    }

    static uint CalculateChecksum(byte[] data, int checksumOffset)
    {
        long checksum = 0;
        var top = Math.Pow(2, 32);

        for (var i = 0; i < data.Length / 4; i++)
        {
            if (i == checksumOffset / 4)
            {
                continue;
            }
            var dword = BitConverter.ToUInt32(data, i * 4);
            checksum = (checksum & 0xffffffff) + dword + (checksum >> 32);
            if (checksum > top)
            {
                checksum = (checksum & 0xffffffff) + (checksum >> 32);
            }
        }

        checksum = (checksum & 0xffff) + (checksum >> 16);
        checksum = (checksum) + (checksum >> 16);
        checksum = checksum & 0xffff;

        checksum += (uint)data.Length;
        return (uint)checksum;

    }

    private void PadToFileAlignment(List<byte> bytes)
    {
        var paddedCount = IntExtensions.RoundUpToNearestMultipleOfFactor((uint)bytes.Count, OptionalHeader32.FileAlignment);
        while (bytes.Count < paddedCount)
        {
            bytes.Add(0);
        }
    }

    private void SetVirtualAddress(Section? section, ref uint currentVirtualAddressInPE)
    {
        if (section == null) return;
        section.RelativeVirtualAddress = currentVirtualAddressInPE;
        currentVirtualAddressInPE += section.VirtualSize.RoundUpToNearestMultipleOfFactor(OptionalHeader32.SectionAlignment);
    }

    private void SetPointerToRawData(Section? section, ref uint currentByteOnDiskPointer)
    {
        if (section == null) return;

        section.PointerToRawData = currentByteOnDiskPointer;
        currentByteOnDiskPointer += section.SizeOfRawData;
    }



    public InitializedDataSection InitializedDataSection { get; set; } = new();
    public ImportsSection ImportsSection { get; set; } = new();
    public CodeSection CodeSection { get; set; } = new();
    public RelocationSection? RelocationSection { get; set; }
}

public class  PE32Characteristics
{
    public const int RelocsStripped = 0x0001;          // Relocation info stripped from file.
    public const int ExecutableImage = 0x0002;         // File is executable (no unresolved external references).
    public const int LineNumsStripped = 0x0004;        // Line numbers stripped from file.
    public const int LocalSymsStripped = 0x0008;       // Local symbols stripped from file.
    public const int AggressiveWsTrim = 0x0010;        // Aggressively trim working set.
    public const int LargeAddressAware = 0x0020;       // App can handle >2GB addresses.
    public const int BytesReversedLo = 0x0080;         // Bytes of machine word are reversed.
    public const int Bit32Machine = 0x0100;            // 32-bit word machine.
    public const int DebugStripped = 0x0200;           // Debugging info stripped from file in .DBG file.
    public const int RemovableRunFromSwap = 0x0400;    // If Image is on removable media; copy and run from the swap file.
    public const int NetRunFromSwap = 0x0800;          // If Image is on network media; copy and run from the swap file.
    public const int System = 0x1000;                  // System file.
    public const int Dll = 0x2000;                     // File is a DLL.
    public const int UpSystemOnly = 0x4000;            // File should only be run on a UP machine.
    public const int BytesReversedHi = 0x8000;         // Bytes of machine word are reversed.
}

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