[CmdletBinding()]
param(
    [Parameter(Mandatory)][string]$Year
)

if(Test-Path (Join-Path . $Year)){
    Write-Error "[⚠] Project already exists. Aborting..."
    return
}

Write-Host "[>] Creating folder $Year..."
mkdir $Year | Out-Null
pushd $Year

Write-Host "[>] Creating solution and respective project..."
dotnet new console | Out-Null
dotnet new sln | Out-Null
dotnet sln add . | Out-Null

Write-Host "[>] Removing <RootNamespace> from $Year.csproj"
(Get-Content .\2024.csproj) | % { 
    if($_ -notlike '*RootNamespace*'){
         $_
    }
} | Set-Content -Path ".\$Year.csproj"

Write-Host "[>] Adding Program.cs..."
Remove-Item 'Program.cs'
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
    mkdir "day$formatedDay" | Out-Null
    pushd "day$formatedDay"
    $classContents =
    @"
namespace AdventOfCode$Year;

public static class Day$formatedDay
{
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
        
        Console.WriteLine($"[AoC $Year - Day $formatedDay - Part 1] Result: {sum}");
        Console.WriteLine($"[AoC $Year - Day $formatedDay - Part 2] Result: {sum}");
    }
}
"@
    Set-Content -Path "Day$formatedDay.cs" -Value $classContents -NoNewLine
    echo '' > input.txt
    echo '' > problem.txt
    popd
}

Write-Host "[>] Compiling..."
dotnet build | Out-Null

Write-Host "[>] Done"
popd