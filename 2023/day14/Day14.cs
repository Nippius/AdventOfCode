namespace AdventOfCode2023;

public static class Day14
{
    public static void Execute()
    {
        int sum = 0;
        StringReader sr = new(File.ReadAllText("./day14/input.txt"));
        string? line = sr.ReadLine();
        while (line != null)
        {
            if (line != string.Empty)
            {
                //TODO
            }
            line = sr.ReadLine();
        }

        Console.WriteLine($"[AoC 2023 - Day 14] Result: {sum}");
    }
}
