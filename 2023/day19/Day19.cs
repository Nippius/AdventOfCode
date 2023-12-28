namespace AdventOfCode2023;

public static class Day19
{
    private record Range(int Low = 1, int High = 4000) { };

    private class Part
    {
        public int X { get; init; }
        public int M { get; init; }
        public int A { get; init; }
        public int S { get; init; }

        public Part(string partDescriptor)
        {
            string[] categories = partDescriptor[1..^1].Split(',');
            foreach (string category in categories)
            {
                // [0] -> category | [1] -> ranking
                string[] categoryDescriptor = category.Split('=');

                switch (categoryDescriptor[0])
                {
                    case "x": X = int.Parse(categoryDescriptor[1]); break;
                    case "m": M = int.Parse(categoryDescriptor[1]); break;
                    case "a": A = int.Parse(categoryDescriptor[1]); break;
                    case "s": S = int.Parse(categoryDescriptor[1]); break;
                }
            }
        }
    }

    private class Workflow
    {
        public string Name { get; init; }
        public List<Rule> Rules { get; } = [];
        public Workflow(string workflowDescriptor)
        {
            int ruleStartDelimiterIndex = workflowDescriptor.IndexOf('{');
            Name = workflowDescriptor[..ruleStartDelimiterIndex];
            string[] rulesDescriptor = workflowDescriptor
                .Substring(ruleStartDelimiterIndex + 1, workflowDescriptor.Length - ruleStartDelimiterIndex - 2)
                .Split(",");

            foreach (string ruleDescriptor in rulesDescriptor)
            {
                Rules.Add(new Rule(ruleDescriptor));
            }
        }

        public string ApplyRulesToPart(Part part)
        {
            foreach (Rule rule in Rules)
            {
                if (rule.AppliesToPart(part, out string destinationWorkFlow))
                {
                    return destinationWorkFlow;
                }
            }
            return "R"; // If no rules apply, reject the part but this shouldn't happen
        }
    }

    private class Rule
    {
        public Condition Condition { get; init; }
        public readonly string destinationWorkflow;

        public Rule(string ruleDescriptor)
        {
            string[] ruleData = ruleDescriptor.Split(':');
            if (ruleData.Length == 2)
            {
                Condition = new Condition(ruleData[0]);
                destinationWorkflow = ruleData[1];
            }
            else
            {
                destinationWorkflow = ruleData[0];
            }
        }

        public bool AppliesToPart(Part part, out string destinationWorkflow)
        {
            destinationWorkflow = string.Empty;
            if (Condition is null || Condition.Matches(part))
            {
                destinationWorkflow = this.destinationWorkflow;
                return true;
            }
            return false;
        }
    }

    private class Condition(string conditionDescriptor)
    {
        private readonly char partCategory = conditionDescriptor[0];
        private readonly int ranking = int.Parse(conditionDescriptor[2..]);
        public char Operator { get; init; } = conditionDescriptor[1];

        public bool Matches(Part part)
        {
            return Operator switch
            {
                '<' => partCategory switch
                {
                    'x' => part.X < ranking,
                    'm' => part.M < ranking,
                    'a' => part.A < ranking,
                    's' => part.S < ranking,
                    _ => false
                },
                '>' => partCategory switch
                {
                    'x' => part.X > ranking,
                    'm' => part.M > ranking,
                    'a' => part.A > ranking,
                    's' => part.S > ranking,
                    _ => false
                },
                _ => false
            };
        }

        public (Range newCurrentRange, Range nextRange) GetRange(char category, Range currentRange)
        {
            Range newCurrentRange = currentRange;
            Range nextRange = currentRange;

            if (category != partCategory)
            {
                return (newCurrentRange, nextRange);
            }

            if (Operator == '<')
            {
                if (ranking > currentRange.Low && ranking < currentRange.High)
                {
                    newCurrentRange = currentRange with { Low = ranking, High = currentRange.High };
                    nextRange = currentRange with { Low = currentRange.Low, High = ranking - 1 };
                }
                // else if (ranking < currentRange.Low)
                // {
                //     newCurrentRange = currentRange;
                //     nextRange = currentRange with { Low = 1, High = 1 };
                // }
                // else if (ranking > currentRange.High)
                // {
                //     newCurrentRange = currentRange with { Low = 1, High = 1 };
                //     nextRange = currentRange;
                // }
            }
            else if (Operator == '>')
            {
                if (ranking > currentRange.Low && ranking < currentRange.High)
                {
                    newCurrentRange = currentRange with { Low = currentRange.Low, High = ranking };
                    nextRange = currentRange with { Low = ranking + 1, High = currentRange.High };
                }
                // else if (ranking < currentRange.Low)
                // {
                //     newCurrentRange = currentRange with { Low = 1, High = 1 };
                //     nextRange = currentRange;
                // }
                // else if (ranking > currentRange.High)
                // {
                //     newCurrentRange = currentRange;
                //     nextRange = currentRange with { Low = 1, High = 1 };
                // }
            }

            return (newCurrentRange, nextRange);
        }
    }

