using Assembler.Core.Constants;
using Assembler.Core.Extensions;
using Assembler.Core.Models;
using Assembler.Core.PortableExecutable;
using Assembler.Core.PortableExecutable.Models;

namespace Assembler.Core.Instructions
{
    public class Jmp : X86Instruction
    {
        public string Label { get; set; }

        public Jmp(string label)
        {
            Label = label;
        }
        public override string Emit()
        {
            return $"jmp {Label}";
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            var address = GetAddressOrThrow(resolvedLabels, Label);
            byte opCode = 0xE9;
            var offset = address.VirtualAddress - (absoluteInstructionPointer + GetVirtualSize());
            return opCode.Encode(offset.ToBytes());
        }

        public override void AddRelocationEntry(BaseRelocationBlock baseRelocationBlock, ushort currentVirtualOffsetFromSectionStart)
        {
            baseRelocationBlock.AddEntry(currentVirtualOffsetFromSectionStart + 1); // currentVirtualOffset + 1 (size of opcodes)
        }
        public override uint GetSizeOnDisk() => 5;
        public override uint GetVirtualSize() => 5;
    }

    public class JmpGt : Jmp
    {
        public JmpGt(string label) : base(label)
        {
            Label = label;
        }

        public override string Emit()
        {
            return $"jg {Label}";
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            var address = GetAddressOrThrow(resolvedLabels, Label);
            List<byte> opCodes = [0x0F, 0x8F];
            var offset = address.VirtualAddress - (absoluteInstructionPointer + GetVirtualSize());
            return opCodes.Concat(offset.ToBytes()).ToArray();
        }

        public override void AddRelocationEntry(BaseRelocationBlock baseRelocationBlock, ushort currentVirtualOffsetFromSectionStart)
        {
            baseRelocationBlock.AddEntry(currentVirtualOffsetFromSectionStart + 2); // currentVirtualOffset + 2 (size of opcodes)
        }
        public override uint GetSizeOnDisk() => 6;
        public override uint GetVirtualSize() => 6;
    }

    public class JmpLt : Jmp
    {
        public JmpLt(string label) : base(label)
        {
            Label = label;
        }

        public override string Emit()
        {
            return $"jl {Label}";
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            var address = GetAddressOrThrow(resolvedLabels, Label);
            List<byte> opCodes = [0x0F, 0x8C];
            var offset = address.VirtualAddress - (absoluteInstructionPointer + GetVirtualSize());
            return opCodes.Concat(offset.ToBytes()).ToArray();
        }
        public override void AddRelocationEntry(BaseRelocationBlock baseRelocationBlock, ushort currentVirtualOffsetFromSectionStart)
        {
            baseRelocationBlock.AddEntry(currentVirtualOffsetFromSectionStart + 2); // currentVirtualOffset + 2 (size of opcodes)
        }
        public override uint GetSizeOnDisk() => 6;
        public override uint GetVirtualSize() => 6;
    }

    public class JmpGte : Jmp
    {

        public JmpGte(string label) : base(label)
        {
            Label = label;
        }

        public override string Emit()
        {
            return $"jge {Label}";
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            var address = GetAddressOrThrow(resolvedLabels, Label);
            List<byte> opCodes = [0x0F, 0x8D];
            var offset = address.VirtualAddress - (absoluteInstructionPointer + GetVirtualSize());
            return opCodes.Concat(offset.ToBytes()).ToArray();
        }
        public override void AddRelocationEntry(BaseRelocationBlock baseRelocationBlock, ushort currentVirtualOffsetFromSectionStart)
        {
            baseRelocationBlock.AddEntry(currentVirtualOffsetFromSectionStart + 2); // currentVirtualOffset + 2 (size of opcodes)
        }
        public override uint GetSizeOnDisk() => 6;
        public override uint GetVirtualSize() => 6;

    }

    public class JmpLte : Jmp
    {
        public JmpLte(string label) : base(label)
        {
            Label = label;
        }

        public override string Emit()
        {
            return $"jle {Label}";
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            var address = GetAddressOrThrow(resolvedLabels, Label);
            List<byte> opCodes = [0x0F, 0x8E];
            var offset = address.VirtualAddress - (absoluteInstructionPointer + GetVirtualSize());
            return opCodes.Concat(offset.ToBytes()).ToArray();
        }
        public override void AddRelocationEntry(BaseRelocationBlock baseRelocationBlock, ushort currentVirtualOffsetFromSectionStart)
        {
            baseRelocationBlock.AddEntry(currentVirtualOffsetFromSectionStart + 2); // currentVirtualOffset + 2 (size of opcodes)
        }
        public override uint GetSizeOnDisk() => 6;
        public override uint GetVirtualSize() => 6;
    }

