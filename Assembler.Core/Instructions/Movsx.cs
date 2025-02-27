using Assembler.Core.Constants;
using Assembler.Core.Extensions;
using Assembler.Core.Models;
using Assembler.Core.PortableExecutable;

namespace Assembler.Core.Instructions
{

    public class Mov_Register_Offset__Byte : X86Instruction
    {
        public X86ByteRegister Destination { get; set; }
        public RegisterOffset Source { get; set; }
        public Mov_Register_Offset__Byte(X86ByteRegister destination, RegisterOffset source)
        {
            Destination = destination;
            Source = source;
        }

        public override string Emit()
        {
            return $"mov {Destination}, {Source}";
        }

        public override byte[] Assemble(Section section, Dictionary<string, Address> resolvedLabels)
        {
            byte opCode = 0x8A;
            return opCode.Encode(Source.EncodeAsRM(Destination));
        }

        public override uint GetSizeOnDisk() => 1 + (uint)Source.EncodeAsRM(Destination).Length;
        public override uint GetVirtualSize() => 1 + (uint)Source.EncodeAsRM(Destination).Length;
    }

    public class Movsx_Register_Offset_Byte : X86Instruction
    {
        public X86Register Destination { get; set; }
        public RegisterOffset Source { get; set; }
        public Movsx_Register_Offset_Byte(X86Register destination, RegisterOffset source)
        {
            Destination = destination;
            Source = source;
        }

        public override string Emit()
        {
            return $"movsx {Destination}, {Source}";
        }

        public override byte[] Assemble(Section section, Dictionary<string, Address> resolvedLabels)
        {
            byte[] opCodes = [0x0F, 0xBE];
            return opCodes.Concat(Source.EncodeAsRM(Destination)).ToArray();
        }

        public override uint GetSizeOnDisk() => 2 + (uint)Source.EncodeAsRM(Destination).Length;
        public override uint GetVirtualSize() => 2 + (uint)Source.EncodeAsRM(Destination).Length;
    }

    public class Movsx_Register_SymbolOffset__Byte : X86Instruction
    {
        public X86Register Destination { get; set; }
        public SymbolOffset Source { get; set; }
        public Movsx_Register_SymbolOffset__Byte(X86Register destination, SymbolOffset source)
        {
            Destination = destination;
            Source = source;
        }

        public override string Emit()
        {
            return $"movsx {Destination}, {Source}";
        }

        public override byte[] Assemble(Section section, Dictionary<string, Address> resolvedLabels)
        {
            var address = GetAddressOrThrow(resolvedLabels, Source.Symbol);
            byte[] opCodes = [0x0F, 0xBE];
            return opCodes.Concat(Source.EncodeAsRM(Destination, address)).ToArray();
        }

        public override uint GetSizeOnDisk() => 7;
        public override uint GetVirtualSize() => 7;
    }

    public class Movzx_Register_Offset_Byte : X86Instruction
    {
        public X86Register Destination { get; set; }
        public RegisterOffset Source { get; set; }
        public Movzx_Register_Offset_Byte(X86Register destination, RegisterOffset source)
        {
            Destination = destination;
            Source = source;
        }

        public override string Emit()
        {
            return $"movzx {Destination}, {Source}";
        }

        public override byte[] Assemble(Section section, Dictionary<string, Address> resolvedLabels)
        {
            byte[] opCodes = [0x0F, 0xB6];
            return opCodes.Concat(Source.EncodeAsRM(Destination)).ToArray();
        }

        public override uint GetSizeOnDisk() => 2 + (uint)Source.EncodeAsRM(Destination).Length;
        public override uint GetVirtualSize() => 2 + (uint)Source.EncodeAsRM(Destination).Length;
    }

    public class Mov_Register_Immediate__Byte : X86Instruction
    {
        public X86ByteRegister Destination { get; set; }
        public byte ImmediateValue { get; set; }
        public Mov_Register_Immediate__Byte(X86ByteRegister destination, byte immediateValue)
        {
            Destination = destination;
            ImmediateValue = immediateValue;
        }

        public override string Emit()
        {
            return $"mov {Destination}, {ImmediateValue}";
        }
        public override byte[] Assemble(Section section, Dictionary<string, Address> resolvedLabels)
        {
            byte opCode = 0xB0;

            return [opCode.ApplyRegister(Destination), ImmediateValue];
        }

        public override uint GetSizeOnDisk() => 2;
        public override uint GetVirtualSize() => 2;

    }
}