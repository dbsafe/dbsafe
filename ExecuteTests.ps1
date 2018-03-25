$ErrorActionPreference = "Stop"

# ls

Write-Host Execute Tests Script

$vstest_console = '"D:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe"'

Write-Host vstest.console.exe path: $vstest_console 

Get-ChildItem -Path $PSScriptRoot -Filter *Test*.dll -Exclude *VisualStudio* -Recurse -File -Name | ForEach-Object {
	if ($_ -match "bin") {
        Write-Host Executing Test: $_
		
		$command = $vstest_console + ' "' + $_ + '"'
		Write-Host command: $command
		Invoke-Expression "& $command"
    }
}
