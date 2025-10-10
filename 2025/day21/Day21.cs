namespace AdventOfCode2025.Day21;

public static class Day21
{
    private static List<string> ParseInput(StringReader sr){
        using(sr)
        {
            List<string> input = [];
            string? line = sr?.ReadLine();
            while (line is not null)
            {
                if (line != string.Empty)
                {
                    input.Add(line);
                }
                line = sr?.ReadLine();
            }
            return input;
        }
    }

    private static int Part1(int sum)
    {
        return sum;
    }

    private static int Part2(int sum)
    {
        return sum;
    }

    public static void Execute()
    {
        int sum = 0;
        using StringReader? sr = new(File.ReadAllText("./day21/input.txt"));
        
        List<string> input = ParseInput(sr);
        
        Console.WriteLine($"[AoC 2025 - Day 21 - Part 1] Result: {Part1(sum)}");
        Console.WriteLine($"[AoC 2025 - Day 21 - Part 2] Result: {Part2(sum)}");
    }
}