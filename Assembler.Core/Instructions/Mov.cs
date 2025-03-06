using Assembler.Core.Constants;
using Assembler.Core.Extensions;
using Assembler.Core.Interfaces;
using Assembler.Core.Models;
using Assembler.Core.PortableExecutable;
using Assembler.Core.PortableExecutable.Models;

namespace Assembler.Core.Instructions
{
    public class Mov_Register_RegisterOffset : X86Instruction, IRegister_RegisterOffset
    {
        public X86Register Destination { get; set; }
        public RegisterOffset Source { get; set; }
        public Mov_Register_RegisterOffset(X86Register destination, RegisterOffset source)
        {
            Destination = destination;
            Source = source;
        }

        public override string Emit()
        {
            return $"mov {Destination}, {Source}";
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            byte opCode = 0x8B;
            return opCode.Encode(Source.EncodeAsRM(Destination));
        }
        public override uint GetSizeOnDisk() => 1 + (uint)Source.EncodeAsRM(Destination).Length;
        public override uint GetVirtualSize() => 1 + (uint)Source.EncodeAsRM(Destination).Length;
    }

    public class Mov_Register_SymbolOffset : X86Instruction, IRegister_SymbolOffset
    {
        public X86Register Destination { get; set; }
        public SymbolOffset Source { get; set; }
        public Mov_Register_SymbolOffset(X86Register destination, SymbolOffset source)
        {
            Destination = destination;
            Source = source;
        }

        public override string Emit()
        {
            return $"mov {Destination}, {Source}";
        }

        public override void AddRelocationEntry(BaseRelocationBlock baseRelocationBlock, ushort currentVirtualOffsetFromSectionStart)
        {
            baseRelocationBlock.AddEntry(currentVirtualOffsetFromSectionStart + 2);
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            byte opCode = 0x8B;
            var address = GetAddressOrThrow(resolvedLabels, Source.Symbol);
            return opCode.Encode(Source.EncodeAsRM(Destination, address));
        }
        public override uint GetSizeOnDisk() => 6;
        public override uint GetVirtualSize() => 6;
    }


    public class Mov_RegisterOffset_Register : X86Instruction, IRegisterOffset_Register
    {
        public RegisterOffset Destination { get; set; }
        public X86Register Source { get; set; }
        public Mov_RegisterOffset_Register(RegisterOffset destination, X86Register source)
        {
            Destination = destination;
            Source = source;
        }

        public override string Emit()
        {
            return $"mov {Destination}, {Source}";
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            byte opCode = 0x89;
            return opCode.Encode(Destination.EncodeAsRM(Source));
        }

        public override uint GetSizeOnDisk() => 1 + (uint)Destination.EncodeAsRM(Source).Length;
        public override uint GetVirtualSize() => 1 + (uint)Destination.EncodeAsRM(Source).Length;
    }

    public class Mov_Register_Register : X86Instruction, IRegister_Register
    {
        public X86Register Destination { get; set; }
        public X86Register Source { get; set; }
        public Mov_Register_Register(X86Register destination, X86Register source)
        {
            Destination = destination;
            Source = source;
        }

        public override string Emit()
        {
            return $"mov {Destination}, {Source}";
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            // 8B /r	MOV r32, r/m32	RM	Valid	Valid	Move r/m32 to r32.
            byte opCode = 0x8B;
            byte ModRM = Mod.RegisterDirect.ApplyOperand1(Destination).ApplyOperand2(Source);
            return [opCode, ModRM];
        }

        public override uint GetSizeOnDisk() => 2;
        public override uint GetVirtualSize() => 2;
    }

    public class Mov_Register_Immediate : X86Instruction, IRegister_Immediate
    {
        public X86Register Destination { get; set; }
        public int ImmediateValue { get; set; }
        public Mov_Register_Immediate(X86Register destination, int immediateValue)
        {
            Destination = destination;
            ImmediateValue = immediateValue;
        }

        public override string Emit()
        {
            return $"mov {Destination}, {ImmediateValue}";
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            byte opCode = 0xB8;
            opCode = opCode.ApplyRegister(Destination);
            return opCode.Encode(ImmediateValue.ToBytes());
        }
        public override uint GetSizeOnDisk() => 5;
        public override uint GetVirtualSize() => 5;

    }

