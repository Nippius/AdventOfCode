using System.Numerics;

namespace AdventOfCode2024;

public static class Day08
{
    private static readonly int MAX_GRID_SIZE = 50;

    private record Antenna(Vector2 Position, char Frequency);

    private static List<Antenna> ParseInput(StringReader sr)
    {
        List<Antenna> antennas = [];
        int x = 0;
        int y = 0;
        string? line = sr?.ReadLine();
        while (line is not null)
        {
            if (line != string.Empty)
            {
                foreach (char c in line)
                {
                    if (c != '.')
                    {
                        antennas.Add(new Antenna(new Vector2(x, y), c));
                    }
                    y++;
                }
            }
            line = sr?.ReadLine();
            x++;
            y = 0;
        }
        return antennas;
    }

    private static bool AntinodeIsInBounds(Vector2 antinode)
    {
        if (antinode.X >= 0
            && antinode.Y >= 0
            && antinode.X < MAX_GRID_SIZE
            && antinode.Y < MAX_GRID_SIZE)
        {
            return true;
        }
        return false;
    }

    private static List<Vector2> CalculateAntinodes(Antenna a1, Antenna a2, bool onlyCalculateFirstAntinode = true)
    {
        List<Vector2> antinodes = [];
        Vector2 diffVect = a1.Position - a2.Position;

        if (onlyCalculateFirstAntinode)
        {
            Vector2 antinode = a1.Position + diffVect;
            if (AntinodeIsInBounds(antinode))
            {
                antinodes.Add(antinode);
            }
        }
        else
        {
            Vector2 antinode = a2.Position + diffVect;
            while (AntinodeIsInBounds(antinode))
            {
                antinodes.Add(antinode);
                antinode += diffVect;
            }
        }

        return antinodes;
    }

    private static int CountDistinctAntinodeLocations(IReadOnlyList<Antenna> antennas, bool onlyCalculateFirstAntinode = true)
    {
        IEnumerable<List<Antenna>> groupedAntennas = antennas.GroupBy(a => a.Frequency).Select(a => a.ToList());
        List<Vector2> antinodes = [];

        foreach (List<Antenna> groupOfAntennas in groupedAntennas)
        {
            for (int i = 0; i < groupOfAntennas.Count; i++)
            {
                for (int j = 0; j < groupOfAntennas.Count; j++)
                {
                    if (i != j)
                    {
                        antinodes.AddRange(CalculateAntinodes(groupOfAntennas[i], groupOfAntennas[j], onlyCalculateFirstAntinode));
                    }
                }
            }
        }

        return antinodes.Distinct().Count();
    }

    public static void Execute()
    {
        using StringReader? sr = new(File.ReadAllText("./day08/input.txt"));

        IReadOnlyList<Antenna> antennas = ParseInput(sr);

        Console.WriteLine($"[AoC 2024 - Day 08 - Part 1] Result: {CountDistinctAntinodeLocations(antennas)}");
        Console.WriteLine($"[AoC 2024 - Day 08 - Part 2] Result: {CountDistinctAntinodeLocations(antennas, false)}");
    }
}