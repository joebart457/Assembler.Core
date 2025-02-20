using Assembler.Core.Constants;
using Assembler.Core.Extensions;
using Assembler.Core.Models;
using System.Text;


namespace Assembler.Core;

public class X86AssemblyContext
{
    private X86Function? _currentFunctionTarget;
    private int _uniqueLabelIndex = 0;

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
    public void SetOutputTarget(OutputTarget target)
    {
        OutputTarget = target;
    }

    public void SetEntryPoint(X86Function function)
    {
        EntryPoint = function;
    }

    public X86Function SetEntryPoint(string name)
    {
        var foundFunction  = FunctionData.Find(x => x.FunctionLabel == name || x.GetDecoratedFunctionLabel() == name);
        if (foundFunction == null) throw new InvalidOperationException($"function corresponding to entry point {name} has not been defined");
        return foundFunction;
    }

    public List<(string functionIdentifier, string exportedSymbol)> ExportedFunctions => FunctionData.Where(x => x.IsExported)
        .OrderBy(x => x.ExportedSymbol, StringComparer.Ordinal) // Must be exported in ordinal order
        .Select(x => (x.GetDecoratedFunctionLabel(), x.ExportedSymbol))
        .ToList();
    public void AddImportedFunction(ImportedFunction importedFunction)
    {
        var foundLibrary = ImportLibraries.Find(x => x.LibraryAlias == importedFunction.LibraryAlias);
        if (foundLibrary == null) throw new Exception($"import library with alias {importedFunction.LibraryAlias} is not defined");
        foundLibrary.AddImportedFunction(importedFunction);
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
    }

    public void ExitFunction()
    {
        if (_currentFunctionTarget == null) throw new InvalidOperationException();
        FunctionData.Add(_currentFunctionTarget);
        _currentFunctionTarget = null;
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

    #region DataDefinitions

    public class ImportedFunction
    { 
        public string LibraryAlias { get; set; }
        public string Symbol { get; set; } // The symbol to import 
        public string FunctionIdentifier { get; set; } // the alias to be used to reference the imported symbol
        public ImportedFunction(string libraryAlias, string symbol, string functionIdentifier)
        {
            LibraryAlias = libraryAlias;
            Symbol = symbol;
            FunctionIdentifier = functionIdentifier;
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
            return $"{Label} db {EscapeString(Value)},0".Indent(indentLevel);
        }

        private static string EscapeString(string value)
        {
            if (value.Length == 0) return "0";
            var bytes = Encoding.UTF8.GetBytes(value);
            var strBytes = BitConverter.ToString(bytes);
            return $"0x{strBytes.Replace("-", ",0x")}";
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