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

# Start the changelog
$changelog = "# CHANGELOG`n`n"


# Sort versions by their version number in ascending order
$sortedVersions = $json.versions | Sort-Object version

if ($json.versions.Count -ne 1)
{
     # Handle multiple versions, starting with the latest
    $latestVersion = $sortedVersions[-1]
    $changelog += "### Latest Release`n**$($latestVersion.version)**`n$($latestVersion.changes)`n`n---------`n`n"

    # Loop through the middle versions
    for ($i = $sortedVersions.Count - 2; $i -gt 0; $i--)
    {
        $version = $sortedVersions[$i]
        $changelog += "**$($version.version)**`n$($version.changes)`n`n---------`n`n"
    }

    # Conclude with the initial release
    $initialVersion = $sortedVersions[0]
    $changelog += "### Initial Release`n**$($initialVersion.version)**`n$($initialVersion.changes)"
}
else
{
    # With only one version, just set up the initial version
    $version = $sortedVersions[0]
    $changelog += "### Initial Release`n**$($version.version)**`n$($version.changes)"
}

$changelogPath = [Environment]::ExpandEnvironmentVariables($json.changelogPath)
Set-Content $changelogPath -Value $changelog -NoNewLine