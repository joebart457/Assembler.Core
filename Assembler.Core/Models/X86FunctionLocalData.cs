
namespace Assembler.Core.Models;

public class X86FunctionLocalData
{
    public string Alias { get; set; }
    public int StackSize { get; set; }

    public X86FunctionLocalData(string alias, int stackSize)
    {
        Alias = alias;
        StackSize = stackSize;
    }

}