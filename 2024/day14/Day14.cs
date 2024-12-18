using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO.Compression;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2024;

public static partial class Day14
{
    [GeneratedRegex("""\d+|-\d+""")]
    private static partial Regex NumberParser();

    // This value was found via trial and error by looking at the ontropy of the grid after each second
    private const int ENTROPY_THRESHOLD = 400;
    private const int SIMULATION_SECONDS = 100;
    private const int MAX_GRID_WIDTH = 101;
    private const int MAX_GRID_HEIGHT = 103;
    private static readonly int MAX_CONCURENT_TASKS = Environment.ProcessorCount;

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

    private static async Task<int> FindEasterEggAsync(List<Robot> robots)
    {
        ConcurrentQueue<(int seconds, byte[] grid)> gridQueue = new(); // Queue of grids to process
        ConcurrentQueue<(int seconds, long entropy)> entropyQueue = new(); // Queue of entropy results to process
        using CancellationTokenSource cts = new();
        CancellationToken token = cts.Token;

        // Launch the tasks that will calculate the entropy of the grids
        List<Task> workingTasks = [];
        for (int i = 0; i < MAX_CONCURENT_TASKS; i++)
        {
            workingTasks.Add(Task.Run(async () =>
            {
                while (token.IsCancellationRequested is not true)
                {
                    if (gridQueue.TryDequeue(out (int seconds, byte[] grid) work))
                    {
                        entropyQueue.Enqueue((work.seconds, await CalculateEntropyAsync(work.grid)));
                    }
                }
            }));
        }

        int seconds = 1;
        while (token.IsCancellationRequested is not true)
        {
            // Check if there's any results for us to process
            if (entropyQueue.TryDequeue(out (int seconds, long entropy) calculatedEntropy))
            {
                if (calculatedEntropy.entropy < ENTROPY_THRESHOLD)
                {
                    cts.Cancel();
                    await Task.WhenAll(workingTasks);

                    return calculatedEntropy.seconds;
                }
            }

            byte[] grid = new byte[MAX_GRID_WIDTH * MAX_GRID_HEIGHT];

            // While waiting for the results to come in generate new grids to calculate the entropy in the future
            foreach (Robot robot in robots)
            {
                robot.Move();
                grid[((int)robot.Position.X * MAX_GRID_WIDTH) + (int)robot.Position.Y]++;
            }

            gridQueue.Enqueue((seconds++, grid));
        }

        return 0;
    }

    /*
        In simple terms, entropy is the amount of randomness.
        A file with a lot of random stuff is hard to compress because there aren't many repeating bytes, 
            resulting in a large file. This means the original file has high entropy.
        On the other end, a file with little random stuff can be highly compress resulting 
            in a small file. This means the original file has low entropy.
        To form the easter egg, lots of robots must end up side-by-side to form an image resulting in 
            a highly compressible "file", or in other words, a low entropy value.
    */
    private static async Task<long> CalculateEntropyAsync(byte[] grid)
    {
        using MemoryStream unCompressedGridStream = new(grid);
        using MemoryStream compressedGridStream = new();
        using DeflateStream compressedStream = new(compressedGridStream, CompressionLevel.SmallestSize);

        await unCompressedGridStream.CopyToAsync(compressedStream);
        await compressedStream.FlushAsync();

        long entropy = compressedGridStream.Length;

        compressedStream.Close();

        return entropy;
    }

    public static async Task ExecuteAsync()
    {
        List<Robot> robots = ParseInput(new StringReader(File.ReadAllText("./day14/input.txt")));
        List<Robot> robotsCopy = [.. robots.Select(x => (Robot)x.Clone())];

        Console.WriteLine($"[AoC 2024 - Day 14 - Part 1] Result: {CalculateSafetyFactor(robots)}");
        Console.WriteLine($"[AoC 2024 - Day 14 - Part 2] Result: {await FindEasterEggAsync(robotsCopy)}");
    }
}