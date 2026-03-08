using System;
using System.Collections.Generic;
using System.Text;
using ReqnRoll_Playwright_BDD_MSTEST_Framework.PageObjects;
using Reqnroll;
using ReqnRoll_Playwright_BDD_MSTEST_Framework.Utils;
using static Microsoft.Playwright.Assertions;

namespace ReqnRoll_Playwright_BDD_MSTEST_Framework.StepDefinitions
{
    [Binding]
    public class StepDefinitions
    {
        private readonly LoginPage _loginPage;
        private readonly ForgotPasswordPage _forgotPasswordPage;
        private readonly RegisterPage _registerPage;
        private readonly DashboardPage _dashboardPage;

        public StepDefinitions(LoginPage loginPage, 
            ForgotPasswordPage forgotPasswordPage, 
            RegisterPage registerPage,
            DashboardPage dashboardPage) 
        {
            _loginPage = loginPage;
            _forgotPasswordPage = forgotPasswordPage;
            _registerPage = registerPage;
            _dashboardPage = dashboardPage;
        }

        // --- Common Steps ---

        [Given("I am on the login page of the test system,")]
        public async Task GivenIAmOnTheLoginPageOfTheTestSystem()
        {
            var BaseUrl = ConfigReader.getValue("BaseUrl");
             await _loginPage.goToPage(BaseUrl);
             await _loginPage.isUserOnLoginPage("Login", BaseUrl, "Login");
        }

        [Then("I should see a url change to contain {string},")]
        [Then("I should see a url change to contain {string}")]
        public async Task ThenIShouldSeeAUrlChangeToContain(string partialUrl)
        {
            await _dashboardPage.isUserRedirectedToDashboard(partialUrl);
        }

        // --- Login Steps ---

        [When("I enter a valid email address,")]
        public async Task WhenIEnterAValidEmailAddress()
        {
            var validEmail = ConfigReader.getValue("EmpUsername");
            await _loginPage.enterEmail(validEmail);
        }

        [When("I enter a valid password,")]
        public async Task WhenIEnterAValidPassword()
        {
           var validPassword = ConfigReader.getValue("EmpPassword");
            Console.WriteLine($"Valid Password: {validPassword}");
            await _loginPage.enterPassword(validPassword);
        }

        [When("I enter a admin email address,")]
        public async Task WhenIEnterAAdminEmailAddress()
        {
            var adminEmail = ConfigReader.getValue("AdminEmail");
            await _loginPage.enterEmail(adminEmail);
        }

        [When("I enter an admin password,")]
        public async Task WhenIEnterAdminPassword()
        {
            var adminPassword = ConfigReader.getValue("AdminPassword");
            await _loginPage.enterPassword(adminPassword);
        }

        [When("I click the login button,")]
        public async Task WhenIClickTheLoginButton()
        {
            await _loginPage.clickLoginButton();
        }

        [Then("see a welcome message of {string} on the page.")]
        public async Task ThenSeeAWelcomeMessageOfOnThePage(string message)
        {
            await _dashboardPage.isUserOnDashboard(message);
        }

        [Then("see the tabs for for my role {string}, {string}, {string}, {string}, {string}, {string} and Logout.")]
        public async Task ThenSeeTheTabsForForMyRole(string tab1, string tab2, string tab3, string tab4, string tab5, string tab6)
        {
            await _dashboardPage.isEmployeeOnDashboard(tab1, tab2, tab3, tab4,tab5, tab6);
        }

        [Then("see the tabs for for my role DashBoard, Project, Employee, Clients, Timesheet, Room Booking, Permissions, Work Allocation, Leave, Report, Logout.")]
        public async Task ThenSeeTheTabsForForMyRoleAdmin()
        {
            await _dashboardPage.isAdminOnDashboard();
        }

        [When("I enter an invalid email address {string},")]
        public async Task WhenIEnterAnInvalidEmailAddress(string email)
        {
            await _loginPage.enterEmail(email);
        }

