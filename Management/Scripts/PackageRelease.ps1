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
$zipPath = Join-Path -Path $json.releasePath -ChildPath $zipName

# Ensure the release directory exists, create if not
if (-not (Test-Path -Path $json.releasePath))
{
    New-Item -ItemType Directory -Path $json.releasePath
}

# Ensure the gitignore exists, create if not
$ignorePath = Join-Path -Path $json.releasePath -ChildPath '.gitignore'

if (-not (Test-Path -Path $ignorePath))
{
    Set-Content -Path $ignorePath -Value "*`n!.gitignore"
}

# Create a temporary directory for zipping to avoid modifying original data
$temp = Join-Path -Path $env:TEMP -ChildPath ([Guid]::NewGuid().ToString())
New-Item -ItemType Directory -Path $temp

# Copy all necessary files to the temporary directory
$contents = Join-Path -Path $json.contentPath -ChildPath "*"
Copy-Item -Path $contents -Destination $temp -Recurse
Copy-Item -Path $json.dllPath -Destination $temp

# Compress and clean up
Compress-Archive -Path "$temp\*" -DestinationPath $zipPath -Force
Remove-Item -Path $temp -Recurse