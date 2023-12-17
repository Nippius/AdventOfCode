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
    private const char SQUARE_ROCK_SYMB = '#';
    private const int SQUARE_ROCK = -1;
    private const int BOARD_SIZE = 100;

    private static int[,] ParseInput(StringReader sr)
    {
        int[,] board = new int[BOARD_SIZE, BOARD_SIZE];
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
        for (int x = 0; x < BOARD_SIZE; x++) // rows
        {
            int lineTotal = 0;
            for (int y = 0; y < BOARD_SIZE; y++) // columns
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

    private static bool BoardsAreEqual(int[,] boardA, int[,] boardB)
    {
        for (int x = 0; x < BOARD_SIZE; x++)
        {
            for (int y = 0; y < BOARD_SIZE; y++)
            {
                int comp = boardA[x, y] - boardB[x, y];
                if (comp != 0)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private static int[,] CreateBoardCopy(int[,] board)
    {
        int[,] newBoard = new int[BOARD_SIZE, BOARD_SIZE];
        Array.Copy(board, newBoard, BOARD_SIZE * BOARD_SIZE);
        return newBoard;
    }

    private static int TotalLoadOnTheSupportBeams(int[,] board)
    {
        TiltBoard(board, TiltDirection.North);

        return CalculateTotalLoad(board);
    }

    private static int TotalLoadOnTheSupportBeamsAfterSpinCycling(int[,] board)
    {
        int cycleStart = 0;
        int cycleEnd = 0;

        // Add the initial board before spinning
        List<int[,]> boardsSeen = [];
        boardsSeen.Add(CreateBoardCopy(board));

        int index = 0;
        while (true) //We will find a loop eventually
        {
            SpinCycle(board);
            if (boardsSeen.Exists(b => BoardsAreEqual(b, board)))
            {
                // We found a board we've seen before. This
                // means we found our loop because we started finding
                // repeated boards.
                cycleEnd = index;
                break;
            }
            boardsSeen.Add(CreateBoardCopy(board));
            index++;
        }

        // Find the first instance of the repeated board to get the start of the cycle
        cycleStart = boardsSeen.FindIndex(b => BoardsAreEqual(b, board));

        // Now we need to find the board that correspondes to the 1000000000 iteration
        // and calculate it's load.
        // We do this using a form of Min-Max normalization with rescaling
        int[,] boardToCalculateLoad = boardsSeen[1000000000 % (cycleEnd - cycleStart) + cycleStart];

        return CalculateTotalLoad(boardToCalculateLoad);

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

        int[,] boardDay1 = ParseInput(sr);
        int[,] boardDay2 = CreateBoardCopy(boardDay1);

        Console.WriteLine($"[AoC 2023 - Day 14 - Part 1] Result: {TotalLoadOnTheSupportBeams(boardDay1)}");
        Console.WriteLine($"[AoC 2023 - Day 14 - Part 2] Result: {TotalLoadOnTheSupportBeamsAfterSpinCycling(boardDay2)}");
    }
}
