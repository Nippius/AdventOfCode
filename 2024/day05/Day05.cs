namespace AdventOfCode2024;

public static class Day05
{
    private record PageOrderingRule(int FirstPageNumber, int SecondPageNumber);

    private static void ParseInput(StringReader sr, out List<int[]> updates, out Dictionary<int, HashSet<PageOrderingRule>> pageOrderingRules)
    {
        updates = [];
        pageOrderingRules = [];
        bool parsingPageOrderingRules = true;

        using (sr)
        {
            string? line = sr?.ReadLine();
            while (line is not null)
            {
                if (parsingPageOrderingRules)
                {
                    if (line == string.Empty)
                    {
                        parsingPageOrderingRules = false;
                    }
                    else
                    {
                        int[] parsedPageRule = [.. line.Split("|").Select(int.Parse)];
                        PageOrderingRule rule = new(parsedPageRule[0], parsedPageRule[1]);

                        // Add the same rule twice, once for each value in the rule because later the validation must be done forward and backward
                        if (pageOrderingRules.TryGetValue(rule.FirstPageNumber, out HashSet<PageOrderingRule>? value))
                        { value.Add(rule); }
                        else
                        { pageOrderingRules[rule.FirstPageNumber] = [rule]; }

                        if (pageOrderingRules.TryGetValue(rule.SecondPageNumber, out value))
                        { value.Add(rule); }
                        else
                        { pageOrderingRules[rule.SecondPageNumber] = [rule]; }
                    }
                }
                else
                {
                    updates.Add([.. line.Split(",").Select(int.Parse).ToArray()]);
                }

                line = sr?.ReadLine();
            }
        }
    }

    private static HashSet<PageOrderingRule> GetRulesBrokenByUpdate(int[] update, Dictionary<int, HashSet<PageOrderingRule>> pageOrderingRules)
    {
        HashSet<PageOrderingRule> brokenRules = [];

        for (int i = 0; i < update.Length; i++)
        {
            HashSet<PageOrderingRule> rules = pageOrderingRules[update[i]];
            foreach (PageOrderingRule rule in rules)
            {
                if (rule.FirstPageNumber == update[i]) // search backward (reversed to look for fails)
                {
                    for (int j = i - 1; j >= 0; j--)
                    {
                        if (update[j] == rule.SecondPageNumber)
                        {
                            brokenRules.Add(rule);
                        }
                        // if we get to the end, nothing could be concluded because the number could be missing
                    }
                }
                else if (rule.SecondPageNumber == update[i]) // search forward (reversed to look for fails)
                {
                    for (int j = i + 1; j < update.Length; j++)
                    {
                        if (update[j] == rule.FirstPageNumber)
                        {
                            brokenRules.Add(rule);
                        }
                        // if we get to the end, nothing could be concluded because the number could be missing
                    }
                }
            }
        }

        return brokenRules;
    }

    private static void ReOrderUpdate(int[] update, Dictionary<int, HashSet<PageOrderingRule>> pageOrderingRules, HashSet<PageOrderingRule> brokenRules)
    {
        while (brokenRules.Count is not 0)
        {
            for (int i = 0; i < update.Length; i++)
            {
                foreach (PageOrderingRule rule in brokenRules)
                {
                    if (rule.FirstPageNumber == update[i]) // search backward (reversed to look for fails)
                    {
                        for (int j = i - 1; j >= 0; j--)
                        {
                            if (update[j] == rule.SecondPageNumber)
                            {
                                (update[j], update[i]) = (update[i], update[j]); // Uuuu fancy way of swaping values 👀
                            }
                        }
                    }
                    else if (rule.SecondPageNumber == update[i]) // search forward (reversed to look for fails)
                    {
                        for (int j = i + 1; j < update.Length; j++)
                        {
                            if (update[j] == rule.FirstPageNumber)
                            {
                                (update[j], update[i]) = (update[i], update[j]);
                            }
                        }
                    }
                }
            }
            // After applying all the rules, the new update might break new rules, so we may need to order again
            brokenRules = GetRulesBrokenByUpdate(update, pageOrderingRules);
        }
    }

    private static void GetOrderedUpdates(
        List<int[]> updates,
        Dictionary<int, HashSet<PageOrderingRule>> pageOrderingRules,
        out List<int[]> orderedUpdates,
        out List<int[]> reOrderedUpdates)
    {
        orderedUpdates = [];
        reOrderedUpdates = [];

        foreach (int[] update in updates)
        {
            HashSet<PageOrderingRule> brokenRules = GetRulesBrokenByUpdate(update, pageOrderingRules);
            if (brokenRules.Count is not 0)
            {
                ReOrderUpdate(update, pageOrderingRules, brokenRules); // Reorder update in place
                reOrderedUpdates.Add(update);
            }
            else
            {
                orderedUpdates.Add(update);
            }
        }
    }

    private static (int, int) SumMidlePageNumbers(List<int[]> updates, Dictionary<int, HashSet<PageOrderingRule>> pageOrderingRules)
    {
        int middlePagesSumForOrderedUpdates = 0;
        int middlePagesSumForReOrderedUpdates = 0;

        GetOrderedUpdates(updates, pageOrderingRules, out List<int[]> orderedUpdates, out List<int[]> reOrderedUpdates);

        foreach (int[] update in orderedUpdates)
        {
            middlePagesSumForOrderedUpdates += update[update.Length / 2]; // All updates are odd numbered
        }

        foreach (int[] update in reOrderedUpdates)
        {
            middlePagesSumForReOrderedUpdates += update[update.Length / 2]; // All updates are odd numbered
        }

        return (middlePagesSumForOrderedUpdates, middlePagesSumForReOrderedUpdates);
    }

    public static void Execute()
    {
        ParseInput(new(File.ReadAllText("./day05/input.txt")), out List<int[]> updates, out Dictionary<int, HashSet<PageOrderingRule>> pageOrderingRules);

        (int sumForOrderedUpdates, int sumForReOrderedUdpates) = SumMidlePageNumbers(updates, pageOrderingRules);

        Console.WriteLine($"[AoC 2024 - Day 05 - Part 1] Result: {sumForOrderedUpdates}");
        Console.WriteLine($"[AoC 2024 - Day 05 - Part 2] Result: {sumForReOrderedUdpates}");
    }
}