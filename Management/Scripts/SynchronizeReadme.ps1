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

$contentPath = [Environment]::ExpandEnvironmentVariables($json.contentPath)
$gitReadmePath = [Environment]::ExpandEnvironmentVariables($json.gitReadmePath)

# Copy the README from our content directory to our git README
$sourcePath = Join-Path -Path $contentPath -ChildPath "README.md"

if (Test-Path -Path $sourcePath)
{
    Copy-Item -Path $sourcePath -Destination $gitReadmePath -Force
}