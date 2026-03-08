
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace ReqnRoll_Playwright_BDD_MSTEST_Framework.PageObjects
{
    public class OTPPage : BasePage
    {
        public OTPPage(IPage page) : base(page)
        {
        }

        //locators 
        ILocator loc_h1PageHeader => _page.Locator("xpath=//h1[text()=\"Enter the OTP sent to your email\"]");
        ILocator loc_txtOtp => _page.Locator("xpath=//input[@id=\"txtOTP\"]");

        ILocator loc_btnVerfifyOTp => _page.Locator("xpath=//button[@id=\"btnVerify\"]");

        ILocator loc_lnkLogin => _page.Locator("xpath=//a[@id=\"lnkRegister\"]");

        ILocator loc_mdlSucessPopup => _page.GetByTestId("successPopup");

        ILocator loc_lblInvalidOTPError => _page.Locator("//span[@id=\"lblMessage\"]");

        //Web Action methods
        public async Task enterOTP(String otp) => await populateInputField(loc_txtOtp, otp);
        public async Task clickVerifyOTP() => await clickElement(loc_btnVerfifyOTp);

        public async Task handleSucessPopup(string expectedPopupText, string btnName) => await handlePopupModel(loc_mdlSucessPopup, expectedPopupText, btnName);

        public async Task clickLoginLink() => await clickElement(loc_lnkLogin);
        public async Task verifyIncorrectOTPEntry(string expectedMsg) =>  await Expect(loc_lblInvalidOTPError).ToHaveTextAsync(expectedMsg);
    }
}
