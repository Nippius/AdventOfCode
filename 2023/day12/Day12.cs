namespace AdventOfCode2023;

public static class Day12
{
    public static void Execute()
    {
        int sum = 0;
        StringReader sr = new(File.ReadAllText("./day12/input.txt"));
        string? line = sr.ReadLine();
        while (line != null)
        {
            if (line != string.Empty)
            {
                //TODO
            }
            line = sr.ReadLine();
        }

        Console.WriteLine($"[AoC 2023 - Day 12] Result: {sum}");
    }
}
