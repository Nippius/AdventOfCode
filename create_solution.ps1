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
using AdventOfCode$Year;

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
namespace AdventOfCode$Year;

public static class Day$formatedDay
{
    public static int Part1(int sum)
    {
        return sum;
    }

    public static int Part2(int sum)
    {
        return sum;
    }

    public static void Execute()
    {
        int sum = 0;
        using StringReader? sr = new(File.ReadAllText("./day$formatedDay/input.txt"));
        string? line = sr?.ReadLine();
        while (line is not null)
        {
            if (line != string.Empty)
            {
                // TODO
            }
            line = sr?.ReadLine();
        }
        
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