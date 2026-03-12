using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Playwright.MSTest;
using static Microsoft.Playwright.Assertions;

namespace ReqnRoll_Playwright_BDD_MSTEST_Framework.PageObjects
{
    public class LoginPage : BasePage
    {
        private readonly Utils.DialogState _dialogState;
        public LoginPage(IPage page, Utils.DialogState dialogState) : base(page)
        {
            _dialogState = dialogState;
        }

        //Locators
        private ILocator loc_txtEmail => _page.Locator("#txtEmail");
        private ILocator loc_txtPassword => _page.Locator("#txtPassword");
        private ILocator loc_btnLogin => _page.GetByRole(AriaRole.Button, new() { Name = "Login" });
        private ILocator loc_lnkForgotPass => _page.Locator("#lnkForgotPass");
        private ILocator loc_lnkRegister => _page.Locator("#lnkRegister");
        private ILocator loc_h3PageHeader => _page.Locator("h3");
        private ILocator loc_lblError => _page.Locator(".invalid-feedback").First;

        //Web Action methods
        public async Task navigateTo(string testURL, string scenarioTitle) => await goToPage(testURL, scenarioTitle);
       
        public async Task navigatetoRegisterPage(string scenarioTitle) => await clickElement(loc_lnkRegister, scenarioTitle);
      
        public async Task navigatetoForgotPasswordPage(string scenarioTitle) => await clickElement(loc_lnkForgotPass, scenarioTitle);

        public async Task enterEmail(String email, string scenarioTitle) => await populateInputField(loc_txtEmail, email, scenarioTitle);

        public async Task enterPassword(String password, string scenarioTitle) => await populateInputField(loc_txtPassword, password, scenarioTitle);

        public async Task clickLoginButton(string scenarioTitle) => await clickElement(loc_btnLogin, scenarioTitle);
        
        public async Task isUserStillOnLoginPage(string expectedHeaderText, string expectedUrl) 
        {
            try
            {
                await Expect(loc_h3PageHeader).ToHaveTextAsync(expectedHeaderText);
                await Expect(_page).ToHaveURLAsync(new System.Text.RegularExpressions.Regex(expectedUrl));
            }
            catch (Exception ex) 
            {
                Log.Information($"Exception hit when verifying user is still on login page...");
                _errorTranslator.Translate(ex);
            }
        }

        public async Task VerifyErrorMessage(string expectedErrorMessage)
        {
            try
            {
                await Expect(loc_lblError).ToContainTextAsync(new System.Text.RegularExpressions.Regex(expectedErrorMessage.Trim()));
            }
            catch (Exception ex) 
            { 
             Log.Information($"Exception hit when verifying error message on login page...");
             _errorTranslator.Translate(ex);
            }
        }

        public async Task loginProcedure(String email, String password, string scenarioTitle)
        {
                await enterEmail(email, scenarioTitle);
                await enterPassword(password, scenarioTitle);
                await clickLoginButton(scenarioTitle);
        }

        //assertion methods
        public async Task HandleInvalidLoginAlert(string expectedMessage)
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

        public async Task isUserOnLoginPage(string expectedHeaderText, string expectedUrl, string expectedBtnText)
        {
            try
            {
                await Expect(loc_h3PageHeader).ToHaveTextAsync(expectedHeaderText);
                await Expect(_page).ToHaveURLAsync(new System.Text.RegularExpressions.Regex(expectedUrl));
                await Expect(loc_btnLogin).ToHaveValueAsync(expectedBtnText);
            }
            catch (Exception ex) 
            {
               Log.Information($"Exception hit when verifying user is on login page...");
               _errorTranslator.Translate(ex);
            }
        }
    }
}
