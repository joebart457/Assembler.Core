using Assembler.Core.Constants;
using Assembler.Core.Extensions;
using Assembler.Core.Instructions;
using Assembler.Core.Models;
using Language.Experimental.Compiler.Instructions;
using System.Data.SqlTypes;
using System.Runtime.InteropServices;
using System.Text;


namespace Assembler.Core;

public class X86AssemblyContext
{
    private X86Function? _currentFunctionTarget;
    private int _uniqueLabelIndex = 0;
    private string? _exportFileName;

    private Stack<(string continueLabel, string breakLabel)> _loopLabels = new Stack<(string continueLabel, string breakLabel)>();
    private List<StringData> _stringData = new();
    private List<SinglePrecisionFloatingPointData> _floatingPointData = new();
    private List<IntegerData> _integerData = new();
    private List<ByteData> _byteData = new();
    private List<PointerData> _pointerData = new();
    private List<UnitializedData> _unitializedData = new();
    public int PointerSize => 4;
    public List<StringData> StaticStringData => _stringData;
    public List<SinglePrecisionFloatingPointData> StaticFloatingPointData => _floatingPointData;
    public List<IntegerData> StaticIntegerData => _integerData;
    public List<ByteData> StaticByteData => _byteData;
    public List<PointerData> StaticPointerData => _pointerData;
    public List<UnitializedData> StaticUnitializedData => _unitializedData;
    public IconData? ProgramIcon { get; set; }
    public List<X86Function> FunctionData { get; private set; } = new();
    public List<ImportLibrary> ImportLibraries { get; private set; } = new();
    public OutputTarget OutputTarget { get; set; }
    public X86Function? EntryPoint { get; set; }
    public X86Function GetEntryPoint() => EntryPoint ?? throw new InvalidOperationException("entry point has not been defined");
    public string GetExportFileName() => _exportFileName ?? throw new InvalidOperationException("export filepath was not defined");
    public string? OutputToFile(string outputFilePath) => X86AssemblyGenerator.OutputToFile(this, outputFilePath);
    public string? OutputToMemory(out StringBuilder generatedX86Code) => X86AssemblyGenerator.OutputToMemory(this, out generatedX86Code);
    public void SetOutputTarget(OutputTarget target)
    {
        OutputTarget = target;
    }

    /// <summary>
    /// Sets the name of the resulting dll.
    /// Used only when OutputTarget == OutputTarget.Dll and at least one exported function.
    /// </summary>
    /// <param name="filePath"></param>
    public void SetExportFileName(string fileName)
    {
        _exportFileName = fileName;
    }

    public X86Function SetEntryPoint(X86Function function)
    {
        EntryPoint = function;
        return function;
    }

    public X86Function SetEntryPoint(string name)
    {
        var foundFunction  = FunctionData.Find(x => x.FunctionLabel == name || x.GetDecoratedFunctionLabel() == name);
        if (foundFunction == null) throw new InvalidOperationException($"function corresponding to entry point {name} has not been defined");

        return SetEntryPoint(foundFunction);
    }

    public List<(string functionIdentifier, string exportedSymbol)> ExportedFunctions => FunctionData.Where(x => x.IsExported)
        .OrderBy(x => x.ExportedSymbol, StringComparer.Ordinal) // Must be exported in ordinal order
        .Select(x => (x.GetDecoratedFunctionLabel(), x.ExportedSymbol))
        .ToList();
    public ImportedFunction AddImportedFunction(ImportedFunction importedFunction)
    {
        var foundLibrary = ImportLibraries.Find(x => x.LibraryAlias == importedFunction.LibraryAlias);
        if (foundLibrary == null) throw new Exception($"import library with alias {importedFunction.LibraryAlias} is not defined");
        foundLibrary.AddImportedFunction(importedFunction);
        return importedFunction;
    }

    public void AddImportLibrary(ImportLibrary library)
    {
        var foundLibrary = ImportLibraries.Find(x => x.LibraryAlias == library.LibraryAlias);
        if (foundLibrary != null) throw new Exception($"import library with alias {library.LibraryAlias} is already defined");
        ImportLibraries.Add(library);
    }


