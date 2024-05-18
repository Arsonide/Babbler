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

# Copy the README from our content directory to our git README
$sourcePath = Join-Path -Path $json.contentPath -ChildPath "README.md"

if (Test-Path -Path $sourcePath)
{
    Copy-Item -Path $sourcePath -Destination $json.gitReadmePath -Force
}