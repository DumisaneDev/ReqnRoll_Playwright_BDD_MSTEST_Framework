using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Playwright;
using ReqnRoll_Playwright_BDD_MSTEST_Framework.Utils;
using ReqnRoll_Playwright_BDD_MSTEST_Framework.StepDefinitions;
using static Microsoft.Playwright.Assertions;

namespace ReqnRoll_Playwright_BDD_MSTEST_Framework.PageObjects
{
    public class EditUserPage : BasePage
    {
        public EditUserPage(IPage page) : base(page)
        {
        }

        // Locators
        private ILocator loc_txtEmail => _page.Locator("xpath=//input[@id='txtEmail']");
        private ILocator loc_txtFirstName => _page.Locator("xpath=//input[@id='txtFname']");
        private ILocator loc_txtLastName => _page.Locator("xpath=//input[@id='txtLname']");
        private ILocator loc_btnUpdate => _page.Locator("xpath=//input[@id='btnUpdate']");
        private ILocator loc_mdlSuccess => _page.Locator("xpath=//div[@id='successModal']");
        private ILocator loc_btnSuccessOk => _page.Locator("xpath=//div[@id='successModal']//button[text()='OK']");

        private ILocator getPermissionDropdown(string permission)
        {
            // Search for label containing the text, case-insensitive
            string searchName = permission.ToLower();
            // Handle common mapping if needed
            if (searchName == "allocation") searchName = "allocation"; // already contains allocation

            return _page.Locator($"xpath=//label[contains(translate(text(), 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), '{searchName}')]/following-sibling::select | //label[contains(translate(., 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), '{searchName}')]/following-sibling::select").First;
        }

        // Web Action Methods
        public async Task setPermissionStatus(string permission, string status, string scenarioTitle)
        {
            // status should be "Visible" or "Hidden"
            await selectDropdownValue(getPermissionDropdown(permission), status, scenarioTitle);
        }

        public async Task clickUpdate(string scenarioTitle)
        {
            await clickElement(loc_btnUpdate, scenarioTitle);
        }

        public async Task clickSuccessOk(string scenarioTitle)
        {
            await clickElement(loc_btnSuccessOk, scenarioTitle);
        }

        // Validation Methods
        public async Task verifyUpdateSuccess(string scenarioTitle)
        {
            try
            {
                await Expect(loc_mdlSuccess).ToBeVisibleAsync();
                await Expect(loc_mdlSuccess).ToContainTextAsync("Successfully updated");
                await _screenshotManager.captureScreenshot(scenarioTitle, Hooks.ScreenshotsPath, "Update_Success_Modal");
            }
            catch (Exception ex)
            {
                Log.Information($"Exception hit when verifying update success...{ex.Message}");
                _errorTranslator.Translate(ex);
            }
        }
    }
}
