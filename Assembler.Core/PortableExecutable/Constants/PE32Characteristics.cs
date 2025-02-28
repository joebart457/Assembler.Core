
namespace Assembler.Core.PortableExecutable.Constants;
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