    public void AddInstruction(X86Instruction x86Instruction)
    {
        if (_currentFunctionTarget == null) throw new InvalidOperationException();
        _currentFunctionTarget.AddInstruction(x86Instruction);
    }

    public string AddStringData(string value, string? symbol = null)
    {
        var label = !string.IsNullOrWhiteSpace(symbol) ? Decorate(symbol) : CreateUniqueLabel();
        _stringData.Add(new(label, value));
        return label;
    }

    public string AddSinglePrecisionFloatingPointData(float value, string? symbol = null)
    {
        var label = !string.IsNullOrWhiteSpace(symbol) ? Decorate(symbol) : CreateUniqueLabel();
        _floatingPointData.Add(new(label, value));
        return label;
    }

    public string AddIntegerData(int value, string? symbol = null)
    {
        var label = !string.IsNullOrWhiteSpace(symbol) ? Decorate(symbol) : CreateUniqueLabel();
        _integerData.Add(new(label, value));
        return label;
    }

    public string AddByteData(byte value, string? symbol = null)
    {
        var label = !string.IsNullOrWhiteSpace(symbol) ? Decorate(symbol) : CreateUniqueLabel();
        _byteData.Add(new(label, value));
        return label;
    }

    public string AddPointerData(int value, string? symbol = null)
    {
        var label = !string.IsNullOrWhiteSpace(symbol) ? Decorate(symbol) : CreateUniqueLabel();
        _pointerData.Add(new(label, value));
        return label;
    }

    public string AddUninitializedData(int bytesToReserve, string? symbol = null)
    {
        var label = !string.IsNullOrWhiteSpace(symbol) ? Decorate(symbol) : CreateUniqueLabel();
        _unitializedData.Add(new(label, bytesToReserve));
        return label;
    }

    public RegisterOffset GetIdentifierOffset(string identifier)
    {
        var foundParameterIndex = CurrentFunction.Parameters.FindIndex(x => x.Alias == identifier);
        if (foundParameterIndex != -1) return new RegisterOffset(X86Register.ebp, 8 + (foundParameterIndex * 4));
        var foundLocalVariableIndex = CurrentFunction.LocalData.FindIndex(x => x.Alias == identifier);
        if (foundLocalVariableIndex != -1) return new RegisterOffset(X86Register.ebp, -4 - CurrentFunction.LocalData.Take(foundLocalVariableIndex + 1).Sum(x => x.StackSize));
        throw new Exception($"local variable {identifier} does not exist");
    }

    public RegisterOffset GetIdentifierOffset(string identifier, out bool isParameterOffset)
    {
        isParameterOffset = true;
        var foundParameterIndex = CurrentFunction.Parameters.FindIndex(x => x.Alias == identifier);
        if (foundParameterIndex != -1) return new RegisterOffset(X86Register.ebp, 8 + (foundParameterIndex * 4));
        isParameterOffset = false;
        var foundLocalVariableIndex = CurrentFunction.LocalData.FindIndex(x => x.Alias == identifier);
        if (foundLocalVariableIndex != -1) return new RegisterOffset(X86Register.ebp, -4 - CurrentFunction.LocalData.Take(foundLocalVariableIndex + 1).Sum(x => x.StackSize));
        throw new Exception($"local variable {identifier} does not exist");
    }

    public SymbolOffset GetGlobalOffset(string identifier)
    {
        return Offset.CreateSymbolOffset(Decorate(identifier), 0);
    }

    public void EnterFunction(X86Function function)
    {
        if (_currentFunctionTarget != null) throw new InvalidOperationException();
        _currentFunctionTarget = function;
        FunctionData.Add(_currentFunctionTarget);
    }

    public void ExitFunction()
    {
        if (_currentFunctionTarget == null) throw new InvalidOperationException();
        _currentFunctionTarget = null;
    }

