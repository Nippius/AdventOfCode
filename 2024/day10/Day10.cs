namespace AdventOfCode2024;

using System;
using System.Numerics;
using System.Collections.Generic;
using Map = int[,];

public static class Day10
{
    private class Direction
    {
        public static Vector2 Up { get; } = new Vector2(-1, 0);
        public static Vector2 Right { get; } = new Vector2(0, 1);
        public static Vector2 Down { get; } = new Vector2(1, 0);
        public static Vector2 Left { get; } = new Vector2(0, -1);
    }

    private static readonly Vector2[] directions = [Direction.Up, Direction.Right, Direction.Down, Direction.Left];

    private static Map ParseInput(StringReader sr)
    {
        int x = 0;
        Map map = new int[0, 0];
        string? line = sr?.ReadLine();
        while (line is not null)
        {
            if (line != string.Empty)
            {
                if (map.Length == 0)
                {
                    map = new int[line.Length, line.Length];
                }

                for (int y = 0; y < line.Length; y++)
                {
                    map[x, y] = (int)char.GetNumericValue(line[y]);
                }
            }
            line = sr?.ReadLine();
            x++;
        }
        return map;
    }

    private static void CalculateTrailheadsScoreAndRatingSums(Map map, List<Vector2> trailheads, out int scoreSum, out int ratingsSum)
    {
        scoreSum = 0;
        ratingsSum = 0;

        foreach (Vector2 trailhead in trailheads)
        {
            Dictionary<Vector2, int> trailtailsVisited = [];
            FindTrailheadScore(map, trailhead, trailtailsVisited);
            scoreSum += trailtailsVisited.Count; // Count: How many trailtails we reached from the trailhead
            ratingsSum += trailtailsVisited.Sum(x => x.Value); // Value: How many times each trailhead was reached
        }
    }

    private static void FindTrailheadScore(int[,] map, Vector2 currentlocation, Dictionary<Vector2, int> trailtailsVisited)
    {
        if (!VectorIsInBounds(map, currentlocation))
        {
            return;
        }

        int valueAtCurrentLocation = map[(int)currentlocation.X, (int)currentlocation.Y];
        Stack<Vector2> locationsToVisit = new();
        foreach (Vector2 direction in directions)
        {
            Vector2 nextLocation = currentlocation + direction;

            if (VectorIsInBounds(map, nextLocation))
            {
                int valueAtNextLocation = map[(int)nextLocation.X, (int)nextLocation.Y];
                if (valueAtNextLocation - valueAtCurrentLocation == 1)
                {
                    locationsToVisit.Push(nextLocation);
                }
            }
        }

        while (locationsToVisit.TryPop(out Vector2 location))
        {
            if (map[(int)location.X, (int)location.Y] == 9)
            {
                if (trailtailsVisited.TryGetValue(location, out int value))
                {
                    trailtailsVisited[location] = ++value;
                }
                else
                {
                    trailtailsVisited.Add(location, 1);
                }
            }
            else
            {
                FindTrailheadScore(map, location, trailtailsVisited);
            }
        }
    }

    private static bool VectorIsInBounds(int[,] map, Vector2 currentlocation)
    {
        return currentlocation.X >= 0
               && currentlocation.Y >= 0
               && currentlocation.X < map.GetLength(0)
               && currentlocation.Y < map.GetLength(1);
    }

    private static List<Vector2> FindTrailheads(Map map)
    {
        List<Vector2> lowestPoints = [];

        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                if (map[x, y] == 0)
                {
                    lowestPoints.Add(new Vector2(x, y));
                }
            }
        }
        return lowestPoints;
    }

    public static void Execute()
    {
        using StringReader? sr = new(File.ReadAllText("./day10/input.txt"));

        Map map = ParseInput(sr);

        List<Vector2> trailheads = FindTrailheads(map);
        CalculateTrailheadsScoreAndRatingSums(map, trailheads, out int scoreSum, out int ratingsSum);

        Console.WriteLine($"[AoC 2024 - Day 10 - Part 1] Result: {scoreSum}");
        Console.WriteLine($"[AoC 2024 - Day 10 - Part 2] Result: {ratingsSum}");
    }
}