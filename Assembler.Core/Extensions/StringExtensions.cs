
namespace Assembler.Core.Extensions;

public static class StringExtensions
{
    public static string Indent(this string s, int indentLevel = 1)
    {
        return $"{new string('\t', indentLevel)}{s}";
    }
}