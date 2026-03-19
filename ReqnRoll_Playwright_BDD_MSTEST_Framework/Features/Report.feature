Feature: Report feature for the coreops web application

As An Admin User,
I want to be able to download and view timesheet and allocation reports 
So that I can track employee work and project allocations.

  Background:
    Given I am on the login page of the test system,
    When I login to the test site as an admin user.
    Then I should see a url change to contain "dashboard",
 
  Scenario: Admin can generate a Timesheet Report for an employee
    Given I click on the "Report" tab
    And I navigate to the "Timesheet Report" page
    When I select the employee "Bruce Wayne" from the dropdown
    And I select the month "November" from the dropdown
    And I click the "Generate report" button
    Then a timesheet report for "Bruce Wayne" should be downloaded
    And the report should contain a graph showing work performance


  Scenario: Admin can generate an Allocation Report for an employee
    Given I click on the "Report" tab
    And I navigate to the "Allocation Report" page
    When I select the employee "Potent Serum" from the dropdown
    And I select a start date "2025-11-11" and end date "2025-11-28"
    And I click the "Generate report" button
    Then an allocation report for "Potent Serum" should be downloaded


  Scenario: Admin can navigate back to dashboard from Report page
    Given I click on the "Report" tab
    And I navigate to the "Timesheet Report" page
    When I click the "Back" button
    Then I should be redirected to the "dashboard" page

   
   Scenario: Admin can see an error message when generating a Timesheet report with missing required fields
    Given I click on the "Report" tab
    And I navigate to the "Timesheet Report" page
    When I click the "Generate report" button without selecting an employee or month
	Then I should see an error message "Please enter an employee or month for the desired report"

  Scenario: Admin can see an error message when generating a Allocation report with missing required fields
    Given I click on the "Report" tab
    And I navigate to the "Timesheet Report" page
    When I click the "Generate report" button without selecting an employee or month
	Then I should see an error message "Please select an employee and start-end dates."


