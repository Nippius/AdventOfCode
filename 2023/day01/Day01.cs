namespace AdventOfCode2023;

public static class Day01
{
    private static SortedList<int, int> FindAllDigitsInString(string s)
    {
        SortedList<int, int> numbers = [];
        for (int i = 0; i < s.Length; i++)
        {
            if (char.IsDigit(s[i]))
            {
                numbers.Add(i, s[i] - 48);
            }
        }
        return numbers;
    }

    private static SortedList<int, int> FindAllDigitsAsTextInString(string s)
    {
        (string digitWord, int digit)[] digitsAsText = [
            ("one", 1),
            ("two", 2),
            ("three", 3),
            ("four",4),
            ("five",5),
            ("six",6),
            ("seven",7),
            ("eight",8),
            ("nine",9),
            ("zero",0)
        ];

        SortedList<int, int> numbers = [];

        foreach (var (digitWord, digit) in digitsAsText)
        {
            int foundIndex = -1;
            int nextIndex = 0;
            do
            {
                foundIndex = s.IndexOf(digitWord, nextIndex);
                if (foundIndex > -1)
                {
                    numbers.Add(foundIndex, digit);
                    nextIndex = foundIndex + digitWord.Length;
                }
            } while (foundIndex > -1);
        }

        return numbers;
    }

    public static void Execute()
    {
        int sum = 0;
        StringReader sr = new(File.ReadAllText("./Day01/input.txt"));
        string? line = sr.ReadLine();
        while (line != null)
        {
            if (line != string.Empty)
            {
                SortedList<int, int> numbers = FindAllDigitsInString(line);
                foreach(var (key, value) in FindAllDigitsAsTextInString(line)){
                    numbers.Add(key, value);
                }
                sum += numbers.FirstOrDefault().Value*10+numbers.LastOrDefault().Value;
            }
            line = sr.ReadLine();
        }

        Console.WriteLine($"[AoC 2023 - Day 01] Result: {sum}");
    }
}