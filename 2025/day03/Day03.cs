using System.Text;

namespace AdventOfCode2025.Day03;

public static class Day03
{
    private static List<int[]> ParseInput(StringReader sr)
    {
        using (sr)
        {
            List<int[]> input = [];
            string? line = sr?.ReadLine();
            while (line is not null)
            {
                if (line != string.Empty)
                {
                    input.Add([.. line.Select((x, _) => x - '0')]);
                }
                line = sr?.ReadLine();
            }
            return input;
        }
    }

    // Adapted from here: https://old.reddit.com/r/adventofcode/comments/1pcxkif/2025_day_3_mega_tutorial/ns1fe8f/
    private static long GetMaxJolted(int[] batteryBank, int n)
    {
        int removalsLeft = batteryBank.Length - n;
        LinkedList<int> stack = new();

        foreach (int battery in batteryBank)
        {
            while (stack.Count > 0 && removalsLeft > 0 && stack.Last!.Value < battery)
            {
                stack.RemoveLast();
                removalsLeft--;
            }
            stack.AddLast(battery);
        }

        while (stack.Count > n)
        {
            stack.RemoveLast();
        }

        StringBuilder sb = new(n);
        foreach (int battery in stack)
        {
            sb.Append(battery);
        }

        return long.Parse(sb.ToString());
    }

    private static long CalculateTotal(List<int[]> batteryBanks, int maxBatteries)
    {
        long totalOutputVoltage = 0;
        foreach (int[] bank in batteryBanks)
        {
            totalOutputVoltage += GetMaxJolted(bank, maxBatteries);
        }
        return totalOutputVoltage;
    }

    public static void Execute()
    {
        using StringReader? sr = new(File.ReadAllText("./day03/input.txt"));

        List<int[]> batteryBanks = ParseInput(sr);

        Console.WriteLine($"[AoC 2025 - Day 03 - Part 1] Result: {CalculateTotal(batteryBanks, 2)}");
        Console.WriteLine($"[AoC 2025 - Day 03 - Part 2] Result: {CalculateTotal(batteryBanks, 12)}");
    }
}