    public class JmpEq : Jmp
    {
        public JmpEq(string label) : base(label)
        {
            Label = label;
        }

        public override string Emit()
        {
            return $"je {Label}";
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            var address = GetAddressOrThrow(resolvedLabels, Label);
            List<byte> opCodes = [0x0F, 0x84];
            var offset = address.VirtualAddress - (absoluteInstructionPointer + GetVirtualSize());
            return opCodes.Concat(offset.ToBytes()).ToArray();
        }
        public override void AddRelocationEntry(BaseRelocationBlock baseRelocationBlock, ushort currentVirtualOffsetFromSectionStart)
        {
            baseRelocationBlock.AddEntry(currentVirtualOffsetFromSectionStart + 2); // currentVirtualOffset + 2 (size of opcodes)
        }
        public override uint GetSizeOnDisk() => 6;
        public override uint GetVirtualSize() => 6;
    }

    public class JmpNeq : Jmp
    {
        public JmpNeq(string label) : base(label)
        {
            Label = label;
        }

        public override string Emit()
        {
            return $"jne {Label}";
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            var address = GetAddressOrThrow(resolvedLabels, Label);
            List<byte> opCodes = [0x0F, 0x85];
            var offset = address.VirtualAddress - (absoluteInstructionPointer + GetVirtualSize());
            return opCodes.Concat(offset.ToBytes()).ToArray();
        }
        public override void AddRelocationEntry(BaseRelocationBlock baseRelocationBlock, ushort currentVirtualOffsetFromSectionStart)
        {
            baseRelocationBlock.AddEntry(currentVirtualOffsetFromSectionStart + 2); // currentVirtualOffset + 2 (size of opcodes)
        }
        public override uint GetSizeOnDisk() => 6;
        public override uint GetVirtualSize() => 6;
    }

    public class Jz : Jmp
    {
        public Jz(string label) : base(label)
        {
            Label = label;
        }

        public override string Emit()
        {
            return $"jz {Label}";
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            var address = GetAddressOrThrow(resolvedLabels, Label);
            List<byte> opCodes = [0x0F, 0x84];
            var offset = address.VirtualAddress - (absoluteInstructionPointer + GetVirtualSize());
            return opCodes.Concat(offset.ToBytes()).ToArray();
        }
        public override void AddRelocationEntry(BaseRelocationBlock baseRelocationBlock, ushort currentVirtualOffsetFromSectionStart)
        {
            baseRelocationBlock.AddEntry(currentVirtualOffsetFromSectionStart + 2); // currentVirtualOffset + 2 (size of opcodes)
        }
        public override uint GetSizeOnDisk() => 6;
        public override uint GetVirtualSize() => 6;
    }

    public class Jnz : Jmp
    {
        public Jnz(string label) : base(label)
        {
            Label = label;
        }

        public override string Emit()
        {
            return $"jnz {Label}";
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            var address = GetAddressOrThrow(resolvedLabels, Label);
            List<byte> opCodes = [0x0F, 0x85];
            var offset = address.VirtualAddress - (absoluteInstructionPointer + GetVirtualSize());
            return opCodes.Concat(offset.ToBytes()).ToArray();
        }
        public override void AddRelocationEntry(BaseRelocationBlock baseRelocationBlock, ushort currentVirtualOffsetFromSectionStart)
        {
            baseRelocationBlock.AddEntry(currentVirtualOffsetFromSectionStart + 2); // currentVirtualOffset + 2 (size of opcodes)
        }
        public override uint GetSizeOnDisk() => 6;
        public override uint GetVirtualSize() => 6;
    }

    public class Js : Jmp
    {
        public Js(string label) : base(label)
        {
            Label = label;
        }

        public override string Emit()
        {
            return $"js {Label}";
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            var address = GetAddressOrThrow(resolvedLabels, Label);
            List<byte> opCodes = [0x0F, 0x88];
            var offset = address.VirtualAddress - (absoluteInstructionPointer + GetVirtualSize());
            return opCodes.Concat(offset.ToBytes()).ToArray();
        }
        public override void AddRelocationEntry(BaseRelocationBlock baseRelocationBlock, ushort currentVirtualOffsetFromSectionStart)
        {
            baseRelocationBlock.AddEntry(currentVirtualOffsetFromSectionStart + 2); // currentVirtualOffset + 2 (size of opcodes)
        }
        public override uint GetSizeOnDisk() => 6;
        public override uint GetVirtualSize() => 6;
    }

