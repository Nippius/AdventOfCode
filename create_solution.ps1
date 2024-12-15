[CmdletBinding()]
param(
    [Parameter(Mandatory)][string]$Year
)

if(Test-Path (Join-Path . $Year)){
    Write-Error "[⚠] Project already exists. Aborting..."
    return
}

Write-Host "[>] Creating folder $Year..."
New-Item "$Year" -ItemType Directory | Out-Null
Push-Location $Year

Write-Host "[>] Creating solution and respective project..."
dotnet new console | Out-Null
dotnet new sln | Out-Null
dotnet sln add . | Out-Null

Write-Host "[>] Removing <RootNamespace> from $Year.csproj..."
(Get-Content ".\$($Year).csproj") | ForEach-Object { 
    if($_ -notlike '*RootNamespace*'){
         $_
    }
} | Set-Content -Path ".\$Year.csproj"

Write-Host "[>] Adding Program.cs..."
Remove-Item 'Program.cs' # This was added by the 'dotnet new console' step above
$programContents = 
@"
using AdventOfCode$Year.Day01;
using AdventOfCode$Year.Day02;
using AdventOfCode$Year.Day03;
using AdventOfCode$Year.Day04;
using AdventOfCode$Year.Day05;
using AdventOfCode$Year.Day06;
using AdventOfCode$Year.Day07;
using AdventOfCode$Year.Day08;
using AdventOfCode$Year.Day09;
using AdventOfCode$Year.Day10;
using AdventOfCode$Year.Day11;
using AdventOfCode$Year.Day12;
using AdventOfCode$Year.Day13;
using AdventOfCode$Year.Day14;
using AdventOfCode$Year.Day15;
using AdventOfCode$Year.Day16;
using AdventOfCode$Year.Day17;
using AdventOfCode$Year.Day18;
using AdventOfCode$Year.Day19;
using AdventOfCode$Year.Day20;
using AdventOfCode$Year.Day21;
using AdventOfCode$Year.Day22;
using AdventOfCode$Year.Day23;
using AdventOfCode$Year.Day24;
using AdventOfCode$Year.Day25;

Day01.Execute();
Day02.Execute();
Day03.Execute();
Day04.Execute();
Day05.Execute();
Day06.Execute();
Day07.Execute();
Day08.Execute();
Day09.Execute();
Day10.Execute();
Day11.Execute();
Day12.Execute();
Day13.Execute();
Day14.Execute();
Day15.Execute();
Day16.Execute();
Day17.Execute();
Day18.Execute();
Day19.Execute();
Day20.Execute();
Day21.Execute();
Day22.Execute();
Day23.Execute();
Day24.Execute();
Day25.Execute();
"@
Set-Content -Path "Program.cs" -Value $programContents -NoNewLine

Write-Host "[>] Creating dummy implementations for each day..."
foreach($day in 1..25)
{
    $formatedDay = '{0:d2}' -f $day
    New-Item "day$formatedDay" -ItemType Directory | Out-Null
    Push-Location "day$formatedDay"
    $classContents =
    @"
namespace AdventOfCode$Year.Day$formatedDay;

public static class Day$formatedDay
{
    private static List<string> ParseInput(StringReader sr){
        using(sr)
        {
            List<string> input = [];
            string? line = sr?.ReadLine();
            while (line is not null)
            {
                if (line != string.Empty)
                {
                    input.Add(line);
                }
                line = sr?.ReadLine();
            }
            return input;
        }
    }

    private static int Part1(int sum)
    {
        return sum;
    }

    private static int Part2(int sum)
    {
        return sum;
    }

    public static void Execute()
    {
        int sum = 0;
        using StringReader? sr = new(File.ReadAllText("./day$formatedDay/input.txt"));
        
        List<string> input = ParseInput(sr);
        
        Console.WriteLine($"[AoC $Year - Day $formatedDay - Part 1] Result: {Part1(sum)}");
        Console.WriteLine($"[AoC $Year - Day $formatedDay - Part 2] Result: {Part2(sum)}");
    }
}
"@
    Set-Content -Path "Day$formatedDay.cs" -Value $classContents -NoNewLine
    New-Item input.txt | Out-Null
    New-Item problem.txt | Out-Null
    Pop-Location
}

Write-Host "[>] Compiliing and running the new project..."
dotnet run

Write-Host "[>] Done"
Pop-Location