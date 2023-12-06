namespace AdventOfCode2023;

public static class Day06
{
    private class Race(long Duration, long DistanceRecord)
    {
        public long Duration { get; } = Duration;
        public long DistanceRecord { get; } = DistanceRecord;
    }

    private static int MultiplyNumberOfWaysToBeatTheRecords(IList<Race> races)
    {
        int result = 0;
        foreach (Race race in races)
        {
            // Use the quadratic formula to find the zeros
            double firstZero = Math.Ceiling(((race.Duration * -1) + Math.Sqrt(Math.Pow(race.Duration, 2) + (-4 * race.DistanceRecord))) / -2);
            double secondZero = Math.Ceiling(((race.Duration * -1) - Math.Sqrt(Math.Pow(race.Duration, 2) + (-4 * race.DistanceRecord))) / -2);

            int movesThatBreakTheDistanceRecord = (int)(secondZero - firstZero);

            result = (result == 0) ? movesThatBreakTheDistanceRecord : result * movesThatBreakTheDistanceRecord;
        }
        return result;
    }

    public static void Execute()
    {
        using StringReader sr = new(File.ReadAllText("./day06/input.txt"));
        
        IList<Race> races = [];
        string raceTimesInput = sr.ReadLine().Split(':')[1];
        string distanceRecordsInput = sr.ReadLine().Split(':')[1];

        // Part 1 inputs
        int[] raceTimesP1 = raceTimesInput.Split(' ').Where(x => x != " " && !string.IsNullOrEmpty(x)).Select(int.Parse).ToArray();
        int[] distanceRecordsP1 = distanceRecordsInput.Split(' ').Where(x => x != " " && !string.IsNullOrEmpty(x)).Select(int.Parse).ToArray();
        for (int i = 0; i < raceTimesP1.Length; i++)
        {
            races.Add(new Race(raceTimesP1[i], distanceRecordsP1[i]));
        }

        // Part 2 inputs
        long raceTimesP2 = long.Parse(raceTimesInput.Replace(" ", string.Empty));
        long distanceRecordsP2 = long.Parse(distanceRecordsInput.Replace(" ", string.Empty));

        Console.WriteLine($"[AoC 2023 - Day 06 - Part 1] Result: {MultiplyNumberOfWaysToBeatTheRecords(races)}");
        Console.WriteLine($"[AoC 2023 - Day 06 - Part 2] Result: {MultiplyNumberOfWaysToBeatTheRecords(new[] { new Race(raceTimesP2, distanceRecordsP2) })}");
    }
}
