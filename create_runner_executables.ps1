# Specify the folder path where your .cs files are located
$folderPath = "./met_payloads/"

# Remove old xor_met_payloads folder
Get-ChildItem -Path .\\runner_executables\\ -Recurse | ForEach-Object { $_.Delete() }

mkdir .\\runner_executables

# Get all .cs files recursively
$csFiles = Get-ChildItem -Path $folderPath -Recurse -Filter "*.cs"

# Loop through each .cs file
foreach ($csFile in $csFiles) {
    # Construct the output file name (prepend "xor_" and change extension to .txt)
    $outputFileName = Join-Path -Path $csFile.Directory.FullName -ChildPath ("xor_" + $csFile.BaseName + ".txt")

    # Run MyProgram.exe with the .cs file as an argument
    # Redirect the output to the new file
    $XOREncoderPath = '.\XOR Shellcode Encoder\bin\Debug\XOR Shellcode Encoder.exe'
    & $XOREncoderPath $csFile.FullName > $outputFileName

    # Display a message indicating the operation
    Write-Host "Processed $($csFile.Name). Output saved to $outputFileName"
}
