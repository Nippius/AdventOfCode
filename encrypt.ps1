# Encrypt input data and problem information

$7ZipExecutablePath = 'C:\Program Files\7-Zip\7z.exe'
$ZipPassword = $Env:AoCZipPassword
$parallelProcesses  = [System.Collections.Generic.List[System.Diagnostics.Process]]::new()

if($false -eq (Test-Path $7ZipExecutablePath)){
    Write-Error "7zip not found."
}

if([string]::IsNullOrEmpty($ZipPassword))
{
    Write-Error "Zip password not defined"
}

$filesToZip = gci -Include ('input.txt','problem.txt') -r
foreach($fileToZip in $filesToZip){
    $zipedFile = Join-Path $fileToZip.DirectoryName "$($fileToZip.BaseName).zip"

    $7ZipArgs = @(
        "a",
        "-tzip",
        "`"$zipedFile`"",
        "`"$($fileToZip.FullName)`"",
        "-mx9",
        "-p$($ZipPassword)"
    )

    $p = Start-Process $7ZipExecutablePath -ArgumentList $7ZipArgs -PassThru -WindowStyle 'Hidden'
    $parallelProcesses.Add($p)
}

$parallelProcesses | Wait-Process -Timeout 300

# TODO check exit code of each process for errors