        [When("I enter an invalid password {string},")]
        public async Task WhenIEnterAnInvalidPassword(string password)
        {
          await _loginPage.enterPassword(password);
        }

        [Then("I should remain on the login page indicated by the url remaining the same,")]
        public async Task ThenIShouldRemainOnTheLoginPageIndicatedByTheUrlContaining()
        {
            await _loginPage.isUserStillOnLoginPage("Login", ConfigReader.getValue("BaseUrl"));
        }

        [Then("I should see a popup message with the message {string}")]
        public async Task ThenIShouldSeeAPopupMessageWithTheMessage(string p0)
        {
            await _loginPage.HandleInvalidLoginAlert(p0);
        }


        [Then("I should see an error message or browser alert {string} detailing the reason for the failed login attempt.")]
        public async Task ThenIShouldSeeAnErrorMessageOrBrowserAlertDetailingTheReasonForTheFailedLoginAttempt(string expectedMessage)
        {
            await _loginPage.VerifyErrorMessage(expectedMessage);
        }

        // --- Registration Steps ---

        [When("I click on the Register link,")]
        public void WhenIClickOnTheRegisterLink()
        {
            throw new PendingStepException();
        }

        [Given("I am on the registration page of the test system,")]
        public void GivenIAmOnTheRegistrationPageOfTheTestSystem()
        {
            throw new PendingStepException();
        }

        [When("I enter a valid email address {string},")]
        public void WhenIEnterAValidEmailAddressParam(string emailKey)
        {
            throw new PendingStepException();
        }

        [When("I enter a first name {string},")]
        public void WhenIEnterAFirstName(string firstName)
        {
            throw new PendingStepException();
        }

        [When("I enter a last name {string},")]
        public void WhenIEnterALastName(string lastName)
        {
            throw new PendingStepException();
        }

        [When("I enter a phone number {string},")]
        public void WhenIEnterAPhoneNumber(string phoneNumber)
        {
            throw new PendingStepException();
        }

        [When("I select a gender {string},")]
        public void WhenISelectAGender(string gender)
        {
            throw new PendingStepException();
        }

        [When("I enter a valid password {string},")]
        public void WhenIEnterAValidPasswordParam(string password)
        {
            throw new PendingStepException();
        }

        [When("I click the register button,")]
        public void WhenIClickTheRegisterButton()
        {
            throw new PendingStepException();
        }

        [Then("I should see a success message of {string} on the page.")]
        public void ThenIShouldSeeASuccessMessageOfOnThePage(string message)
        {
            throw new PendingStepException();
        }

        [When("I enter an email address {string},")]
        public void WhenIEnterAnEmailAddress(string email)
        {
            throw new PendingStepException();
        }

        [Then("I should remain on the registration page indicated by the url containing {string},")]
        public void ThenIShouldRemainOnTheRegistrationPageIndicatedByTheUrlContaining(string partialUrl)
        {
            throw new PendingStepException();
        }

        [Then("I should see an error message {string} detailing the reason for the failed registration.")]
        public void ThenIShouldSeeAnErrorMessageDetailingTheReasonForTheFailedRegistration(string expectedMessage)
        {
            throw new PendingStepException();
        }

        [When("I enter a weak password {string},")]
        public void WhenIEnterAWeakPassword(string password)
        {
            throw new PendingStepException();
        }

        [Then("I should see a password strength message of {string} on the page,")]
        [Then("I should see a password strength message of {string} on the page.")]
        public void ThenIShouldSeeAPasswordStrengthMessageOfOnThePage(string strength)
        {
            throw new PendingStepException();
        }

        [When("I enter a strong password {string},")]
        public void WhenIEnterAStrongPassword(string password)
        {
            throw new PendingStepException();
        }

        [When("I click the login link on the registration page,")]
        public void WhenIClickTheLoginLinkOnTheRegistrationPage()
        {
            throw new PendingStepException();
        }

        [Then("I should be redirected to the login page indicated by the url containing {string},")]
        public void ThenIShouldBeRedirectedToTheLoginPageIndicatedByTheUrlContaining(string partialUrl)
        {
            throw new PendingStepException();
        }

