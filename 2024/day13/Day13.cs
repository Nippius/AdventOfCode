using System.Text.RegularExpressions;

namespace AdventOfCode2024;

public static partial class Day13
{
    [GeneratedRegex("""\d+""")]
    private static partial Regex NumberParser();

    private record Machine(long AX, long AY, long BX, long BY, long PX, long PY);

    private static List<Machine> ParseInput(StringReader sr)
    {
        List<Machine> machines = [];
        string? line = sr?.ReadLine();
        while (line is not null)
        {
            if (line != string.Empty)
            {
                MatchCollection numbers = NumberParser().Matches(line);
                long aX = long.Parse(numbers[0].Value);
                long aY = long.Parse(numbers[1].Value);

                line = sr?.ReadLine();
                numbers = NumberParser().Matches(line!);
                long bX = long.Parse(numbers[0].Value);
                long bY = long.Parse(numbers[1].Value);

                line = sr?.ReadLine();
                numbers = NumberParser().Matches(line!);
                long pX = long.Parse(numbers[0].Value);
                long pY = long.Parse(numbers[1].Value);
                
                machines.Add(new Machine(aX, aY, bX, bY, pX, pY));
            }
            line = sr?.ReadLine();
        }
        return machines;
    }

    // Using Cramer's rule
    // https://www.youtube.com/watch?v=vXqlIOX2itM
    private static long CalculateTokenCounts(List<Machine> machines)
    {
        long tokenCount = 0;
        foreach (Machine machine in machines)
        {
            long det = machine.AX * machine.BY - machine.AY * machine.BX;
            long A = (machine.PX * machine.BY - machine.PY * machine.BX) / det;
            long B = (machine.AX * machine.PY - machine.AY * machine.PX) / det;
            if(machine.AX * A + machine.BX * B == machine.PX && machine.AY * A + machine.BY * B == machine.PY){
                tokenCount += 3 * A + B;
            }
        }
        return tokenCount;
    }

    public static void Execute()
    {
        using StringReader? sr = new(File.ReadAllText("./day13/input.txt"));

        List<Machine> machines = ParseInput(sr);
        List<Machine> machinesWithOffset = [.. machines.Select(m => m with { PX = m.PX + 10000000000000, PY = m.PY + 10000000000000 })];

        Console.WriteLine($"[AoC 2024 - Day 13 - Part 1] Result: {CalculateTokenCounts(machines)}");
        Console.WriteLine($"[AoC 2024 - Day 13 - Part 2] Result: {CalculateTokenCounts(machinesWithOffset)}");
    }

}