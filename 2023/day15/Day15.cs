namespace AdventOfCode2023;

public static class Day15
{
    private class Box
    {
        public List<Lens> Lenses { get; }
        public Box() { Lenses = []; }
    }

    private class Lens(string Label, int FocalLength) { public string Label { get; } = Label; public int FocalLength { get; set; } = FocalLength; }

    private static int GetHashFromLabel(string label)
    {
        int stepHashResult = 0;
        foreach (char c in label)
        {
            stepHashResult += c;
            stepHashResult *= 17;
            stepHashResult %= 256;
        }
        return stepHashResult;
    }

    private static int TotalSumOfTheResults(string line)
    {
        int sum = 0;
        if (line != null && line != string.Empty)
        {
            string[] steps = line.Split(',');
            foreach (string step in steps)
            {
                sum += GetHashFromLabel(step);
            }
        }
        return sum;
    }

    private static int GetFocusingPower(string line)
    {
        Box[] boxes = new Box[256];
        for (int i = 0; i < boxes.Length; i++) { boxes[i] = new Box(); }
        int totalLensPower = 0;

        if (line != null && line != string.Empty)
        {
            string[] steps = line.Split(',');
            foreach (string step in steps)
            {
                if (step.Contains('-'))
                {
                    string[] operation = step.Split('-');
                    string label = operation[0];
                    int boxId = GetHashFromLabel(label);
                    Box box = boxes[boxId];
                    int lensToRemove = box.Lenses.FindIndex(l => l.Label == label);
                    if (lensToRemove >= 0)
                    {
                        box.Lenses.RemoveAt(lensToRemove);
                    }
                }
                else if (step.Contains('='))
                {
                    string[] operation = step.Split('='); string label = operation[0];
                    int boxId = GetHashFromLabel(label); int focalLength = int.Parse(operation[1]);
                    Box box = boxes[boxId];

                    int lensIndex = box.Lenses.FindIndex(l => l.Label == label);
                    if (lensIndex == -1)
                    {
                        box.Lenses.Add(new Lens(label, focalLength));
                    }
                    else { box.Lenses[lensIndex].FocalLength = focalLength; }
                }
                else
                {
                    // nothing to do
                }
            }

            for (int i = 0; i < boxes.Length; i++)
            {
                int boxId = i;
                Box box = boxes[boxId];
                Lens[] lensList = box.Lenses.ToArray();
                for (int j = 0; j < lensList.Length; j++)
                {
                    totalLensPower += (1 + boxId) * (j + 1) * lensList[j].FocalLength;
                }
            }
        }

        return totalLensPower;
    }

    public static void Execute()
    {
        using StringReader sr = new(File.ReadAllText("./day15/input.txt"));
        string line = sr.ReadLine();
        Console.WriteLine($"[AoC 2023 - Day 15 - Part 1] Result: {TotalSumOfTheResults(line)}");
        Console.WriteLine($"[AoC 2023 - Day 15 - Part 2] Result: {GetFocusingPower(line)}");
    }
}