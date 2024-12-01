namespace AdventOfCode2024;

public static class Day12
{
    public static void Execute()
    {
        int sum = 0;
        using StringReader? sr = new(File.ReadAllText("./day12/input.txt"));
        string? line = sr?.ReadLine();
        while (line is not null)
        {
            if (line != string.Empty)
            {
                // TODO
            }
            line = sr?.ReadLine();
        }
        
        Console.WriteLine($"[AoC 2024 - Day 12 - Part 1] Result: {sum}");
        Console.WriteLine($"[AoC 2024 - Day 12 - Part 2] Result: {sum}");
    }
}