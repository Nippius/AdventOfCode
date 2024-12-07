namespace AdventOfCode2024;

using Grid = char[,];
using Position = (int x, int y);
using Direction = (int dx, int dy);
// A route is a collection of positions walked by the guard and how often the guard walked on the same place
using Route = Dictionary<(int x, int y), int>;
using Step = KeyValuePair<(int x, int y), int>;

public static class Day06
{
    private static readonly int MAX_GRID_SIZE = 130;

    private static void ParseBoard(StringReader sr, out Grid lab, out Position guard)
    {
        lab = new char[MAX_GRID_SIZE, MAX_GRID_SIZE];
        guard = (0, 0);

        int x = 0;
        string? line = sr.ReadLine();
        while (line is not null)
        {
            int y = 0;
            if (line != string.Empty)
            {
                foreach (char c in line)
                {
                    lab[x, y] = c;

                    if (c == '^')
                    {
                        guard = (x, y);
                    }

                    y++;
                }
            }
            line = sr?.ReadLine();
            x++;
        }
    }

    private static Direction CalculateNewDirection(Direction direction)
    {
        if (direction.dx == -1 && direction.dy == 0) return (0, 1);
        if (direction.dx == 0 && direction.dy == 1) return (1, 0);
        if (direction.dx == 1 && direction.dy == 0) return (0, -1);
        return (-1, 0); // back to up the direction
    }

    private static void MoveGuard(
        Grid lab,
        ref Position guard,
        Route route,
        Direction direction,
        out bool hitObstacle,
        out bool exitedBoard,
        out bool loopDetected)
    {
        exitedBoard = false;
        hitObstacle = false;
        loopDetected = false;

        while (true)
        {
            Position currentPosition = guard;
            Position nextPosition = (guard.x + direction.dx, guard.y + direction.dy);
            if (nextPosition.x < 0
                || nextPosition.y < 0
                || nextPosition.x >= MAX_GRID_SIZE
                || nextPosition.y >= MAX_GRID_SIZE)
            {
                route[currentPosition] = 1;
                exitedBoard = true;
                break;
            }
            else if (lab[nextPosition.x, nextPosition.y] == '#')
            {
                hitObstacle = true;
                break;
            }
            else if (!route.ContainsKey(currentPosition)) // Save current guard position if it's the first visit
            {
                route[currentPosition] = 1;
            }
            else if (route[currentPosition] > 4) // If we pass the same place more than twice it's possibly a loop, but 3 times is surely one.
            {
                loopDetected = true;
                break;
            }

            route[currentPosition]++;
            guard.x += direction.dx;
            guard.y += direction.dy;
        }
    }

    // Returns the route walked by the guard until it exits or a loop is detected
    private static Route CalculateGuardRoute(Grid lab, Position guard, out bool loopDetected)
    {
        Route route = [];
        Direction currentDirection = (-1, 0); // go up by default
        bool exitedBoard;

        do
        {
            MoveGuard(
                 lab,
                 ref guard,
                 route,
                 currentDirection,
                 out bool hitObstacle,
                 out exitedBoard,
                 out loopDetected);

            if (hitObstacle)
            {
                currentDirection = CalculateNewDirection(currentDirection);
            }
        } while (!exitedBoard && !loopDetected);

        return route;
    }

    private static int CountStepsInRoute(Grid lab, Position guard)
    {
        return CalculateGuardRoute(lab, guard, out bool _).Count;
    }

    private static int CountHowManyPlacesCouldCauseALoop(Grid lab, Position guard)
    {
        Route route = CalculateGuardRoute(lab, guard, out bool _);

        int loops = 0;
        foreach (Step step in route)
        {
            // Save the priginal piece (either '^' for the guard or  '.' for the floor)
            char originalBoardPiece = lab[step.Key.x, step.Key.y];

            // Replace the current place with an obstacle
            lab[step.Key.x, step.Key.y] = '#';

            // See if the obstacle causes a loop
            _ = CalculateGuardRoute(lab, guard, out bool loopDetected);
            if (loopDetected)
            {
                loops++;
            }

            // Restore the original piece
            lab[step.Key.x, step.Key.y] = originalBoardPiece;
        }
        return loops;
    }

    public static void Execute()
    {
        using StringReader? sr = new(File.ReadAllText("./day06/input.txt"));

        ParseBoard(sr, out Grid lab, out Position guard);

        Console.WriteLine($"[AoC 2024 - Day 06 - Part 1] Result: {CountStepsInRoute(lab, guard)}");
        Console.WriteLine($"[AoC 2024 - Day 06 - Part 2] Result: {CountHowManyPlacesCouldCauseALoop(lab, guard)}");
    }
}