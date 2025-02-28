using Assembler.Core.Extensions;
using System.Runtime.InteropServices;
using System.Text;

namespace Assembler.Core.Models;

public class X86Function
{
    public CallingConvention CallingConvention { get; set; }
    public string FunctionLabel { get; set; }
    public List<X86FunctionLocalData> Parameters { get; set; }
    public List<X86FunctionLocalData> LocalData { get; set; }
    public bool IsExported { get; set; }
    public string ExportedSymbol { get; set; }
    public List<X86Instruction> Instructions { get; set; } = new();
    public X86Function(CallingConvention callingConvention, string functionLabel, List<X86FunctionLocalData> parameters, List<X86FunctionLocalData> localData, bool isExported, string exportedSymbol)
    {
        CallingConvention = callingConvention;
        FunctionLabel = functionLabel;
        Parameters = parameters;
        LocalData = localData;
        IsExported = isExported;
        ExportedSymbol = exportedSymbol;
    }

    public Ty AddInstruction<Ty>(Ty instruction) where Ty: X86Instruction
    {
        Instructions.Add(instruction);
        return instruction; // to allow chaining in the future
    }

    public string Emit(int indentLevel)
    {
        var sb = new StringBuilder();
        foreach (var instruction in Instructions)
        {
            if (instruction == Instructions.First()) sb.AppendLine(instruction.Emit().Indent(indentLevel + 1)); // only indent function label once
            sb.AppendLine(instruction.Emit().Indent(indentLevel + 2));
        }
        return sb.ToString();
    }

    public string GetDecoratedFunctionLabel()
    {
        if (CallingConvention == CallingConvention.Cdecl) return $"_{FunctionLabel}";
        if (CallingConvention == CallingConvention.StdCall) return $"_{FunctionLabel}@{Parameters.Count * 4}";
        throw new NotImplementedException($"No support for calling convention {CallingConvention}");
    }
}