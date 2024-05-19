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
$projectPath = [Environment]::ExpandEnvironmentVariables($json.projectPath)
$manifestPath = [Environment]::ExpandEnvironmentVariables($json.manifestPath)

$packageName = "SOD.Common"

# Update the package using dotnet CLI
dotnet add $projectPath package $packageName

# Retrieve the updated package version
$info = dotnet list $projectPath package | Where-Object { $_ -match $packageName }

# Parse the output to find the current version
if ($info -match ">\s$packageName\s+(\S+)\s+(\S+)")
{
    $currentVersion = $matches[1]

    # Read the content of the manifest file
    $manifestContent = Get-Content -Path $manifestPath -Raw

    # Regex to find and replace the version number
    $pattern = '("Venomaus-SODCommon-)[\d\.]+(")'
    $replacement = "`${1}$currentVersion`$2"

    # Replace the version number in the manifest content
    $updatedContent = $manifestContent -replace $pattern, $replacement

    # Write the updated content back to the manifest file
    Set-Content -Path $manifestPath -Value $updatedContent -NoNewLine
}