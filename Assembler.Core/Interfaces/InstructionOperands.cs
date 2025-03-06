using Assembler.Core.Constants;
using Assembler.Core.Models;

namespace Assembler.Core.Interfaces;

public interface IRegister_Register : IRegister_Destination, IRegister_Source;
public interface IRegister_RegisterOffset : IRegister_Destination, IRegisterOffset_Source;
public interface IRegister_SymbolOffset : IRegister_Destination, ISymbolOffset_Source;
public interface IRegister_Immediate : IRegister_Destination, IImmediate_Source;
public interface IRegister_ByteImmediate : IRegister_Destination, IByteImmediate_Source;


public interface IByteRegister_ByteRegister : IByteRegister_Destination, IByteRegister_Source;
public interface IByteRegister_ByteImmediate : IByteRegister_Destination, IByteImmediate_Source;
public interface IByteRegister_RegisterOffset : IByteRegister_Destination, IRegisterOffset_Source;


public interface IRegisterOffset_Register : IRegisterOffset_Destination, IRegister_Source;
public interface IRegisterOffset_ByteRegister : IRegisterOffset_Destination, IByteRegister_Source;
public interface IRegisterOffset_XmmRegister : IRegisterOffset_Destination, IXmmRegister_Source;
public interface IRegisterOffset_Immediate : IRegisterOffset_Destination, IImmediate_Source;
public interface IRegisterOffset_ByteImmediate : IRegisterOffset_Destination, IByteImmediate_Source;


public interface ISymbolOffset_Register : ISymbolOffset_Destination, IRegister_Source;
public interface ISymbolOffset_ByteRegister : ISymbolOffset_Destination, IByteRegister_Source;
public interface ISymbolOffset_Immediate : ISymbolOffset_Destination, IImmediate_Source;


public interface IXmmRegister_RegisterOffset : IXmmRegister_Destination, IRegisterOffset_Source;
public interface IXmmRegister_XmmRegister : IXmmRegister_Destination, IXmmRegister_Source;


public interface INonAltering_XmmRegister_XmmRegister : IXmmRegister_XmmRegister, INonAltering_XmmRegister_Destination;
public interface INonAltering_XmmRegister_RegisterOffset : IXmmRegister_RegisterOffset, INonAltering_XmmRegister_Destination;
public interface INonAltering_ByteRegister_ByteRegister : IByteRegister_ByteRegister, INonAltering_ByteRegister_Destination;
public interface INonAltering_Register_Register : IRegister_Register, INonAltering_Register_Destination;
public interface INonAltering_Register_RegisterOffset : IRegister_RegisterOffset, INonAltering_Register_Destination;
public interface INonAltering_Register_Immediate : IRegister_Immediate, INonAltering_Register_Destination;


public interface IRegister_Destination
{
    public X86Register Destination { get; set; }
}

public interface IRegister_Source
{
    public X86Register Source { get; set; }
}
public interface IByteRegister_Destination
{
    public X86ByteRegister Destination { get; set; }
}

public interface IByteRegister_Source
{
    public X86ByteRegister Source { get; set; }
}
public interface IXmmRegister_Destination
{
    public XmmRegister Destination { get; set; }
}
public interface IXmmRegister_Source
{
    public XmmRegister Source { get; set; }
}
public interface IRegisterOffset_Destination
{
    public RegisterOffset Destination { get; set; }
}
public interface IRegisterOffset_Source
{
    public RegisterOffset Source { get; set; }
}
public interface ISymbolOffset_Destination
{
    public SymbolOffset Destination { get; set; }
}
public interface ISymbolOffset_Source
{
    public SymbolOffset Source { get; set; }
}

public interface IImmediate_Source
{
    public int ImmediateValue { get; set; }
}
public interface IByteImmediate_Source
{
    public byte ImmediateValue { get; set; }
}


public interface INonAltering_Register_Destination: IRegister_Destination;
public interface INonAltering_ByteRegister_Destination: IByteRegister_Destination;
public interface INonAltering_XmmRegister_Destination: IXmmRegister_Destination;
public interface INonAltering_RegisterOffset_Destination: IRegisterOffset_Destination;
public interface INonAltering_SymbolOffset_Destination: ISymbolOffset_Destination;

public interface IPush;
public interface IPop;
public interface ICall;
public interface IRet;
public interface IJmp
{
    public string Label { get; set; }
}