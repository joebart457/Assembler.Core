
namespace Assembler.Core.Extensions;


internal static class IntExtensions
{
    public static uint RoundUpToNearestMultipleOfFactor(this uint value, uint factor)
    {
        if (factor == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(factor), factor, "Cannot be zero");
        }

        var (div, remainder) = Math.DivRem(value, factor);
        if (remainder == 0) return value;
        return (div + 1) * factor;
       
    }

    public static byte[] ToBytes(this uint value)
    {
        return BitConverter.GetBytes(value);
    }

    public static byte[] ToBytes(this ushort value)
    {
        return BitConverter.GetBytes(value);
    }

    public static byte[] ToBytes(this int value)
    {
        return BitConverter.GetBytes(value);
    }

}