using System.Text.RegularExpressions;

namespace AdventOfCode2023;

public static partial class Day08
{
    private const int NODE_NAME_INDEX = 0;
    private const int LEFT_NEXT_NODE_NAME_INDEX = 1;
    private const int RIGHT_NEXT_NODE_NAME_INDEX = 2;

    [GeneratedRegex(@"\W+")]
    private static partial Regex nodeSplitRegex();

    private static void ParseInput(string input, out string instructions,
        out Dictionary<string, (string LeftNode, string RightNode)> nodes,
        out IReadOnlyCollection<(string node, int stepsToEnd)> startingNodes)
    {
        nodes = [];
        List<(string node, int stepsToEnd)> startingNodesTmp = [];
        StringReader sr = new(input);
        instructions = sr.ReadLine();
        string line = sr.ReadLine(); // Empty line
        while (line != null)
        {
            if (line != string.Empty)
            {
                string[] nodeInfo = nodeSplitRegex().Split(line);

                nodes.Add(nodeInfo[NODE_NAME_INDEX], (nodeInfo[LEFT_NEXT_NODE_NAME_INDEX], nodeInfo[RIGHT_NEXT_NODE_NAME_INDEX]));
                if (nodeInfo[NODE_NAME_INDEX].EndsWith('A'))
                {
                    startingNodesTmp.Add(new(nodeInfo[NODE_NAME_INDEX], NODE_NAME_INDEX));
                }
            }
            line = sr.ReadLine();
        }

        startingNodes = [.. startingNodesTmp];
    }

    private static int CountSteps(string input)
    {
        int steps = NODE_NAME_INDEX;
        string currentNode = "AAA"; // AKA Starting node
        ParseInput(input,
            out string instructions,
            out Dictionary<string, (string LeftNode, string RightNode)> nodes,
            out _);

        // Convert this to a function that starts at a node and finds the end node (either ZZZ or ends with Z)
        int instructionIndex = NODE_NAME_INDEX;
        while (currentNode != "ZZZ")
        {
            (string LeftNode, string RightNode) nodeInfo = nodes[currentNode];
            switch (instructions[instructionIndex])
            {
                case 'L': currentNode = nodeInfo.LeftNode; break;
                case 'R': currentNode = nodeInfo.RightNode; break;
                default: break;
            }
            instructionIndex = (instructionIndex + 1) % instructions.Length;
            steps++;
        }
        // end function

        return steps;
    }

    private static int CountStepsUntilNodesEndingInZ(string input)
    {
        int steps = NODE_NAME_INDEX;

        ParseInput(input,
            out string instructions,
            out Dictionary<string, (string LeftNode, string RightNode)> nodes,
            out IReadOnlyCollection<(string node, int stepsToEnd)> startingNodes);

        // TODO

        return steps;
    }

    public static void Execute()
    {
        string input = File.ReadAllText("./day08/input.txt");

        Console.WriteLine($"[AoC 2023 - Day 08 - Part 1] Result: {CountSteps(input)}");
        Console.WriteLine($"[AoC 2023 - Day 08 - Part 2] Result: {CountStepsUntilNodesEndingInZ(input)}");
    }
}
