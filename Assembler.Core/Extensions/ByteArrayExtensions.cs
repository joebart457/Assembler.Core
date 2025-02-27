using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler.Core.Extensions;


internal static class ByteArrayExtensions
{
    public static List<byte> PadToAlignment(this List<byte> source, uint alignment)
    {
        var nearestMultipleOfAlignment = ((uint)source.Count).RoundUpToNearestMultipleOfFactor(alignment);
        while(source.Count < nearestMultipleOfAlignment)
        {
            source.Add(0x00);
        }
        return source;
    }

    public static byte[] PadToAlignment(this byte[] source, uint alignment)
    {
        return source.ToList().PadToAlignment(alignment).ToArray();
    }

    public static List<byte> Pad(this List<byte> source, uint bytesToAdd)
    {
        var totalBytes = source.Count + bytesToAdd;
        while (source.Count < totalBytes)
        {
            source.Add(0x00);
        }
        return source;
    }

    public static byte[] Encode(this byte opCode, byte[] bytes)
    {
        List<byte> result = [opCode]; 
        result.AddRange(bytes);
        return result.ToArray();
    }

    // The following adds a SIB byte which will result in zero modification to the effective address. (Scale = 1x, Index = 100, Base = 100)
    public static byte[] AddEspSIBByte(this byte opCode)
    {
        return [opCode, 0b00100100];
    }
}