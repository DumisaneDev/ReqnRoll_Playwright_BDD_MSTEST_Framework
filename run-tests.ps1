param (
    [Parameter(Mandatory=$false)]
    [ValidateSet("smoke", "regression", "all")]
    [string]$Category = "all",

    [Parameter(Mandatory=$false)]
    [switch]$Parallel = $false,

    [Parameter(Mandatory=$false)]
    [int]$Workers = 3,

    [Parameter(Mandatory=$false)]
    [switch]$ShowOutput = $false
)

$timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
$resultsDir = ".\TestResults\MSTEST_Report_$timestamp"

Write-Host "--- Test Execution Setup ---" -ForegroundColor Cyan
Write-Host "Category: $Category"
Write-Host "Parallel: $Parallel"
Write-Host "Show Output: $ShowOutput"
Write-Host "Results Directory: $resultsDir"

if (Test-Path -Path ".\TestResults") {
    Write-Host "Cleaning existing TestResults..." -ForegroundColor Cyan
    Remove-Item -Path ".\TestResults" -Recurse -Force
}

Write-Host "Creating new TestResults directory structure..." -ForegroundColor Cyan
New-Item -ItemType Directory -Path ".\TestResults"
New-Item -ItemType Directory -Path ".\TestResults\Screenshots"
New-Item -ItemType Directory -Path ".\TestResults\Videos"
New-Item -ItemType Directory -Path ".\TestResults\Traces"

$projectPath = ".\ReqnRoll_Playwright_BDD_MSTEST_Framework\ReqnRoll_Playwright_BDD_MSTEST_Framework.csproj"
$settingsPath = ".\ReqnRoll_Playwright_BDD_MSTEST_Framework\.runsettings"

$command = "dotnet test $projectPath --filter `"TestCategory=$Category`" -s $settingsPath --results-directory $resultsDir --logger `"trx;LogFileName=TestReport.trx`""

if ($ShowOutput) {
    Write-Host "Console Output: Enabled (Detailed)" -ForegroundColor Green
    $command += " --logger `"console;verbosity=detailed`""
}

if ($Parallel) {
    Write-Host "Executing in Parallel with $Workers workers..." -ForegroundColor Green
    $command += " -- MSTest.Parallelize.Workers=$Workers MSTest.Parallelize.Scope=MethodLevel"
} else {
    Write-Host "Executing Sequentially (1 worker)..." -ForegroundColor Yellow
    $command += " -- MSTest.Parallelize.Workers=1 MSTest.Parallelize.Scope=MethodLevel"
}

Write-Host "Running command: $command" -ForegroundColor Gray
Invoke-Expression $command
