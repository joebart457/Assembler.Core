
namespace Assembler.Core.PortableExecutable.Constants;

public static class DllCharacteristics
{
    public const ushort HighEntropyVA = 0x0020;           // IMAGE_DLLCHARACTERISTICS_HIGH_ENTROPY_VA
    public const ushort DynamicBase = 0x0040;             // IMAGE_DLLCHARACTERISTICS_DYNAMIC_BASE
    public const ushort ForceIntegrity = 0x0080;          // IMAGE_DLLCHARACTERISTICS_FORCE_INTEGRITY
    public const ushort NxCompat = 0x0100;                // IMAGE_DLLCHARACTERISTICS_NX_COMPAT
    public const ushort NoIsolation = 0x0200;             // IMAGE_DLLCHARACTERISTICS_NO_ISOLATION
    public const ushort NoSEH = 0x0400;                   // IMAGE_DLLCHARACTERISTICS_NO_SEH
    public const ushort NoBind = 0x0800;                  // IMAGE_DLLCHARACTERISTICS_NO_BIND
    public const ushort AppContainer = 0x1000;            // IMAGE_DLLCHARACTERISTICS_APPCONTAINER
    public const ushort WdmDriver = 0x2000;               // IMAGE_DLLCHARACTERISTICS_WDM_DRIVER
    public const ushort GuardCF = 0x4000;                 // IMAGE_DLLCHARACTERISTICS_GUARD_CF
    public const ushort TerminalServerAware = 0x8000;     // IMAGE_DLLCHARACTERISTICS_TERMINAL_SERVER_AWARE
}