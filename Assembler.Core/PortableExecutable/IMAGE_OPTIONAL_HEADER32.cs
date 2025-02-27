﻿using Assembler.Core.Extensions;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class IMAGE_OPTIONAL_HEADER32
{
    public UInt16 Magic;
    public Byte MajorLinkerVersion;
    public Byte MinorLinkerVersion;
    public UInt32 SizeOfCode;
    public UInt32 SizeOfInitializedData;
    public UInt32 SizeOfUninitializedData;
    public UInt32 AddressOfEntryPoint;
    public UInt32 BaseOfCode;
    public UInt32 BaseOfData;
    public UInt32 ImageBase;
    public UInt32 SectionAlignment;
    public UInt32 FileAlignment;
    public UInt16 MajorOperatingSystemVersion;
    public UInt16 MinorOperatingSystemVersion;
    public UInt16 MajorImageVersion;
    public UInt16 MinorImageVersion;
    public UInt16 MajorSubsystemVersion;
    public UInt16 MinorSubsystemVersion;
    public UInt32 Win32VersionValue;
    public UInt32 SizeOfImage;
    public UInt32 SizeOfHeaders;
    public UInt32 CheckSum;
    public UInt16 Subsystem;
    public UInt16 DllCharacteristics;
    public UInt32 SizeOfStackReserve;
    public UInt32 SizeOfStackCommit;
    public UInt32 SizeOfHeapReserve;
    public UInt32 SizeOfHeapCommit;
    public UInt32 LoaderFlags;
    public UInt32 NumberOfDataDirectories;           

    public IMAGE_DATA_DIRECTORY ExportTable = IMAGE_DATA_DIRECTORY.Zero;
    public IMAGE_DATA_DIRECTORY ImportTable = IMAGE_DATA_DIRECTORY.Zero;
    public IMAGE_DATA_DIRECTORY ResourceTable = IMAGE_DATA_DIRECTORY.Zero;
    public IMAGE_DATA_DIRECTORY ExceptionTable = IMAGE_DATA_DIRECTORY.Zero;
    public IMAGE_DATA_DIRECTORY CertificateTable = IMAGE_DATA_DIRECTORY.Zero;
    public IMAGE_DATA_DIRECTORY BaseRelocationTable = IMAGE_DATA_DIRECTORY.Zero;
    public IMAGE_DATA_DIRECTORY Debug = IMAGE_DATA_DIRECTORY.Zero;
    public IMAGE_DATA_DIRECTORY Architecture = IMAGE_DATA_DIRECTORY.Zero;
    public IMAGE_DATA_DIRECTORY GlobalPtr = IMAGE_DATA_DIRECTORY.Zero;
    public IMAGE_DATA_DIRECTORY TLSTable = IMAGE_DATA_DIRECTORY.Zero;
    public IMAGE_DATA_DIRECTORY LoadConfigTable = IMAGE_DATA_DIRECTORY.Zero;
    public IMAGE_DATA_DIRECTORY BoundImport = IMAGE_DATA_DIRECTORY.Zero;
    public IMAGE_DATA_DIRECTORY IAT = IMAGE_DATA_DIRECTORY.Zero;
    public IMAGE_DATA_DIRECTORY DelayImportDescriptor = IMAGE_DATA_DIRECTORY.Zero;
    public IMAGE_DATA_DIRECTORY CLRRuntimeHeader = IMAGE_DATA_DIRECTORY.Zero;
    public IMAGE_DATA_DIRECTORY Reserved = IMAGE_DATA_DIRECTORY.Zero;

    public List<byte> GetBytes()
    {
        var result = new List<byte>();
        result.AddRange(Magic.ToBytes());
        result.Add(MajorLinkerVersion);
        result.Add(MinorLinkerVersion);
        result.AddRange(SizeOfCode.ToBytes());
        result.AddRange(SizeOfInitializedData.ToBytes());
        result.AddRange(SizeOfUninitializedData.ToBytes());
        result.AddRange(AddressOfEntryPoint.ToBytes());
        result.AddRange(BaseOfCode.ToBytes());
        result.AddRange(BaseOfData.ToBytes());
        result.AddRange(ImageBase.ToBytes());
        result.AddRange(SectionAlignment.ToBytes());
        result.AddRange(FileAlignment.ToBytes());
        result.AddRange(MajorOperatingSystemVersion.ToBytes());
        result.AddRange(MinorOperatingSystemVersion.ToBytes());
        result.AddRange(MajorImageVersion.ToBytes());
        result.AddRange(MinorImageVersion.ToBytes());
        result.AddRange(MajorSubsystemVersion.ToBytes());
        result.AddRange(MinorSubsystemVersion.ToBytes());
        result.AddRange(Win32VersionValue.ToBytes());
        result.AddRange(SizeOfImage.ToBytes());
        result.AddRange(SizeOfHeaders.ToBytes());
        result.AddRange(CheckSum.ToBytes());
        result.AddRange(Subsystem.ToBytes());
        result.AddRange(DllCharacteristics.ToBytes());
        result.AddRange(SizeOfStackReserve.ToBytes());
        result.AddRange(SizeOfStackCommit.ToBytes());
        result.AddRange(SizeOfHeapReserve.ToBytes());
        result.AddRange(SizeOfHeapCommit.ToBytes());
        result.AddRange(LoaderFlags.ToBytes());
        result.AddRange(NumberOfDataDirectories.ToBytes());

        result.AddRange(ExportTable.GetBytes());
        result.AddRange(ImportTable.GetBytes());
        result.AddRange(ResourceTable.GetBytes());
        result.AddRange(ExceptionTable.GetBytes());
        result.AddRange(CertificateTable.GetBytes());
        result.AddRange(BaseRelocationTable.GetBytes());
        result.AddRange(Debug.GetBytes());
        result.AddRange(Architecture.GetBytes());
        result.AddRange(GlobalPtr.GetBytes());
        result.AddRange(TLSTable.GetBytes());
        result.AddRange(LoadConfigTable.GetBytes());
        result.AddRange(BoundImport.GetBytes());
        result.AddRange(IAT.GetBytes());
        result.AddRange(DelayImportDescriptor.GetBytes());
        result.AddRange(CLRRuntimeHeader.GetBytes());
        result.AddRange(Reserved.GetBytes());

        return result;
    }

    public ushort Size => (ushort)GetBytes().Count;
}
