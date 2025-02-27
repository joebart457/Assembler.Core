



using Assembler.Core.Instructions;
using Assembler.Core.Models;

internal class Program
{
    private static void Main(string[] args)
    {
        var peFile = new PEFile();
        peFile.ImportsSection.PEFile = peFile;
        peFile.InitializedDataSection.PEFile = peFile;
        peFile.CodeSection.PEFile = peFile;

        peFile.InitializedDataSection.AddInstruction(new Label("msg"));
        peFile.InitializedDataSection.AddInstruction(new DefineByte("Hello, world!"));

        peFile.CodeSection.AddInstruction(new Label("main"));
        peFile.CodeSection.AddInstruction(new DefineByte([0x55, 0x89, 0xE5]));
        peFile.CodeSection.AddInstruction(new Push_Address("msg"));
        peFile.CodeSection.AddInstruction(new Call("printf", true));
        peFile.CodeSection.AddInstruction(new DefineByte([0x89, 0xEC, 0x5D, 0x31, 0xC0]));
        peFile.CodeSection.AddInstruction(new Ret());



        peFile.ImportsSection.AddInstruction(new DefineDoubleWord_Rva(Rva.Create("ilt")));
        peFile.ImportsSection.AddInstruction(new DefineDoubleWord(0));
        peFile.ImportsSection.AddInstruction(new DefineDoubleWord(0));
        peFile.ImportsSection.AddInstruction(new DefineDoubleWord_Rva(Rva.Create("libpath")));
        peFile.ImportsSection.AddInstruction(new DefineDoubleWord_Rva(Rva.Create("iat")));
        peFile.ImportsSection.AddInstruction(new DefineDoubleWord([0,0,0,0,0]));

        peFile.ImportsSection.AddInstruction(new Label("libpath"));
        peFile.ImportsSection.AddInstruction(new DefineByte("msvcrt.dll"));
        if (peFile.ImportsSection.VirtualSize % 2 == 1) peFile.ImportsSection.AddInstruction(new DefineByte(0));

        peFile.ImportsSection.AddInstruction(new Label("ilt"));
        peFile.ImportsSection.AddInstruction(new DefineDoubleWord_Rva(Rva.Create("int")));
        peFile.ImportsSection.AddInstruction(new DefineDoubleWord(0));

        peFile.ImportsSection.AddInstruction(new Label("iat"));
        peFile.ImportsSection.AddInstruction(new Label("printf"));
        peFile.ImportsSection.AddInstruction(new DefineDoubleWord_Rva(Rva.Create("int")));
        peFile.ImportsSection.AddInstruction(new DefineDoubleWord(0));

        peFile.ImportsSection.AddInstruction(new Label("int"));
        peFile.ImportsSection.AddInstruction(new DefineByte(0));
        peFile.ImportsSection.AddInstruction(new DefineByte(0));
        peFile.ImportsSection.AddInstruction(new DefineByte("printf"));
        peFile.ImportsSection.AddInstruction(new DefineByte(0));
        


        //peFile.ImportsSection.AddInstruction(new DefineDoubleWord_Address("lib_ilt"));
        //peFile.ImportsSection.AddInstruction(new DefineDoubleWord(0));
        //peFile.ImportsSection.AddInstruction(new DefineDoubleWord(0));
        //peFile.ImportsSection.AddInstruction(new DefineDoubleWord_Rva(Rva.Create("lib_name")));
        //peFile.ImportsSection.AddInstruction(new DefineDoubleWord_Rva(Rva.Create("lib_iat")));
        //
        //peFile.ImportsSection.AddInstruction(new DefineDoubleWord([0,0,0,0,0]));
        //
        //peFile.ImportsSection.AddInstruction(new Label("lib_name"));
        //peFile.ImportsSection.AddInstruction(new DefineByte("msvcrt.dll"));
        //if (peFile.ImportsSection.VirtualSize % 2 != 0) peFile.ImportsSection.AddInstruction(new DefineByte(0));
        //
        //peFile.ImportsSection.AddInstruction(new Label("lib_ilt"));
        //peFile.ImportsSection.AddInstruction(new DefineDoubleWord_Rva(Rva.Create("printf")));
        //
        //
        //peFile.ImportsSection.AddInstruction(new Label("lib_iat"));
        //peFile.ImportsSection.AddInstruction(new Label("printf"));
        //peFile.ImportsSection.AddInstruction(new DefineDoubleWord_Rva(Rva.Create("!printf")));
        //peFile.ImportsSection.AddInstruction(new DefineDoubleWord(0));
        //peFile.ImportsSection.AddInstruction(new Label("!printf"));
        //peFile.ImportsSection.AddInstruction(new DefineByte([0,0]));
        //peFile.ImportsSection.AddInstruction(new DefineByte("printf"));
        //if (peFile.ImportsSection.VirtualSize % 2 != 0) peFile.ImportsSection.AddInstruction(new DefineByte(0));


        var assembledBytes = peFile.AssembleProgram("main");
        File.WriteAllBytes("second.exe", assembledBytes);
    }
}