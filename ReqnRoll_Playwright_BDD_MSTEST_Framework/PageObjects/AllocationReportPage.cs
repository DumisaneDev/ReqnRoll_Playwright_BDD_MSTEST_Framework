using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Playwright;
using ReqnRoll_Playwright_BDD_MSTEST_Framework.StepDefinitions;
using ReqnRoll_Playwright_BDD_MSTEST_Framework.Utils;
using static Microsoft.Playwright.Assertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ReqnRoll_Playwright_BDD_MSTEST_Framework.PageObjects
{
    public class AllocationReportPage : BasePage
    {
        private readonly DialogState _dialogState;

        public AllocationReportPage(IPage page, DialogState dialogState) : base(page)
        {
            _dialogState = dialogState;
        }

        // Locators
        private ILocator loc_h1PageHeader => _page.Locator("h1.card-title");
        private ILocator loc_ddlEmployees => _page.Locator("#ddlEmployees");
        private ILocator loc_txtStartDate => _page.Locator("#txtStartDate");
        private ILocator loc_txtEndDate => _page.Locator("#txtEndDate");
        private ILocator loc_btnGenerateReport => _page.Locator("#btnGenerateReport");
        private ILocator loc_btnBack => _page.Locator("#btnBack");

        // Sidebar Navigation
        private ILocator loc_lnkReports => _page.GetByRole(AriaRole.Link, new() { Name = "Report" });
        private ILocator loc_lnkAllocationReport => _page.Locator("xpath=//a[@href='AllocationReport.aspx']");

        public async Task navigateToAllocationReport(string scenarioTitle)
        {
            await clickElement(loc_lnkReports, scenarioTitle);
            await clickElement(loc_lnkAllocationReport, scenarioTitle);
        }

        public async Task selectEmployee(string employeeName, string scenarioTitle)
        {
            await selectDropdownValue(loc_ddlEmployees, employeeName, scenarioTitle);
        }

        public async Task enterStartDate(string startDate, string scenarioTitle)
        {
            // Format for date input is YYYY-MM-DD
            await populateInputField(loc_txtStartDate, startDate, scenarioTitle);
        }

        public async Task enterEndDate(string endDate, string scenarioTitle)
        {
            await populateInputField(loc_txtEndDate, endDate, scenarioTitle);
        }

        public async Task clickGenerateReport(string scenarioTitle)
        {
            await clickElement(loc_btnGenerateReport, scenarioTitle);
        }

        public async Task clickBack(string scenarioTitle)
        {
            await clickElement(loc_btnBack, scenarioTitle);
        }

        public async Task verifyNoAllocationAlert(string expectedMessage)
        {
            // Give some time for the alert to be captured
            for (int i = 0; i < 5; i++)
            {
                if (!string.IsNullOrEmpty(_dialogState.LastMessage))
                    break;
                await Task.Delay(500);
            }

            Assert.AreEqual(expectedMessage, _dialogState.LastMessage, "The alert message did not match the expected message.");
        }

        public async Task isUserOnAllocationReportPage(string expectedHeader)
        {
            await Expect(loc_h1PageHeader).ToBeVisibleAsync();
            await Expect(loc_h1PageHeader).ToHaveTextAsync(expectedHeader);
        }

    }
}
