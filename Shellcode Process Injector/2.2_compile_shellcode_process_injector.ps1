param (
    [Parameter(Mandatory = $true)]
    [string]$ProgramCsFilePath,

    [Parameter(Mandatory = $true)]
    [string]$XORPayloadFilePath
)

# COMPILATION
# Locate the csc.exe binary
#$targetDirectory = "C:\\Windows\\Microsoft.NET"
$targetDirectory = "C:\\Program Files (x86)\\Microsoft Visual Studio\\2019\\Community\\MSBuild\\Current\\Bin\\Roslyn"

try {
    $filesInDirectory = Get-ChildItem -Path $targetDirectory -File -Recurse -Filter csc.exe
    Write-Host "Found files in ${targetDirectory}:"
    $cscExePath = $filesInDirectory | ForEach-Object { $_.FullName }
    if ($cscExePath.Count -gt 1) {
        Write-Host "Multiple versions of csc.exe found in ${targetDirectory}"
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
    $outputExe = "ShellcodeProcessInjector_${payload_name}.exe"
    & $cscPath64 /langversion:latest /target:exe /out:$outputExe $ProgramCsFilePath
    Write-Host "Compilation successful. Output saved as $outputExe"
}
catch {
    Write-Host "Error: Compilation failed. Check your source file and try again."
    exit -1
}