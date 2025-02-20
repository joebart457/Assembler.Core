using Assembler.Core.Constants;
using Assembler.Core.Extensions;
using Assembler.Core.Models;
using System.Text;

namespace Assembler.Core;

public static class X86AssemblyGenerator
{
    public static string? OutputToFile(X86AssemblyContext assemblyContext, AssemblerOptions assemblerOptions)
    {
        SantizeAssemblyFilePath(assemblerOptions);
        SantizeOutputFilePath(assemblyContext.OutputTarget, assemblerOptions);
        var errorMessage = OutputX86Assembly(assemblyContext, assemblerOptions, out var generatedCode);
        if (errorMessage != null) return errorMessage;
        File.WriteAllText(assemblerOptions.AssemblyFilePath, generatedCode.ToString());
        return null;
    }

    public static string? OutputToMemory(X86AssemblyContext assemblyContext, AssemblerOptions assemblerOptions, out StringBuilder generatedX86Code)
    {
        SantizeAssemblyFilePath(assemblerOptions);
        SantizeOutputFilePath(assemblyContext.OutputTarget, assemblerOptions);
        var errorMessage = OutputX86Assembly(assemblyContext, assemblerOptions, out generatedX86Code);
        return errorMessage;
    }

    private static string? OutputX86Assembly(X86AssemblyContext assemblyContext, AssemblerOptions assemblerOptions, out StringBuilder generatedX86Code)
    {
        var sb = new StringBuilder();

        if (assemblyContext.OutputTarget == OutputTarget.Exe)
        {
            sb.AppendLine("format PE console");
            if (assemblyContext.EntryPoint == null)
                assemblyContext.SetEntryPoint("Main");
        }
        else if (assemblyContext.OutputTarget == OutputTarget.Dll)
        {
            sb.AppendLine("format PE DLL");
            if (assemblyContext.EntryPoint == null)
                assemblyContext.SetEntryPoint("DllEntryPoint");
        }
        else throw new Exception($"unable to generate code for output target {assemblyContext.OutputTarget}");

        sb.AppendLine($"entry {assemblyContext.GetEntryPoint().GetDecoratedFunctionLabel()}");

        // Output static data
        sb.AppendLine("section '.data' data readable writeable".Indent(0));
        foreach (var stringData in assemblyContext.StaticStringData)
        {
            sb.AppendLine(stringData.Emit(1));
        }

        foreach (var floatingPointData in assemblyContext.StaticFloatingPointData)
        {
            sb.AppendLine(floatingPointData.Emit(1));
        }

        foreach (var integerData in assemblyContext.StaticIntegerData)
        {
            sb.AppendLine(integerData.Emit(1));
        }

        foreach (var byteData in assemblyContext.StaticByteData)
        {
            sb.AppendLine(byteData.Emit(1));
        }

        foreach (var pointerData in assemblyContext.StaticPointerData)
        {
            sb.AppendLine(pointerData.Emit(1));
        }

        foreach (var unitializedData in assemblyContext.StaticUnitializedData)
        {
            sb.AppendLine(unitializedData.Emit(1));
        }

        // Output User Functions
        sb.AppendLine("section '.text' code readable executable");
        foreach (var proc in assemblyContext.FunctionData)
        {
            sb.Append(proc.Emit(1));
        }

        if (assemblyContext.ImportLibraries.Any())
        {
            // Output imported functions
            sb.AppendLine("section '.idata' import data readable writeable");
            int libCounter = 0;
            foreach (var importLibrary in assemblyContext.ImportLibraries)
            {
                sb.AppendLine($"dd !lib_{libCounter}_ilt,0,0,RVA !lib_{libCounter}_name, RVA !lib_{libCounter}_iat".Indent(1));
                libCounter++;
            }
            sb.AppendLine($"dd 0,0,0,0,0".Indent(1));
            libCounter = 0;
            foreach (var importLibrary in assemblyContext.ImportLibraries)
            {
                sb.AppendLine($"!lib_{libCounter}_name db '{importLibrary.LibraryPath}',0".Indent(1));
                sb.AppendLine("rb RVA $ and 1".Indent(1));
                libCounter++;
            }

            libCounter = 0;
            foreach (var importLibrary in assemblyContext.ImportLibraries)
            {
                sb.AppendLine("rb(-rva $) and 3".Indent(1));

                sb.AppendLine($"!lib_{libCounter}_ilt:".Indent(1));
                foreach (var importedFunction in importLibrary.ImportedFunctions)
                {
                    sb.AppendLine($"dd RVA !{importedFunction.FunctionIdentifier}".Indent(1));
                }
                sb.AppendLine($"dd 0".Indent(1));

                sb.AppendLine($"!lib_{libCounter}_iat:".Indent(1));
                foreach (var importedFunction in importLibrary.ImportedFunctions)
                {
                    sb.AppendLine($"{importedFunction.FunctionIdentifier} dd RVA !{importedFunction.FunctionIdentifier}".Indent(1));
                }
                sb.AppendLine($"dd 0".Indent(1));

                foreach (var importedFunction in importLibrary.ImportedFunctions)
                {
                    sb.AppendLine($"!{importedFunction.FunctionIdentifier} dw 0".Indent(1));
                    sb.AppendLine($"db '{importedFunction.Symbol}',0".Indent(1));
                    if (importedFunction != importLibrary.ImportedFunctions.Last()) sb.AppendLine("rb RVA $ and 1".Indent(1));
                }

                libCounter++;
            }
        }




        // Output exported user functions
        if (assemblyContext.OutputTarget == OutputTarget.Dll)
        {
            sb.AppendLine("section '.edata' export data readable");

            sb.AppendLine($"dd 0,0,0, RVA !lib_name, 1".Indent(1));
            sb.AppendLine($"dd {assemblyContext.ExportedFunctions.Count},{assemblyContext.ExportedFunctions.Count}, RVA !exported_addresses, RVA !exported_names, RVA !exported_ordinals".Indent(1));

            sb.AppendLine($"!exported_addresses:".Indent(1));
            foreach (var exportedFunction in assemblyContext.ExportedFunctions)
            {
                sb.AppendLine($"dd RVA {exportedFunction.functionIdentifier}".Indent(2));
            }

            sb.AppendLine($"!exported_names:".Indent(1));
            int exportedNamesCounter = 0;
            foreach (var exportedFunction in assemblyContext.ExportedFunctions)
            {
                sb.AppendLine($"dd RVA !exported_{exportedNamesCounter}".Indent(2));
                exportedNamesCounter++;
            }
            exportedNamesCounter = 0;
            sb.AppendLine($"!exported_ordinals:".Indent(1));
            foreach (var exportedFunction in assemblyContext.ExportedFunctions)
            {
                sb.AppendLine($"dw {exportedNamesCounter}".Indent(2));
                exportedNamesCounter++;
            }

            exportedNamesCounter = 0;
            sb.AppendLine($"!lib_name db '{Path.GetFileName(assemblerOptions.ExecutableFilePath)}',0".Indent(1));
            foreach (var exportedFunction in assemblyContext.ExportedFunctions)
            {
                sb.AppendLine($"!exported_{exportedNamesCounter} db '{exportedFunction.exportedSymbol}',0".Indent(1));
                exportedNamesCounter++;
            }

            sb.AppendLine("section '.reloc' fixups data readable discardable");
            sb.AppendLine("if $= $$".Indent(1));
            sb.AppendLine($"dd 0,8".Indent(2));
            sb.AppendLine("end if".Indent(1));

        }
        else if (assemblyContext.ProgramIcon != null)
        {
            // Only include program icon for exe target
            sb.AppendLine("section '.rsrc'resource data readable");
            int RT_ICON = 3;
            int RT_GROUP_ICON = 14;
            int IDR_ICON = 17;
            int LANG_NEUTRAL = 0;
            sb.AppendLine($"root@resource dd 0, %t, 0, 2 shl 16".Indent(1));
            sb.AppendLine($"dd {RT_ICON}, 80000000h + !icons - root@resource".Indent(1));
            sb.AppendLine($"dd {RT_GROUP_ICON}, 80000000h + !group_icons - root@resource".Indent(1));

            sb.AppendLine($"!icons:".Indent(1));
            sb.AppendLine($"dd      0, %t, 0, 1 shl 16".Indent(1));
            sb.AppendLine($"dd      1, 80000000h + !icon_data.directory - root@resource".Indent(1));
            sb.AppendLine($"!icon_data.directory dd 0, %t, 0, 10000h, {LANG_NEUTRAL}, !icon_data - root@resource".Indent(1));
            sb.AppendLine($"!group_icons:".Indent(1));
            sb.AppendLine($"dd      0, %t, 0, 1 shl 16".Indent(1));
            sb.AppendLine($"dd {IDR_ICON}, 80000000h + !main_icon.directory - root@resource".Indent(1));
            sb.AppendLine($"!main_icon.directory dd 0, %t, 0, 10000h, {LANG_NEUTRAL}, !main_icon - root@resource".Indent(1));

            sb.AppendLine($"!icon_data dd RVA !data, !size, 0, 0".Indent(1));
            sb.AppendLine($"virtual at 0".Indent(1));
            sb.AppendLine($"file '{assemblyContext.ProgramIcon.FilePath}':6, 16".Indent(1));
            sb.AppendLine($"load !size dword from 8".Indent(1));
            sb.AppendLine($"load !position dword from 12".Indent(1));
            sb.AppendLine($"end virtual".Indent(1));
            sb.AppendLine($"!data file '{assemblyContext.ProgramIcon.FilePath}':!position, !size".Indent(1));
            sb.AppendLine($"align 4".Indent(1));
            sb.AppendLine($"!main_icon dd RVA !header, 6+1*14, 0, 0".Indent(1));
            sb.AppendLine($"!header dw 0, 1, 1".Indent(1));
            sb.AppendLine($"file '{assemblyContext.ProgramIcon.FilePath}':6, 12".Indent(1));
            sb.AppendLine($"dw 1".Indent(1));
        }

        generatedX86Code = sb;
        return null; // return null since there are no errors
    }

    private static void SantizeAssemblyFilePath(AssemblerOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.AssemblyFilePath))
        {
            options.AssemblyFilePath = Path.ChangeExtension(Path.GetTempFileName(), ".asm");
            return;
        }
        options.AssemblyFilePath = Path.GetFullPath(options.AssemblyFilePath);
        if (Path.GetExtension(options.AssemblyFilePath) != ".asm") options.AssemblyFilePath = $"{options.AssemblyFilePath}.asm";
    }

    private static void SantizeOutputFilePath(OutputTarget outputTarget, AssemblerOptions options)
    {
        if (string.IsNullOrEmpty(options.ExecutableFilePath)) options.ExecutableFilePath = Path.GetFullPath(Path.GetFileNameWithoutExtension(options.AssemblyFilePath));
        var outputPath = Path.GetFullPath(options.ExecutableFilePath);
        if (outputTarget == OutputTarget.Exe && Path.GetExtension(outputPath) != ".exe") outputPath = $"{outputPath}.exe";
        if (outputTarget == OutputTarget.Dll && Path.GetExtension(outputPath) != ".dll") outputPath = $"{outputPath}.dll";
        options.ExecutableFilePath = outputPath;
    }

}