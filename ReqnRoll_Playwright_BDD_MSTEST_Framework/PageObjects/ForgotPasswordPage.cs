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

        //Web Action methods
        public async Task enterEmail(String email) => await populateInputField(loc_txtEmail, email);

        public async Task clickSubmitButton() => await clickElement(loc_btnSubmit);

        //assertion methods
        public async Task isUserOnForgotPasswordPage(string expectedHeaderText, string expectedUrl, string expectedBtnText)
        {
            await Expect(loc_h1pageHeader).ToHaveTextAsync(expectedHeaderText);
            await Expect(_page).ToHaveURLAsync(expectedUrl);
            await Expect(loc_btnSubmit).ToHaveValueAsync(expectedBtnText);
        }
    }
}
