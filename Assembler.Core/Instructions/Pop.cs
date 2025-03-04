using Assembler.Core.Constants;
using Assembler.Core.Extensions;
using Assembler.Core.Interfaces;
using Assembler.Core.Models;
using Assembler.Core.PortableExecutable;
using Assembler.Core.PortableExecutable.Models;

namespace Assembler.Core.Instructions
{
    public class Pop_Register : X86Instruction, IRegister_Destination, IPop
    {
        public X86Register Destination { get; set; }

        public Pop_Register(X86Register destination)
        {
            Destination = destination;
        }

        public override string Emit()
        {
            return $"pop {Destination}";
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            byte opCode = 0x58;
            return [opCode.ApplyRegister(Destination)];
        }

        public override uint GetSizeOnDisk() => 1;
        public override uint GetVirtualSize() => 1;
    }
}
