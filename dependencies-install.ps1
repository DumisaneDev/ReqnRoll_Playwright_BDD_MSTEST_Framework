$ErrorActionPreference = "Stop"

Write-Host "Restoring Project Dependencies for ReqnRoll Playwright" -ForegroundColor Cyan
dotnet restore

Write-Host "Installing Playwright Browsers & Reporting dependencies and plugins." -ForegroundColor Cyan
dotnet add package Microsoft.Playwright --project .\ReqnRoll_Playwright_BDD_MSTEST_Framework\ReqnRoll_Playwright_BDD_MSTEST_Framework.csproj
dotnet add package Microsoft.Playwright.MSTest  --project .\ReqnRoll_Playwright_BDD_MSTEST_Framework\ReqnRoll_Playwright_BDD_MSTEST_Framework.csproj
dotnet build ReqnRoll_Playwright_BDD_MSTEST_Framework/ReqnRoll_Playwright_BDD_MSTEST_Framework.csproj
powershell -ExecutionPolicy Bypass -File .\ReqnRoll_Playwright_BDD_MSTEST_Framework\bin\Debug\net10.0\playwright.ps1 install



