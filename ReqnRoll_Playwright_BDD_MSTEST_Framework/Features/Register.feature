Feature: Registration Functionality for CoreOps Web Application
As a prospective Lubanzi Employee,
I want to create a new account in the CoreOps Employee Management system,
So that I can access the platform and perform my operational functions.

Background: 
 Given I am on the login page of the test system,
 When I click on the Register link,

Scenario: Register successfully as a new user 
Given I am on the registration page of the test system,
When I enter a valid email address,
And I enter my first name,
And I enter my last name,
And I enter my phone number,
And I enter my gender,
And I enter a valid password,
And I click the register button, 
Then I will be redirected to the otp page seeing the Verify OTP button
And I should recieve an email with a unique otp code. 
When on the otp page, i enter my otp code in the OTP code field
And click the verify OTP button
Then i should see a popup message with the message "Your account has been created successfully. You can now login with your credentials."
And when I click the "OK" button on the popup message
And I should be redirected to the login page of the test system,
And be able to login with the credentials I used during registration ,seeing the dashboard message of "Welcome To Our Dashboard" after i login successfully.

Scenario: Unsuccessful registration attempt with an already registered email address
Given I am on the registration page of the test system
When I enter a pre-existant email address,
And I enter my first name,
And I enter my last name,
And I enter my phone number,
And I enter my gender,
And I enter a valid password,
And I click the register button, 
Then I should remain on the registration page indicated by the url containing "/Register",
And an error message should be displayed on the page with the message "Email is already registered. Please use a different email.".

Scenario Outline: Unsuccessful Registration Attempts with Invalid Data
	Given I am on the registration page of the test system,
	When I enter an email address "<email>",
	And I enter a first name "<firstName>",
	And I enter a last name "<lastName>",
	And I enter a phone number "<phoneNumber>",
	And I select a gender "<gender>",
	And I enter a password "<password>",
	And I click the register button,
	Then I should remain on the registration page indicated by the url containing "/Register",
	And I should see an error message "<Expected_message>" detailing the reason for the failed registration.

Examples:
	| email                      | firstName | lastName | phoneNumber | gender | password   | Expected_message            |
	| invalid-email              | Bruce     | Wayne    | 07300841554 | Male   | wE7?wG8Qh  | Invalid email format        |
	| bruce.wayne@mailsac.com    |           | Wayne    | 07300841554 | Male   | wE7?wG8Qh  | First name is required      |
	| bruce.wayne@mailsac.com    | Bruce     |          | 07300841554 | Male   | wE7?wG8Qh  | Last name is required       |
	| bruce.wayne@mailsac.com    | Bruce     | Wayne    |             | Male   | wE7?wG8Qh  | Phone number is required    |
	| bruce.wayne@mailsac.com    | Bruce     | Wayne    | 07300841554 |        | wE7?wG8Qh  | Gender selection is required|
	| bruce.wayne@mailsac.com    | Bruce     | Wayne    | 07300841554 | Male   |            | Password is required        |

Scenario: Verify Password Strength Indicator
	Given I am on the registration page of the test system,
	When I enter a weak password "123",
	Then I should see a password strength message of "Weak" on the page,
	When I enter a strong password "Abc@123456789!",
	Then I should see a password strength message of "Strong" on the page.

Scenario: Navigate back to Login Page from Registration Page
	Given I am on the registration page of the test system,
	When I click the login link on the registration page,
	Then I should be redirected to the login page indicated by the url containing "/Login",
	And I should see the login page header "Login to your account".