using Assembler.Core.Extensions;

public class ImageDosHeader
{      // DOS .EXE header
    public UInt16 e_magic;              // Magic number
    public UInt16 e_cblp;               // Bytes on last page of file
    public UInt16 e_cp;                 // Pages in file
    public UInt16 e_crlc;               // Relocations
    public UInt16 e_cparhdr;            // Size of header in paragraphs
    public UInt16 e_minalloc;           // Minimum extra paragraphs needed
    public UInt16 e_maxalloc;           // Maximum extra paragraphs needed
    public UInt16 e_ss;                 // Initial (relative) SS value
    public UInt16 e_sp;                 // Initial SP value
    public UInt16 e_csum;               // Checksum
    public UInt16 e_ip;                 // Initial IP value
    public UInt16 e_cs;                 // Initial (relative) CS value
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
