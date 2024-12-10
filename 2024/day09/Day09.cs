namespace AdventOfCode2024;

public static class Day09
{
    private record MySpan(int Start, int Length);
    public static int[] ParseInput(StringReader sr)
    {
        string? line = sr?.ReadLine();
        if (line is not null && line != string.Empty)
        {
            return Array.ConvertAll<char, int>([.. line], c => (int)char.GetNumericValue(c));
        }
        return [];
    }

    private static void FillBlock(Span<int> block, int value)
    {
        for (int i = 0; i < block.Length; i++)
        {
            block[i] = value;
        }
    }

    private static void LayoutDiskContents(int[] diskMap, int[] disk)
    {
        int fileId = 0;
        int i = 0;
        int writeHeadIdx = 0;
        do
        {
            if (i % 2 == 0)
            {
                FillBlock(disk.AsSpan(writeHeadIdx, diskMap[i]), fileId++);
            }
            else
            {
                // Use -1 to represent empty space
                FillBlock(disk.AsSpan(writeHeadIdx, diskMap[i]), -1);
            }
            writeHeadIdx += diskMap[i];
            i++;
        } while (i < diskMap.Length);
    }

    private static void CompactDiskBlocks(int[] disk)
    {
        int writeHeadIdx = 0;
        int readHeadIdx = disk.Length - 1;
        while (writeHeadIdx <= readHeadIdx)
        {
            if (disk[writeHeadIdx] != -1)
            {
                writeHeadIdx++;
            }
            else
            {
                disk[writeHeadIdx] = disk[readHeadIdx];
                disk[readHeadIdx--] = -1;
            }
        }
    }

    private static long CalculateDiskChecksum(int[] disk)
    {
        long checksum = 0;
        for (int i = 0; i < disk.Length; i++)
        {
            if (disk[i] != -1) // -1 =  empty block
            {
                checksum += i * disk[i];
            }
        }
        return checksum;
    }

    public static void Execute()
    {
        using StringReader? sr = new(File.ReadAllText("./day09/input.txt"));

        int[] diskMap = ParseInput(sr);
        int[] disk = new int[diskMap.Sum()];

        LayoutDiskContents(diskMap, disk);

        int[] diskCopy = new int[diskMap.Sum()];
        Array.Copy(disk, diskCopy, disk.Length);

        CompactDiskBlocks(disk);
        //CompactDiskFiles(diskCopy);

        Console.WriteLine($"[AoC 2024 - Day 09 - Part 1] Result: {CalculateDiskChecksum(disk)}");
        Console.WriteLine($"[AoC 2024 - Day 09 - Part 2] Result: {CalculateDiskChecksum(diskCopy)}");
    }
}