namespace AdventOfCode2023;

public static class Day16
{
    #region Contants
    private enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    private const int CONTRAPTION_SIZE = 110;
    private const char UP_BEAM_SYMB = '^';
    private const char DOWN_BEAM_SYMB = 'v';
    private const char LEFT_BEAM_SYMB = '<';
    private const char RIGHT_BEAM_SYMB = '>';
    private const char EMPTY_TILE = '.';
    private const char FORWARD_MIRROR_TILE = '/';
    private const char BACKWARD_MIRROR_TILE = '\\';
    private const char HORIZONTAL_SPLITTER_TILE = '-';
    private const char VERTICAL_SPLITTER_TILE = '|';
    #endregion Contants

    private class Beam
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public Direction Direction { get; private set; }

        private (int x, int y) directionIncrement;

        public Beam(int x, int y, Direction direction)
        {
            X = x;
            Y = y;
            ChangeDirection(direction);
        }

        public void ChangeDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up: Direction = Direction.Up; directionIncrement = (-1, 0); break;
                case Direction.Down: Direction = Direction.Down; directionIncrement = (1, 0); break;
                case Direction.Left: Direction = Direction.Left; directionIncrement = (0, -1); break;
                case Direction.Right: Direction = Direction.Right; directionIncrement = (0, 1); break;
                default: break;
            }
        }

        public void Move()
        {
            X += directionIncrement.x;
            Y += directionIncrement.y;
        }
    }

    private static void ParseInput(char[,] contraption, StringReader sr)
    {
        int x = 0;
        string line = sr.ReadLine();
        while (line != null)
        {
            if (line != string.Empty)
            {
                char[] inputTiles = line.ToCharArray();
                for (int y = 0; y < CONTRAPTION_SIZE; y++)
                {
                    contraption[x, y] = inputTiles[y];
                }
            }
            line = sr.ReadLine();
            x++;
        }
    }

    // Return 1 if a tile was energized by a passing beam. 0 if it was already energized
    private static int CalculateResultingTile(char[,] contraption, char[,] tileState, Beam beam)
    {
        if (tileState[beam.X, beam.Y] == default)
        {
            char originalTile = contraption[beam.X, beam.Y];

            // Store "physical" tiles so that we only count their energy once
            if (originalTile is FORWARD_MIRROR_TILE or BACKWARD_MIRROR_TILE or HORIZONTAL_SPLITTER_TILE or VERTICAL_SPLITTER_TILE)
            {
                tileState[beam.X, beam.Y] = originalTile;
            }
            else
            {
                // We are in a empty tile in the contraption so store the beam direction to detect loops
                switch (beam.Direction)
                {
                    case Direction.Up:
                        tileState[beam.X, beam.Y] = UP_BEAM_SYMB;
                        break;
                    case Direction.Down:
                        tileState[beam.X, beam.Y] = DOWN_BEAM_SYMB;
                        break;
                    case Direction.Left:
                        tileState[beam.X, beam.Y] = LEFT_BEAM_SYMB;
                        break;
                    case Direction.Right:
                        tileState[beam.X, beam.Y] = RIGHT_BEAM_SYMB;
                        break;
                }
            }
            return 1;
        }
        return 0;
    }

    private static void RedirectBeam(Beam beam, char beamRedirectorTile)
    {
        switch (beam.Direction)
        {
            case Direction.Up:
                switch (beamRedirectorTile)
                {
                    case FORWARD_MIRROR_TILE: beam.ChangeDirection(Direction.Right); break;
                    case BACKWARD_MIRROR_TILE: beam.ChangeDirection(Direction.Left); break;
                    default: break;
                }
                break;
            case Direction.Down:
                switch (beamRedirectorTile)
                {
                    case FORWARD_MIRROR_TILE: beam.ChangeDirection(Direction.Left); break;
                    case BACKWARD_MIRROR_TILE: beam.ChangeDirection(Direction.Right); break;
                    default: break;
                }
                break;
            case Direction.Right:
                switch (beamRedirectorTile)
                {
                    case FORWARD_MIRROR_TILE: beam.ChangeDirection(Direction.Up); break;
                    case BACKWARD_MIRROR_TILE: beam.ChangeDirection(Direction.Down); break;
                    default: break;
                }
                break;
            case Direction.Left:
                switch (beamRedirectorTile)
                {
                    case FORWARD_MIRROR_TILE: beam.ChangeDirection(Direction.Down); break;
                    case BACKWARD_MIRROR_TILE: beam.ChangeDirection(Direction.Up); break;
                    default: break;
                }
                break;
        }
    }

    private static bool TrySplitAndRedirectBeam(Beam beam, out Beam newBeam, char beamSplitterTile)
    {
        newBeam = null;
        switch (beam.Direction)
        {
            case Direction.Up:
            case Direction.Down:
                switch (beamSplitterTile)
                {
                    case HORIZONTAL_SPLITTER_TILE:
                        beam.ChangeDirection(Direction.Left);
                        newBeam = new Beam(beam.X, beam.Y, Direction.Right);
                        break;
                    case VERTICAL_SPLITTER_TILE:
                    default: return false;
                }
                break;
            case Direction.Right:
            case Direction.Left:
                switch (beamSplitterTile)
                {
                    case VERTICAL_SPLITTER_TILE:
                        beam.ChangeDirection(Direction.Up);
                        newBeam = new Beam(beam.X, beam.Y, Direction.Down);
                        break;
                    case HORIZONTAL_SPLITTER_TILE:
                    default: return false;
                }
                break;
            default: return false;
        }
        return true;
    }

    private static bool IsBeamLooping(Beam beam, char[,] tileState)
    {
        // A beam is looping if the resulting contraption already has a beam symbol
        // in the same direction the beam is already going.
        // For example, if a beam is going right and there's already a RIGHT_BEAM_SYMB
        // then the beam has looped
        switch (beam.Direction)
        {
            case Direction.Up: return tileState[beam.X, beam.Y] == UP_BEAM_SYMB;
            case Direction.Down: return tileState[beam.X, beam.Y] == DOWN_BEAM_SYMB;
            case Direction.Left: return tileState[beam.X, beam.Y] == LEFT_BEAM_SYMB;
            case Direction.Right: return tileState[beam.X, beam.Y] == RIGHT_BEAM_SYMB;
            default: return false;
        }
    }

    private static int FireLaserBeam(char[,] contraption, Beam startingBeam)
    {
        char[,] resultingTileState = new char[CONTRAPTION_SIZE, CONTRAPTION_SIZE];
        int energizedTilesCount = 0;
        Stack<Beam> beams = new Stack<Beam>();
        beams.Push(startingBeam);

        // Simulate one beam at a time, adding other beams as necessary
        while (beams.Count > 0)
        {
            Beam beam = beams.Pop();

            while (beam.X >= 0 && beam.X < CONTRAPTION_SIZE && beam.Y >= 0 && beam.Y < CONTRAPTION_SIZE)
            {
                if (IsBeamLooping(beam, resultingTileState))
                {
                    break;
                }

                energizedTilesCount += CalculateResultingTile(contraption, resultingTileState, beam);

                char tile = contraption[beam.X, beam.Y];
                switch (tile)
                {
                    case EMPTY_TILE:
                        beam.Move();
                        break;
                    case FORWARD_MIRROR_TILE:
                    case BACKWARD_MIRROR_TILE:
                        RedirectBeam(beam, tile);
                        beam.Move();
                        break;
                    case HORIZONTAL_SPLITTER_TILE:
                    case VERTICAL_SPLITTER_TILE:
                        Beam newBeam;
                        if (TrySplitAndRedirectBeam(beam, out newBeam, tile))
                        {
                            beams.Push(newBeam);
                            newBeam.Move();
                        }
                        beam.Move();
                        break;
                    default: return -1; // This should never happen
                }
            }
        }
        return energizedTilesCount;
    }

    private static int CountEnergizedTiles(char[,] contraption)
    {
        return FireLaserBeam(contraption, new Beam(0, 0, Direction.Right));
    }

    private static int GetMostEnergizedTiles(char[,] contraption)
    {
        int mostEnergizedTiles = 0;

        for (int x = 0; x < CONTRAPTION_SIZE; x++)
        {
            mostEnergizedTiles = Math.Max(FireLaserBeam(contraption, new Beam(x, CONTRAPTION_SIZE, Direction.Left)), mostEnergizedTiles);
            mostEnergizedTiles = Math.Max(FireLaserBeam(contraption, new Beam(x, 0, Direction.Right)), mostEnergizedTiles);
        }

        for (int y = 0; y < CONTRAPTION_SIZE; y++)
        {
            mostEnergizedTiles = Math.Max(FireLaserBeam(contraption, new Beam(CONTRAPTION_SIZE, y, Direction.Up)), mostEnergizedTiles);
            mostEnergizedTiles = Math.Max(FireLaserBeam(contraption, new Beam(0, y, Direction.Down)), mostEnergizedTiles);
        }
        return mostEnergizedTiles;
    }

    public static void Execute()
    {
        char[,] contraption = new char[CONTRAPTION_SIZE, CONTRAPTION_SIZE];
        using StringReader sr = new(File.ReadAllText("./day16/input.txt"));

        ParseInput(contraption, sr);

        Console.WriteLine($"[AoC 2023 - Day 16 - Part 1] Result: {CountEnergizedTiles(contraption)}");
        Console.WriteLine($"[AoC 2023 - Day 16 - Part 2] Result: {GetMostEnergizedTiles(contraption)}");
    }
}
