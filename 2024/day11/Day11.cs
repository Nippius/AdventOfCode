namespace AdventOfCode2024;

public static class Day11
{
    // Cache operations done on the rocks because they tend to repeat a lot...
    private static readonly Dictionary<long, List<long>> rockOperationResultCache = [];

    private static long BlinkAway(List<long> initialRockSet, int numberOfBlinks)
    {
        // Pre-load initial rocks and respective counters
        Dictionary<long, long> rocks = [];
        foreach (long rock in initialRockSet)
        {
            rocks.Add(rock, 1);
        }

        for (int i = 0; i < numberOfBlinks; i++)
        {
            // New rock counters for each blink
            Dictionary<long, long> newRocks = [];
            foreach (long rock in rocks.Keys)
            {
                BlinkRock(rock, newRocks, rocks[rock]);
            }
            rocks = newRocks; // store new calculated rocks for next iteration
        }

        return rocks.Sum(x => x.Value);
    }

    private static void BlinkRock(long rock, Dictionary<long, long> rocks, long currentRockCounter)
    {
        List<long> rockOperationResult;
        if (rockOperationResultCache.TryGetValue(rock, out List<long>? cachedRockOperationResult))
        {
            rockOperationResult = cachedRockOperationResult;
        }
        else
        {
            if (rock == 0)
            {
                rockOperationResultCache.Add(rock, [1]);
                rockOperationResult = rockOperationResultCache[rock];
            }
            else if (Math.Floor(Math.Log10(rock) - 1) % 2 == 0)
            {
                rockOperationResult = SplitRock(rock);
                rockOperationResultCache.Add(rock, rockOperationResult);
            }
            else
            {
                rockOperationResultCache.Add(rock, [rock * 2024]);
                rockOperationResult = rockOperationResultCache[rock];
            }
        }

        foreach (long newRock in rockOperationResult)
        {
            if (!rocks.TryAdd(newRock, currentRockCounter))
            {
                rocks[newRock] += currentRockCounter;
            }
        }
    }

    private static List<long> SplitRock(long rockToSplit)
    {
        string valueAsString = Convert.ToString(rockToSplit);
        int halfSize = valueAsString.Length / 2;
        long left = int.Parse(valueAsString.AsSpan(0, halfSize));
        long right = int.Parse(valueAsString.AsSpan(halfSize, halfSize));
        return [left, right];
    }

    public static void Execute()
    {
        List<long> rocks = [.. File.ReadAllText("./day11/input.txt").Split(" ").Select(int.Parse)];

        Console.WriteLine($"[AoC 2024 - Day 11 - Part 1] Result: {BlinkAway(rocks, 25)}");
        Console.WriteLine($"[AoC 2024 - Day 11 - Part 2] Result: {BlinkAway(rocks, 75)}");
    }
}