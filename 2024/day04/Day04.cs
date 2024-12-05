namespace AdventOfCode2024;

using System;
using Grid = char[,];
using Position = (int x, int y); // Where we start our search

public static partial class Day04
{
    private static int MAX_GRID_SIZE = 141;

    private static void ParseGridAndFindAllLocationsNeeded(
        StringReader sr,
        out Grid charGrid,
        out IReadOnlyList<Position> listOfXLocations,
        out IReadOnlyList<Position> listOfALocations)
    {
        charGrid = new char[MAX_GRID_SIZE, MAX_GRID_SIZE];
        List<Position> tempListOfXLocations = [];
        List<Position> tempListOfALocations = [];

        int x = 0;
        string? line = sr.ReadLine();
        while (line is not null)
        {
            int y = 0;
            if (line != string.Empty)
            {
                foreach (char c in line)
                {
                    charGrid[x, y] = c;

                    if (c == 'X')
                    {
                        tempListOfXLocations.Add(new(x, y));
                    }
                    else if (c == 'A')
                    {
                        tempListOfALocations.Add(new(x, y));
                    }

                    y++;
                }
            }
            line = sr?.ReadLine();
            x++;
        }

        listOfXLocations = tempListOfXLocations.AsReadOnly();
        listOfALocations = tempListOfALocations.AsReadOnly();
    }

    /*
        Imagine a star formed by the word "XMAS":

            S . . S . . S
            . A . A . A .
            . . M M M . .
            S A M X M A S
            . . M M M . .
            . A . A . A .
            S . . S . . S

        What this method does is apply this star as a sort of mask centered on the starCenter position passed to the method
            and count how many "arms" land on the correct letters in the grid, returning said count
    */
    private static int CountXmasOccurencesFromPosition(Grid charGrid, Position starCenter)
    {
        char[] xmas = ['X', 'M', 'A', 'S'];

        StarArm[] star = [
            new StarArm(starCenter, (-1,0)),    // Up
            new StarArm(starCenter, (-1,1)),    // UpRight 
            new StarArm(starCenter, (0,1)),     // Right
            new StarArm(starCenter, (1,1)),     // RightDown
            new StarArm(starCenter, (1,0)),     // Down
            new StarArm(starCenter, (1,-1)),    // DownLeft
            new StarArm(starCenter, (0,-1)),    // Left
            new StarArm(starCenter, (-1,-1))    // LeftUp
        ];

        // Check for the expected letter in each arm going outward...
        for (int i = 0; i < xmas.Length; i++)
        {
            char c = xmas[i];
            foreach (StarArm arm in star) // ...and clockwise
            {
                if (arm.ContainsXmas == true) // while it's still matching "XMAS"
                {
                    // Arm outside the bounds of the grid so mark it has not containg "XMAS"
                    if (arm.X < 0 || arm.Y < 0 || arm.X >= MAX_GRID_SIZE || arm.Y >= MAX_GRID_SIZE)
                    {
                        arm.MarkXmasNotFound();
                        continue;
                    }

                    if (charGrid[arm.X, arm.Y] != c)
                    {
                        arm.MarkXmasNotFound();
                    }

                    arm.IncrementPosition();
                }
            }
        }

        return star.Where(arm => arm.ContainsXmas).Count();
    }

    private static int CountXDashMasOccurencesFromPosition(Grid charGrid, Position starCenter)
    {
        // If this is true, the cross would end up out-of-bounds
        if (starCenter.x < 1
            || starCenter.y < 1
            || starCenter.x >= MAX_GRID_SIZE - 1
            || starCenter.y >= MAX_GRID_SIZE - 1)
        {
            return 0;
        }

        Grid xDashMas = {
            {'M','.','S'},
            {'.','A','.'},
            {'M','.','S'}
        };

        // Meh... do it the lazy way and check manually all combinations
        for (int i = 0; i < 4; i++) // Try all variations of the cross
        {
            if (charGrid[starCenter.x - 1, starCenter.y - 1] == xDashMas[0, 0])
            {
                if (charGrid[starCenter.x - 1, starCenter.y + 1] == xDashMas[0, 2])
                {
                    if (charGrid[starCenter.x + 1, starCenter.y + 1] == xDashMas[2, 2])
                    {
                        if (charGrid[starCenter.x + 1, starCenter.y - 1] == xDashMas[2, 0])
                        {
                            return 1;
                        }
                    }
                }
            }
            RotateGrid(ref xDashMas);
        }

        return 0;

        static void RotateGrid(ref Grid grid)
        {
            char temp = grid[0, 0];
            grid[0, 0] = grid[0, 2];
            grid[0, 2] = grid[2, 2];
            grid[2, 2] = grid[2, 0];
            grid[2, 0] = temp;
        }
    }

    private static int CountXmasOccurences(
        Grid charGrid,
        IReadOnlyList<Position> locationsToSearch,
        Func<Grid, Position, int> searchFunction)
    {
        int occurencesCounter = 0;

        foreach (Position startingLocation in locationsToSearch)
        {
            occurencesCounter += searchFunction(charGrid, startingLocation);
        }

        return occurencesCounter;
    }

    public static void Execute()
    {
        using StringReader? sr = new(File.ReadAllText("./day04/input.txt"));

        ParseGridAndFindAllLocationsNeeded(
            sr,
            out Grid charGrid,
            out IReadOnlyList<Position> xLocations,
            out IReadOnlyList<Position> aLocations);

        Console.WriteLine($"[AoC 2024 - Day 04 - Part 1] Result: {CountXmasOccurences(charGrid, xLocations, CountXmasOccurencesFromPosition)}");
        Console.WriteLine($"[AoC 2024 - Day 04 - Part 2] Result: {CountXmasOccurences(charGrid, aLocations, CountXDashMasOccurencesFromPosition)}");
    }
}