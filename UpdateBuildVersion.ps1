param([string]$assemblyVersion)
$ErrorActionPreference = "Stop"

Write-Host Setting AssemblyVersion to: $assemblyVersion

# ls

Get-ChildItem -Path $PSScriptRoot -Filter *.csproj -Recurse -File -Name | ForEach-Object {
    $projectName = [System.IO.Path]::GetFullPath($_)    
    Write-Host Searching Project: $projectName
    $updated = 0

    $xml = [xml](Get-Content $projectName)
    $nodes = $xml.Project.ChildNodes
    foreach ($node in $nodes) {
        if ($node.Name -eq "PropertyGroup") {
            
            $current = "[" + $node.AssemblyVersion + "]"
            if ($current -ne "[]") {
                $node.AssemblyVersion = $assemblyVersion
                $updated = 1;
            }

            $current = "[" + $node.FileVersion + "]"
            if ($current -ne "[]") {
                $node.FileVersion = $assemblyVersion
                $updated = 1;
            }
            
            $current = "[" + $node.Version + "]"
            if ($current -ne "[]") {
                $node.Version = $assemblyVersion
                $updated = 1;
            }
        }
    }

    if ($updated -eq 1) {
        Write-Host Updated Project: $projectName
        $xml.Save($projectName)
        
        Get-Content $projectName | foreach {Write-Output $_}
    }
}