    public void DiscardFunction()
    {
        FunctionData.Remove(CurrentFunction);
    }

    public X86Function CurrentFunction => _currentFunctionTarget ?? throw new InvalidOperationException("CurrentFunction was null");

    private string CreateUniqueLabel()
    {
        return $"!L{_uniqueLabelIndex++}";
    }

    private string Decorate(string symbol)
    {
        return $"!SYM_{symbol}";
    }

    public string EnterLoop(string continueLabel)
    {
        var breakLabel = CreateUniqueLabel();
        _loopLabels.Push((continueLabel, breakLabel));
        return breakLabel;
    }

    public void ExitLoop()
    {
        _loopLabels.Pop();
    }


    public string GetLoopBreakLabel()
    {
        if (_loopLabels.Count == 0) throw new InvalidOperationException();
        return _loopLabels.Peek().breakLabel;
    }

    public string GetLoopContinueLabel()
    {
        if (_loopLabels.Count == 0) throw new InvalidOperationException();
        return _loopLabels.Peek().continueLabel;
    }

    public void SetProgramIcon(string iconFilePath)
    {
        if (ProgramIcon != null) throw new InvalidOperationException("program icon must only be defined once");
        ProgramIcon = new IconData(iconFilePath);
    }


    #region Instructions

    public void SetupStackFrame()
    {
        Push(X86Register.ebp);
        Mov(X86Register.ebp, X86Register.esp);
        if (CurrentFunction.Parameters.Any())
            Sub(X86Register.esp, CurrentFunction.Parameters.Sum(x => x.StackSize));
    }

    public void TeardownStackFrame()
    {
        Mov(X86Register.esp, X86Register.ebp);
        Pop(X86Register.ebp);
    }

    public void Return()
    {
        if (CurrentFunction.CallingConvention == CallingConvention.StdCall)
            Ret(CurrentFunction.Parameters.Sum(x => x.StackSize));
        else if (CurrentFunction.CallingConvention == CallingConvention.Cdecl)
            Ret();
        else throw new NotImplementedException($"calling convention {CurrentFunction.CallingConvention} has not been implemented");
    }

    public void CallImportedFunction(string functionIdentifier)
    {
        var foundFunction = ImportLibraries.SelectMany(x => x.ImportedFunctions).FirstOrDefault(x => x.FunctionIdentifier == functionIdentifier);
        if (foundFunction == null) throw new InvalidOperationException($"definition for imported function {functionIdentifier} was not provided");
        CallImportedFunction(foundFunction);
    }

    public void CallImportedFunction(ImportedFunction importedFunction)
    {
        Call(importedFunction.FunctionIdentifier, true);
        if (importedFunction.CallingConvention == CallingConvention.StdCall)
            Ret(importedFunction.Parameters.Sum(x => x.StackSize));
        else if (importedFunction.CallingConvention == CallingConvention.Cdecl)
            Ret();
        else throw new NotImplementedException($"calling convention {importedFunction.CallingConvention} has not been implemented");
    }

    public Cdq Cdq() => CurrentFunction.AddInstruction(new Cdq());

    public  Push_Register Push(X86Register register) => CurrentFunction.AddInstruction(new Push_Register(register));
    public  Push_Offset Push(RegisterOffset offset) => CurrentFunction.AddInstruction(new Push_Offset(offset));
    public  Push_Address Push(string address) => CurrentFunction.AddInstruction(new Push_Address(address));
    public  Push_Immediate<int> Push(int immediateValue) => CurrentFunction.AddInstruction(new Push_Immediate<int>(immediateValue));
    public Push_Immediate<float> Push(float immediateValue) => CurrentFunction.AddInstruction(new Push_Immediate<float>(immediateValue));
    public  Push_SymbolOffset Push(SymbolOffset offset) => CurrentFunction.AddInstruction(new Push_SymbolOffset(offset));

