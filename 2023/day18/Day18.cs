namespace AdventOfCode2023;

public static class Day18
{
    private readonly record struct Point(long X, long Y);

    private static List<Point> ParseInput(StringReader sr, bool decodeColor, out long perimeter)
    {
        List<Point> vertices = [];
        perimeter = 0;
        long x = 0, y = 0;
        (long vX, long vY) direction;
        long distance;

        string line = sr.ReadLine();
        while (line != null)
        {
            if (line != string.Empty)
            {
                string[] digCommandInput = line.Split(' ');

                if (!decodeColor)
                {
                    distance = long.Parse(digCommandInput[1]);
                    direction = digCommandInput[0] switch
                    {
                        "U" => (distance * -1, 0),
                        "D" => (distance, 0),
                        "L" => (0, distance * -1),
                        "R" => (0, distance),
                        _ => (0, 0)
                    };
                }
                else
                {
                    // Too lazy to use regex...
                    string encodedInstruction = digCommandInput[2].Split('(')[1].Split(')')[0].Split('#')[1];
                    distance = Convert.ToInt64(encodedInstruction[..5], 16);
                    direction = encodedInstruction[^1] switch
                    {
                        '3' => (distance * -1, 0),
                        '1' => (distance, 0),
                        '2' => (0, distance * -1),
                        '0' => (0, distance),
                        _ => (0, 0)
                    };
                }

                vertices.Add(new(x += direction.vX, y += direction.vY));
                perimeter += distance; // Precalculate the perimeter here to save time.
            }
            line = sr.ReadLine();
        }
        return vertices;
    }

    private static long CalculatePolygonArea(IReadOnlyList<Point> vertices)
    {
        // Use shoelace algorithm to get the area of the poligon bounded by vertices
        // https://en.wikipedia.org/wiki/Shoelace_formula
        long area = 0L;
        int n = vertices.Count;

        for (int i = 0; i < vertices.Count; i++)
        {
            area += vertices[i].X * vertices[(i + 1) % n].Y - vertices[(i + 1) % n].X * vertices[i].Y;
        }

        return Math.Abs(area) / 2;
    }

    private static long CalculateVolume(IReadOnlyList<Point> vertices, long perimeter)
    {
        long area = CalculatePolygonArea(vertices);
        //  The interior area of a polygon can be calculated using Pick's Theorem:
        //  I = A - B/2 + 1
        // The formula is slightly rearranged to calculate the number of interior points (the interior area)
        // https://en.wikipedia.org/wiki/Pick%27s_theorem
        long interior = area - perimeter / 2 + 1;

        return interior + perimeter;
    }

    public static void Execute()
    {
        StringReader sr = new(File.ReadAllText("./day18/input.txt"));

        List<Point> vertices1 = ParseInput(sr, false, out long perimeter1);

        sr = new(File.ReadAllText("./day18/input.txt"));
        // "true" because in part 2, the "color" actually contains the instructions
        List<Point> vertices2 = ParseInput(sr, true, out long perimeter2);

        Console.WriteLine($"[AoC 2023 - Day 18 - Part 1] Result: {CalculateVolume(vertices1, perimeter1)}");
        Console.WriteLine($"[AoC 2023 - Day 18 - Part 2] Result: {CalculateVolume(vertices2, perimeter2)}");
    }
}
