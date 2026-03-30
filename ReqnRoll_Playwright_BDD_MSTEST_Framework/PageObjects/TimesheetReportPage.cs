using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Playwright;
using ReqnRoll_Playwright_BDD_MSTEST_Framework.StepDefinitions;
using ReqnRoll_Playwright_BDD_MSTEST_Framework.Utils;
using static Microsoft.Playwright.Assertions;

namespace ReqnRoll_Playwright_BDD_MSTEST_Framework.PageObjects
{
    public class TimesheetReportPage : BasePage
    {
        public TimesheetReportPage(IPage page) : base(page)
        {
        }

        // Locators
        private ILocator loc_h1PageHeader => _page.Locator("h1.card-title");
        private ILocator loc_ddlEmployees => _page.Locator("#ddlEmployees");
        private ILocator loc_ddlMonth => _page.Locator("#ddlMonth");
        private ILocator loc_btnGenerateReport => _page.Locator("#btnGenerateReport");
        private ILocator loc_btnBack => _page.Locator("#btnBack");
    
        public async Task selectEmployee(string employeeName, string scenarioTitle)
        {
            await selectDropdownValue(loc_ddlEmployees, employeeName, scenarioTitle);
        }

        public async Task selectMonth(string month, string scenarioTitle)
        {
            await selectDropdownValue(loc_ddlMonth, month, scenarioTitle);
        }

        public async Task clickGenerateReport(string scenarioTitle)
        {
            await clickElement(loc_btnGenerateReport, scenarioTitle);
        }

        public async Task generateReport(string scenarioTitle)
        {
            try
            {
                // The user mentioned it downloads a file called TimesheetReport.pdf
                var download = await _page.RunAndWaitForDownloadAsync(async () =>
                {
                    await clickElement(loc_btnGenerateReport, scenarioTitle);
                });

                Log.Information($"Report downloaded: {download.SuggestedFilename}");

                // Optionally save it
                string downloadPath = Path.Combine(Hooks.ScreenshotsPath, "..", "Downloads");
                Directory.CreateDirectory(downloadPath);
                await download.SaveAsAsync(Path.Combine(downloadPath, download.SuggestedFilename));
            }
            catch (Exception ex)
            {
                Log.Error($"Exception hit during report generation: {ex.Message}");
                // Handle the case where the download times out, likely due to a validation error/alert
                string errorMessage = ex.Message.Contains("Timeout") 
                    ? "Timeout waiting for report download. No download was triggered, and no validation alert was detected or handled." 
                    : ex.Message;
                
                _errorTranslator.Translate(ex, $"Generating Timesheet Report - {errorMessage}");
            }
        }

        public async Task clickBack(string scenarioTitle)
        {
            await clickElement(loc_btnBack, scenarioTitle);
        }

        public async Task isUserOnTimesheetReportPage(string scenarioTitle, string expectedHeader)
        {
            try
            {
                await Expect(loc_h1PageHeader).ToBeVisibleAsync();
                await Expect(loc_h1PageHeader).ToHaveTextAsync(expectedHeader);
            }
            catch (Exception ex) 
            {
                Log.Information($"Failure verifying user navigation...{ex.Message}");
                await _screenshotManager.captureScreenshot(scenarioTitle, _screenshotPath, "Failure_Verifying_Navigation_To_Timesheet_Report_Page");
                _errorTranslator.Translate(ex);
            }
        }

    }
}