    public  Lea_Register_Offset Lea(X86Register destination, RegisterOffset source) => CurrentFunction.AddInstruction(new Lea_Register_Offset(destination, source));
    public  Lea_Register_SymbolOffset Lea(X86Register destination, SymbolOffset source) => CurrentFunction.AddInstruction(new Lea_Register_SymbolOffset(destination, source));

    public  Mov_Register_Offset Mov(X86Register destination, RegisterOffset source) => CurrentFunction.AddInstruction(new Mov_Register_Offset(destination, source));
    public  Mov_Offset_Register Mov(RegisterOffset destination, X86Register source) => CurrentFunction.AddInstruction(new Mov_Offset_Register(destination, source));
    public  Mov_Offset_Immediate Mov(RegisterOffset destination, int immediate) => CurrentFunction.AddInstruction(new Mov_Offset_Immediate(destination, immediate));
    public  Mov_Register_Register Mov(X86Register destination, X86Register source) => CurrentFunction.AddInstruction(new Mov_Register_Register(destination, source));
    public  Mov_Register_Immediate Mov(X86Register destination, int immediate) => CurrentFunction.AddInstruction(new Mov_Register_Immediate(destination, immediate));

    public  Mov_SymbolOffset_Register Mov(SymbolOffset destination, X86Register source) => CurrentFunction.AddInstruction(new Mov_SymbolOffset_Register(destination, source));
    public  Mov_SymbolOffset_Register__Byte Mov(SymbolOffset destination, X86ByteRegister source) => CurrentFunction.AddInstruction(new Mov_SymbolOffset_Register__Byte(destination, source));
    public  Mov_SymbolOffset_Immediate Mov(SymbolOffset destination, int immediateValue) => CurrentFunction.AddInstruction(new Mov_SymbolOffset_Immediate(destination, immediateValue));

    public  Mov_SymbolOffset_Byte_Register__Byte Mov(SymbolOffset_Byte destination, X86ByteRegister source) => CurrentFunction.AddInstruction(new Mov_SymbolOffset_Byte_Register__Byte(destination, source));
    public  Mov_RegisterOffset_Byte_Register__Byte Mov(RegisterOffset_Byte destination, X86ByteRegister source) => CurrentFunction.AddInstruction(new Mov_RegisterOffset_Byte_Register__Byte(destination, source));

    public  Mov_Offset_Register__Byte Mov(RegisterOffset destination, X86ByteRegister source) => CurrentFunction.AddInstruction(new Mov_Offset_Register__Byte(destination, source));
    public  Movsx_Register_Offset Movsx(X86Register destination, RegisterOffset_Byte source) => CurrentFunction.AddInstruction(new Movsx_Register_Offset(destination, source));
    public  Movsx_Register_SymbolOffset__Byte Movsx(X86Register destination, SymbolOffset_Byte source) => CurrentFunction.AddInstruction(new Movsx_Register_SymbolOffset__Byte(destination, source));

    public  Sub_Register_Immediate Sub(X86Register destination, int valueToSubtract) => CurrentFunction.AddInstruction(new Sub_Register_Immediate(destination, valueToSubtract));
    public  Sub_Register_Register Sub(X86Register destination, X86Register source) => CurrentFunction.AddInstruction(new Sub_Register_Register(destination, source));

    public  Add_Register_Immediate Add(X86Register destination, int value) => CurrentFunction.AddInstruction(new Add_Register_Immediate(destination, value));
    public  Add_Register_Register Add(X86Register destination, X86Register source) => CurrentFunction.AddInstruction(new Add_Register_Register(destination, source));


    public  And_Register_Register And(X86Register destination, X86Register source) => CurrentFunction.AddInstruction(new And_Register_Register(destination, source));
    public  Or_Register_Register Or(X86Register destination, X86Register source) => CurrentFunction.AddInstruction(new Or_Register_Register(destination, source));
    public  Xor_Register_Register Xor(X86Register destination, X86Register source) => CurrentFunction.AddInstruction(new Xor_Register_Register(destination, source));


    public  Pop_Register Pop(X86Register destination) => CurrentFunction.AddInstruction(new Pop_Register(destination));

