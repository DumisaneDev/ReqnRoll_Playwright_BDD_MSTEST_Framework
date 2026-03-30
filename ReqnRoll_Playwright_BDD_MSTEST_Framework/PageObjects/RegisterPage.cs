using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Playwright.MSTest;
using static Microsoft.Playwright.Assertions;
using ReqnRoll_Playwright_BDD_MSTEST_Framework.Utils;
using ReqnRoll_Playwright_BDD_MSTEST_Framework.StepDefinitions;

namespace ReqnRoll_Playwright_BDD_MSTEST_Framework.PageObjects
{
    public class RegisterPage : BasePage
    {
        private readonly DialogState _dialogState;
        public RegisterPage(IPage page, DialogState dialogState) : base(page)
        {
            _dialogState = dialogState;
        }

        //Locators
        ILocator loc_txtEmail => _page.Locator("xpath=//input[@id=\"txtEmail\"]");
        ILocator loc_txtFirstName => _page.Locator("xpath=//input[@id=\"txtFname\"]");
        ILocator loc_txtLastName => _page.Locator("xpath=//input[@id=\"txtLname\"]");
        ILocator loc_txtPhoneNumber => _page.Locator("xpath=//input[@id=\"txtPhone\"]");
        ILocator loc_txtGender => _page.Locator("xpath=//select[@id=\"txtGender\"]");
        ILocator loc_txtPassword => _page.Locator("xpath=//input[@id=\"txtPassword\"]");
        ILocator loc_msgPasswordStrength => _page.Locator("xpath=//div[@id=\"strengthText\"]");
        ILocator loc_h3PageHeader => _page.Locator("xpath=//h3");
        ILocator loc_btnRegister => _page.Locator("xpath=//input[@id=\"btnRegister\"]");
        ILocator loc_lnkLogin => _page.Locator("xpath=//a[@id=\"lnkRegister\"]");
        ILocator loc_msgRegisteredUserError => _page.Locator("xpath=//span[@id=\"lblMessage\"]");

        //Web Action methods
        public async Task enterEmail(String email, string scenarioTitle)
        {
            await populateInputField(loc_txtEmail, email, scenarioTitle);
        }

        public async Task enterFirstName(String firstName, string scenerioTitle)
        {
            await populateInputField(loc_txtFirstName, firstName, scenerioTitle);
        }

        public async Task enterLastName(String lastName, string scenerioTitle)
        {
            await populateInputField(loc_txtLastName, lastName, scenerioTitle);
        }

        public async Task enterPhoneNumber(String phoneNumber, string scenerioTitle)
        {
            await populateInputField(loc_txtPhoneNumber, phoneNumber, scenerioTitle);
        }

        public async Task selectGender(String gender, string scenarioTitle)
        {
            await selectDropdownValue(loc_txtGender, gender, scenarioTitle);
        }

        public async Task enterPassword(String password, string scenarioTitle)
        {
            await populateInputField(loc_txtPassword, password, scenarioTitle);
        }
        public async Task clickRegisterButton(string scenarioTitle)
        {
            await clickElement(loc_btnRegister, scenarioTitle);
        }
        public async Task navigatetoLoginPage(string scenarioTitle)
        {
            await clickElement(loc_lnkLogin, scenarioTitle);
        }

        public async Task<string> getPasswordStrengthMessage()
        {
            return await getElementContent(loc_msgPasswordStrength);
        }

        //assertion methods
        ILocator loc_mdlSuccess => _page.Locator("#successModal");
        ILocator loc_btnSuccessOk => _page.Locator("#successModal button", new() { HasText = "OK" });

        public async Task verifySuccessModal(string expectedMessage)
        {
            try
            {
                await Expect(loc_mdlSuccess).ToBeVisibleAsync();
                await Expect(loc_mdlSuccess).ToContainTextAsync(expectedMessage);
            }
            catch (Exception ex) 
            {
                Log.Error($"Failure verifying success model on the register page...{ex.Message}");
                _errorTranslator.Translate(ex);
            }
        }

        public async Task clickSuccessOk(string scenarioTitle)
        {
            await clickElement(loc_btnSuccessOk, scenarioTitle);
        }

        public async Task disableHtml5Validation()
        {
            
                await _page.EvaluateAsync("document.querySelectorAll('form').forEach(f => f.setAttribute('novalidate', 'novalidate'))");
        }

