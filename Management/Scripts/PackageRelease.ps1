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

# Retrieve data from the json
$latestVersion = $json.versions | Sort-Object version -Descending | Select-Object -First 1
$zipName = "$($json.creatorName)-$($json.modName)-$($latestVersion.version).zip"
$releasePath = [Environment]::ExpandEnvironmentVariables($json.releasePath)
$zipPath = Join-Path -Path $releasePath -ChildPath $zipName
$contentPath = [Environment]::ExpandEnvironmentVariables($json.contentPath);
$dllPath = [Environment]::ExpandEnvironmentVariables($json.dllPath);

# Ensure the release directory exists, create if not
if (-not (Test-Path -Path $releasePath))
{
    New-Item -ItemType Directory -Path $releasePath
}

# Ensure the gitignore exists, create if not
$ignorePath = Join-Path -Path $releasePath -ChildPath '.gitignore'

if (-not (Test-Path -Path $ignorePath))
{
    Set-Content -Path $ignorePath -Value "*`n!.gitignore"
}

# Create a temporary directory for zipping to avoid modifying original data
$temp = Join-Path -Path $env:TEMP -ChildPath ([Guid]::NewGuid().ToString())
New-Item -ItemType Directory -Path $temp

# Copy all necessary files to the temporary directory
$contents = Join-Path -Path $contentPath -ChildPath "*"
Copy-Item -Path $contents -Destination $temp -Recurse
Copy-Item -Path $dllPath -Destination $temp

# Compress and clean up
Compress-Archive -Path "$temp\*" -DestinationPath $zipPath -Force
Remove-Item -Path $temp -Recurse