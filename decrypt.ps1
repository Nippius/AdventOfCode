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

$filesToUnzip = gci -Include ('input.zip','problem.zip') -r
foreach($fileToUnzip in $filesToUnzip){

    $7ZipArgs = @(
        "e",
        "-o`"$($fileToUnzip.DirectoryName)`""
        "`"$($fileToUnzip.FullName)`"",
        "-p$($ZipPassword)"
    )

    $p = Start-Process $7ZipExecutablePath -ArgumentList $7ZipArgs -PassThru -WindowStyle 'Hidden'
    $parallelProcesses.Add($p)
}

$parallelProcesses | Wait-Process -Timeout 300

# TODO check exit code of each process for errors