    public  Neg_Offset Neg(RegisterOffset destination) => CurrentFunction.AddInstruction(new Neg_Offset(destination));
    public  Not_Offset Not(RegisterOffset destination) => CurrentFunction.AddInstruction(new Not_Offset(destination));

    public  Inc_Register Inc(X86Register destination) => CurrentFunction.AddInstruction(new Inc_Register(destination));
    public  Dec_Register Dec(X86Register destination) => CurrentFunction.AddInstruction(new Dec_Register(destination));
    public  Inc_Offset Inc(RegisterOffset destination) => CurrentFunction.AddInstruction(new Inc_Offset(destination));
    public  Dec_Offset Dec(RegisterOffset destination) => CurrentFunction.AddInstruction(new Dec_Offset(destination));

    public  IDiv_Offset IDiv(RegisterOffset divisor) => CurrentFunction.AddInstruction(new IDiv_Offset(divisor));
    public  IMul_Register_Register IMul(X86Register destination, X86Register source) => CurrentFunction.AddInstruction(new IMul_Register_Register(destination, source));
    public  IMul_Register_Immediate IMul(X86Register destination, int immediate) => CurrentFunction.AddInstruction(new IMul_Register_Immediate(destination, immediate));
    public  Add_Register_Offset Add(X86Register destination, RegisterOffset source) => CurrentFunction.AddInstruction(new Add_Register_Offset(destination, source));


    public  Jmp Jmp(string label) => CurrentFunction.AddInstruction(new Jmp(label));
    public  JmpGt JmpGt(string label) => CurrentFunction.AddInstruction(new JmpGt(label));
    public  JmpGte JmpGte(string label) => CurrentFunction.AddInstruction(new JmpGte(label));
    public  JmpLt JmpLt(string label) => CurrentFunction.AddInstruction(new JmpLt(label));
    public  JmpLte JmpLte(string label) => CurrentFunction.AddInstruction(new JmpLte(label));
    public  JmpEq JmpEq(string label) => CurrentFunction.AddInstruction(new JmpEq(label));
    public  JmpNeq JmpNeq(string label) => CurrentFunction.AddInstruction(new JmpNeq(label));
    public  Jz Jz(string label) => CurrentFunction.AddInstruction(new Jz(label));
    public  Jnz Jnz(string label) => CurrentFunction.AddInstruction(new Jnz(label));
    public  Js Js(string label) => CurrentFunction.AddInstruction(new Js(label));
    public  Jns Jns(string label) => CurrentFunction.AddInstruction(new Jns(label));
    public  Ja Ja(string label) => CurrentFunction.AddInstruction(new Ja(label));
    public  Jae Jae(string label) => CurrentFunction.AddInstruction(new Jae(label));
    public  Jb Jb(string label) => CurrentFunction.AddInstruction(new Jb(label));
    public  Jbe Jbe(string label) => CurrentFunction.AddInstruction(new Jbe(label));

    public  Test_Register_Register Test(X86Register operand1, X86Register operand2) => CurrentFunction.AddInstruction(new Test_Register_Register(operand1, operand2));
    public  Test_Register_Offset Test(X86Register operand1, RegisterOffset operand2) => CurrentFunction.AddInstruction(new Test_Register_Offset(operand1, operand2));
    public  Cmp_Register_Register Cmp(X86Register operand1, X86Register operand2) => CurrentFunction.AddInstruction(new Cmp_Register_Register(operand1, operand2));
    public  Cmp_Register_Immediate Cmp(X86Register operand1, int operand2) => CurrentFunction.AddInstruction(new Cmp_Register_Immediate(operand1, operand2));
    public  Cmp_Byte_Byte Cmp(X86ByteRegister operand1, X86ByteRegister operand2) => CurrentFunction.AddInstruction(new Cmp_Byte_Byte(operand1, operand2));

