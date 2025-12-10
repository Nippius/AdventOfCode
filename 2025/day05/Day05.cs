namespace AdventOfCode2025.Day05;

public static class Day05
{
    private class IngredientsDatabase(List<IngredientsDatabase.IdRange> idRanges, List<long> availableIngredients)
    {
        public record IdRange(long min, long max); // Ranges are inclusive
        public List<IdRange> FreshIngredientRanges { get; private set; } = idRanges;
        public List<long> AvailableIngredients { get; private set; } = availableIngredients;
    }

    private static IngredientsDatabase ParseInput(StringReader sr)
    {
        using (sr)
        {
            List<IngredientsDatabase.IdRange> idRanges = [];
            List<long> availableIngredients = [];
            string? line = sr?.ReadLine();
            while (line is not null)
            {
                if (line != string.Empty)
                {
                    var temp = line.Split('-');
                    if (temp.Length == 2)
                    {
                        idRanges.Add(new(long.Parse(temp[0]), long.Parse(temp[1])));
                    }
                    else
                    {
                        availableIngredients.Add(long.Parse(temp[0]));
                    }
                }
                line = sr?.ReadLine();
            }
            return new IngredientsDatabase(idRanges, availableIngredients);
        }
    }

    private static int Part1(IngredientsDatabase database)
    {
        int freshIngredients = 0;
        foreach (long id in database.AvailableIngredients)
        {
            bool ingredientInAtLeastOneRange = false;
            foreach (IngredientsDatabase.IdRange range in database.FreshIngredientRanges)
            {
                if (id >= range.min && id <= range.max)
                {
                    ingredientInAtLeastOneRange = true;
                }
            }
            if (ingredientInAtLeastOneRange)
            {
                freshIngredients++;
            }
        }
        return freshIngredients;
    }

    private static long Part2(IngredientsDatabase database)
    {
        long validIds = 0;

        IngredientsDatabase.IdRange[] freshIngredientRanges = [.. database.FreshIngredientRanges];
        freshIngredientRanges.Sort((x, y) => (x?.min >= y?.min) ? 1 : -1);
        List<IngredientsDatabase.IdRange> mergedRanges = [];

        for (int i = 0; i < freshIngredientRanges.Length - 1;)
        {
            var currentRange = freshIngredientRanges[i];
            int lastMergedRangeIdx = 0;
            // Merge the next ranges as many times as we can
            for (int j = i + 1; j < freshIngredientRanges.Length; j++)
            {
                var possibleRangeToMerge = freshIngredientRanges[j];

                // Since ranges are ordered, we know that after this point they can't be merged
                if (possibleRangeToMerge.min > currentRange.max)
                {
                    lastMergedRangeIdx = j;
                    break;
                }

                // Expand range by merging with previous range
                if (possibleRangeToMerge.min >= currentRange.min && possibleRangeToMerge.max >= currentRange.max)
                {
                    // Ranges are sorted by min so the possibleRangeToMerge either ends "inside" the current
                    // range, which we can ignore, or ends "outside" the current range and so we merge
                    // with the currentRange expanding it.
                    currentRange = new(currentRange.min, possibleRangeToMerge.max);
                    lastMergedRangeIdx = j;
                }
            }

            mergedRanges.Add(currentRange);

            if (lastMergedRangeIdx == 0) // If zero, no ranges were merged so we move on to the next range
            {
                i++;
            }
            else // else move to the next range that wasn't merged
            {
                i = lastMergedRangeIdx;
            }
        }

        foreach (var range in mergedRanges)
        {
            validIds += range.max - range.min + 1;
        }

        return validIds;
    }

    public static void Execute()
    {
        using StringReader? sr = new(File.ReadAllText("./day05/input.txt"));

        IngredientsDatabase ingredientsDatabase = ParseInput(sr);

        Console.WriteLine($"[AoC 2025 - Day 05 - Part 1] Result: {Part1(ingredientsDatabase)}");
        Console.WriteLine($"[AoC 2025 - Day 05 - Part 2] Result: {Part2(ingredientsDatabase)}");
    }
}