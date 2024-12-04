namespace AdventOfCode2024;

public static class Day02
{
    private static bool LevelsAreSafe(IList<int> levels)
    {
        bool originalOrdering = levels[0] < levels[1]; // true -> ascending | false -> descending

        for (int i = 1; i < levels.Count; i++)
        {
            int levelRange = Math.Abs(levels[i - 1] - levels[i]);
            bool levelRangeOutsideLimits = levelRange < 1 || levelRange > 3;
            bool orderChanged = originalOrdering != levels[i - 1] < levels[i];

            if (levelRangeOutsideLimits || orderChanged) { return false; }
        }

        return true;
    }

    private static bool LevelsAreSafeWithTolerance(IList<int> levels)
    {
        // Bruteforce all possibilities.
        // If we are here, we know the original report is not safe
        //   so we remove each level and check until with find a safe report
        for (int i = 0; i < levels.Count; i++)
        {
            if (LevelsAreSafe([.. levels.Where((_, j) => j != i)])) return true;
        }

        return false;
    }

    private static void CheckReportSafety(IList<int> levels, ref int safeReportCounter, ref int safeReportWithToleranceCounter)
    {
        bool levelsAreSafe = LevelsAreSafe(levels);

        if (levelsAreSafe) { safeReportCounter++; }
        if (levelsAreSafe || LevelsAreSafeWithTolerance(levels)) { safeReportWithToleranceCounter++; }
    }

    public static void Execute()
    {
        int safeReportCount = 0;
        int safeReportCountWithTolerance = 0;
        string inputContents = File.ReadAllText("./day02/input.txt");
        using StringReader? sr = new(inputContents);
        string? reportInput = sr?.ReadLine();

        while (reportInput is not null)
        {
            if (reportInput != string.Empty)
            {
                // A report is a list of levels
                IList<int> report = [.. reportInput.Split(' ').Select(int.Parse)];

                CheckReportSafety(report, ref safeReportCount, ref safeReportCountWithTolerance);

                reportInput = sr?.ReadLine();
            }
        }

        Console.WriteLine($"[AoC 2024 - Day 02 - Part 1] Result: {safeReportCount}");
        Console.WriteLine($"[AoC 2024 - Day 02 - Part 2] Result: {safeReportCountWithTolerance}");
    }
}