    private static void ParseInput(StringReader sr, out Dictionary<string, Workflow> workflows, out List<Part> parts)
    {
        workflows = [];
        parts = [];
        string line = sr.ReadLine();

        // Create workflows
        while (line != string.Empty)
        {
            Workflow newWorkflow = new(line);
            workflows[newWorkflow.Name] = newWorkflow;
            line = sr.ReadLine();
        }

        line = sr.ReadLine();

        // Create parts
        while (line != null)
        {
            parts.Add(new Part(line));
            line = sr.ReadLine();
        }
    }

    private static int SumRatingOfAllAcceptedParts(Dictionary<string, Workflow> workflows, List<Part> parts)
    {
        int ratingsSum = 0;

        foreach (Part p in parts)
        {
            Workflow workflow = workflows["in"];

            while (true)
            {
                string nextWorkflow = workflow.ApplyRulesToPart(p);
                if (nextWorkflow == "A")
                {
                    ratingsSum += p.X + p.M + p.A + p.S;
                    goto NextPart;
                }
                else if (nextWorkflow == "R")
                {
                    goto NextPart; // Just an excude to use a goto :p
                }
                else
                {
                    workflow = workflows[nextWorkflow];
                }
            }
        NextPart:
            continue;
        }
        return ratingsSum;
    }

    private static ulong CountDistinctCombinationsOfRatings(Dictionary<string, Workflow> workflows)
    {
        Stack<(string name, Range X, Range M, Range A, Range S)> pendingWorkflows = [];
        pendingWorkflows.Push(("in", new(), new(), new(), new()));

        ulong combinations = 0;

        while (pendingWorkflows.Count > 0)
        {
            var (name, X, M, A, S) = pendingWorkflows.Pop();

            if (name == "R")
            {
                // do nothing...
            }
            else if (name == "A")
            {
                combinations += (ulong)(X.High + 1 - X.Low)
                             * (ulong)(M.High + 1 - M.Low)
                             * (ulong)(A.High + 1 - A.Low)
                             * (ulong)(S.High + 1 - S.Low);
            }
            else
            {
                Workflow currentWorkflow = workflows[name];

                foreach (Rule rule in currentWorkflow.Rules)
                {
                    if (rule.Condition is not null)
                    {
                        var tempX = rule.Condition.GetRange('x', X);
                        var tempM = rule.Condition.GetRange('m', M);
                        var tempA = rule.Condition.GetRange('a', A);
                        var tempS = rule.Condition.GetRange('s', S);

                        pendingWorkflows.Push((rule.destinationWorkflow, tempX.nextRange, tempM.nextRange, tempA.nextRange, tempS.nextRange));

                        X = tempX.newCurrentRange;
                        M = tempM.newCurrentRange;
                        A = tempA.newCurrentRange;
                        S = tempS.newCurrentRange;
                    }
                    else
                    {
                        pendingWorkflows.Push((rule.destinationWorkflow, X, M, A, S));
                    }
                }
            }
        }

        return combinations;
    }

    public static void Execute()
    {
        StringReader sr = new(File.ReadAllText("./day19/input.txt"));

        ParseInput(sr, out Dictionary<string, Workflow> workflows, out List<Part> parts);

        Console.WriteLine($"[AoC 2023 - Day 19 - Part 1] Result: {SumRatingOfAllAcceptedParts(workflows, parts)}");
        Console.WriteLine($"[AoC 2023 - Day 19 - Part 2] Result: {CountDistinctCombinationsOfRatings(workflows)}");
    }
}