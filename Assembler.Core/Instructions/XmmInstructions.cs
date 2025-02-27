using Assembler.Core.Constants;
using Assembler.Core.Extensions;
using Assembler.Core.Models;
using Assembler.Core.PortableExecutable;
namespace Assembler.Core.Instructions;

public class Movss_Offset_Register : X86Instruction
{
    public RegisterOffset Destination { get; set; }
    public XmmRegister Source { get; set; }
    public Movss_Offset_Register(RegisterOffset destination, XmmRegister source)
    {
        Destination = destination;
        Source = source;
    }

    public override string Emit()
    {
        return $"movss {Destination}, {Source}";
    }

    public override byte[] Assemble(Section section, Dictionary<string, Address> resolvedLabels)
    {
        byte[] opCodes = [0xF3, 0x0F, 0x11];
        return opCodes.Concat(Destination.EncodeAsRM(Source)).ToArray();
    }

    public override uint GetSizeOnDisk() => 3 + (uint)Destination.EncodeAsRM(Source).Length;
    public override uint GetVirtualSize() => 3 + (uint)Destination.EncodeAsRM(Source).Length;
}

public class Movss_Register_Offset : X86Instruction
{
    public XmmRegister Destination { get; set; }
    public RegisterOffset Source { get; set; }
    public Movss_Register_Offset(XmmRegister destination, RegisterOffset source)
    {
        Destination = destination;
        Source = source;
    }

    public override string Emit()
    {
        return $"movss {Destination}, {Source}";
    }

    public override byte[] Assemble(Section section, Dictionary<string, Address> resolvedLabels)
    {
        byte[] opCodes = [0xF3, 0x0F, 0x11];
        return opCodes.Concat(Source.EncodeAsRM(Destination)).ToArray();
    }

    public override uint GetSizeOnDisk() => 3 + (uint)Source.EncodeAsRM(Destination).Length;
    public override uint GetVirtualSize() => 3 + (uint)Source.EncodeAsRM(Destination).Length;
}

public class Movss_Register_Register : X86Instruction
{
    public XmmRegister Destination { get; set; }
    public XmmRegister Source { get; set; }
    public Movss_Register_Register(XmmRegister destination, XmmRegister source)
    {
        Destination = destination;
        Source = source;
    }

    public override string Emit()
    {
        return $"movss {Destination}, {Source}";
    }

    public override byte[] Assemble(Section section, Dictionary<string, Address> resolvedLabels)
    {
        byte[] opCodes = [0xF3, 0x0F, 0x10];
        var modRM = Mod.RegisterDirect.ApplyOperand1(Source).ApplyOperand2(Destination);
        return opCodes.Append(modRM).ToArray();
    }

    public override uint GetSizeOnDisk() => 4;
    public override uint GetVirtualSize() => 4;
}


public class Comiss_Register_Offset : X86Instruction
{
    public XmmRegister Operand1 { get; set; }
    public RegisterOffset Operand2 { get; set; }
    public Comiss_Register_Offset(XmmRegister operand1, RegisterOffset operand2)
    {
        Operand1 = operand1;
        Operand2 = operand2;
    }

    public override string Emit()
    {
        return $"comiss {Operand1}, {Operand2}";
    }
    public override byte[] Assemble(Section section, Dictionary<string, Address> resolvedLabels)
    {
        byte[] opCodes = [0x0F, 0x2F];
        return opCodes.Concat(Operand2.EncodeAsRM(Operand1)).ToArray();
    }

    public override uint GetSizeOnDisk() => 2 + (uint)Operand2.EncodeAsRM(Operand1).Length;
    public override uint GetVirtualSize() => 2 + (uint)Operand2.EncodeAsRM(Operand1).Length;
}

public class Comiss_Register_Register : X86Instruction
{
    public XmmRegister Operand1 { get; set; }
    public XmmRegister Operand2 { get; set; }
    public Comiss_Register_Register(XmmRegister operand1, XmmRegister operand2)
    {
        Operand1 = operand1;
        Operand2 = operand2;
    }

    public override string Emit()
    {
        return $"comiss {Operand1}, {Operand2}";
    }

    public override byte[] Assemble(Section section, Dictionary<string, Address> resolvedLabels)
    {
        byte[] opCodes = [0x0F, 0x2F];
        var modRM = Mod.RegisterDirect.ApplyOperand1(Operand1).ApplyOperand2(Operand2);
        return opCodes.Append(modRM).ToArray();
    }

    public override uint GetSizeOnDisk() => 3;
    public override uint GetVirtualSize() => 3;
}

public class Ucomiss_Register_Register : X86Instruction
{
    public XmmRegister Operand1 { get; set; }
    public XmmRegister Operand2 { get; set; }
    public Ucomiss_Register_Register(XmmRegister operand1, XmmRegister operand2)
    {
        Operand1 = operand1;
        Operand2 = operand2;
    }

    public override string Emit()
    {
        return $"ucomiss {Operand1}, {Operand2}";
    }

    public override byte[] Assemble(Section section, Dictionary<string, Address> resolvedLabels)
    {
        byte[] opCodes = [0x0F, 0x2E];
        var modRM = Mod.RegisterDirect.ApplyOperand1(Operand1).ApplyOperand2(Operand2);
        return opCodes.Append(modRM).ToArray();
    }

