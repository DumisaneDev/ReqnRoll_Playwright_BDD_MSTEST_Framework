@regression
Feature: Registration Functionality for CoreOps Web Application
As a prospective Lubanzi Employee,
I want to create a new account in the CoreOps Employee Management system,
So that I can access the platform and perform my operational functions.

Background: 
 Given I am on the login page of the test system,
 When I click on the Register link,

 @cleanup_register_user
Scenario: Register successfully as a new user  
Given I am on the registration page of the test system,
When I enter a valid email address in the register page,
And I enter my first name,
And I enter my last name,
And I enter my phone number,
And I enter my gender,
And I enter a valid password on the register page,
And I click the register button, 
Then I will be redirected to the otp page seeing the Verify OTP button
When on the otp page, i enter my otp code in the OTP code field
And click the verify OTP button
Then i should see a popup message with the message "Your account has been created successfully. You can now login with your credentials."
And I click the "OK" button on the popup message
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