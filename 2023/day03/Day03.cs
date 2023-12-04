namespace AdventOfCode2023;


public static class Day03
{
    private record PartNumber(int StartX, int EndX, int Y, int Value) { }
    private record Symbol(int X, int Y, char Value) { }
    private record SymbolWithPartNumbers(Symbol Symbol, IList<PartNumber> PartNumbers) { }

    private const char GearSymbol = '*';

    private static IList<SymbolWithPartNumbers> ParseEngineLayout(StringReader sr)
    {
        int y = 0; // Or current row/line index
        IList<PartNumber> PartNumbers = [];
        IList<Symbol> Symbols = [];
        IList<SymbolWithPartNumbers> SymbolsWithPartNumbers = [];

        // Parse input to get all symbols and part numbers separately
        string? line = sr.ReadLine();
        while (line != null)
        {
            for (int x = 0; x < line.Length; x++)
            {
                char c = line[x];
                if (c == '.')
                {
                    // ignore
                }
                else if (char.IsDigit(c))
                {
                    int endPos = x; // Where the number starts

                    // Get all remaining CONSECUTIVE digits to form a number
                    do
                    {
                        endPos++;
                    } while (endPos < line.Length && char.IsDigit(line[endPos]));

                    int endX = endPos - 1;
                    int value = int.Parse(line[x..endPos]);
                    PartNumbers.Add(new PartNumber(x, endX, y, value));
                    // Set x position so the for loop can move on on the correct position.
                    x = endX;
                }
                else
                {
                    Symbols.Add(new Symbol(x, y, c));
                }
            }

            line = sr.ReadLine();
            y++;
        }

        // Pair up each symbol with the corresponding part numbers
        foreach (Symbol s in Symbols)
        {
            SymbolsWithPartNumbers.Add(new SymbolWithPartNumbers(s, GetAllPartNumbersForSymbol(s, PartNumbers)));
        }

        return SymbolsWithPartNumbers;
    }

    private static IList<PartNumber> GetAllPartNumbersForSymbol(Symbol Symbol, IList<PartNumber> PartNumbers)
    {
        IList<PartNumber> PartNumbersForSymbol = [];
        foreach (PartNumber p in PartNumbers)
        {
            // Is the point in the same of adjacent lines?
            if (Symbol.Y - 1 == p.Y || Symbol.Y == p.Y || Symbol.Y + 1 == p.Y)
            {
                // The point starts or ends in the same or adjacent column?
                if (Symbol.X - 1 == p.StartX || Symbol.X - 1 == p.EndX
                    || Symbol.X == p.StartX || Symbol.X == p.EndX
                    || Symbol.X + 1 == p.StartX || Symbol.X + 1 == p.EndX
                    || (Symbol.X >= p.StartX && Symbol.X <= p.EndX))
                {
                    PartNumbersForSymbol.Add(p);
                }
            }
        }

        return PartNumbersForSymbol;
    }

    private static int SumOfAllThePartNumbers(IList<SymbolWithPartNumbers> SymbolsWithPartNumbers)
    {
        int sum = 0;
        foreach (SymbolWithPartNumbers symbolWithPartNumbers in SymbolsWithPartNumbers)
        {
            foreach (PartNumber p in symbolWithPartNumbers.PartNumbers)
            {
                sum += p.Value;
            }
        }
        return sum;
    }

    private static int SumOfAllGearRatios(IList<SymbolWithPartNumbers> SymbolsWithPartNumbers)
    {
        int sum = 0;
        foreach (SymbolWithPartNumbers symbolWithPartNumbers in SymbolsWithPartNumbers)
        {
            if (symbolWithPartNumbers.Symbol.Value == GearSymbol)
            {
                if (symbolWithPartNumbers.PartNumbers.Count == 2)
                {
                    sum += symbolWithPartNumbers.PartNumbers[0].Value * symbolWithPartNumbers.PartNumbers[1].Value;
                }
            }

        }
        return sum;
    }

    public static void Execute()
    {
        IList<SymbolWithPartNumbers> SymbolsAndPartNumbers = [];

        StringReader sr = new(File.ReadAllText("./day03/input.txt"));
        SymbolsAndPartNumbers = ParseEngineLayout(sr);

        Console.WriteLine($"[AoC 2023 - Day 03 - Part 1] Result: {SumOfAllThePartNumbers(SymbolsAndPartNumbers)}");
        Console.WriteLine($"[AoC 2023 - Day 03 - Part 2] Result: {SumOfAllGearRatios(SymbolsAndPartNumbers)}");
    }
}
