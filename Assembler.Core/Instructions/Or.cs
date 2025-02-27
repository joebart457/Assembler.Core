using Assembler.Core.Constants;
using Assembler.Core.Extensions;
using Assembler.Core.Models;
using Assembler.Core.PortableExecutable;

namespace Assembler.Core.Instructions
{
    public class Or_Register_Register : X86Instruction
    {
        public X86Register Destination { get; set; }
        public X86Register Source { get; set; }

        public Or_Register_Register(X86Register destination, X86Register source)
        {
            Destination = destination;
            Source = source;
        }

        public override string Emit()
        {
            return $"or {Destination}, {Source}";
        }

        public override byte[] Assemble(Section section, Dictionary<string, Address> resolvedLabels)
        {
            byte opCode = 0x09;
            var modRM = Mod.RegisterDirect.ApplyOperand1(Source).ApplyOperand2(Destination);
            return [opCode, modRM];
        }

        public override uint GetSizeOnDisk() => 2;
        public override uint GetVirtualSize() => 2;
    }

    public class Or_Register_Register__Byte : X86Instruction
    {
        public X86ByteRegister Destination { get; set; }
        public X86ByteRegister Source { get; set; }

        public Or_Register_Register__Byte(X86ByteRegister destination, X86ByteRegister source)
        {
            Destination = destination;
            Source = source;
        }

        public override string Emit()
        {
            return $"or {Destination}, {Source}";
        }

        public override byte[] Assemble(Section section, Dictionary<string, Address> resolvedLabels)
        {
            byte opCode = 0x08;
            var modRM = Mod.RegisterDirect.ApplyOperand1(Source).ApplyOperand2(Destination);
            return [opCode, modRM];
        }

        public override uint GetSizeOnDisk() => 2;
        public override uint GetVirtualSize() => 2;
    }
}
