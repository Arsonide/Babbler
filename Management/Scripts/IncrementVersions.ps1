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

# Determine the latest version from JSON data
$latestVersion = $json.versions | Sort-Object version -Descending | Select-Object -First 1

# Update specified files with the latest version number
foreach ($operation in $json.operations)
{
    $fileContent = Get-Content $operation.file -Raw
    $newContent = $fileContent -replace $operation.search, $operation.replace -replace '{version}', $latestVersion.version
    Set-Content $operation.file -Value $newContent -NoNewLine
}