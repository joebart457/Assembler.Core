using Assembler.Core.Extensions;
using Assembler.Core.PortableExecutable;
using Assembler.Core.PortableExecutable.Constants;
using Assembler.Core.PortableExecutable.Models;


public class PEFile {

    public ImageDosHeader DosHeader = new();
    // this usually contains a DOS Stub program that displays the message 'This program cannot be run in DOS mode'.  But that is not necessary so we will leave it blank
    public readonly byte[] DosStub = new byte[64]; 
    public ImageFileHeader FileHeader = new();
    public ImageOptionalHeader32 OptionalHeader32 = new();

    private DataSection _dataSection;
    private CodeSection _codeSection;
    private ImportsSection _importsSection;
    private ExportsSection? _exportsSection;
    private BssSection? _bssSection;
    private RelocationsSection? _relocationsSection;

    public DataSection DataSection { get => _dataSection; set { _dataSection = value; SetPEFileReference(value); } }
    public CodeSection CodeSection { get => _codeSection; set { _codeSection = value; SetPEFileReference(value); } }
    public ImportsSection ImportsSection { get => _importsSection; set { _importsSection = value; SetPEFileReference(value); } }
    public ExportsSection? ExportsSection { get => _exportsSection; set { _exportsSection = value; SetPEFileReference(value); } }
    public BssSection? BssSection { get => _bssSection; set { _bssSection = value; SetPEFileReference(value); } }
    // Relocations section cannot be set from outside of this class
    // this is because the assembly process will automatically create the relocations if needed
    public RelocationsSection? RelocationsSection { get => _relocationsSection; protected set { _relocationsSection = value; SetPEFileReference(value); } }

    public PEFile()
    {
        _dataSection = new(); SetPEFileReference(_dataSection);
        _codeSection = new(); SetPEFileReference(_codeSection);
        _importsSection = new(); SetPEFileReference(_importsSection);
        CreateDosHeader();
        CreateNTHeader();
        CreateOptionalHeader32();
        Array.Fill<byte>(DosStub, 0x00);
    }

    public uint GetImageSize()
    {
        var headerBytes = AssembleHeaders();
        var size = ((uint)headerBytes.Length).RoundUpToNearestMultipleOfFactor(OptionalHeader32.SectionAlignment);
        size += DataSection.TotalVirtualSize;
        size += CodeSection.TotalVirtualSize;
        size += ImportsSection.TotalVirtualSize;
        size += ExportsSection?.TotalVirtualSize ?? 0;
        size += BssSection?.TotalVirtualSize ?? 0;
        size += RelocationsSection?.TotalVirtualSize ?? 0;
        return size;
    }

    public ushort GetNumberOfSections()
    {
        ushort result = 3;
        if (ExportsSection != null) result++;
        if (BssSection != null) result++;
        if (RelocationsSection != null) result++;
        return result;
    }

    private byte[] AssembleHeaders()
    {
        var assembledBytes = new List<byte>();
        assembledBytes.AddRange(DosHeader.GetBytes());
        assembledBytes.AddRange(DosStub);
        assembledBytes.AddRange(FileHeader.GetBytes());
        assembledBytes.AddRange(OptionalHeader32.GetBytes());
        assembledBytes.AddRange(DataSection.ImageSectionHeader.GetBytes());
        assembledBytes.AddRange(CodeSection.ImageSectionHeader.GetBytes());
        assembledBytes.AddRange(ImportsSection.ImageSectionHeader.GetBytes());
        if (ExportsSection != null) assembledBytes.AddRange(ExportsSection.ImageSectionHeader.GetBytes());
        if (BssSection != null) assembledBytes.AddRange(BssSection.ImageSectionHeader.GetBytes());
        if (RelocationsSection != null) assembledBytes.AddRange(RelocationsSection.ImageSectionHeader.GetBytes());
        PadToFileAlignment(assembledBytes);
        return assembledBytes.ToArray();
    }

    public void MarkAsDLL()
    {
        OptionalHeader32.DllCharacteristics = DllCharacteristics.DynamicBase;
        FileHeader.Characteristics = PE32Characteristics.ExecutableImage | PE32Characteristics.LineNumsStripped | PE32Characteristics.LocalSymsStripped | PE32Characteristics.Bit32Machine |PE32Characteristics.Dll;

        if (RelocationsSection == null) RelocationsSection = new();
    }

    public void MarkAsExe()
    {
        OptionalHeader32.DllCharacteristics = 0x0000;
        FileHeader.Characteristics = PE32Characteristics.ExecutableImage | PE32Characteristics.LineNumsStripped | PE32Characteristics.LocalSymsStripped | PE32Characteristics.Bit32Machine | PE32Characteristics.RelocsStripped;
    }

    public bool IsMarkedAsDLL => (OptionalHeader32.DllCharacteristics & DllCharacteristics.DynamicBase) == DllCharacteristics.DynamicBase;

