using System;
using System.Collections.Generic;
using System.Text;
using ReqnRoll_Playwright_BDD_MSTEST_Framework.StepDefinitions;
using ReqnRoll_Playwright_BDD_MSTEST_Framework.Utils;
using static Microsoft.Playwright.Assertions;

namespace ReqnRoll_Playwright_BDD_MSTEST_Framework.PageObjects
{
    public class DashboardPage : BasePage
    {
        public DashboardPage(IPage page) : base(page)
        {
        }
        //locators
        ILocator loc_bnrWelcome => _page.Locator("xpath=//h1[text()=\"Welcome To Our Dashboard\"]");
        ILocator loc_lblActiveProjects => _page.Locator("xpath=//span[@id=\"lblActiveProjects\"]");
        ILocator loc_lblEmployees => _page.Locator("xpath=//span[@id=\"lblTeamMembers\"]");
        ILocator loc_lblTimesheetsSubmitted => _page.Locator("xpath=//span[@id=\"lblTimesheetsSubmitted\"]");
        ILocator loc_lblReports => _page.Locator("xpath=//span[@id=\"lblReports\"]");
        ILocator loc_h3Performance => _page.Locator("xpath=//h3[text()=\"Monthly Performance\"]");
        ILocator loc_PerformanceChart => _page.Locator("xpath=//canvas[@id=\"myChart\"]");

        ILocator loc_lnkDashboard => _page.Locator("xpath=//a[@href=\"Dashboard.aspx\"]");
        ILocator loc_lnkProject => _page.Locator("xpath=//a[@href=\"Project.aspx\"]");
        ILocator loc_lnkEmployee => _page.Locator("xpath=//a[@href=\"Employee.aspx\"]");
        ILocator loc_lnkClient => _page.Locator("xpath=//a[@href=\"ClientList.aspx\"]");
        ILocator loc_lnkTimesheet => _page.Locator("xpath=//a[@href=\"Timesheet.aspx\"]");
        ILocator loc_lnkRoomBooking => _page.Locator("xpath=//a[@href=\"meetingRoomBooking.aspx\"]");
        ILocator loc_lnkPermissions => _page.Locator("xpath=//a[@href=\"Permissions.aspx\"]");
        ILocator loc_lnkWorkAllocationList => _page.Locator("xpath=//a[@href=\"WorkAllocationList.aspx\"]");
        ILocator loc_ddlLeave => _page.GetByRole(AriaRole.Link, new() { Name = "Leave" });
        ILocator loc_lnkLeaveRequest => _page.Locator("xpath=//a[@href=\"LeaveRequestPage.aspx\"]");
        ILocator loc_lnkLeaveRequestList => _page.Locator("xpath=//a[@href=\"LeaveRequestList.aspx\"]");
        ILocator loc_lnkReports => _page.GetByRole(AriaRole.Link, new() { Name = "Report" });
        ILocator loc_lnkAllocationReport => _page.Locator("xpath=//a[@href=\"AllocationReport.aspx\"]");
        ILocator loc_lnkLogout => _page.Locator("xpath=//a[@href=\"Login.aspx\"]");

        //Web Action methods
        public async Task navigateToPermission(string scenarioTitle) 
        {
         await clickElement(loc_lnkPermissions, scenarioTitle);
        }

        public async Task navigateToAllocationReport(string scenarioTitle) 
        {
            await clickElement(loc_lnkReports, scenarioTitle);
            await clickElement(loc_lnkAllocationReport, scenarioTitle);
        }
        public async Task isUserOnDashboard(string expectedHeader) 
        {
            try
            {
                await Expect(loc_bnrWelcome).ToBeVisibleAsync();
                await Expect(loc_bnrWelcome).ToHaveTextAsync(expectedHeader);
            }
            catch (Exception ex) 
            {
                Log.Error($"Failure verifying if User is on the dashboard...{ex.Message}");
                _errorTranslator.Translate(ex);
            }
        }

        public async Task isUserRedirectedToDashboard(string partialURL) 
        {
            // Use a longer timeout and regex to handle variations like Dashboard.aspx or different environments
            await Expect(_page).ToHaveURLAsync(new System.Text.RegularExpressions.Regex(partialURL, System.Text.RegularExpressions.RegexOptions.IgnoreCase), new() { Timeout = 15000 });
        }
        public async Task isEmployeeOnDashboard(string dashTabText, 
            string projectTabText,
            string employeeTabText,
            string timesheetTabText,
            string roomBookingTabText,
            string permissionsTabText,
            string scenarioTitle
            ) 
        {
            try
            {
                await Expect(loc_lnkDashboard).ToHaveTextAsync(dashTabText);
                await Expect(loc_lnkProject).ToHaveTextAsync(projectTabText);
                await Expect(loc_lnkEmployee).ToHaveTextAsync(employeeTabText);
                await Expect(loc_lnkTimesheet).ToHaveTextAsync(timesheetTabText);
                await Expect(loc_lnkRoomBooking).ToHaveTextAsync(roomBookingTabText);
                await Expect(loc_lnkPermissions).ToHaveTextAsync(permissionsTabText);
                await Expect(loc_lnkLogout).ToBeVisibleAsync();
                await _screenshotManager.captureScreenshot(scenarioTitle, Hooks.ScreenshotsPath, $"Validating successfully employee entry");
            }
            catch (Exception ex) 
            {
                Log.Information($"Exception hit when verifying employee dashboard tabs...{ex.Message}");
                _errorTranslator.Translate(ex);
            }
        }

        public async Task isAdminOnDashboard(string scenarioTitle) 
        {
            try
            {
                await Expect(loc_lnkDashboard).ToBeVisibleAsync();
                await Expect(loc_lnkProject).ToBeVisibleAsync();
                await Expect(loc_lnkEmployee).ToBeVisibleAsync();
                await Expect(loc_lnkClient).ToBeVisibleAsync();
                await Expect(loc_lnkTimesheet).ToBeVisibleAsync();
                await Expect(loc_lnkRoomBooking).ToBeVisibleAsync();
                await Expect(loc_lnkPermissions).ToBeVisibleAsync();
                await Expect(loc_lnkWorkAllocationList).ToBeVisibleAsync();
                await Expect(loc_ddlLeave).ToBeVisibleAsync();
                await Expect(loc_lnkReports).ToBeVisibleAsync();
                await Expect(loc_lnkLogout).ToBeVisibleAsync();
                await _screenshotManager.captureScreenshot(scenarioTitle, Hooks.ScreenshotsPath, $"Validating successfully admin entry");
            }
            catch (Exception ex) 
            {
                Log.Information($"Exception hit when verifying admin dashboard tabs...{ex.Message}");
                _errorTranslator.Translate(ex);
            }
        }
    }
}