    public  Call Call(string callee, bool isIndirect) => CurrentFunction.AddInstruction(new Call(callee, isIndirect));
    public  Call_RegisterOffset Call(RegisterOffset offset) => CurrentFunction.AddInstruction(new Call_RegisterOffset(offset));
    public  Call_Register Call(X86Register register) => CurrentFunction.AddInstruction(new Call_Register(register));
    public  Label Label(string text) => CurrentFunction.AddInstruction(new Label(text));
    public  Ret Ret() => CurrentFunction.AddInstruction(new Ret());
    public  Ret_Immediate Ret(int immediate) => CurrentFunction.AddInstruction(new Ret_Immediate(immediate));

    public  Fstp_Offset Fstp(RegisterOffset destination) => CurrentFunction.AddInstruction(new Fstp_Offset(destination));
    public  Fld_Offset Fld(RegisterOffset source) => CurrentFunction.AddInstruction(new Fld_Offset(source));

    public  Movss_Offset_Register Movss(RegisterOffset destination, XmmRegister source) => CurrentFunction.AddInstruction(new Movss_Offset_Register(destination, source));
    public  Movss_Register_Offset Movss(XmmRegister destination, RegisterOffset source) => CurrentFunction.AddInstruction(new Movss_Register_Offset(destination, source));
    public  Movss_Register_Register Movss(XmmRegister destination, XmmRegister source) => CurrentFunction.AddInstruction(new Movss_Register_Register(destination, source));
    public  Comiss_Register_Offset Comiss(XmmRegister destination, RegisterOffset source) => CurrentFunction.AddInstruction(new Comiss_Register_Offset(destination, source));
    public  Comiss_Register_Register Comiss(XmmRegister destination, XmmRegister source) => CurrentFunction.AddInstruction(new Comiss_Register_Register(destination, source));
    public  Ucomiss_Register_Register Ucomiss(XmmRegister destination, XmmRegister source) => CurrentFunction.AddInstruction(new Ucomiss_Register_Register(destination, source));
    public  Addss_Register_Offset Addss(XmmRegister destination, RegisterOffset source) => CurrentFunction.AddInstruction(new Addss_Register_Offset(destination, source));
    public  Subss_Register_Offset Subss(XmmRegister destination, RegisterOffset source) => CurrentFunction.AddInstruction(new Subss_Register_Offset(destination, source));
    public  Mulss_Register_Offset Mulss(XmmRegister destination, RegisterOffset source) => CurrentFunction.AddInstruction(new Mulss_Register_Offset(destination, source));
    public  Divss_Register_Offset Divss(XmmRegister destination, RegisterOffset source) => CurrentFunction.AddInstruction(new Divss_Register_Offset(destination, source));
    public  Cvtsi2ss_Register_Offset Cvtsi2ss(XmmRegister destination, RegisterOffset source) => CurrentFunction.AddInstruction(new Cvtsi2ss_Register_Offset(destination, source));
    public  Cvtss2si_Register_Offset Cvtss2si(X86Register destination, RegisterOffset source) => CurrentFunction.AddInstruction(new Cvtss2si_Register_Offset(destination, source));



    #endregion


    #region DataDefinitions

    public class ImportedFunction
    {
        public CallingConvention CallingConvention { get; set; }
        public string LibraryAlias { get; set; }
        public string Symbol { get; set; } // The symbol to import 
        public string FunctionIdentifier { get; set; } // the alias to be used to reference the imported symbol
        public List<X86FunctionLocalData> Parameters { get; set; }
        public ImportedFunction(CallingConvention callingConvention, string libraryAlias, string symbol, string functionIdentifier, List<X86FunctionLocalData> parameters)
        {
            CallingConvention = callingConvention;
            LibraryAlias = libraryAlias;
            Symbol = symbol;
            FunctionIdentifier = functionIdentifier;
            Parameters = parameters;
        }
    }

    public class ImportLibrary
    {
        public string LibraryPath { get; set; }
        public string LibraryAlias { get; set; }
        public List<ImportedFunction> ImportedFunctions { get; set; } = new();
        public ImportLibrary(string libraryPath, string libraryAlias)
        {
            LibraryPath = libraryPath;
            LibraryAlias = libraryAlias;
        }