        public async Task isUserOnRegisterPage(string expectedHeaderText, string expectedUrl, string expectedBtnText)
        {
            try
            {
                await Expect(loc_h3PageHeader).ToHaveTextAsync(new System.Text.RegularExpressions.Regex(expectedHeaderText, System.Text.RegularExpressions.RegexOptions.IgnoreCase));
                await Expect(_page).ToHaveURLAsync(new System.Text.RegularExpressions.Regex(expectedUrl, System.Text.RegularExpressions.RegexOptions.IgnoreCase));
                await Expect(loc_btnRegister).ToHaveValueAsync(expectedBtnText);
            }
            catch (Exception ex) 
            {
                Log.Error($"Failure when verify User entry into the register page...{ex.Message}");
                _errorTranslator.Translate(ex);
            }
        }
        public async Task HandleInvalidAlert(string expectedMessage)
        {
            // Give some time for the alert to be captured if it hasn't been already
            for (int i = 0; i < 5; i++)
            {
                if (!string.IsNullOrEmpty(_dialogState.LastMessage))
                    break;
                await Task.Delay(500);
            }

            string alertMessage = _dialogState.LastMessage;
            Assert.AreEqual(expectedMessage, alertMessage, "The alert message did not match the expected message.");
        }

        public async Task isRegisterUnsucessful(string expectedMsg) 
        {
            Log.Information($"Waiting for error message: {expectedMsg}");
            
            // Wait loop to allow the application to process and display errors
            for (int i = 0; i < 10; i++) // 5 seconds total (10 * 500ms)
            {
                // 1. Check lblMessage
                if (await loc_msgRegisteredUserError.IsVisibleAsync())
                {
                    string actualMsg = await loc_msgRegisteredUserError.InnerTextAsync();
                    if (!string.IsNullOrEmpty(actualMsg) && actualMsg.Contains(expectedMsg, StringComparison.OrdinalIgnoreCase))
                    {
                        Log.Information($"Found expected error in lblMessage: {actualMsg}");
                        return;
                    }
                }

                // 2. Check HTML5 validation messages
                var inputs = new Dictionary<string, ILocator> {
                    { "Email", loc_txtEmail },
                    { "FirstName", loc_txtFirstName },
                    { "LastName", loc_txtLastName },
                    { "PhoneNumber", loc_txtPhoneNumber },
                    { "Gender", loc_txtGender },
                    { "Password", loc_txtPassword }
                };

                foreach (var inputEntry in inputs)
                {
                    string validationMsg = await inputEntry.Value.EvaluateAsync<string>("e => e.validationMessage");
                    if (!string.IsNullOrEmpty(validationMsg))
                    {
                        if (validationMsg.Contains(expectedMsg, StringComparison.OrdinalIgnoreCase) || 
                           (validationMsg.Contains("fill out this field", StringComparison.OrdinalIgnoreCase) && expectedMsg.Contains("required", StringComparison.OrdinalIgnoreCase)))
                        {
                            Log.Information($"Found expected error in HTML5 validation of {inputEntry.Key}: {validationMsg}");
                            return;
                        }
                    }
                }

                // 3. Check Dialogs
                if (!string.IsNullOrEmpty(_dialogState.LastMessage) && _dialogState.LastMessage.Contains(expectedMsg, StringComparison.OrdinalIgnoreCase))
                {
                    Log.Information($"Found expected error in Dialog: {_dialogState.LastMessage}");
                    return;
                }

                await Task.Delay(500);
            }

            // Final fallback: try a standard assertion which will trigger the ExceptionTranslator if it fails
            await Expect(loc_msgRegisteredUserError).ToHaveTextAsync(new System.Text.RegularExpressions.Regex(expectedMsg, System.Text.RegularExpressions.RegexOptions.IgnoreCase), new() { Timeout = 1000 });
        }

        public async Task validatePasswordStrength(string expectedPasswordStrength, ScenarioContext scenarioContext) 
        {
            try
            {
                await Expect(loc_msgPasswordStrength).ToBeVisibleAsync();
                await Expect(loc_msgPasswordStrength).ToHaveTextAsync(expectedPasswordStrength);
            }
            catch (Exception ex) 
            {
                Log.Information($"Issue when validating password strength...{ex.Message}");
                await _screenshotManager.captureScreenshot(scenarioContext.ScenarioInfo.Title, Hooks.ScreenshotsPath, "Verifying_Password_strength_indicator");
                _errorTranslator.Translate(ex);
            }
        }

    }
}
