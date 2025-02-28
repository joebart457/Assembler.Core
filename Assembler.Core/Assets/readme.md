# Assembler.Core
## Overview

**X86Assembler.Core** is a C# NuGet package designed to assist developers with turning x86 assembly code into executable files in the PE32 file format.

## Features

- Support for the most generally used subset of x86 assembly language
- Output PE32 files (exe and dll)
- Convience methods to make assembly code generation easier

## NonFeatures
- No 16 or 64 bit support. This is a 32 bit x86 assembler only.
- This is not a production grade assembler. For that I'd recommend the `Netwide Assembler` or `Flat Assembler` (which I absolutely love!) \*
- Most of the instructions I have added are ones I found need for when working on my compiler hobby project, I will add more as I need them
-  otherwise, feel free to derive a class from the `X86Instruction` class and add support for whichever instruction you may need

\*There are two great `.NET` projects out there that wrap the Flat Assembler check out `Reloaded.Assembler` or `Fasm.NET` projects.

## Usage

### Basic Example

Here is an example of how to use the X86AssemblyContext to import a printf from a mscvcrt.dll and to define a main function that calls it:


```csharp
using Assembler.Core.Constants;
using Assembler.Core.Models;
using System.Runtime.InteropServices;

namespace Assembler.Core
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var assemblyContext = new X86AssemblyContext();

            // Import printf from mscvrt.dll
            assemblyContext.AddImportLibrary(new X86AssemblyContext.ImportLibrary("msvcrt.dll", "msvcrt"));
            var printf = assemblyContext.AddImportedFunction(new X86AssemblyContext.ImportedFunction(CallingConvention.Cdecl, "msvcrt", "printf", "printf", [new("msg", DataSize.Dword), new("number", DataSize.Dword)]));

            // Add constant string data
            var messageLabel = assemblyContext.AddStringData("Q: What is the answer to life the world and everything?\r\n A: %d");

            List<X86FunctionLocalData> localVariables = [new("localDataOne", DataSize.Dword)];

            assemblyContext.EnterFunction(new X86Function(CallingConvention.StdCall, "Main", new(), localVariables, false, ""));
            // Setup stack frame
            assemblyContext.SetupStackFrame();

            // Function body
            assemblyContext.Mov(assemblyContext.GetIdentifierOffset("localDataOne"), 42);
            assemblyContext.Push(assemblyContext.GetIdentifierOffset("localDataOne"));
            assemblyContext.Push(messageLabel);
            assemblyContext.CallImportedFunction(printf);// CallImportedFunction automatically cleans up the stack based on the imported functions calling convention

            assemblyContext.Xor(X86Register.eax, X86Register.eax); // Trick to zero out eax register for return

            // Teardown stack frame
            assemblyContext.TeardownStackFrame();

            var error = assemblyContext.OutputToFile("test.exe");
            if (error != null) Console.WriteLine(error);

            Console.WriteLine("Done!");

        }
    }
}
```

The above is functionally the same as:

```csharp
using Assembler.Core.Constants;
using Assembler.Core;
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

        // Manually teardown stack frame
        assemblyContext.Mov(X86Register.esp, X86Register.ebp);
        assemblyContext.Pop(X86Register.ebp);
        assemblyContext.Ret();


        var error = assemblyContext.OutputToFile("test.exe");
        if (error != null) Console.WriteLine(error);

        Console.WriteLine("Done!");
    }
}
```

Example usage and output:

```plaintext
PS C:\Users\Usr\Desktop> ./test.exe
Q: What is the answer to life the world and everything?
 A: 42
```