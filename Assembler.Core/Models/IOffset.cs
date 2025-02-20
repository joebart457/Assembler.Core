namespace Assembler.Core.Models
{
    public interface IOffset
    {
        public int Offset { get; set; }
        public IOffset ToByteOffset();
    }
}