    public class Mov_Register_Address : X86Instruction, IRegister_Destination
    {
        public X86Register Destination { get; set; }
        public string Address { get; set; }
        public Mov_Register_Address(X86Register destination, string address)
        {
            Destination = destination;
            Address = address;
        }

        public override string Emit()
        {
            return $"mov {Destination}, {Address}";
        }

        public override void AddRelocationEntry(BaseRelocationBlock baseRelocationBlock, ushort currentVirtualOffsetFromSectionStart)
        {
            baseRelocationBlock.AddEntry(currentVirtualOffsetFromSectionStart + 1);
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            byte opCode = 0xB8;
            opCode = opCode.ApplyRegister(Destination);
            var address = GetAddressOrThrow(resolvedLabels, Address);
            return opCode.Encode(address.VirtualAddress.ToBytes());
        }
        public override uint GetSizeOnDisk() => 5;
        public override uint GetVirtualSize() => 5;

    }


    public class Mov_RegisterOffset_Immediate : X86Instruction, IRegisterOffset_Immediate
    {
        public RegisterOffset Destination { get; set; }
        public int ImmediateValue { get; set; }
        public Mov_RegisterOffset_Immediate(RegisterOffset destination, int immediateValue)
        {
            Destination = destination;
            ImmediateValue = immediateValue;
        }

        public override string Emit()
        {
            return $"mov {Destination}, {ImmediateValue}";
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            byte opCode = 0xC7;
            // here eax is opcode extension 0
            return opCode
                .Encode(Destination.EncodeAsRM(X86Register.eax))
                .Concat(ImmediateValue.ToBytes()).ToArray();
        }

        public override uint GetSizeOnDisk() => 5 + (uint)Destination.EncodeAsRM(X86Register.eax).Length;
        public override uint GetVirtualSize() => 5 + (uint)Destination.EncodeAsRM(X86Register.eax).Length;
    }

    public class Mov_RegisterOffset_Address : X86Instruction, IRegisterOffset_Destination
    {
        public RegisterOffset Destination { get; set; }
        public string Address { get; set; }
        public Mov_RegisterOffset_Address(RegisterOffset destination, string address)
        {
            Destination = destination;
            Address = address;
        }

        public override string Emit()
        {
            return $"mov {Destination}, {Address}";
        }

        public override void AddRelocationEntry(BaseRelocationBlock baseRelocationBlock, ushort currentVirtualOffsetFromSectionStart)
        {
            baseRelocationBlock.AddEntry(currentVirtualOffsetFromSectionStart + ((ushort)GetVirtualSize() - 4));
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            byte opCode = 0xC7;
            // here eax is opcode extension 0
            var address = GetAddressOrThrow(resolvedLabels, Address);
            return opCode
                .Encode(Destination.EncodeAsRM(X86Register.eax))
                .Concat(address.VirtualAddress.ToBytes()).ToArray();
        }

        public override uint GetSizeOnDisk() => 5 + (uint)Destination.EncodeAsRM(X86Register.eax).Length;
        public override uint GetVirtualSize() => 5 + (uint)Destination.EncodeAsRM(X86Register.eax).Length;
    }

    public class Mov_SymbolOffset_Register : X86Instruction, ISymbolOffset_Register
    {
        public SymbolOffset Destination { get; set; }
        public X86Register Source { get; set; }
        public Mov_SymbolOffset_Register(SymbolOffset destination, X86Register source)
        {
            Destination = destination;
            Source = source;
        }

        public override string Emit()
        {
            return $"mov {Destination}, {Source}";
        }
        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            var address = GetAddressOrThrow(resolvedLabels, Destination.Symbol);
            byte opCode = 0x89;
            return opCode.Encode(Destination.EncodeAsRM(Source, address));
        }

        public override void AddRelocationEntry(BaseRelocationBlock baseRelocationBlock, ushort currentVirtualOffsetFromSectionStart)
        {
            baseRelocationBlock.AddEntry(currentVirtualOffsetFromSectionStart + ((ushort)GetVirtualSize() - 4)); // symbol address is placed at last 4 bytes of instruction encoding
        }

