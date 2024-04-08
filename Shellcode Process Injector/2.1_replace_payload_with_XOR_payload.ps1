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