        [Then("I should see the login page header {string}.")]
        public void ThenIShouldSeeTheLoginPageHeader(string header)
        {
            throw new PendingStepException();
        }

        [When("I enter my first name,")]
        public void WhenIEnterMyFirstName() => throw new PendingStepException();

        [When("I enter my last name,")]
        public void WhenIEnterMyLastName() => throw new PendingStepException();

        [When("I enter my phone number,")]
        public void WhenIEnterMyPhoneNumber() => throw new PendingStepException();

        [When("I enter my gender,")]
        public void WhenIEnterMyGender() => throw new PendingStepException();

        [When("I enter a password {string},")]
        public void WhenIEnterAPasswordParam(string password) => throw new PendingStepException();

        [Then("I will be redirected to the otp page seeing the Verify OTP button")]
        public void ThenIWillBeRedirectedToTheOtpPageSeeingTheVerifyOTPButton() => throw new PendingStepException();

        [Then("I should recieve an email with a unique otp code.")]
        public void ThenIShouldRecieveAnEmailWithAUniqueOtpCode() => throw new PendingStepException();

        [When("on the otp page, i enter my otp code in the OTP code field")]
        public void WhenOnTheOtpPageIEnterMyOtpCodeInTheOTPCodeField() => throw new PendingStepException();

        [When("click the verify OTP button")]
        public void WhenClickTheVerifyOTPButton() => throw new PendingStepException();


        [When("I click the {string} button on the popup message")]
        [Then("when I click the {string} button on the popup message")]
        public void WhenIClickTheButtonOnThePopupMessage(string btnName) => throw new PendingStepException();

        [Then("I should be redirected to the login page of the test system.")]
        public void ThenIShouldBeRedirectedToTheLoginPageOfTheTestSystem() => throw new PendingStepException();

        [Then("be able to login with the credentials I used during registration ,seeing the dashboard message of {string} after i login successfully.")]
        public void ThenBeAbleToLoginWithTheCredentialsIUsedDuringRegistrationSeeingTheDashboardMessageOfAfterILoginSuccessfully(string message) => throw new PendingStepException();

        [When("I enter a pre-existant email address,")]
        public void WhenIEnterAPre_ExistantEmailAddress() => throw new PendingStepException();

        [Then("an error message should be displayed on the page with the message {string}.")]
        public void ThenAnErrorMessageShouldBeDisplayedOnThePageWithTheMessage(string message) => throw new PendingStepException();

        // --- Forgot Password Steps ---

        [Given("I am on the Forgot Password page")]
        public void GivenIAmOnTheForgotPasswordPage()
        {
            throw new PendingStepException();
        }

        [When("I enter a registered email {string}")]
        public void WhenIEnterARegisteredEmail(string email)
        {
            throw new PendingStepException();
        }

        [When("I click the submit button")]
        public void WhenIClickTheSubmitButton()
        {
            throw new PendingStepException();
        }

        [Then("I should receive a password reset link in my email {string}")]
        public void ThenIShouldReceiveAPasswordResetLinkInMyEmail(string email)
        {
            throw new PendingStepException();
        }

        [Then("I should be able to navigate to the reset password page via the link")]
        public void ThenIShouldBeAbleToNavigateToTheResetPasswordPageViaTheLink()
        {
            throw new PendingStepException();
        }

        [When("I enter an unregistered email {string}")]
        public void WhenIEnterAnUnregisteredEmail(string email)
        {
            throw new PendingStepException();
        }

        [Then("I should see an error message {string}")]
        public void ThenIShouldSeeAnErrorMessage(string expectedMessage)
        {
            throw new PendingStepException();
        }

        [When("I enter an invalid email {string}")]
        public void WhenIEnterAnInvalidEmail(string email)
        {
            throw new PendingStepException();
        }

        [Then("I should see a validation error {string}")]
        public void ThenIShouldSeeAValidationError(string expectedMessage)
        {
            throw new PendingStepException();
        }
    }
}