    public override uint GetSizeOnDisk() => 3;
    public override uint GetVirtualSize() => 3;
}

public class Addss_Register_Offset : X86Instruction
{
    public XmmRegister Destination { get; set; }
    public RegisterOffset Source { get; set; }
    public Addss_Register_Offset(XmmRegister destination, RegisterOffset source)
    {
        Destination = destination;
        Source = source;
    }

    public override string Emit()
    {
        return $"addss {Destination}, {Source}";
    }

    public override byte[] Assemble(Section section, Dictionary<string, Address> resolvedLabels)
    {
        byte[] opCodes = [0xF3, 0x0F, 0x58];
        return opCodes.Concat(Source.EncodeAsRM(Destination)).ToArray();
    }

    public override uint GetSizeOnDisk() => 3 + (uint)Source.EncodeAsRM(Destination).Length;
    public override uint GetVirtualSize() => 3 + (uint)Source.EncodeAsRM(Destination).Length;
}

public class Subss_Register_Offset : X86Instruction
{
    public XmmRegister Destination { get; set; }
    public RegisterOffset Source { get; set; }
    public Subss_Register_Offset(XmmRegister destination, RegisterOffset source)
    {
        Destination = destination;
        Source = source;
    }

    public override string Emit()
    {
        return $"subss {Destination}, {Source}";
    }

    public override byte[] Assemble(Section section, Dictionary<string, Address> resolvedLabels)
    {
        byte[] opCodes = [0xF3, 0x0F, 0x5C];
        return opCodes.Concat(Source.EncodeAsRM(Destination)).ToArray();
    }

    public override uint GetSizeOnDisk() => 3 + (uint)Source.EncodeAsRM(Destination).Length;
    public override uint GetVirtualSize() => 3 + (uint)Source.EncodeAsRM(Destination).Length;
}

public class Divss_Register_Offset : X86Instruction
{
    public XmmRegister Destination { get; set; }
    public RegisterOffset Source { get; set; }
    public Divss_Register_Offset(XmmRegister destination, RegisterOffset source)
    {
        Destination = destination;
        Source = source;
    }

    public override string Emit()
    {
        return $"divss {Destination}, {Source}";
    }

    public override byte[] Assemble(Section section, Dictionary<string, Address> resolvedLabels)
    {
        byte[] opCodes = [0xF3, 0x0F, 0x5E];
        return opCodes.Concat(Source.EncodeAsRM(Destination)).ToArray();
    }

    public override uint GetSizeOnDisk() => 3 + (uint)Source.EncodeAsRM(Destination).Length;
    public override uint GetVirtualSize() => 3 + (uint)Source.EncodeAsRM(Destination).Length;
}

public class Mulss_Register_Offset : X86Instruction
{
    public XmmRegister Destination { get; set; }
    public RegisterOffset Source { get; set; }
    public Mulss_Register_Offset(XmmRegister destination, RegisterOffset source)
    {
        Destination = destination;
        Source = source;
    }

    public override string Emit()
    {
        return $"divss {Destination}, {Source}";
    }

    public override byte[] Assemble(Section section, Dictionary<string, Address> resolvedLabels)
    {
        byte[] opCodes = [0xF3, 0x0F, 0x59];
        return opCodes.Concat(Source.EncodeAsRM(Destination)).ToArray();
    }

    public override uint GetSizeOnDisk() => 3 + (uint)Source.EncodeAsRM(Destination).Length;
    public override uint GetVirtualSize() => 3 + (uint)Source.EncodeAsRM(Destination).Length;
}

public class Cvtsi2ss_Register_Offset : X86Instruction
{
    public XmmRegister Destination { get; set; }
    public RegisterOffset Source { get; set; }
    public Cvtsi2ss_Register_Offset(XmmRegister destination, RegisterOffset source)
    {
        Destination = destination;
        Source = source;
    }

    public override string Emit()
    {
        return $"cvtsi2ss {Destination}, {Source}";
    }

    public override byte[] Assemble(Section section, Dictionary<string, Address> resolvedLabels)
    {
        byte[] opCodes = [0xF3, 0x0F, 0x2A];
        return opCodes.Concat(Source.EncodeAsRM(Destination)).ToArray();
    }

    public override uint GetSizeOnDisk() => 3 + (uint)Source.EncodeAsRM(Destination).Length;
    public override uint GetVirtualSize() => 3 + (uint)Source.EncodeAsRM(Destination).Length;
}

public class Cvtss2si_Register_Offset : X86Instruction
{
    public X86Register Destination { get; set; }
    public RegisterOffset Source { get; set; }
    public Cvtss2si_Register_Offset(X86Register destination, RegisterOffset source)
    {
        Destination = destination;
        Source = source;
    }

    public override string Emit()
    {
        return $"cvtss2si {Destination}, {Source}";
    }

    public override byte[] Assemble(Section section, Dictionary<string, Address> resolvedLabels)
    {
        byte[] opCodes = [0xF3, 0x0F, 0x2A];
        return opCodes.Concat(Source.EncodeAsRM(Destination)).ToArray();
    }

    public override uint GetSizeOnDisk() => 3 + (uint)Source.EncodeAsRM(Destination).Length;
    public override uint GetVirtualSize() => 3 + (uint)Source.EncodeAsRM(Destination).Length;
}