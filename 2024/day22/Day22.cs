namespace AdventOfCode2024;

public static class Day22
{
    public static void Execute()
    {
        int sum = 0;
        using StringReader? sr = new(File.ReadAllText("./day22/input.txt"));
        string? line = sr?.ReadLine();
        while (line is not null)
        {
            if (line != string.Empty)
            {
                // TODO
            }
            line = sr?.ReadLine();
        }
        
        Console.WriteLine($"[AoC 2024 - Day 22 - Part 1] Result: {sum}");
        Console.WriteLine($"[AoC 2024 - Day 22 - Part 2] Result: {sum}");
    }
}