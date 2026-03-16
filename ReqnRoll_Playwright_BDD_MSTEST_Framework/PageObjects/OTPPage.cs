using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Playwright;
using ReqnRoll_Playwright_BDD_MSTEST_Framework.StepDefinitions;
using static Microsoft.Playwright.Assertions;

namespace ReqnRoll_Playwright_BDD_MSTEST_Framework.PageObjects
{
    public class OTPPage : BasePage
    {
        public OTPPage(IPage page) : base(page)
        {
        }

        //RegExp Pattern for retriving OTP Code
        string pattern = @"(?<=Your OTP code is:\s+)\d{6}";

        //locators 
        ILocator loc_h1PageHeader => _page.Locator("xpath=//h1");
        ILocator loc_txtOtp => _page.Locator("xpath=//input[@id=\"txtOtp\"]");

        ILocator loc_btnVerifyOTp => _page.Locator("xpath=//input[@id=\"btnVerify\"]");

        ILocator loc_lnkLogin => _page.Locator("xpath=//a[@id=\"lnkRegister\"]");

        ILocator loc_mdlSucessPopup => _page.Locator("xpath=//*[@id=\"successModal\"]");

        ILocator loc_lblInvalidOTPError => _page.Locator("xpath=//span[@id=\"lblMessage\"]");

        public async Task enterOTP(string email, string scenarioTitle) 
        {
            string otp = await retriveOTPCodeFromEmail(email, pattern);
            
            if (string.IsNullOrEmpty(otp))
            {
                throw new Exception($"Failed to retrieve OTP from email {email} within the timeout period.");
            }

            await populateInputField(loc_txtOtp, otp, scenarioTitle);
        }
        public async Task clickVerifyOTP(string scenarioTitle) => await clickElement(loc_btnVerifyOTp, scenarioTitle);

        public async Task handleSucessPopup(string expectedPopupText, string btnName) => await handlePopupModel(loc_mdlSucessPopup, expectedPopupText, btnName);

        public async Task clickLoginLink(string scenarioTitle) => await clickElement(loc_lnkLogin, scenarioTitle);
        public async Task verifyIncorrectOTPEntry(string expectedMsg) =>  await Expect(loc_lblInvalidOTPError).ToHaveTextAsync(expectedMsg);

        //Assertion Methods
        public async Task isUserOnOTPPage(string scenarioTitle, string expectedbtnText) 
        {
            try
            {
                // Try both text and value for the button
                await Expect(loc_btnVerifyOTp).ToBeVisibleAsync(new() { Timeout = 10000 });
                var btnText = await loc_btnVerifyOTp.InnerTextAsync();
                if (string.IsNullOrWhiteSpace(btnText)) btnText = await loc_btnVerifyOTp.GetAttributeAsync("value");
                
                Assert.IsTrue(btnText.Contains(expectedbtnText, StringComparison.OrdinalIgnoreCase), $"Expected button text to contain '{expectedbtnText}' but found '{btnText}'");
                
                await _screenshotManager.captureScreenshot(scenarioTitle, Hooks.ScreenshotsPath, "Validate_OTP_Navigation");
            }
            catch (Exception e)
            {
                Log.Information($"Issue Validating OTP Navigation...{e.Message}");
                await _screenshotManager.captureScreenshot(scenarioTitle, Hooks.ScreenshotsPath, "Failure_to_navigate_to_OTP_Page");
                throw; // Re-throw to fail the step
            }
        }
    }
}