    public byte[] AssembleProgram(string entryPoint)
    {
        if (!IsMarkedAsDLL) MarkAsExe();
        var headerBytes = AssembleHeaders();

        var currentBytesOnDisk = (uint)headerBytes.Length;
        SetPointerToRawData(DataSection, ref currentBytesOnDisk);
        SetPointerToRawData(CodeSection, ref currentBytesOnDisk);
        SetPointerToRawData(ImportsSection, ref currentBytesOnDisk);
        SetPointerToRawData(ExportsSection, ref currentBytesOnDisk);
        SetPointerToRawData(BssSection, ref currentBytesOnDisk);
        SetPointerToRawData(RelocationsSection, ref currentBytesOnDisk);

        var currentVirtualAddress = ((uint)headerBytes.Length).RoundUpToNearestMultipleOfFactor(OptionalHeader32.SectionAlignment);
        SetVirtualAddress(DataSection, ref currentVirtualAddress);
        SetVirtualAddress(CodeSection, ref currentVirtualAddress);
        SetVirtualAddress(ImportsSection, ref currentVirtualAddress);
        SetVirtualAddress(ExportsSection, ref currentVirtualAddress);
        SetVirtualAddress(BssSection, ref currentVirtualAddress);

        // It is very important that relocs section is done last.
        // Currently, it is empty and thus its actual virtual size will almost certainly be larger than what it is currently (most likely 0 at this point)
        // Having its address calculated last ensures that the other virtual addresses will not be thrown off if this section grows once relocations are calculated
        SetVirtualAddress(RelocationsSection, ref currentVirtualAddress); 

        var baseRelocations = new List<BaseRelocationBlock>();
        ExtractRelocations(DataSection, baseRelocations);
        ExtractRelocations(CodeSection, baseRelocations);
        ExtractRelocations(ImportsSection, baseRelocations);
        ExtractRelocations(ExportsSection, baseRelocations);
        ExtractRelocations(BssSection, baseRelocations);
        if (RelocationsSection != null) RelocationsSection.Blocks = baseRelocations;    

        var labelsWithAddresses = new Dictionary<string, Address>();
        ExtractLabelAddresses(DataSection, labelsWithAddresses);
        ExtractLabelAddresses(CodeSection, labelsWithAddresses);
        ExtractLabelAddresses(ImportsSection, labelsWithAddresses);
        ExtractLabelAddresses(ExportsSection, labelsWithAddresses);
        ExtractLabelAddresses(BssSection, labelsWithAddresses);
        ExtractLabelAddresses(RelocationsSection, labelsWithAddresses);        

        if (!labelsWithAddresses.TryGetValue(entryPoint, out var addressOfEntryPoint)) throw new InvalidOperationException($"unable to locate entry point {entryPoint}");

        OptionalHeader32.AddressOfEntryPoint = addressOfEntryPoint.RelativeVirtualAddress;
        OptionalHeader32.SizeOfCode = CodeSection.SizeOfRawData;
        OptionalHeader32.SizeOfInitializedData = DataSection.SizeOfRawData 
            + ImportsSection.SizeOfRawData 
            + (ExportsSection?.SizeOfRawData ?? 0) 
            + (RelocationsSection?.SizeOfRawData ?? 0);
        OptionalHeader32.BaseOfData = DataSection.RelativeVirtualAddress;
        OptionalHeader32.BaseOfCode = CodeSection.RelativeVirtualAddress;
        OptionalHeader32.SizeOfImage = GetImageSize();
        OptionalHeader32.SizeOfHeaders = DataSection.PointerToRawData;
        // Define Data Directories
        OptionalHeader32.ImportTable = new(ImportsSection.RelativeVirtualAddress, ImportsSection.RawInstructionSize);
        OptionalHeader32.BaseRelocationTable = RelocationsSection == null? ImageDataDirectory.Zero : RelocationsSection.ToDataDirectory();
        OptionalHeader32.ExportTable = ExportsSection == null? ImageDataDirectory.Zero : ExportsSection.ToDataDirectory();
        OptionalHeader32.SizeOfUninitializedData = BssSection?.VirtualSize ?? 0;
        FileHeader.NumberOfSections = GetNumberOfSections();
        var assembledBytes = AssembleHeaders().ToList();
        var checksumOffset = 0xD8;

        AssembleSection(DataSection, assembledBytes, labelsWithAddresses);
        AssembleSection(CodeSection, assembledBytes, labelsWithAddresses);
        AssembleSection(ImportsSection, assembledBytes, labelsWithAddresses);
        AssembleSection(ExportsSection, assembledBytes, labelsWithAddresses);
        AssembleSection(BssSection, assembledBytes, labelsWithAddresses);
        AssembleSection(RelocationsSection, assembledBytes, labelsWithAddresses);

        OptionalHeader32.CheckSum = CalculateChecksum(assembledBytes.ToArray(), checksumOffset);
        var checksumBytes = BitConverter.GetBytes(OptionalHeader32.CheckSum);
        for(int i = 0; i < checksumBytes.Length; i++)
        {
            assembledBytes[checksumOffset + i] = checksumBytes[i]; 
        }

        return assembledBytes.ToArray();
    }

