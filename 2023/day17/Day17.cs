using Grid = int[,];

namespace AdventOfCode2023;

public static class Day17
{
    private enum Direction
    {
        Up,
        Down,
        Left,
        Right,
        NoChange
    }

    private const int MAP_SIZE = 141;

    private record Point(int X, int Y);

    private record Node(Point Point, int ConsecutiveBlocks, Direction Direction);

    public static Grid ParseInput(StringReader sr)
    {
        Grid heatMap = new int[MAP_SIZE, MAP_SIZE];
        string line = sr.ReadLine();
        int x = 0;
        while (line != null)
        {
            if (line != string.Empty)
            {
                int y = 0;
                foreach (char c in line)
                {
                    heatMap[x, y++] = c - 48;
                }
            }
            line = sr.ReadLine();
            x++;
        }
        return heatMap;
    }

    public static int FindLeastHeat(Grid heatMap, int minConsecutiveBlocks, int maxConsecutiveBlocks)
    {
        Point start = new(0, 0);
        Point end = new(MAP_SIZE - 1, MAP_SIZE - 1);

        // Queue of nodes and their respective heat loss
        PriorityQueue<Node, int> queue = new PriorityQueue<Node, int>();

        // Add starting nodes
        queue.Enqueue(new Node(start, 0, Direction.Right), 0);
        queue.Enqueue(new Node(start, 0, Direction.Down), 0);

        // Add cost for starting nodes
        Dictionary<Node, int> cost = new()
        {
            [new Node(start, 0, Direction.Right)] = 0,
            [new Node(start, 0, Direction.Down)] = 0
        };

        // Process each node
        while (queue.TryDequeue(out Node node, out int heatLoss))
        {
            // All done, return calculated heat loss
            if (node.Point == end && node.ConsecutiveBlocks >= minConsecutiveBlocks)
            {
                return heatLoss;
            }

            // Try all direction for this node
            foreach (Direction turnTo in new[] { Direction.Left, Direction.Right, Direction.NoChange })
            {
                Direction nextDirection = CalculateNextDirection(node, turnTo);
                Point nextPoint = CalculateNextPoint(node, nextDirection);
                int nextConsecutiveBlock;

                if (turnTo == Direction.NoChange)
                {
                    if (node.ConsecutiveBlocks + 1 > maxConsecutiveBlocks)
                    {
                        continue;
                    }
                    nextConsecutiveBlock = node.ConsecutiveBlocks + 1;
                }
                else
                {
                    if (node.ConsecutiveBlocks < minConsecutiveBlocks)
                    {
                        continue;
                    }
                    nextConsecutiveBlock = 1;
                }

                if (nextPoint.X >= 0 && nextPoint.X < MAP_SIZE && nextPoint.Y >= 0 && nextPoint.Y < MAP_SIZE)
                {
                    // Cost of the current node plus the cost of the new node
                    int accumulatedHeatLoss = cost[node] + heatMap[nextPoint.X, nextPoint.Y];
                    Node nextNode = new Node(nextPoint, nextConsecutiveBlock, nextDirection);
                    // Update the cost of the node or add it if it does not exist
                    if (!cost.TryGetValue(nextNode, out int tempAccumulatedHeatLoss) || accumulatedHeatLoss < tempAccumulatedHeatLoss)
                    {
                        cost[nextNode] = accumulatedHeatLoss;
                        queue.Enqueue(nextNode, accumulatedHeatLoss);
                    }
                }
            }
        }

        return 0;
    }

    private static Point CalculateNextPoint(Node node, Direction nextDirection)
    {
        return nextDirection switch
        {
            Direction.Up => new Point(node.Point.X - 1, node.Point.Y),
            Direction.Down => new Point(node.Point.X + 1, node.Point.Y),
            Direction.Left => new Point(node.Point.X, node.Point.Y - 1),
            Direction.Right => new Point(node.Point.X, node.Point.Y + 1),
            _ => throw new ArgumentOutOfRangeException(nameof(nextDirection)),
        };
    }

    private static Direction CalculateNextDirection(Node node, Direction turn)
    {
        switch (node.Direction)
        {
            case Direction.Up:
                switch (turn)
                {
                    case Direction.Left: return Direction.Left;
                    case Direction.Right: return Direction.Right;
                }
                break;
            case Direction.Down:
                switch (turn)
                {
                    case Direction.Left: return Direction.Left;
                    case Direction.Right: return Direction.Right;
                }
                break;
            case Direction.Left:
                switch (turn)
                {
                    case Direction.Left: return Direction.Down;
                    case Direction.Right: return Direction.Up;
                }
                break;
            case Direction.Right:
                switch (turn)
                {
                    case Direction.Left: return Direction.Up;
                    case Direction.Right: return Direction.Down;
                }
                break;
        }
        return node.Direction; // No change
    }

    public static int LeastHeatLoss(Grid heatMap)
    {
        return FindLeastHeat(heatMap, 0, 3);
    }

    public static int LeastHeatLossWithUltraCrucibles(Grid heatMap)
    {
        return FindLeastHeat(heatMap, 4, 10);
    }

    public static void Execute()
    {
        using StringReader sr = new(File.ReadAllText("./day17/input.txt"));

        Grid heatMap = ParseInput(sr);

        Console.WriteLine($"[AoC 2023 - Day 17 - Part 1] Result: {LeastHeatLoss(heatMap)}");
        Console.WriteLine($"[AoC 2023 - Day 17 - Part 2] Result: {LeastHeatLossWithUltraCrucibles(heatMap)}");
    }
}
