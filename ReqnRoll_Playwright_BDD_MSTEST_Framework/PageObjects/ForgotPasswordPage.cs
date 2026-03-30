using System;
using System.Collections.Generic;
using System.Text;
using ReqnRoll_Playwright_BDD_MSTEST_Framework.PageObjects;
using static Microsoft.Playwright.Assertions;

namespace ReqnRoll_Playwright_BDD_MSTEST_Framework.PageObjects
{
    public class ForgotPasswordPage : BasePage
    {
        public ForgotPasswordPage(IPage page) : base(page)
        {
        }

        //locators
        ILocator loc_h1pageHeader => _page.Locator("xpath=//h1[text()=\"Forgot Password\"]");
        ILocator loc_txtEmail => _page.Locator("xpath=//input[@id=\"txtEmail\"]");

        ILocator loc_btnSubmit => _page.Locator("xpath=//input[@id=\"btnSubmit\"]");
        ILocator loc_lblMessage => _page.Locator("#lblMessage");

        //Web Action methods
        public async Task goToForgotPasswordPage(string baseUrl, string scenarioTitle)
        {
            await goToPage(baseUrl + "/ForgotPassword", scenarioTitle);
        }

        public async Task enterEmail(String email, string scenarioTitle)
        {
            await populateInputField(loc_txtEmail, email, scenarioTitle);
        }

        public async Task clickSubmitButton(string scenarioTitle)
        {
            await clickElement(loc_btnSubmit, scenarioTitle);
        }

        //assertion methods
        public async Task isUserOnForgotPasswordPage(string expectedHeaderText, string expectedUrl, string expectedBtnText)
        {
            try
            {
                await Expect(loc_h1pageHeader).ToHaveTextAsync(expectedHeaderText);
                await Expect(_page).ToHaveURLAsync(new System.Text.RegularExpressions.Regex(expectedUrl));
                await Expect(loc_btnSubmit).ToHaveValueAsync(expectedBtnText);
            }
            catch (Exception ex) 
            {
                Log.Information($"Exception hit when verifying user is on forgot password page...");
                _errorTranslator.Translate(ex);
            }
        }

        public async Task verifyErrorMessage(string expectedMsg)
        {
            try
            {
                await Expect(loc_lblMessage).ToHaveTextAsync(new System.Text.RegularExpressions.Regex(expectedMsg, System.Text.RegularExpressions.RegexOptions.IgnoreCase));
            }
            catch (Exception ex)
            {
                Log.Information($"Exception hit when verifying error message on forgot password page...");
                _errorTranslator.Translate(ex);
            }
        }

        public async Task verifyValidationError(string expectedMsg)
        {
            try
            {
                // Check HTML5 validation message
                string validationMsg = await loc_txtEmail.EvaluateAsync<string>("e => e.validationMessage");
                if (!string.IsNullOrEmpty(validationMsg) && (validationMsg.Contains(expectedMsg, StringComparison.OrdinalIgnoreCase) || 
                    (validationMsg.Contains("fill out this field", StringComparison.OrdinalIgnoreCase) && expectedMsg.Contains("fill out this field", StringComparison.OrdinalIgnoreCase))))
                {
                    Log.Information($"Found expected validation error: {validationMsg}");
                    return;
                }

                // Fallback to lblMessage if it's not a browser bubble
                await verifyErrorMessage(expectedMsg);
            }
            catch (Exception ex)
            {
                Log.Information($"Exception hit when verifying validation error on forgot password page...");
                _errorTranslator.Translate(ex);
            }
        }
    }
}
