$ErrorActionPreference = "Stop"

# ls

Write-Host Deployment Script

Get-ChildItem -Path $PSScriptRoot -Filter *.nupkg -Exclude *.symbols.nupkg -Recurse -File -Name | ForEach-Object {
    if ($_ -match "bin\\Release") {
        # Write-Host Publishing $_
        # nuget.exe push $_ -Source https://www.nuget.org/api/v2/package -ApiKey $env:api_key
    }
}
