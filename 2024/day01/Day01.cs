namespace AdventOfCode2024;

public static class Day01
{
    private static int CalculateSimilarityScore(int[] leftSorted, int[] rightSorted)
    {
        int similarityScore = 0;
        Dictionary<int, int> repeatedNumbers = [];
        
        // Count repeated numbers in right list
        for (int i = 0; i < rightSorted.Length; i++)
        {
            repeatedNumbers.TryGetValue(rightSorted[i], out int value);
            repeatedNumbers[rightSorted[i]] = value + 1;
        }

        // Calculate similarity score
        for (int i = 0; i < leftSorted.Length; i++)
        {
            int numberToFind = leftSorted[i];
            repeatedNumbers.TryGetValue(numberToFind, out int repeatedCount);
            similarityScore += numberToFind * repeatedCount;
        }

        return similarityScore;
    }

    private static int TotalDistanceBetweenTwoLists(int[] leftSorted, int[] rightSorted)
    {
        int totalDistance = 0;
        for (int i = 0; i < leftSorted.Length; i++)
        {
            totalDistance += Math.Abs(leftSorted[i] - rightSorted[i]);
        }
        return totalDistance;
    }

    public static void Execute()
    {
        int[] leftSorted = new int[1000];
        int[] rightSorted = new int[1000];
        int insertIdx = 0;
        using StringReader? sr = new(File.ReadAllText("./day01/input.txt"));
        string? line = sr?.ReadLine();
        while (line is not null)
        {
            if (line != string.Empty)
            {
                string[] splittedLine = line.Split("   ");
                leftSorted[insertIdx] = int.Parse(splittedLine[0]);
                rightSorted[insertIdx] = int.Parse(splittedLine[1]);
                insertIdx++;
            }
            line = sr?.ReadLine();
        }

        Array.Sort(leftSorted);
        Array.Sort(rightSorted);

        Console.WriteLine($"[AoC 2024 - Day 01 - Part 1] Result: {TotalDistanceBetweenTwoLists(leftSorted, rightSorted)}");
        Console.WriteLine($"[AoC 2024 - Day 01 - Part 2] Result: {CalculateSimilarityScore(leftSorted, rightSorted)}");
    }
}