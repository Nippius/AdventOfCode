namespace AdventOfCode2023;

public record Game(int Id, IList<Hand> Hands) { }

public record Hand(int redCubes, int greenCubes, int blueCubes) { }

public class GameParser
{
    private const char HandSeparator = ';';
    private const char CubeResultsSeparator = ',';
    private const char GameInfoSeparator = ':';
    private const char CubeInfoSeparator = ' ';

    private static Hand ParseHand(string handInput)
    {
        // A hand input might not have all the colors
        // or they might appear out of order
        // so we need to parse it a bit more generically
        // by handling whatever cube + color appears in the 
        // input.

        int redCubes = 0;
        int greenCubes = 0;
        int blueCubes = 0;

        // Split hand input into each cubes quantity + color
        string[] cubeResults = handInput.Split(CubeResultsSeparator);
        foreach (var cubeResult in cubeResults)
        {
            // [0] how many cubes of given color appear in the hand
            // [1] color of the cubes
            string[] cubeInfo = cubeResult.Trim().Split(CubeInfoSeparator);
            int quantity = int.Parse(cubeInfo[0]);
            switch (cubeInfo[1])
            {
                case "red": redCubes = quantity; break;
                case "green": greenCubes = quantity; break;
                case "blue": blueCubes = quantity; break;
                default: break;
            }
        }

        return new Hand(redCubes, greenCubes, blueCubes);
    }

    private static List<Hand> ParseHands(string handInputs)
    {
        List<Hand> hands = [];
        string[] handResults = handInputs.Split(HandSeparator);
        foreach (var handResult in handResults)
        {
            hands.Add(ParseHand(handResult));
        }

        return hands;
    }

    private static int ParseGameId(string gameInput)
    {
        string[] temp = gameInput.Split(' ');
        return int.Parse(temp[1]);
    }

    public static Game ParseGame(string gameInput)
    {
        string[] gameInfo = gameInput.Split(GameInfoSeparator);
        int gameId = ParseGameId(gameInfo[0]);
        IList<Hand> hands = ParseHands(gameInfo[1]);

        return new Game(gameId, hands);
    }
}

public static class Day02
{
    private static int SumOfAllValidGames(IList<Game> Games)
    {
        int sum = 0;
        int availableRedCubes = 12;
        int availableGreenCubes = 13;
        int availableBlueCubes = 14;
        foreach (Game game in Games)
        {
            bool gameIsPossible = true;
            foreach (var hand in game.Hands)
            {
                if (hand.redCubes > availableRedCubes
                    || hand.greenCubes > availableGreenCubes
                    || hand.blueCubes > availableBlueCubes)
                {
                    gameIsPossible = false;
                    break;
                }
            }
            
            if (gameIsPossible)
            {
                sum += game.Id;
            }
        }
        return sum;
    }

    private static int SumOfThePowerOfAllSets(IList<Game> Games)
    {
        int totalPower = 0;
        foreach (Game game in Games)
        {
            int minRedCubes = 0;
            int minGreenCubes = 0;
            int minBlueCubes = 0;
            int gamePower = 0;

            foreach (Hand hand in game.Hands)
            {
                if (hand.redCubes > minRedCubes) minRedCubes = hand.redCubes;
                if (hand.greenCubes > minGreenCubes) minGreenCubes = hand.greenCubes;
                if (hand.blueCubes > minBlueCubes) minBlueCubes = hand.blueCubes;
            }

            gamePower = minRedCubes * minGreenCubes * minBlueCubes;

            totalPower += gamePower;
        }

        return totalPower;
    }

    public static void Execute()
    {
        IList<Game> games = new List<Game>();
        StringReader sr = new(File.ReadAllText("./day02/input.txt"));
        string? line = sr.ReadLine();
        while (line != null)
        {
            if (line != string.Empty)
            {
                games.Add(GameParser.ParseGame(line));
            }
            line = sr.ReadLine();
        }

        Console.WriteLine($"[AoC 2023 - Day 02 - Part 1] Result: {SumOfAllValidGames(games)}");
        Console.WriteLine($"[AoC 2023 - Day 02 - Part 2] Result: {SumOfThePowerOfAllSets(games)}");
    }
}
