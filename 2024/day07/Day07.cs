using System.Collections.ObjectModel;
using System.Linq;

namespace AdventOfCode2024;

public static class Day07
{
    private record Equation(long Value, long[] Numbers);

    private static ReadOnlyCollection<Equation> ParseInput(StringReader sr)
    {
        IList<Equation> equations = [];
        string? line = sr?.ReadLine();
        while (line is not null)
        {
            if (line != string.Empty)
            {
                string[] input = line.Split(": ");
                long testValue = long.Parse(input[0]);
                long[] numbers = [.. input[1].Split(" ").Select(long.Parse)];
                equations.Add(new Equation(testValue, numbers));
            }
            line = sr?.ReadLine();
        }

        return equations.AsReadOnly();
    }

    private static Dictionary<(int, int), List<List<char>>> OperationSetsCache = [];

    // example: 81 40 27 -> numberOfOperations = 2
    private static List<List<char>> GenerateOperationSets(int numberOfDistinctOperators, int numberOfOperations)
    {
        List<List<char>> operationSets;

        if (OperationSetsCache.TryGetValue((numberOfDistinctOperators, numberOfOperations), out operationSets!) is false)
        {
            operationSets = [];
            char[] operators = ['-', '/', '|'];
            int[] counters = new int[numberOfOperations];
            int totalNumberOfOperationSets = (int)Math.Pow(numberOfDistinctOperators, numberOfOperations);

            for (int i = 0; i < totalNumberOfOperationSets; i++)
            {
                List<char> operationSet = [];

                for (int j = 0; j < counters.Length; j++)
                {
                    operationSet.Add(operators[counters[j]]);
                }

                operationSets.Add(operationSet);

                IncrementCounters(counters, numberOfDistinctOperators);
            }

            OperationSetsCache.Add((numberOfDistinctOperators, numberOfOperations), operationSets);
        }

        return operationSets;
    }

    private static void IncrementCounters(int[] counters, int numberOfDistinctOperators)
    {
        //  full adder but for a base-N numeral system
        //  binary version: 
        //      - https://en.wikipedia.org/wiki/Adder_(electronics)
        //      - https://en.wikipedia.org/wiki/Binary_number#Addition
        int carry = 0;
        for (int j = counters.Length - 1; j >= 0; j--)
        {
            // If we are the least significant counter also add 1
            if (j == counters.Length - 1)
            {
                counters[j] = counters[j] + 1 + carry;
            }

            // Add carry from previous result
            counters[j] = counters[j] + carry;

            // Calculate carry if needed
            // this only works because we are always adding 1.
            //      in practise, the carry and the current values
            //      should be properly calculated
            if (counters[j] == numberOfDistinctOperators)
            {
                counters[j] = 0;
                carry = 1;
            }
            else
            {
                carry = 0;
            }
        }
    }

    private static bool EquationMatchesTestValue(Equation equation, int numberOfDistinctOperators)
    {
        var operationSets = GenerateOperationSets(numberOfDistinctOperators, equation.Numbers.Length - 1);

        foreach (List<char> operationSet in operationSets)
        {
            int position = equation.Numbers.Length - 1;
            long intermediateResult = equation.Value;

            // OperationSets are simetric (in a sense) so, if it's easier to reason about,
            //  think of this loop as applying the operations in reverse order.
            //  Example:
            //      For 3 operations with 2 positions, the sets will be:
            //          +, +
            //          +, *
            //          +, |
            //          *, +
            //          *, *
            //          *, |
            //          |, +
            //          |, *
            //          |, |
            //      So it doesn't matter if we star by doing |, * or *, |
            //          because both exist in the set and both will be tested
            foreach (char operation in operationSet)
            {
                switch (operation)
                {
                    case '-':
                        {
                            intermediateResult -= equation.Numbers[position];
                            break;
                        }
                    case '/':
                        {
                            if (intermediateResult % equation.Numbers[position] == 0)
                            {
                                intermediateResult /= equation.Numbers[position];
                            }
                            else
                            {
                                goto nextOperation; // Skip remaining operations because they aren't valid anyway
                            }
                            break;
                        }
                    case '|':
                        {
                            // Splitting a number without using strings
                            // https://www.tutorialspoint.com/check-if-a-number-ends-with-another-number-or-not
                            long ending = equation.Numbers[position];
                            long digits = (long)Math.Floor(Math.Log10(ending) + 1);
                            long divisor = (long)Math.Pow(10, digits);
                            if (intermediateResult % divisor == ending)
                            {
                                intermediateResult = (long)Math.Floor((double)(intermediateResult / divisor));
                            }
                            else
                            {
                                goto nextOperation; // Skip remaining operations because they aren't valid anyway
                            }
                            break;
                        }
                    default: break;
                }

                if (intermediateResult == equation.Numbers[0])
                {
                    return true;
                }
                else if (intermediateResult < equation.Numbers[0])
                {
                    break;
                }

                position--;
            }
        nextOperation:
            ;
        }

        return false;
    }

    private static long SumValidTestValues(ReadOnlyCollection<Equation> equations, int numberOfDistinctOperators)
    {
        return equations.Where(eq => EquationMatchesTestValue(eq, numberOfDistinctOperators)).Sum(eq => eq.Value);
    }

    public static void Execute()
    {
        using StringReader? sr = new(File.ReadAllText("./day07/input.txt"));

        ReadOnlyCollection<Equation> equations = ParseInput(sr);

        Console.WriteLine($"[AoC 2024 - Day 07 - Part 1] Result: {SumValidTestValues(equations, 2)}");
        Console.WriteLine($"[AoC 2024 - Day 07 - Part 2] Result: {SumValidTestValues(equations, 3)}");
    }
}