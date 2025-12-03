using System.Text.RegularExpressions;

namespace AdventOfCode2025.Day02;

public static partial class Day02
{
    [GeneratedRegex(@"\b(\d+)\1+\b")]
    private static partial Regex RepeatedSequence();

    private record IdRange(long Start, long End);

    private static List<IdRange> ParseInput(StringReader sr)
    {
        using (sr)
        {
            List<IdRange> input = [];
            string? line = sr?.ReadLine();
            while (line is not null)
            {
                if (line != string.Empty)
                {
                    string[] idRanges = line.Split(',');
                    foreach (var idRange in idRanges)
                    {
                        string[] range = idRange.Split('-');
                        input.Add(new(long.Parse(range[0]), long.Parse(range[1])));
                    }
                }
                line = sr?.ReadLine();
            }
            return input;
        }
    }

    private static long Part1(IReadOnlyList<IdRange> idRanges)
    {
        long sumOfInvalidIds = 0;
        foreach (IdRange range in idRanges)
        {
            for (long i = range.Start; i <= range.End; i++)
            {
                ReadOnlySpan<char> nAsString = i.ToString().AsSpan();
                if (nAsString.Length % 2 == 1) continue;
                var a = nAsString[..(nAsString.Length / 2)]; // First half
                var b = nAsString[(nAsString.Length / 2)..nAsString.Length]; // Second half

                if (MemoryExtensions.Equals(a, b, StringComparison.Ordinal))
                {
                    sumOfInvalidIds += i;
                }
            }
        }

        return sumOfInvalidIds;
    }

    private static long Part2(IReadOnlyList<IdRange> idRanges)
    {
        long sumOfInvalidIds = 0;
        foreach (IdRange range in idRanges)
        {
            for (long i = range.Start; i <= range.End; i++)
            {
                string nAsString = i.ToString();
                if (RepeatedSequence().IsMatch(nAsString))
                {
                    sumOfInvalidIds += i;
                }
            }
        }

        return sumOfInvalidIds;
    }

    public static void Execute()
    {
        using StringReader? sr = new(File.ReadAllText("./day02/input.txt"));

        List<IdRange> idRanges = ParseInput(sr);

        Console.WriteLine($"[AoC 2025 - Day 02 - Part 1] Result: {Part1(idRanges)}");
        Console.WriteLine($"[AoC 2025 - Day 02 - Part 2] Result: {Part2(idRanges)}");
    }
}