    private void AssembleSection(Section? section, List<byte> assembledBytes, Dictionary<string, Address> labelAddresses)
    {
        if (section == null) return;
        assembledBytes.AddRange(section.AssembleAndAlign(labelAddresses));
    }

    private static uint CalculateChecksum(byte[] data, int checksumOffset)
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

    private void ExtractLabelAddresses(Section? section, Dictionary<string, Address> labelsWithAddresses)
    {
        if (section == null) return;

        section.ExtractLabelAddresses(this, labelsWithAddresses);
    }

    private void ExtractRelocations(Section? section, List<BaseRelocationBlock> baseRelocationBlocks)
    {
        if (section == null) return;

        baseRelocationBlocks.AddRange(section.GetRelocations(this));
    }


    private void CreateDosHeader()
    {
        DosHeader.e_magic = Defaults.DOSMagic;
        DosHeader.e_cblp = 0;
        DosHeader.e_cblp = 0;
        DosHeader.e_cp = 0;
        DosHeader.e_crlc = 0;
        DosHeader.e_cparhdr = 0;
        DosHeader.e_minalloc = 0;
        DosHeader.e_maxalloc = 0;
        DosHeader.e_ss = 0;
        DosHeader.e_sp = 0;
        DosHeader.e_csum = 0;
        DosHeader.e_ip = 0;
        DosHeader.e_cs = 0;
        DosHeader.e_lfarlc = 0;
        DosHeader.e_ovno = 0;
        DosHeader.e_res_0 = 0;
        DosHeader.e_res_1 = 0;
        DosHeader.e_res_2 = 0;
        DosHeader.e_res_3 = 0;
        DosHeader.e_oemid = 0;
        DosHeader.e_oeminfo = 0;
        DosHeader.e_res2_0 = 0;
        DosHeader.e_res2_1 = 0;
        DosHeader.e_res2_2 = 0;
        DosHeader.e_res2_3 = 0;
        DosHeader.e_res2_4 = 0;
        DosHeader.e_res2_5 = 0;
        DosHeader.e_res2_6 = 0;
        DosHeader.e_res2_7 = 0;
        DosHeader.e_res2_8 = 0;
        DosHeader.e_res2_9 = 0;
        DosHeader.e_lfanew = ImageDosHeader.Size + (uint)DosStub.Length;
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
        OptionalHeader32.ExportTable = ImageDataDirectory.Zero;
        OptionalHeader32.ImportTable = ImageDataDirectory.Zero; // Must be set at assemble time
        OptionalHeader32.ResourceTable = ImageDataDirectory.Zero;
        OptionalHeader32.ExceptionTable = ImageDataDirectory.Zero;
        OptionalHeader32.CertificateTable = ImageDataDirectory.Zero;
        OptionalHeader32.BaseRelocationTable = ImageDataDirectory.Zero; // Must be set at assemble time
        OptionalHeader32.Debug = ImageDataDirectory.Zero;
        OptionalHeader32.Architecture = ImageDataDirectory.Zero;
        OptionalHeader32.GlobalPtr = ImageDataDirectory.Zero;
        OptionalHeader32.TLSTable = ImageDataDirectory.Zero;
        OptionalHeader32.LoadConfigTable = ImageDataDirectory.Zero;
        OptionalHeader32.BoundImport = ImageDataDirectory.Zero;
        OptionalHeader32.IAT = ImageDataDirectory.Zero;
        OptionalHeader32.DelayImportDescriptor = ImageDataDirectory.Zero;
        OptionalHeader32.CLRRuntimeHeader = ImageDataDirectory.Zero;
        OptionalHeader32.Reserved = ImageDataDirectory.Zero;
    }

    private void CreateNTHeader()
    {
        FileHeader.Signature = Defaults.PEMagic;
        FileHeader.NumberOfSections = 0x0000; // Must be set at assemble time
        FileHeader.Machine = Defaults.Machine;
        FileHeader.TimeDateStamp = (uint)DateTime.Now.Ticks;
        FileHeader.PointerToSymbolTable = 0;
        FileHeader.NumberOfSymbols = 0;
        FileHeader.SizeOfOptionalHeader = OptionalHeader32.Size;
        FileHeader.Characteristics = PE32Characteristics.ExecutableImage | PE32Characteristics.LineNumsStripped | PE32Characteristics.LocalSymsStripped | PE32Characteristics.Bit32Machine;
    }


    // Creates or replaces the bss section
    public BssSection CreateBssSection()
    {
        BssSection = new BssSection();
        return BssSection;
    }

    // Creates or replaces the edata section
    public ExportsSection CreateExportsSection()
    {
        ExportsSection = new ExportsSection();
        return ExportsSection;
    }

    private void PadToFileAlignment(List<byte> bytes)
    {
        var paddedCount = IntExtensions.RoundUpToNearestMultipleOfFactor((uint)bytes.Count, OptionalHeader32.FileAlignment);
        while (bytes.Count < paddedCount)
        {
            bytes.Add(0);
        }
    }

    private void SetPEFileReference(Section? section)
    {
        if (section == null) return;
        section.PEFile = this;
    }
}
