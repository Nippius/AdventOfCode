namespace AdventOfCode2024;

public static partial class Day04
{
    private class StarArm((int x, int y) Origin, (int x, int y) DirectionIncrement, bool XmasFound = true)
    {
        public int X { get; private set; } = Origin.x;
        public int Y { get;  private set;} = Origin.y;
        public bool ContainsXmas { get; private set; } = XmasFound;

        public void IncrementPosition()
        {
            X += DirectionIncrement.x;
            Y += DirectionIncrement.y;
        }

        public void MarkXmasNotFound(){
            ContainsXmas = false;
        }
    }
}