using System.Text.RegularExpressions;

namespace AdventOfCode2024;

public static partial class Day03
{
    [GeneratedRegex("""(?<inst>(?<op>mul)\((?<op1>\d+),(?<op2>\d+)\))|(?<op>do)\(\)|(?<op>don\'t)\(\)""")]
    private static partial Regex CorruptedMemoryInstructionParser();

    private static void ExecuteInstructionsInCorruptedMemory(string corruptedMemory, out int multiplicationsSum, out int multiplicationsSumWithEnableInstructions)
    {
        bool instructionsAreEnabled = true;
        multiplicationsSum = 0;
        multiplicationsSumWithEnableInstructions = 0;

        foreach (Match instruction in CorruptedMemoryInstructionParser().Matches(corruptedMemory))
        {
            GroupCollection groups = instruction.Groups;
            if (groups["op"].Value == "do") { instructionsAreEnabled = true; }
            else if (groups["op"].Value == "don't") { instructionsAreEnabled = false; }
            else
            {
                int instructionExecutionResult = int.Parse(groups["op1"].Value) * int.Parse(groups["op2"].Value);
                multiplicationsSum += instructionExecutionResult;
                if(instructionsAreEnabled){ multiplicationsSumWithEnableInstructions += instructionExecutionResult;}
            }
        }
    }

    public static void Execute()
    {
        // Assume the \n in the file are part of the corruption. Think of it as single long string
        string memoryContents = File.ReadAllText("./day03/input.txt");

        ExecuteInstructionsInCorruptedMemory(memoryContents, out int multiplicationsSum, out int multiplicationsSumWithEnableInstructions);

        Console.WriteLine($"[AoC 2024 - Day 03 - Part 1] Result: {multiplicationsSum}");
        Console.WriteLine($"[AoC 2024 - Day 03 - Part 2] Result: {multiplicationsSumWithEnableInstructions}");
    }
}