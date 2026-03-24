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
    # Try to remove files and folders individually to handle long paths and locks better
    Get-ChildItem -Path ".\TestResults" -Recurse | Remove-Item -Recurse -Force -ErrorAction SilentlyContinue
    Remove-Item -Path ".\TestResults" -Recurse -Force -ErrorAction SilentlyContinue
    
    # Wait a moment for OS to release locks
    Start-Sleep -Milliseconds 500
    
    if (Test-Path -Path ".\TestResults") {
        Write-Host "Manual cleanup failed, trying one more time..." -ForegroundColor Yellow
        Remove-Item -Path ".\TestResults" -Recurse -Force -ErrorAction SilentlyContinue
    }
}

Write-Host "Creating new TestResults directory structure..." -ForegroundColor Cyan
# Ensure directory is created, use -Force to overwrite if it somehow still exists
New-Item -ItemType Directory -Path ".\TestResults" -Force | Out-Null
New-Item -ItemType Directory -Path ".\TestResults\Screenshots" -Force | Out-Null
New-Item -ItemType Directory -Path ".\TestResults\Traces" -Force | Out-Null

$projectPath = ".\ReqnRoll_Playwright_BDD_MSTEST_Framework\ReqnRoll_Playwright_BDD_MSTEST_Framework.csproj"
$settingsPath = ".\ReqnRoll_Playwright_BDD_MSTEST_Framework\.runsettings"

$command = "dotnet test $projectPath --filter `"TestCategory=$Category`" -s $settingsPath --results-directory $resultsDir --logger `"trx;LogFileName=TestReport.trx`""

if ($ShowOutput) {
    Write-Host "Console Output: Enabled (Detailed)" -ForegroundColor Green
    $command += " --logger `"console;verbosity=detailed`""
}

if ($Parallel) {
    Write-Host "Executing in Parallel with $Workers workers..." -ForegroundColor Green
    $env:TEST_WORKERS = $Workers
    $command += " -- MSTest.Parallelize.Workers=$Workers MSTest.Parallelize.Scope=MethodLevel"
} else {
    Write-Host "Executing Sequentially (1 worker)..." -ForegroundColor Yellow
    $env:TEST_WORKERS = 1
    $command += " -- MSTest.Parallelize.Workers=1 MSTest.Parallelize.Scope=MethodLevel"
}

Write-Host "Running command: $command" -ForegroundColor Gray
Invoke-Expression $command

# Capture the exit code but exit with 0 so the pipeline task doesn't turn red.
# The 'Publish Test Results' task in the pipeline will handle marking the build as failed if tests didn't pass.
$exitCode = $LASTEXITCODE
if ($exitCode -ne 0) {
    Write-Host "--- Tests finished with some failures (Exit Code: $exitCode) ---" -ForegroundColor Yellow
} else {
    Write-Host "--- All tests passed successfully! ---" -ForegroundColor Green
}

exit 0
