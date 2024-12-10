namespace AdventOfCode2024;

public static class Day09
{
    private const int EMPTY_BLOCK = -1;

    public static int[] ParseInput(string input)
    {
        return Array.ConvertAll([.. input], c => (int)char.GetNumericValue(c));
    }

    private static void LayoutDiskContents(int[] diskMap, int[] disk)
    {
        int fileId = 0, i = 0, writeHeadIdx = 0;
        do
        {
            if (i % 2 == 0)
            {
                disk.AsSpan(writeHeadIdx, diskMap[i]).Fill(fileId++);
            }
            else
            {
                disk.AsSpan(writeHeadIdx, diskMap[i]).Fill(EMPTY_BLOCK);
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
            if (disk[writeHeadIdx] != EMPTY_BLOCK)
            {
                writeHeadIdx++;
            }
            else
            {
                disk[writeHeadIdx] = disk[readHeadIdx];
                disk[readHeadIdx--] = EMPTY_BLOCK;
            }
        }
    }

    private static bool TryFindGap(int[] disk, int fileStartIdx, int minGapSize, out Span<int> emptyGap)
    {
        emptyGap = [];
        int gapStartIdx = 0;
        if (gapStartIdx > fileStartIdx)
        {
            return false;
        }
        else
        {
            // Files can only be moved left and only if there's a big enough gap
            //  so there's no point looking for gaps after the current file we are trying to move
            while (gapStartIdx < fileStartIdx)
            {
                // Found the beginning of the gap
                if (disk[gapStartIdx] == EMPTY_BLOCK)
                {
                    int gapEndIdx = gapStartIdx;
                    while (gapEndIdx != disk.Length && disk[gapEndIdx] == EMPTY_BLOCK)
                    { 
                        gapEndIdx++; 
                    }

                    emptyGap = disk.AsSpan(gapStartIdx, gapEndIdx - gapStartIdx);
                    if (emptyGap.Length >= minGapSize)
                    {
                        return true;
                    }
                }
                gapStartIdx++;
            }
        }
        return false;
    }

    private static bool FindNextFile(int[] disk, int startIdx, out Span<int> file)
    {
        file = [];

        if (startIdx < 0)
        {
            return false;
        }
        else
        {
            while (startIdx >= 0)
            {
                // Found beginning of the file
                if (disk[startIdx] != EMPTY_BLOCK)
                {
                    int fileId = disk[startIdx]; // Because files could be back to back with no empty space in between
                    int endIdx = startIdx;
                    while (endIdx >= 0 && disk[endIdx] == fileId)
                    {
                        endIdx--;
                    }

                    file = disk.AsSpan(endIdx + 1, startIdx - endIdx);

                    return true;
                }
                startIdx--;
            }
        }
        return false;
    }

    private static void CompactDiskFiles(int[] disk)
    {
        int readHeadIdx = disk.Length - 1;

        while (FindNextFile(disk, readHeadIdx, out Span<int> file))
        {
            // We search files backwards. ence the weird arithematic. This wouls actually be the start of the file
            int fileEndIdx = readHeadIdx - file.Length + 1;
            while (TryFindGap(disk, fileEndIdx, file.Length, out Span<int> emptyGap))
            {
                if (file.Length <= emptyGap.Length)
                {
                    file.CopyTo(emptyGap);
                    file.Fill(EMPTY_BLOCK);
                    break;
                }
            }

            readHeadIdx -= file.Length;
            while (readHeadIdx >= 0 && disk[readHeadIdx] == EMPTY_BLOCK)
            {
                readHeadIdx--;
            }
        }
    }

    private static long CalculateDiskChecksum(int[] disk)
    {
        long checksum = 0;
        for (int i = 0; i < disk.Length; i++)
        {
            if (disk[i] == EMPTY_BLOCK) // -1 =  empty block
            {
                continue;
            }
            checksum += i * disk[i];
        }
        return checksum;
    }

    public static void Execute()
    {
        int[] diskMap = ParseInput(File.ReadAllText("./day09/input.txt"));

        int[] disk = new int[diskMap.Sum()];
        int[] diskCopy = new int[disk.Length];

        LayoutDiskContents(diskMap, disk);
        Array.Copy(disk, diskCopy, disk.Length);

        CompactDiskBlocks(disk);
        CompactDiskFiles(diskCopy);

        Console.WriteLine($"[AoC 2024 - Day 09 - Part 1] Result: {CalculateDiskChecksum(disk)}");
        Console.WriteLine($"[AoC 2024 - Day 09 - Part 2] Result: {CalculateDiskChecksum(diskCopy)}");
    }
}