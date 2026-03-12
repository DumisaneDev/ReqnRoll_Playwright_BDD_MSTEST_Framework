using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Playwright.MSTest;
using static Microsoft.Playwright.Assertions;

namespace ReqnRoll_Playwright_BDD_MSTEST_Framework.PageObjects
{
    public class RegisterPage : BasePage
    {
        public RegisterPage(IPage page) : base(page)
        {
        }

        //Locators
        ILocator loc_txtEmail => _page.Locator("xpath=//input[@id=\"txtEmail\"]");
        ILocator loc_txtFirstName => _page.Locator("xpath=//input[@id=\"txtFName\"]");
        ILocator loc_txtLastName => _page.Locator("xpath=//input[@id=\"txtLName\"]");
        ILocator loc_txtPhoneNumber => _page.Locator("xpath=//input[@id=\"txtPhone\"]");
        ILocator loc_txtGender => _page.Locator("xpath=//select[@id=\"txtGender\"]");
        ILocator loc_txtPassword => _page.Locator("xpath=//input[@id=\"txtPassword\"]");
        ILocator loc_msgPasswordStrength => _page.Locator("xpath=//div[@id=\"strengthText\"]");
        ILocator loc_h3PageHeader => _page.Locator("xpath=//h3");
        ILocator loc_btnRegister => _page.Locator("xpath=//button[@id=\"btnRegister\"]");
        ILocator loc_lnkLogin => _page.Locator("xpath=//a[@id=\"lnkRegister\"]");

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
        public async Task isUserOnRegisterPage(string expectedHeaderText, string expectedUrl, string expectedBtnText)
        {
            await Expect(loc_h3PageHeader).ToHaveTextAsync(expectedHeaderText);
            await Expect(_page).ToHaveURLAsync(expectedUrl);
            await Expect(loc_btnRegister).ToHaveTextAsync(expectedBtnText);
        }
    }
}
