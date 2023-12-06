[CmdletBinding()]
param(
    [Parameter(Mandatory)][string]$Year = '2023'
)

foreach($day in 2..25)
{
    $formatedDay = '{0:d2}' -f $day
    mkdir "day$formatedDay"
    pushd "day$formatedDay"
    $classContents =
    @"
namespace AdventOfCode$Year;

public static class Day$formatedDay
{
    public static void Execute()
    {
        int sum = 0;
        using StringReader sr = new(File.ReadAllText("./day06/input.txt"));

        string? line = sr.ReadLine();
        while (line != null)
        {
            if (line != string.Empty)
            {
                //TODO
            }
            line = sr.ReadLine();
        }

        Console.WriteLine($"[AoC $Year - Day $formatedDay - Part 1] Result: {sum}");
        Console.WriteLine($"[AoC $Year - Day $formatedDay - Part 2] Result: {sum}");
    }
}
"@
    Set-Content -Path "Day$formatedDay.cs" -Value $classContents -Encoding 'utf8'
    echo '' > input.txt
    echo '' > problem.txt
    popd
}