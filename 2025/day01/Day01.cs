using System.Text.RegularExpressions;

namespace AdventOfCode2025.Day01;

public static partial class Day01
{
    [GeneratedRegex(@"(\w)(\d+)")]
    private static partial Regex RotationParser();

    private record Rotation(char Direction, int Distance);

    private static List<Rotation> ParseInput(StringReader sr)
    {
        using (sr)
        {
            List<Rotation> input = [];
            string? line = sr?.ReadLine();
            while (line is not null)
            {
                if (line != string.Empty)
                {
                    foreach (Match rotationData in RotationParser().Matches(line))
                    {
                        input.Add(new(char.Parse(rotationData.Groups[1].Value), int.Parse(rotationData.Groups[2].Value)));
                    }
                }
                line = sr?.ReadLine();
            }
            return input;
        }
    }

    private static int Part1(List<Rotation> Rotations)
    {
        int dialHitZero = 0;
        int dialPosition = 50;
        foreach (Rotation rotation in Rotations)
        {
            switch (rotation.Direction)
            {
                case 'L':
                    int rotDistMod = rotation.Distance % 100;
                    dialPosition = (dialPosition < rotDistMod) ? 100 - (rotDistMod - dialPosition) : dialPosition - rotDistMod;
                    break;
                default:
                    dialPosition = (dialPosition + rotation.Distance) % 100;
                    break;
            }
            if (dialPosition == 0) dialHitZero++;
        }

        return dialHitZero;
    }

    private static int Part2(List<Rotation> Rotations)
    {
        int dialHitZero = 0;
        int dialPosition = 50;
        foreach (Rotation rotation in Rotations)
        {
            for (int i = 0; i < rotation.Distance; i++)
            {
                if(rotation.Direction == 'L')
                {
                    dialPosition--;
                    if(dialPosition < 0)
                    {
                        dialPosition = 99;
                    }
                }
                else
                {
                    dialPosition++;
                    if(dialPosition == 100)
                    {
                        dialPosition = 0;
                    }
                }
                if(dialPosition == 0) dialHitZero++;
            }
        }

        return dialHitZero;
    }

    public static void Execute()
    {
        using StringReader? sr = new(File.ReadAllText("./day01/input.txt"));

        List<Rotation> rotations = ParseInput(sr);

        Console.WriteLine($"[AoC 2025 - Day 01 - Part 1] Result: {Part1(rotations)}");
        Console.WriteLine($"[AoC 2025 - Day 01 - Part 2] Result: {Part2(rotations)}");
    }
}