using Assembler.Core.Constants;
using Assembler.Core.Instructions;
using Assembler.Core.Models;



namespace Assembler.Core;

public static class X86AssemblyGenerator
{
    public static string? OutputToFile(X86AssemblyContext assemblyContext, string pathToFinalBinary)
    {
        var errorMessage = OutputX86Assembly(assemblyContext, out var generatedCode);
        if (errorMessage != null) return errorMessage;
        File.WriteAllBytes(pathToFinalBinary, generatedCode);
        return null;
    }

    public static string? OutputToMemory(X86AssemblyContext assemblyContext, out byte[] generatedPEFileBytes)
    {
        var errorMessage = OutputX86Assembly(assemblyContext, out generatedPEFileBytes);
        return errorMessage;
    }

    private static string? OutputX86Assembly(X86AssemblyContext assemblyContext, out byte[] generatedPEFileBytes)
    {
        var peFile = new PEFile();

        if (assemblyContext.OutputTarget == OutputTarget.Exe)
        {
            if (assemblyContext.EntryPoint == null)
                assemblyContext.SetEntryPoint("Main");
            peFile.MarkAsExe();
        }
        else if (assemblyContext.OutputTarget == OutputTarget.Dll)
        {
            if (assemblyContext.EntryPoint == null)
                assemblyContext.SetEntryPoint("DllEntryPoint");
            peFile.MarkAsDLL();

        }
        else throw new Exception($"unable to generate binary for output target {assemblyContext.OutputTarget}");


        // Add static data
        foreach (var stringData in assemblyContext.StaticStringData)
        {
            peFile.DataSection
                .AddInstruction(new Label(stringData.Label))
                .AddInstruction(new DefineByte(stringData.Value));
        }

        foreach (var floatingPointData in assemblyContext.StaticFloatingPointData)
        {
            peFile.DataSection
                .AddInstruction(new Label(floatingPointData.Label))
                .AddInstruction(new DefineByte(BitConverter.GetBytes(floatingPointData.Value)));
        }

        foreach (var integerData in assemblyContext.StaticIntegerData)
        {
            peFile.DataSection
                .AddInstruction(new Label(integerData.Label))
                .AddInstruction(new DefineByte(BitConverter.GetBytes(integerData.Value)));
        }

        foreach (var byteData in assemblyContext.StaticByteData)
        {
            peFile.DataSection
                .AddInstruction(new Label(byteData.Label))
                .AddInstruction(new DefineByte(byteData.Value));
        }

        foreach (var pointerData in assemblyContext.StaticPointerData)
        {
            peFile.DataSection
                .AddInstruction(new Label(pointerData.Label))
                .AddInstruction(new DefineByte(BitConverter.GetBytes(pointerData.Value)));
        }

        foreach (var unitializedData in assemblyContext.StaticUnitializedData)
        {
            peFile.CreateBssSection()
                .AddInstruction(new Label(unitializedData.Label))
                .AddInstruction(new ReserveByte(unitializedData.BytesToReserve));
        }

        // Add User Functions
        foreach (var proc in assemblyContext.FunctionData)
        {
            foreach(var instruction in proc.Instructions)
            {
                peFile.CodeSection.AddInstruction(instruction);
            }
        }

        if (assemblyContext.ImportLibraries.Any())
        {
            // Add imported functions
            int libCounter = 0;
            foreach (var importLibrary in assemblyContext.ImportLibraries)
            {
                peFile.ImportsSection.AddInstruction(new DefineDoubleWord_Rva(Rva.Create($"%ilt_{libCounter}")));
                peFile.ImportsSection.AddInstruction(new DefineDoubleWord(0));
                peFile.ImportsSection.AddInstruction(new DefineDoubleWord(0));
                peFile.ImportsSection.AddInstruction(new DefineDoubleWord_Rva(Rva.Create($"%libpath_{libCounter}")));
                peFile.ImportsSection.AddInstruction(new DefineDoubleWord_Rva(Rva.Create($"%iat_{libCounter}")));
                libCounter++;
            }
            peFile.ImportsSection.AddInstruction(new DefineDoubleWord([0, 0, 0, 0, 0]));
            libCounter = 0;
            foreach (var importLibrary in assemblyContext.ImportLibraries)
            {
                peFile.ImportsSection.AddInstruction(new Label($"%libpath_{libCounter}"));
                peFile.ImportsSection.AddInstruction(new DefineByte(importLibrary.LibraryPath));
                if (peFile.ImportsSection.VirtualSize % 2 == 1) peFile.ImportsSection.AddInstruction(new DefineByte(0));
                libCounter++;
            }

            libCounter = 0;
            foreach (var importLibrary in assemblyContext.ImportLibraries)
            {

                peFile.ImportsSection.AddInstruction(new Label($"%ilt_{libCounter}"));

                foreach (var importedFunction in importLibrary.ImportedFunctions)
                {
                    peFile.ImportsSection.AddInstruction(new DefineDoubleWord_Rva(Rva.Create($"%int_{libCounter}_{importedFunction.FunctionIdentifier}")));
                }  
                peFile.ImportsSection.AddInstruction(new DefineDoubleWord(0));

                peFile.ImportsSection.AddInstruction(new Label($"%iat_{libCounter}"));
                foreach (var importedFunction in importLibrary.ImportedFunctions)
                {
                    peFile.ImportsSection.AddInstruction(new Label(importedFunction.FunctionIdentifier));
                    peFile.ImportsSection.AddInstruction(new DefineDoubleWord_Rva(Rva.Create($"%int_{libCounter}_{importedFunction.FunctionIdentifier}")));
                }
                peFile.ImportsSection.AddInstruction(new DefineDoubleWord(0));

                foreach (var importedFunction in importLibrary.ImportedFunctions)
                {
                    peFile.ImportsSection.AddInstruction(new Label($"%int_{libCounter}_{importedFunction.FunctionIdentifier}"));
                    peFile.ImportsSection.AddInstruction(new DefineByte(0));
                    peFile.ImportsSection.AddInstruction(new DefineByte(0));
                    peFile.ImportsSection.AddInstruction(new DefineByte(importedFunction.Symbol));
                    if (peFile.ImportsSection.VirtualSize % 2 == 1) peFile.ImportsSection.AddInstruction(new DefineByte(0));
                }

                libCounter++;
            }
        }


        // Add exported user functions
        if (assemblyContext.ExportedFunctions.Any())
        {
            peFile.CreateExportsSection();

            peFile.CreateExportsSection()
                .AddInstruction(new DefineDoubleWord(0))
                .AddInstruction(new DefineDoubleWord(0))
                .AddInstruction(new DefineDoubleWord(0))
                .AddInstruction(new DefineDoubleWord_Rva(Rva.Create("%library_name")))
                .AddInstruction(new DefineDoubleWord(1));

            peFile.ExportsSection!
                .AddInstruction(new DefineDoubleWord(assemblyContext.ExportedFunctions.Count))
                .AddInstruction(new DefineDoubleWord(assemblyContext.ExportedFunctions.Count))
                .AddInstruction(new DefineDoubleWord_Rva(Rva.Create("%exported_addresses")))
                .AddInstruction(new DefineDoubleWord_Rva(Rva.Create("%exported_names")))
                .AddInstruction(new DefineDoubleWord_Rva(Rva.Create("%exported_ordinals")));

            peFile.ExportsSection!.AddInstruction(new Label("%exported_addresses"));
            foreach (var exportedFunction in assemblyContext.ExportedFunctions)
            {
                peFile.ExportsSection!.AddInstruction(new DefineDoubleWord_Rva(Rva.Create(exportedFunction.functionIdentifier)));
            }

            peFile.ExportsSection!.AddInstruction(new Label("%exported_names"));
            int exportedNamesCounter = 0;
            foreach (var exportedFunction in assemblyContext.ExportedFunctions)
            {
                peFile.ExportsSection!.AddInstruction(new DefineDoubleWord_Rva(Rva.Create($"%exported_{exportedNamesCounter}")));
                exportedNamesCounter++;
            }
            exportedNamesCounter = 0;
            peFile.ExportsSection!.AddInstruction(new Label("%exported_ordinals"));
            foreach (var exportedFunction in assemblyContext.ExportedFunctions)
            {
                peFile.ExportsSection!.AddInstruction(new DefineWord((short)exportedNamesCounter));
                exportedNamesCounter++;
            }

            exportedNamesCounter = 0;
            peFile.ExportsSection!.AddInstruction(new Label("%library_name"))
                .AddInstruction(new DefineByte(Path.GetFileName(assemblyContext.GetExportFileName())));

            foreach (var exportedFunction in assemblyContext.ExportedFunctions)
            {
                peFile.ExportsSection!
                    .AddInstruction(new Label($"%exported_{exportedNamesCounter}"))
                    .AddInstruction(new DefineByte(exportedFunction.exportedSymbol));
                exportedNamesCounter++;
            }
        }

        generatedPEFileBytes = peFile.AssembleProgram(assemblyContext.GetEntryPoint().GetDecoratedFunctionLabel());
        return null; // return null since there are no errors
    }
}