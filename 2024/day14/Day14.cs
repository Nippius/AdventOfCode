using System.IO.Compression;
using System.Numerics;
using System.Text.RegularExpressions;

namespace AdventOfCode2024;

public static partial class Day14
{
    [GeneratedRegex("""\d+|-\d+""")]
    private static partial Regex NumberParser();

    private const int MAX_GRID_WIDTH = 101;
    private const int MAX_GRID_HEIGHT = 103;
    // private const int MAX_GRID_WIDTH = 11;
    // private const int MAX_GRID_HEIGHT = 7;
    private static readonly int SIMULATION_SECONDS = 100;

    private class Robot(Vector2 position, Vector2 velocity) : ICloneable
    {
        public Vector2 Position { get; private set; } = position;
        public Vector2 Velocity { get; private set; } = velocity;

        public object Clone()
        {
            return new Robot(Position, Velocity);
        }

        public void Move()
        {
            Position += Velocity;

            if (Position.X < 0)
            {
                Position = Position with { X = MAX_GRID_WIDTH + Position.X };
            }

            if (Position.X >= MAX_GRID_WIDTH)
            {
                Position = Position with { X = Position.X - MAX_GRID_WIDTH };
            }

            if (Position.Y < 0)
            {
                Position = Position with { Y = MAX_GRID_HEIGHT + Position.Y };
            }

            if (Position.Y >= MAX_GRID_HEIGHT)
            {
                Position = Position with { Y = Position.Y - MAX_GRID_HEIGHT };
            }
        }
    }

    private static List<Robot> ParseInput(StringReader sr)
    {
        List<Robot> robots = [];
        using (sr)
        {
            string? line = sr?.ReadLine();
            while (line is not null)
            {
                if (line != string.Empty)
                {
                    MatchCollection numbers = NumberParser().Matches(line);
                    robots.Add(new(
                        new(int.Parse(numbers[0].Value), int.Parse(numbers[1].Value)),
                        new(int.Parse(numbers[2].Value), int.Parse(numbers[3].Value))));
                }
                line = sr?.ReadLine();
            }
        }
        return robots;
    }

    private static int CountRobotsInQuadrant(List<Robot> robots, Vector2 quadrantStart, Vector2 quadrantEnd)
    {
        int robotsInQuadrant = 0;

        foreach (Robot robot in robots)
        {
            if (robot.Position.X >= quadrantStart.X
                && robot.Position.X < quadrantEnd.X
                && robot.Position.Y >= quadrantStart.Y
                && robot.Position.Y < quadrantEnd.Y)
            {
                robotsInQuadrant++;
            }
        }

        return robotsInQuadrant;
    }

    private static int CalculateSafetyFactor(List<Robot> robots)
    {
        for (int i = 0; i < SIMULATION_SECONDS; i++)
        {
            foreach (Robot robot in robots)
            {
                robot.Move();
            }
        }

        /*
            A | B
            -----
            D | C
        */
        return CountRobotsInQuadrant(robots, new Vector2(0, 0), new Vector2(MAX_GRID_WIDTH / 2, MAX_GRID_HEIGHT / 2))
            * CountRobotsInQuadrant(robots, new Vector2((MAX_GRID_WIDTH / 2) + 1, 0), new Vector2(MAX_GRID_WIDTH, MAX_GRID_HEIGHT / 2))
            * CountRobotsInQuadrant(robots, new Vector2((MAX_GRID_WIDTH / 2) + 1, (MAX_GRID_HEIGHT / 2) + 1), new Vector2(MAX_GRID_WIDTH, MAX_GRID_HEIGHT))
            * CountRobotsInQuadrant(robots, new Vector2(0, (MAX_GRID_HEIGHT / 2) + 1), new Vector2(MAX_GRID_WIDTH / 2, MAX_GRID_HEIGHT));
    }


    private static int FindEasterEgg(List<Robot> robots)
    {
        int numberOfSecondsUntilEasterEggFound = -1;
        long lowestEntropyFound = int.MaxValue;

        int i = 1;
        while (lowestEntropyFound > 400) // Kinda arbitrary but based on the output
        {
            byte[] grid = new byte[MAX_GRID_WIDTH * MAX_GRID_HEIGHT];

            foreach (Robot robot in robots)
            {
                robot.Move();
                grid[((int)robot.Position.X * MAX_GRID_WIDTH) + (int)robot.Position.Y]++;
            }

            long gridEntropy = CalculateEntropy(grid);
            if (gridEntropy < lowestEntropyFound)
            {
                lowestEntropyFound = gridEntropy;
                numberOfSecondsUntilEasterEggFound = i;
            }

            i++;
        }
        return numberOfSecondsUntilEasterEggFound;
    }

    private static long CalculateEntropy(byte[] grid)
    {
        MemoryStream unCompressedGridStream, compressedGridStream;
        DeflateStream compressedStream;
        unCompressedGridStream = new(grid);
        compressedGridStream = new();
        compressedStream = new DeflateStream(compressedGridStream, CompressionLevel.SmallestSize);
        unCompressedGridStream.CopyTo(compressedStream);
        compressedStream.Flush();
        long tempSize = compressedGridStream.Length;
        compressedStream.Close();
        return tempSize;
    }

    public static void Execute()
    {
        List<Robot> robots = ParseInput(new StringReader(File.ReadAllText("./day14/input.txt")));
        List<Robot> robotsCopy = [.. robots.Select(x => (Robot)x.Clone())];

        Console.WriteLine($"[AoC 2024 - Day 14 - Part 1] Result: {CalculateSafetyFactor(robots)}");
        Console.WriteLine($"[AoC 2024 - Day 14 - Part 2] Result: {FindEasterEgg(robotsCopy)}");
    }
}