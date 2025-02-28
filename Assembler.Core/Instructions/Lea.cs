using Assembler.Core.Constants;
using Assembler.Core.Extensions;
using Assembler.Core.Models;
using Assembler.Core.PortableExecutable;
using Assembler.Core.PortableExecutable.Models;

namespace Assembler.Core.Instructions
{
    public class Lea_Register_RegisterOffset : X86Instruction
    {
        public X86Register Destination { get; set; }
        public RegisterOffset Source { get; set; }
        public Lea_Register_RegisterOffset(X86Register destination, RegisterOffset source)
        {
            Destination = destination;
            Source = source;
        }

        public override string Emit()
        {
            return $"lea {Destination}, {Source}";
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            byte opCode = 0x8D;
            return opCode.Encode(Source.EncodeAsRM(Destination));
        }
        public override uint GetSizeOnDisk() => 1 + (uint)Source.EncodeAsRM(Destination).Length;
        public override uint GetVirtualSize() => 1 + (uint)Source.EncodeAsRM(Destination).Length;
    }

    public class Lea_Register_SymbolOffset : X86Instruction
    {
        public X86Register Destination { get; set; }
        public SymbolOffset Source { get; set; }
        public Lea_Register_SymbolOffset(X86Register destination, SymbolOffset source)
        {
            Destination = destination;
            Source = source;
        }

        public override string Emit()
        {
            return $"lea {Destination}, {Source}";
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            var address = GetAddressOrThrow(resolvedLabels, Source.Symbol);
            byte opCode = 0x8D;
            return opCode.Encode(Source.EncodeAsRM(Destination, address));
        }
        public override uint GetSizeOnDisk() => 6;
        public override uint GetVirtualSize() => 6;
    }
}
