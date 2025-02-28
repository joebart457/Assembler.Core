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