Feature: Forgot Password
  As a user who has forgotten their password
  I want to be able to reset my password
  So that I can regain access to my account

  Background:
    Given I am on the Forgot Password page

  Scenario: Successfully requesting a password reset link
    When I enter a registered email "employee@mailsac.com"
    And I click the submit button
    Then I should receive a password reset link in my email "employee@mailsac.com"
    And I should be able to navigate to the reset password page via the link

  Scenario: Requesting a password reset link with an unregistered email
    When I enter an unregistered email "unregistered@example.com"
    And I click the submit button
    Then I should see an error message "User with this email does not exist."

  Scenario: Requesting a password reset link with an invalid email format
    When I enter an invalid email "invalid-email"
    And I click the submit button
    Then I should see a validation error "Please enter a valid email address"

  Scenario: Requesting a password reset link with an empty email field
    When I click the submit button
    Then I should see a validation error "Please fill out this field"
