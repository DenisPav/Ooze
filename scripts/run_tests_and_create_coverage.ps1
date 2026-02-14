$directories = Get-ChildItem -Path . -Recurse -Directory -Force -ErrorAction SilentlyContinue |
    Where-Object { $_.Name -eq "CoverageReport" -or $_.Name -eq "TestResults" }

if ($directories) {
    $count = ($directories | Measure-Object).Count

    foreach ($dir in $directories) {
        Remove-Item -Path $dir.FullName -Recurse -Force -ErrorAction SilentlyContinue
    }

}

Write-Host "Running test projects" -ForegroundColor Cyan
dotnet test -- --coverage --coverage-output-format cobertura --ignore-exit-code 8

Write-Host "Creating reports" -ForegroundColor Cyan
dotnet reportgenerator "-reports:tests/**/TestResults/**/*.cobertura.xml" -targetdir:CoverageReport -reporttypes:Html_Dark,Badges