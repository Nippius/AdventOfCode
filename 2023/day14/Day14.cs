// TODO find a better sollution to Part2 using Memoization and Feature scaling
namespace AdventOfCode2023;

public static class Day14
{
    delegate void ActionRef<T0, T1, T2>(T0 board, T1 coord, T2 dir);

    private enum TiltDirection
    {
        North,
        South,
        East,
        West
    }

    private const char ROUNDED_ROCK_SYMB = 'O';
    private const int ROUNDED_ROCK = 1;
    private const char EMPTY_SPACE_SYMB = '.';
    private const int EMPTY_SPACE = 0;
    private const char SQUARE_ROCK_SYMB = '#';   // also -1
    private const int SQUARE_ROCK = -1;   // also -1

    private static void PrintBoard(int[,] board)
    {
        for (int x = 0; x < board.GetLength(0); x++)
        {
            for (int y = 0; y < board.GetLength(1); y++)
            {
                switch (board[x, y])
                {
                    case 1: Console.Write(ROUNDED_ROCK_SYMB); break;
                    case 0: Console.Write(EMPTY_SPACE_SYMB); break;
                    case -1: Console.Write(SQUARE_ROCK_SYMB); break;
                    default: break;
                }
            }
            Console.WriteLine();
        }
    }

    private static int[,] ParseInput(StringReader sr)
    {
        int[,] board = new int[100, 100];
        string line = sr.ReadLine();
        int x = 0;
        while (line != null)
        {
            if (line != string.Empty)
            {
                int y = 0;
                foreach (char c in line)
                {
                    switch (c)
                    {
                        case ROUNDED_ROCK_SYMB: board[x, y] = 1; break;
                        case SQUARE_ROCK_SYMB: board[x, y] = -1; break;
                        case EMPTY_SPACE_SYMB: board[x, y] = 0; break;
                        default: break;
                    }
                    y++;
                }
            }
            line = sr.ReadLine();
            x++;
        }
        return board;
    }

    private static void TiltBoard(int[,] board, TiltDirection direction)
    {
        ActionRef<int[,], int, int> rollFunc = RollRocksVertically; // Stop compiler complaining about nulls
        int inc = 0;
        int idx = 0;

        switch (direction)
        {
            case TiltDirection.North: rollFunc = RollRocksVertically; break;
            case TiltDirection.South: rollFunc = RollRocksVertically; break;
            case TiltDirection.East: rollFunc = RollRocksHorizontally; break;
            case TiltDirection.West: rollFunc = RollRocksHorizontally; break;
            default: break;
        }

        switch (direction)
        {
            case TiltDirection.North: inc = 1; break;
            case TiltDirection.South: inc = -1; break;
            case TiltDirection.East: inc = -1; break;
            case TiltDirection.West: inc = 1; break;
            default: break;
        }

        switch (direction)
        {
            case TiltDirection.North: idx = board.GetLength(1); break;
            case TiltDirection.South: idx = board.GetLength(1); break;
            case TiltDirection.East: idx = board.GetLength(0); break;
            case TiltDirection.West: idx = board.GetLength(0); break;
            default: break;
        }

        for (int i = 0; i < idx; i++)
        {
            rollFunc(board, i, inc);
        }
    }

    private static void RollRocksHorizontally(int[,] board, int x, int dir)
    {
        if (dir > 0)
        {
            int dest = 0;
            for (int orig = 0; orig < board.GetLength(1); orig++)
            {
                if (board[x, orig] == SQUARE_ROCK)
                {
                    dest = orig + dir;
                }
                else if (board[x, orig] == EMPTY_SPACE)
                {
                    continue;
                }
                else
                {
                    board[x, dest] = ROUNDED_ROCK;
                    if (orig != dest) // handle ...#O#...
                    {
                        board[x, orig] = EMPTY_SPACE;
                    }
                    dest += dir;
                }
            }
        }
        else
        {
            int dest = board.GetLength(1) - 1;
            for (int orig = board.GetLength(1) - 1; orig >= 0; orig--)
            {
                if (board[x, orig] == SQUARE_ROCK)
                {
                    dest = orig + dir;
                }
                else if (board[x, orig] == EMPTY_SPACE)
                {
                    continue;
                }
                else
                {
                    board[x, dest] = ROUNDED_ROCK;
                    if (orig != dest) // handle ...#O#...
                    {
                        board[x, orig] = EMPTY_SPACE;
                    }
                    dest += dir;
                }
            }
        }
    }

    private static void RollRocksVertically(int[,] board, int y, int dir)
    {
        if (dir > 0)
        {
            int dest = 0;
            for (int orig = 0; orig < board.GetLength(0); orig++)
            {
                if (board[orig, y] == SQUARE_ROCK)
                {
                    dest = orig + dir;
                }
                else if (board[orig, y] == EMPTY_SPACE)
                {
                    continue;
                }
                else
                {
                    board[dest, y] = ROUNDED_ROCK;
                    if (orig != dest) // handle ...#O#...
                    {
                        board[orig, y] = EMPTY_SPACE;
                    }
                    dest += dir;
                }
            }
        }
        else
        {
            int dest = board.GetLength(0) - 1;
            for (int orig = board.GetLength(0) - 1; orig >= 0; orig--)
            {
                if (board[orig, y] == SQUARE_ROCK)
                {
                    dest = orig + dir;
                }
                else if (board[orig, y] == EMPTY_SPACE)
                {
                    continue;
                }
                else
                {
                    board[dest, y] = ROUNDED_ROCK;
                    if (orig != dest) // handle ...#O#...
                    {
                        board[orig, y] = EMPTY_SPACE;
                    }
                    dest += dir;
                }
            }
        }
    }

    private static int CalculateTotalLoad(int[,] board)
    {
        int loadTotal = 0;
        for (int x = 0; x < board.GetLength(0); x++) // rows
        {
            int lineTotal = 0;
            for (int y = 0; y < board.GetLength(1); y++) // columns
            {
                if (board[x, y] == ROUNDED_ROCK)
                {
                    lineTotal++;
                }
            }
            loadTotal += lineTotal * (board.GetLength(0) - x);
        }
        return loadTotal;
    }

    private static int TotalLoadOnTheSupportBeams(int[,] board)
    {
        TiltBoard(board, TiltDirection.North);

        return CalculateTotalLoad(board);
    }

    private static int TotalLoadOnTheSupportBeamsAfterSpinCycling(int[,] board)
    {
        for (int i = 0; i < 1000; i++) // After a few cycles no more changes happen so we can ignore the rest
        {
            SpinCycle(board);
        }

        return CalculateTotalLoad(board);

        static void SpinCycle(int[,] board)
        {
            TiltBoard(board, TiltDirection.North);
            TiltBoard(board, TiltDirection.West);
            TiltBoard(board, TiltDirection.South);
            TiltBoard(board, TiltDirection.East);
        }
    }

    public static void Execute()
    {
        using StringReader sr = new(File.ReadAllText("./day14/input.txt"));

        int[,] board = ParseInput(sr);

        Console.WriteLine($"[AoC 2023 - Day 14 - Part 1] Result: {TotalLoadOnTheSupportBeams(board)}");
        Console.WriteLine($"[AoC 2023 - Day 14 - Part 2] Result: {TotalLoadOnTheSupportBeamsAfterSpinCycling(board)}");
    }
}
