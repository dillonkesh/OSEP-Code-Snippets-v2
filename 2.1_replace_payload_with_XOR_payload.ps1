param (
    [Parameter(Mandatory = $true)]
    [string]$TargetProjectFolder,

    [Parameter(Mandatory = $true)]
    [string]$XORPayloadFilePath
)

# STRING REPLACEMENT
# Get Program.cs file from project folder
$ProgramFilePath - $null
Get-ChildItem -Path $TargetProjectFolder -Recurse -File | ForEach-Object {
    if ($_.Name -eq "Program.cs") {
        $ProgramFilePath = $_.FullName
        Write-Host "Relative path to Program.cs: $ProgramFilePath"
        return
    }
}

# Read the Program.cs file
$ProgramContent = Get-Content -Path $ProgramFilePath -Raw

# Read the new byte array from the text file
$XORByteArray = Get-Content -Path $XORPayloadFilePath -AsByteStream

# Convert the byte array to a string
$newString = [System.Text.Encoding]::Default.GetString($XORByteArray)

# Search and replace the 'buf' byte array
$ProgramContent = $ProgramContent -replace '(byte\[\] buf = new byte\[)\d+\][^;]+;', $newString

# Save the modified .cs file
$ProgramContent | Set-Content -Path $ProgramFilePath

Write-Host "Contents of $ProgramFilePath updated successfully."

$scriptName = $MyInvocation.MyCommand.Name
Write-Host "${scriptName} done!"