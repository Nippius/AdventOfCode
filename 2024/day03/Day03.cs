using System.Text.RegularExpressions;

namespace AdventOfCode2024;

public static partial class Day03
{
    [GeneratedRegex("""(?<inst>(?<op>mul)\((?<op1>\d+),(?<op2>\d+)\))|(?<op>do)\(\)|(?<op>don\'t)\(\)""")]
    private static partial Regex CorruptedMemoryInstructionParser();

    private static void ExecuteInstructionsInCorruptedMemory(string corruptedMemory, out int partOneResult, out int partTwoResult)
    {
        bool instructionsAreEnabled = true;
        partOneResult = 0;
        partTwoResult = 0;

        foreach (Match instruction in CorruptedMemoryInstructionParser().Matches(corruptedMemory))
        {
            GroupCollection groups = instruction.Groups;
            if (groups["op"].Value == "do") { instructionsAreEnabled = true; }
            else if (groups["op"].Value == "don't") { instructionsAreEnabled = false; }
            else
            {
                int instructionExecutionResult = int.Parse(groups["op1"].Value) * int.Parse(groups["op2"].Value);
                partOneResult += instructionExecutionResult;
                if(instructionsAreEnabled){ partTwoResult += instructionExecutionResult;}
            }
        }
    }

    public static void Execute()
    {
        // Assume the \n in the file are part of the corruption. Think of it as single long string
        string memoryContents = File.ReadAllText("./day03/input.txt");

        ExecuteInstructionsInCorruptedMemory(memoryContents, out int partOneResult, out int partTwoResult);

        Console.WriteLine($"[AoC 2024 - Day 03 - Part 1] Result: {partOneResult}");
        Console.WriteLine($"[AoC 2024 - Day 03 - Part 2] Result: {partTwoResult}");
    }
}