        public void AddImportedFunction(ImportedFunction importedFunction)
        {
            if (ImportedFunctions.Any(x => x.FunctionIdentifier == importedFunction.FunctionIdentifier))
                throw new Exception($"symbol with name {importedFunction.FunctionIdentifier} is already imported");
            ImportedFunctions.Add(importedFunction);
        }
    }

    public class StringData
    {
        public string Label { get; set; }
        public string Value { get; set; }
        public StringData(string label, string value)
        {
            Label = label;
            Value = value;
        }

        public string Emit(int indentLevel)
        {
            return $"{Label} db {EscapeString(Value, indentLevel + 1)}".Indent(indentLevel);
        }

        private static string EscapeString(string value, int indentLevel)
        {
            if (value.Length == 0) return "0x00";
            var bytes = Encoding.UTF8.GetBytes(value);
            if (bytes.Length <= 10) 
                return $"0x{BitConverter.ToString(bytes).Replace("-", ",0x")},0x00";
            var byteChunks = bytes.Chunk(10).ToList();
            var sb = new StringBuilder();
            foreach(var chunk in byteChunks)
            {
                var strBytes = BitConverter.ToString(chunk);
                if (chunk == byteChunks.First()) sb.AppendLine($"0x{strBytes.Replace("-", ",0x")}");    // no indent
                if (chunk == byteChunks.Last()) sb.AppendLine($"0x{strBytes.Replace("-", ",0x")},0x00".Indent(indentLevel)); // null terminated
                sb.AppendLine($"0x{strBytes.Replace("-", ",0x")}".Indent(indentLevel));
            }
            return sb.ToString();
        }
    }

    public class SinglePrecisionFloatingPointData
    {
        public string Label { get; set; }
        public float Value { get; set; }
        public SinglePrecisionFloatingPointData(string label, float value)
        {
            Label = label;
            Value = value;
        }

        public string Emit(int indentLevel)
        {
            return $"{Label} db {ToBytes(Value)}".Indent(indentLevel);
        }

        private static string ToBytes(float value)
        {
            var bytes = BitConverter.GetBytes(value);
            var strBytes = BitConverter.ToString(bytes);
            return $"0x{strBytes.Replace("-", ",0x")}";
        }
    }

    public class IntegerData
    {
        public string Label { get; set; }
        public int Value { get; set; }
        public IntegerData(string label, int value)
        {
            Label = label;
            Value = value;
        }

        public string Emit(int indentLevel)
        {
            return $"{Label} dd {Value}".Indent(indentLevel);
        }
    }

    public class PointerData
    {
        public string Label { get; set; }
        public int Value { get; set; }
        public PointerData(string label, int value)
        {
            Label = label;
            Value = value;
        }

        public string Emit(int indentLevel)
        {
            return $"{Label} dd {Value}".Indent(indentLevel);
        }
    }

    public class ByteData
    {
        public string Label { get; set; }
        public byte Value { get; set; }
        public ByteData(string label, byte value)
        {
            Label = label;
            Value = value;
        }

        public string Emit(int indentLevel)
        {
            return $"{Label} db {EscapeByte(Value)}".Indent(indentLevel);
        }

        private static string EscapeByte(byte value)
        {
            var strBytes = BitConverter.ToString([value]);
            return $"0x{strBytes.Replace("-", ",0x")}";
        }
    }

    public class UnitializedData
    {
        public string Label { get; set; }
        public int BytesToReserve { get; set; }
        public UnitializedData(string label, int bytesToReserve)
        {
            Label = label;
            BytesToReserve = bytesToReserve;
        }

        public string Emit(int indentLevel)
        {
            return $"{Label} rb {BytesToReserve}".Indent(indentLevel);
        }
    }

    public class IconData
    {
        public string FilePath { get; set; }

        public IconData(string filePath)
        {
            FilePath = filePath;
        }
    }


    #endregion

}