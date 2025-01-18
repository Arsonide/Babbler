# Input parameters
param
(
    [Parameter(Mandatory = $true)]
    [object]$json,
    [Parameter(Mandatory = $true)]
    [string]$location
)

# Set the script's working directory
Set-Location -Path $location

# Assign paths from JSON to variables
$interopPath = [Environment]::ExpandEnvironmentVariables($json.interopPath)
$libraryPath = [Environment]::ExpandEnvironmentVariables($json.libraryPath)

if (!(Test-Path -Path $libraryPath))
{
    New-Item -Path $libraryPath -ItemType SymbolicLink -Value $interopPath
}
elseif (!((Get-Item $libraryPath -Force -ea SilentlyContinue).Attributes -band [IO.FileAttributes]::ReparsePoint))
{
    # Get all DLL files in the interop directory
    $interopFiles = Get-ChildItem -Path $interopPath -Filter "*.dll"

    # Iterate through each DLL file in the interop directory
    foreach ($file in $interopFiles)
    {
        $libraryFile = Join-Path -Path $libraryPath -ChildPath $file.Name

        # If file is in library directory, overwrite it there with the interop version
        if (Test-Path -Path $libraryFile)
        {
            Copy-Item -Path $file.FullName -Destination $libraryFile -Force
        }
    }
}