    public class Jns : Jmp
    {
        public Jns(string label) : base(label)
        {
            Label = label;
        }

        public override string Emit()
        {
            return $"jns {Label}";
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            var address = GetAddressOrThrow(resolvedLabels, Label);
            List<byte> opCodes = [0x0F, 0x85];
            var offset = address.VirtualAddress - (absoluteInstructionPointer + GetVirtualSize());
            return opCodes.Concat(offset.ToBytes()).ToArray();
        }
        public override void AddRelocationEntry(BaseRelocationBlock baseRelocationBlock, ushort currentVirtualOffsetFromSectionStart)
        {
            baseRelocationBlock.AddEntry(currentVirtualOffsetFromSectionStart + 2); // currentVirtualOffset + 2 (size of opcodes)
        }
        public override uint GetSizeOnDisk() => 6;
        public override uint GetVirtualSize() => 6;
    }

    public class Ja : Jmp
    {
        public Ja(string label): base(label)
        {
            Label = label;
        }

        public override string Emit()
        {
            return $"ja {Label}";
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            var address = GetAddressOrThrow(resolvedLabels, Label);
            List<byte> opCodes = [0x0F, 0x87];
            var offset = address.VirtualAddress - (absoluteInstructionPointer + GetVirtualSize());
            return opCodes.Concat(offset.ToBytes()).ToArray();
        }
        public override void AddRelocationEntry(BaseRelocationBlock baseRelocationBlock, ushort currentVirtualOffsetFromSectionStart)
        {
            baseRelocationBlock.AddEntry(currentVirtualOffsetFromSectionStart + 2); // currentVirtualOffset + 2 (size of opcodes)
        }
        public override uint GetSizeOnDisk() => 6;
        public override uint GetVirtualSize() => 6;
    }

    public class Jae : Jmp
    {
        public Jae(string label) : base(label)
        {
            Label = label;
        }

        public override string Emit()
        {
            return $"jae {Label}";
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            var address = GetAddressOrThrow(resolvedLabels, Label);
            List<byte> opCodes = [0x0F, 0x83];
            var offset = address.VirtualAddress - (absoluteInstructionPointer + GetVirtualSize());
            return opCodes.Concat(offset.ToBytes()).ToArray();
        }
        public override void AddRelocationEntry(BaseRelocationBlock baseRelocationBlock, ushort currentVirtualOffsetFromSectionStart)
        {
            baseRelocationBlock.AddEntry(currentVirtualOffsetFromSectionStart + 2); // currentVirtualOffset + 2 (size of opcodes)
        }
        public override uint GetSizeOnDisk() => 6;
        public override uint GetVirtualSize() => 6;
    }

    public class Jb : Jmp
    {
        public Jb(string label) : base(label)
        {
            Label = label;
        }

        public override string Emit()
        {
            return $"jb {Label}";
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            var address = GetAddressOrThrow(resolvedLabels, Label);
            List<byte> opCodes = [0x0F, 0x82];
            var offset = address.VirtualAddress - (absoluteInstructionPointer + GetVirtualSize());
            return opCodes.Concat(offset.ToBytes()).ToArray();
        }
        public override void AddRelocationEntry(BaseRelocationBlock baseRelocationBlock, ushort currentVirtualOffsetFromSectionStart)
        {
            baseRelocationBlock.AddEntry(currentVirtualOffsetFromSectionStart + 2); // currentVirtualOffset + 2 (size of opcodes)
        }
        public override uint GetSizeOnDisk() => 6;
        public override uint GetVirtualSize() => 6;
    }

    public class Jbe : Jmp
    {

        public Jbe(string label): base(label)
        {
            Label = label;
        }

        public override string Emit()
        {
            return $"jbe {Label}";
        }

        public override byte[] Assemble(Section section, uint absoluteInstructionPointer, Dictionary<string, Address> resolvedLabels)
        {
            var address = GetAddressOrThrow(resolvedLabels, Label);
            List<byte> opCodes = [0x0F, 0x86];
            var offset = address.VirtualAddress - (absoluteInstructionPointer + GetVirtualSize());
            return opCodes.Concat(offset.ToBytes()).ToArray();
        }
        public override void AddRelocationEntry(BaseRelocationBlock baseRelocationBlock, ushort currentVirtualOffsetFromSectionStart)
        {
            baseRelocationBlock.AddEntry(currentVirtualOffsetFromSectionStart + 2); // currentVirtualOffset + 2 (size of opcodes)
        }
        public override uint GetSizeOnDisk() => 6;
        public override uint GetVirtualSize() => 6;
    }
}
