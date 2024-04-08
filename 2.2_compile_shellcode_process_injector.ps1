param (
    [Parameter(Mandatory = $true)]
    [string]$TargetProjectFolder, # Folder to Target C# Project to compile

    [Parameter(Mandatory = $true)]
    [string]$XORPayloadFilePath
)

# COMPILATION


# Get path to C# Project's Program.cs file
$ProgramCsFilePath = $null
Get-ChildItem -Path $TargetProjectFolder -Recurse -File | ForEach-Object {
    if ($_.Name -eq "Program.cs") {
        $ProgramCsFilePath = $_.FullName.Replace($TargetProjectFolder, "")
        Write-Host "Relative path to Program.cs: $ProgramCsFilePath"
        return
    }
}

# Get path to csc.exe Compiler Executable
#$targetDirectory = "C:\\Windows\\Microsoft.NET"
$CscTargetDirectory = "C:\\Program Files (x86)\\Microsoft Visual Studio\\2019\\Community\\MSBuild\\Current\\Bin\\Roslyn"
try {
    $filesInDirectory = Get-ChildItem -Path $CscTargetDirectory -File -Recurse -Filter csc.exe
    Write-Host "Found files in ${CscTargetDirectory}:"
    $cscExePath = $filesInDirectory | ForEach-Object { $_.FullName }
    if ($cscExePath.Count -gt 1) {
        Write-Host "Multiple versions of csc.exe found in ${CscTargetDirectory}"
        if ($cscExePath[0] -like "*Framework64*") {
            $cscPath64 = $cscExePath[0]
            #$cscPath32 = $cscExePath[1]
        }
        else {
            $cscPath64 = $cscExePath[1]
            #$cscPath32 = $cscExePath[0]
        }
    }
    else {
        $cscPath64 = $cscExePath
    }
}
catch {
    Write-Host "Error: Unable to retrieve files from $targetDirectory."
    Write-Host "Error: csc.exe not found. Make sure .NET Framework is installed."
    exit -1
}

# Compile the program.cs file
try {
    #$payload_name = Split-Path -Path $XORPayloadFilePath -Leaf
    $payload_name = [System.IO.Path]::GetFileNameWithoutExtension($XORPayloadFilePath)
    $outputExe = $TargetProjectFolder
    $outputExe = $outputExe -replace " ", ""
    $outputExe = $outputExe.TrimEnd("\\")
    $outputExe = "${outputExe}_${payload_name}.exe"
    & $cscPath64 /langversion:latest /target:exe /out:$outputExe $ProgramCsFilePath
    Write-Host "Compilation successful. Output saved as $outputExe"
}
catch {
    Write-Host "Error: Compilation failed. Check your source file and try again."
    exit -1
}