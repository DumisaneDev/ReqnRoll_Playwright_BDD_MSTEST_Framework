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

        // Sidebar Navigation
        private ILocator loc_lnkReports => _page.GetByRole(AriaRole.Link, new() { Name = "Report" });
        private ILocator loc_lnkTimesheetReport => _page.Locator("xpath=//a[@href='REPORTS.aspx']");

        public async Task navigateToTimesheetReport(string scenarioTitle)
        {
                await clickElement(loc_lnkReports, scenarioTitle);
                await clickElement(loc_lnkTimesheetReport, scenarioTitle);
        }

        public async Task selectEmployee(string employeeName, string scenarioTitle)
        {
            await selectDropdownValue(loc_ddlEmployees, employeeName, scenarioTitle);
        }

        public async Task selectMonth(string month, string scenarioTitle)
        {
            await selectDropdownValue(loc_ddlMonth, month, scenarioTitle);
        }

        public async Task generateReport(string scenarioTitle)
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

        public async Task clickBack(string scenarioTitle)
        {
            await clickElement(loc_btnBack, scenarioTitle);
        }

        public async Task isUserOnTimesheetReportPage(string expectedHeader)
        {
            await Expect(loc_h1PageHeader).ToBeVisibleAsync();
            await Expect(loc_h1PageHeader).ToHaveTextAsync(expectedHeader);
        }

    }
}
