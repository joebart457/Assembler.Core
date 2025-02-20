

namespace Assembler.Core.Models;

public class AssemblerOptions
{
    public string AssemblyFilePath { get; set; } = ""; // Path to generated assembly (.asm) file
    public string ExecutableFilePath { get; set; } = ""; // Path to final generated PE file (.exe or .dll)
}