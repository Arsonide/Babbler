# Display menu
Write-Host "Select the mod management operations you want to execute (e.g. '1234'):"
Write-Host
Write-Host "1. Increment Version Numbers"
Write-Host "2. Synchronize README Files"
Write-Host "3. Build Changelog"
Write-Host "4. Package Release Zip"
Write-Host
Write-Host "0. Execute Everything"
Write-Host

# Read user input
$input = Read-Host "Enter your selected operations"

# Define script paths
$scriptPaths =
@{
    "1" = ".\Scripts\IncrementVersions.ps1"
    "2" = ".\Scripts\SynchronizeReadme.ps1"
    "3" = ".\Scripts\BuildChangelog.ps1"
    "4" = ".\Scripts\PackageRelease.ps1"
}

# Default to everything if we choose nothing
if ([string]::IsNullOrWhiteSpace($input))
{
    $input = "0"
}

# Do some replacements if we choose the compound shortcuts
if ($input.Contains("0"))
{
    $input = "1234"
}

# Load JSON data
$json = Get-Content '.\Data.json' -Raw | ConvertFrom-Json

# Get the current directory
$location = Get-Location

try
{
    # Execute scripts in a specific order regardless of user selections
    foreach ($key in "1", "2", "3", "4")
    {
        if ($input.Contains($key))
        {
            & $scriptPaths[$key] -json $json -location $location
            Clear-Host
        }
    }
    
    Write-Host
    Write-Host "Selected operations executed successfully. Press ENTER to exit..."
}
catch
{
    Write-Host
    Write-Host "Selected operations executed unsuccessfully due to errors. Press ENTER to exit..."
}

# Wait for user to exit
$null = Read-Host