        public override uint GetSizeOnDisk() => 6;
        public override uint GetVirtualSize() => 6;
    }

    public class Mov_SymbolOffset_Immediate : X86Instruction, ISymbolOffset_Immediate
    {
        public SymbolOffset Destination { get; set; }
        public int ImmediateValue { get; set; }
        public Mov_SymbolOffset_Immediate(SymbolOffset destination, int immediateValue)
        {
            Destination = destination;
            ImmediateValue = immediateValue;
        }

        public override string Emit()
        {
            return $"mov {Destination}, {ImmediateValue}";
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            var address = GetAddressOrThrow (resolvedLabels, Destination.Symbol);
            byte opCode = 0xC7;
            
            return opCode.Encode(Destination.EncodeAsRM(address)).Concat(ImmediateValue.ToBytes()).ToArray();
        }

        public override void AddRelocationEntry(BaseRelocationBlock baseRelocationBlock, ushort currentVirtualOffsetFromSectionStart)
        {
            baseRelocationBlock.AddEntry(currentVirtualOffsetFromSectionStart + 2); 
        }

        public override uint GetSizeOnDisk() => 10;
        public override uint GetVirtualSize() => 10;

    }

    public class Mov_SymbolOffset_Address : X86Instruction, ISymbolOffset_Destination
    {
        public SymbolOffset Destination { get; set; }
        public string Address { get; set; }
        public Mov_SymbolOffset_Address(SymbolOffset destination, string address)
        {
            Destination = destination;
            Address = address;
        }

        public override string Emit()
        {
            return $"mov {Destination}, {Address}";
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            var address = GetAddressOrThrow(resolvedLabels, Destination.Symbol);
            byte opCode = 0xC7;
            var sourceAddress = GetAddressOrThrow(resolvedLabels, Address);

            return opCode.Encode(Destination.EncodeAsRM(address)).Concat(sourceAddress.VirtualAddress.ToBytes()).ToArray();
        }

        public override void AddRelocationEntry(BaseRelocationBlock baseRelocationBlock, ushort currentVirtualOffsetFromSectionStart)
        {
            baseRelocationBlock.AddEntry(currentVirtualOffsetFromSectionStart + 2);
            baseRelocationBlock.AddEntry(currentVirtualOffsetFromSectionStart + 6);
        }

        public override uint GetSizeOnDisk() => 10;
        public override uint GetVirtualSize() => 10;

    }

    public class Mov_SymbolOffset_ByteRegister : X86Instruction, ISymbolOffset_ByteRegister
    {
        public SymbolOffset Destination { get; set; }
        public X86ByteRegister Source { get; set; }
        public Mov_SymbolOffset_ByteRegister(SymbolOffset destination, X86ByteRegister source)
        {
            Destination = destination;
            Source = source;
        }

        public override string Emit()
        {
            return $"mov {Destination}, {Source}";
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            var address = GetAddressOrThrow(resolvedLabels, Destination.Symbol);
            byte opCode = 0x88;

            return opCode.Encode(Destination.EncodeAsRM(Source, address));
        }

        public override void AddRelocationEntry(BaseRelocationBlock baseRelocationBlock, ushort currentVirtualOffsetFromSectionStart)
        {
            baseRelocationBlock.AddEntry(currentVirtualOffsetFromSectionStart + ((ushort)GetVirtualSize() - 4)); // symbol address is placed at last 4 bytes of instruction encoding
        }

        public override uint GetSizeOnDisk() => 6;
        public override uint GetVirtualSize() => 6;
    }


    public class Mov_RegisterOffset_ByteRegister : X86Instruction, IRegisterOffset_ByteRegister
    {
        public RegisterOffset Destination { get; set; }
        public X86ByteRegister Source { get; set; }
        public Mov_RegisterOffset_ByteRegister(RegisterOffset destination, X86ByteRegister source)
        {
            Destination = destination;
            Source = source;
        }

        public override string Emit()
        {
            return $"mov {Destination}, {Source}";
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            byte opCode = 0x88;
            return opCode.Encode(Destination.EncodeAsRM(Source));
        }

        public override uint GetSizeOnDisk() => 1 + (uint)Destination.EncodeAsRM(Source).Length;
        public override uint GetVirtualSize() => 1 + (uint)Destination.EncodeAsRM(Source).Length;
    }

}
