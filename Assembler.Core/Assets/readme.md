# Assembler.Core
## Overview

**Assembler.Core** is a C# NuGet package designed to assist developers with generation of x86 assembly code.

## Features

- Support for the most generally used subset of x86 assembly language
- Output in FASM style syntax but pure assembly (no macros!)
- Convience methods to make assembly code generation easier

## NonFeatures
- While I do hope to create an x86 assembler in C# one day, this is not an x86 assembler and does not emit bytecode and cannot generate executable files.
- If you are looking for an x86 assembler in C# checkout Reloaded.Assembler or Fasm.NET projects.

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

            var error = assemblyContext.OutputToMemory(out var generatedAssembly);
            if (error != null) Console.WriteLine(error); 
            
            Console.WriteLine(generatedAssembly);

        }
    }
}
```

The above is functionally the same as:

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
            

            var error = assemblyContext.OutputToMemory(out var generatedAssembly);
            if (error != null) Console.WriteLine(error); 
            
            Console.WriteLine(generatedAssembly);

        }
    }
}
```

Example output:


```assembly
format PE console
entry _Main@0
section '.data' data readable writeable
        !L0 db 0x51,0x3A,0x20,0x57,0x68,0x61,0x74,0x20,0x69,0x73
                0x51,0x3A,0x20,0x57,0x68,0x61,0x74,0x20,0x69,0x73
                0x20,0x74,0x68,0x65,0x20,0x61,0x6E,0x73,0x77,0x65
                0x72,0x20,0x74,0x6F,0x20,0x6C,0x69,0x66,0x65,0x20
                0x74,0x68,0x65,0x20,0x77,0x6F,0x72,0x6C,0x64,0x20
                0x61,0x6E,0x64,0x20,0x65,0x76,0x65,0x72,0x79,0x74
                0x68,0x69,0x6E,0x67,0x3F,0x0D,0x0A,0x20,0x41,0x3A
                0x20,0x25,0x64,0x00
                0x20,0x25,0x64

section '.text' code readable executable
                _Main@0:
                        push ebp
                        mov ebp, esp
                        sub esp, 4
                        mov dword [ebp - 8], 42
                        push dword [ebp - 8]
                        push !L0
                        call dword [printf]
                        add esp, 8
                        xor eax, eax
                        mov esp, ebp
                        pop ebp
                        ret
section '.idata' import data readable writeable
        dd !lib_0_ilt,0,0,RVA !lib_0_name, RVA !lib_0_iat
        dd 0,0,0,0,0
        !lib_0_name db 'msvcrt.dll',0
        rb RVA $ and 1
        rb(-rva $) and 3
        !lib_0_ilt:
        dd RVA !printf
        dd 0
        !lib_0_iat:
        printf dd RVA !printf
        dd 0
        !printf dw 0
        db 'printf',0
```