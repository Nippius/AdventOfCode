namespace AdventOfCode2025.Day04;

using System;
using Grid = char[,];

public static class Day04
{
    private static int MAX_GRID_SIZE = 138;

    private static Grid ParseInput(StringReader sr)
    {
        using (sr)
        {
            Grid grid = new char[MAX_GRID_SIZE, MAX_GRID_SIZE];
            int x = 0;
            List<string> input = [];
            string? line = sr?.ReadLine();
            while (line is not null)
            {
                if (line != string.Empty)
                {
                    int y = 0;
                    foreach (char c in line)
                    {
                        grid[x, y] = c;
                        y++;
                    }
                }
                line = sr?.ReadLine();
                x++;
            }
            return grid;
        }
    }

    private static int CountSurroundingRolls(Grid grid, int x, int y)
    {
        int surroundingRollsCount = 0;

        if (x - 1 >= 0 && grid[x - 1, y] == '@') // Up
        {
            surroundingRollsCount++;
        }

        if (x - 1 >= 0 && y + 1 < MAX_GRID_SIZE && grid[x - 1, y + 1] == '@') // Up/Right
        {
            surroundingRollsCount++;
        }

        if (y + 1 < MAX_GRID_SIZE && grid[x, y + 1] == '@') // Right
        {
            surroundingRollsCount++;
        }

        if (x + 1 < MAX_GRID_SIZE && y + 1 < MAX_GRID_SIZE && grid[x + 1, y + 1] == '@') // Down/Right
        {
            surroundingRollsCount++;
        }

        if (x + 1 < MAX_GRID_SIZE && grid[x + 1, y] == '@') // Down
        {
            surroundingRollsCount++;
        }

        if (x + 1 < MAX_GRID_SIZE && y - 1 >= 0 && grid[x + 1, y - 1] == '@') // Down/Left
        {
            surroundingRollsCount++;
        }

        if (y - 1 >= 0 && grid[x, y - 1] == '@') // Left
        {
            surroundingRollsCount++;
        }

        if (x - 1 >= 0 && y - 1 >= 0 && grid[x - 1, y - 1] == '@') // Up/Left
        {
            surroundingRollsCount++;
        }

        return surroundingRollsCount;
    }

    private static List<(int x, int y)> GetAccessibleRolls(Grid grid)
    {
        List<(int x, int y)> rollsToRemove = [];
        for (int x = 0; x < MAX_GRID_SIZE; x++)
        {
            for (int y = 0; y < MAX_GRID_SIZE; y++)
            {
                if (grid[x, y] == '@')
                {
                    // A roll is accessible if it has less than 4 other rolls next to it
                    if (CountSurroundingRolls(grid, x, y) < 4)
                    {
                        rollsToRemove.Add((x, y));
                    }
                }
            }
        }
        return rollsToRemove;
    }

    private static int Part1(Grid grid)
    {
        return GetAccessibleRolls(grid).Count;
    }

    private static int Part2(Grid grid)
    {
        int removedRolls = 0;
        List<(int x, int y)> rollsToRemove;
        do
        {
            rollsToRemove = GetAccessibleRolls(grid);

            foreach (var (x, y) in rollsToRemove) { grid[x, y] = '.'; } // "Remove" rolls

            removedRolls += rollsToRemove.Count;
        } while (rollsToRemove.Count > 0);

        return removedRolls;
    }

    public static void Execute()
    {
        using StringReader? sr = new(File.ReadAllText("./day04/input.txt"));

        Grid grid = ParseInput(sr);

        Console.WriteLine($"[AoC 2025 - Day 04 - Part 1] Result: {Part1(grid)}");
        Console.WriteLine($"[AoC 2025 - Day 04 - Part 2] Result: {Part2(grid)}");
    }
}