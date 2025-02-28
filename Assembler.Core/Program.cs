



using Assembler.Core.Constants;
using Assembler.Core;
using Assembler.Core.Instructions;
using Assembler.Core.Models;
using System.Runtime.InteropServices;

internal class Program
{
    private static void Main(string[] args)
    {

        var assemblyContext = new X86AssemblyContext();

        // Import printf from mscvrt.dll
        assemblyContext.AddImportLibrary(new X86AssemblyContext.ImportLibrary("msvcrt.dll", "msvcrt"));
        assemblyContext.AddImportedFunction(new X86AssemblyContext.ImportedFunction(CallingConvention.Cdecl, "msvcrt", "printf", "printf", [new("msg", DataSize.Dword), new("number", DataSize.Dword)]));

        // Add constant string data
        var messageLabel = assemblyContext.AddStringData("Q: What is the answer to life the world and everything?\r\n A: %d");

        List<X86FunctionLocalData> localVariables = [new("localDataOne", DataSize.Dword)];

        assemblyContext.EnterFunction(new X86Function(CallingConvention.StdCall, "Main", new(), localVariables, false, ""));
        // Manually setup stack frame
        assemblyContext.Push(X86Register.ebp);
        assemblyContext.Mov(X86Register.ebp, X86Register.esp);
        assemblyContext.Sub(X86Register.esp, DataSize.Dword); // Reserve space for localDataOne

        // Function body
        assemblyContext.Mov(assemblyContext.GetIdentifierOffset("localDataOne"), 42);
        assemblyContext.Push(assemblyContext.GetIdentifierOffset("localDataOne"));
        assemblyContext.Push(messageLabel);
        assemblyContext.Call("printf", true);    // manual indirect call to printf since it is imported
        assemblyContext.Add(X86Register.esp, 8); // C calling convention means caller must cleanup the stack

        assemblyContext.Xor(X86Register.eax, X86Register.eax); // Trick to zero out eax register for return

        // manually teardown stack frame
        assemblyContext.Mov(X86Register.esp, X86Register.ebp);
        assemblyContext.Pop(X86Register.ebp);
        assemblyContext.Ret();


        var error = assemblyContext.OutputToFile("sixth.exe");
        if (error != null) Console.WriteLine(error);

        Console.WriteLine("Done!");

        //var peFile = new PEFile();
        //
        //peFile.DataSection.AddInstruction(new Label("msg"));
        //peFile.DataSection.AddInstruction(new DefineByte("Hello, world!"));
        //
        //peFile.CodeSection.AddInstruction(new Label("main"));
        //peFile.CodeSection.AddInstruction(new DefineByte([0x55, 0x89, 0xE5]));
        //peFile.CodeSection.AddInstruction(new Push_Address("msg"));
        //peFile.CodeSection.AddInstruction(new Call("printf", true));
        //peFile.CodeSection.AddInstruction(new DefineByte([0x89, 0xEC, 0x5D, 0x31, 0xC0]));
        //peFile.CodeSection.AddInstruction(new Ret());
        //
        //
        //
        //peFile.ImportsSection.AddInstruction(new DefineDoubleWord_Rva(Rva.Create("ilt")));
        //peFile.ImportsSection.AddInstruction(new DefineDoubleWord(0));
        //peFile.ImportsSection.AddInstruction(new DefineDoubleWord(0));
        //peFile.ImportsSection.AddInstruction(new DefineDoubleWord_Rva(Rva.Create("libpath")));
        //peFile.ImportsSection.AddInstruction(new DefineDoubleWord_Rva(Rva.Create("iat")));
        //peFile.ImportsSection.AddInstruction(new DefineDoubleWord([0,0,0,0,0]));
        //
        //peFile.ImportsSection.AddInstruction(new Label("libpath"));
        //peFile.ImportsSection.AddInstruction(new DefineByte("msvcrt.dll"));
        //if (peFile.ImportsSection.VirtualSize % 2 == 1) peFile.ImportsSection.AddInstruction(new DefineByte(0));
        //
        //peFile.ImportsSection.AddInstruction(new Label("ilt"));
        //peFile.ImportsSection.AddInstruction(new DefineDoubleWord_Rva(Rva.Create("int")));
        //peFile.ImportsSection.AddInstruction(new DefineDoubleWord(0));
        //
        //peFile.ImportsSection.AddInstruction(new Label("iat"));
        //peFile.ImportsSection.AddInstruction(new Label("printf"));
        //peFile.ImportsSection.AddInstruction(new DefineDoubleWord_Rva(Rva.Create("int")));
        //peFile.ImportsSection.AddInstruction(new DefineDoubleWord(0));
        //
        //peFile.ImportsSection.AddInstruction(new Label("int"));
        //peFile.ImportsSection.AddInstruction(new DefineByte(0));
        //peFile.ImportsSection.AddInstruction(new DefineByte(0));
        //peFile.ImportsSection.AddInstruction(new DefineByte("printf"));
        //peFile.ImportsSection.AddInstruction(new DefineByte(0));
        //
        //
        //
        //var assembledBytes = peFile.AssembleProgram("main");
        //File.WriteAllBytes("fifth.exe", assembledBytes);
    }
}