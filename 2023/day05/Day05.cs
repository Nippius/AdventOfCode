namespace AdventOfCode2023;

public static class Day05
{
    private class Almanac
    {
        private class InputRange(long RangeStart, long Length)
        {
            public long Start { get; } = RangeStart;
            public long Length { get; } = Length;
        }

        private class RangeReMapper(long Destination, long Source, long Length)
        {
            public bool ValueFitsRange(long value)
            {
                return value >= Source && value <= (Source + Length);
            }

            public long RemapValue(long input)
            {
                return Destination + (input - Source);
            }
        }

        private class Map(string Name)
        {
            private List<RangeReMapper> rangeReMappers = [];

            public void AddMapRange(long destination, long source, long length)
            {
                rangeReMappers.Add(new RangeReMapper(destination, source, length));
            }

            public long MapValue(long value)
            {
                foreach (RangeReMapper remapper in rangeReMappers)
                {
                    if (remapper.ValueFitsRange(value))
                    {
                        return remapper.RemapValue(value);
                    }
                }
                return value;
            }

            public IList<InputRange> MapInputRange(InputRange range){
                // This method assumes ranges never overlap
                IList<InputRange> newInputRanges = [];

                foreach(RangeReMapper remapper in rangeReMappers){

                }

                return newInputRanges;
            }
        }

        private List<long> Seeds = [];
        private List<Map> Maps = [];

        private void ParseSeeds(StringReader sr)
        {
            string seedsInput = sr.ReadLine();
            long[] seedIds = seedsInput.Split(':')[1].Trim().Split(' ').Select(long.Parse).ToArray();
            Seeds.AddRange(seedIds);

            // Eat empty line, assuming there's only one line with seeds
            sr.ReadLine();
        }

        private void ParseMaps(StringReader sr)
        {
            string input = sr.ReadLine();
            while (input != null)
            {
                Map map = new Map(input);
                string rangeInput = sr.ReadLine();
                while (rangeInput != string.Empty && rangeInput != null)
                {
                    long[] rangeDetails = rangeInput.Trim().Split(' ').Select(long.Parse).ToArray<long>();
                    map.AddMapRange(rangeDetails[0], rangeDetails[1], rangeDetails[2]);
                    rangeInput = sr.ReadLine();
                }

                Maps.Add(map);
                input = sr.ReadLine();
            }
        }

        public void ParseInput(StringReader sr)
        {
            ParseSeeds(sr);
            ParseMaps(sr);
        }

        public long GetLowestLocationNumberFromSeeds()
        {
            long res = long.MaxValue;
            foreach (long seed in Seeds)
            {
                long val = seed;
                foreach (Map map in Maps)
                {
                    val = map.MapValue(val);
                }
                if (val < res) { res = val; };
            }
            return res;
        }

        private long GetLowestLocationRecursive(IList<InputRange> ranges, int mapIndex)
        {
            long location = long.MaxValue;

            if (mapIndex < Maps.Count)
            {
                Map map = Maps[mapIndex];
                List<InputRange> newInputRanges = [];

                // Calculate new seed ranges based on the map
                foreach(InputRange range in ranges){
                    newInputRanges.AddRange(map.MapInputRange(range));
                }

                return GetLowestLocationRecursive(newInputRanges, mapIndex + 1);
            }

            //TODO calculate lowest location
            foreach(InputRange range in ranges){
                if(range.Start < location){
                    location = range.Start;
                }
            }
            return location;
        }

        public long GetLowestLocationNumberFromSeedRange()
        {
            long[] seedsRangeArray = [.. Seeds];
            List<InputRange> seedRange = [];

            for (int i = 0; i < seedsRangeArray.Length - 1; i += 2)
            {
                long startingSeed = seedsRangeArray[i];
                long length = startingSeed + seedsRangeArray[i + 1];

                seedRange.Add(new InputRange(startingSeed, length));
            }

            return GetLowestLocationRecursive(seedRange, 0);
        }
    }

    public static void Execute()
    {
        StringReader sr = new(File.ReadAllText("./day05/input.txt"));
        Almanac almanac = new Almanac();
        almanac.ParseInput(sr);

        Console.WriteLine($"[AoC 2023 - Day 05 - Part 1] Result: {almanac.GetLowestLocationNumberFromSeeds()}");
        Console.WriteLine($"[AoC 2023 - Day 05 - Part 2] Result: {almanac.GetLowestLocationNumberFromSeedRange()}");
    }
}



