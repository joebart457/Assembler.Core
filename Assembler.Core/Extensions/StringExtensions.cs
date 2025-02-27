
using System.Text;

namespace Assembler.Core.Extensions;

public static class StringExtensions
{
    public static string Indent(this string s, int indentLevel = 1)
    {
        return $"{new string('\t', indentLevel)}{s}";
    }

    public static byte[] GetBytes(this string s, int paddedLength, byte padValue = 0)
    {
        var rawBytes = Encoding.UTF8.GetBytes(s).ToList();
        if (rawBytes.Count > paddedLength) throw new ArgumentException($"paddedLength cannot be longer than the source string length");
        while (rawBytes.Count < paddedLength)
        {
            rawBytes.Add(padValue);
        }
        return rawBytes.ToArray();
    }
}