
namespace Assembler.Core.PortableExecutable.Constants
{
    internal static class Defaults
    {
        public const ushort DOSMagic = 0x5A4D;
        public const uint PEMagic = 0x00004550;
        public const ushort PE32Magic = 0x010B; // PE32;
        public const ushort Machine = 0x014C; // i386
        public const int SectionAlignment = 0x00001000; // 4096
        public const int FileAlignment = 0x00000200; // 512
        public const uint PageSize = 0x00001000;
        public const int ImageBase = 0x00400000;
        public const int OperatingSystemVersion = 0x00000001; // 1.0
        public const int ImageVersion = 0x00000000; // 0
        public const int SubsystemVersion = 0x000A0003; // 3.10
        public const int Win32VersionValue = 0; // Reserved?
        public const ushort Subsystem = 0x003; // 3 console
        public const int SizeOfStackReserve = 0x00001000;
        public const int SizeOfStackCommit = 0x00001000;
        public const int SizeOfHeapReserve = 0x00010000;
        public const int SizeOfHeapCommit = 0x00000000;
        public const int LoaderFlags = 0; // Obsolete
    }
}
