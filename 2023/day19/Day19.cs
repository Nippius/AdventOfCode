namespace AdventOfCode2023;

public static class Day19
{
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

    private class Condition
    {
        private char partCategory;
        private char @operator;
        private int ranking;

        public Condition(string conditionDescriptor)
        {
            partCategory = conditionDescriptor[0];
            @operator = conditionDescriptor[1];
            ranking = int.Parse(conditionDescriptor[2..]);
        }

        public bool Matches(Part part)
        {
            return @operator switch
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

        public int GetRange(char category, int range)
        {
            if (category != partCategory)
            {
                return range;
            }
            else
            {
                return @operator switch
                {
                    '<' => (range > ranking) ? ranking - 1 : range,
                    '>' => (range > ranking) ? range - ranking + 1 : range,
                    _ => range
                };
            }
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
            Workflow newWorkflow = new Workflow(line);
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

    private static readonly Stack<(string name, int X, int M, int A, int S)> pendingWorkflows = [];

    private static long CountDistinctCombinationsOfRatings(Dictionary<string, Workflow> workflows)
    {
        pendingWorkflows.Push(("in", 4000, 4000, 4000, 4000));

        long combinationsCount = 4000L * 4000L * 4000L * 4000L;

        while (pendingWorkflows.Count > 0)
        {
            var (name, X, M, A, S) = pendingWorkflows.Pop();

            if (name == "R")
            {
                combinationsCount -= X * M * A * S;
            }
            else if (name == "A")
            {
                // do nothing...?
            }
            else
            {
                Workflow currentWorkflow = workflows[name];

                
                foreach (Rule rule in currentWorkflow.Rules)
                {
                    if (rule.Condition is not null)
                    {
                        int tempX = rule.Condition.GetRange('x', X);
                        int tempM = rule.Condition.GetRange('m', X);
                        int tempA = rule.Condition.GetRange('a', X);
                        int tempS = rule.Condition.GetRange('s', X);

                        pendingWorkflows.Push((rule.destinationWorkflow, tempX, tempM, tempA, tempS));
                    }
                    else
                    {
                        pendingWorkflows.Push((rule.destinationWorkflow, X, M, A, S));
                    }
                }
            }
        }



        return combinationsCount;
    }

    public static void Execute()
    {
        StringReader sr = new(File.ReadAllText("./day19/input.txt"));

        ParseInput(sr, out Dictionary<string, Workflow> workflows, out List<Part> parts);

        Console.WriteLine($"[AoC 2023 - Day 19 - Part 1] Result: {SumRatingOfAllAcceptedParts(workflows, parts)}");
        Console.WriteLine($"[AoC 2023 - Day 19 - Part 2] Result: {CountDistinctCombinationsOfRatings(workflows)}");
    }
}
