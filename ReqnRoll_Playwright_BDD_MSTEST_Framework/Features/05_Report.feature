@all
Feature: Report feature for the coreops web application

As An Admin User,
I want to be able to download and view timesheet and allocation reports 
So that I can track employee work and project allocations.

Background:
	Given I am on the login page of the test system,
	When I login to the test site as an admin user.
	Then I should see a url change to contain "dashboard",

@smoke @regression
Scenario Outline: Generate Timesheet Report successfully
	Given I navigate to the "Timesheet Report" page
	When I select an employee "<EmployeeName>" from the dropdown
	And I select a month "<Month>" from the dropdown
	And I click the "Generate Report" button.
	Then a file download should be triggered
	And the report must include work hours summary, project-wise allocation, overtime, and attendance tracking

Examples:
	| EmployeeName | Month    |
	| Bruce Wayne  | November |
	| Potent Serum | March    |

@regression
Scenario: Generate Work Allocation Report successfully
	Given I navigate to the "Work Allocation Report" page
	When I select an employee "Bruce Wayne" from the dropdown
	And I select a valid "Start Date" as "2025-11-11" and "End Date" as "2025-11-28"
	And I click the "Generate Report" button.
	Then a report showing capacity utilization and allocation percentages should be generated
	And I should see "Over-allocation alerts" if the employee exceeds 100% capacity

@regression @cleanup_register_user
Scenario: Navigate back to the dashboard from Timesheet Report
	Given I navigate to the "Timesheet Report" page
	When I click the "Back" button.
	Then I should be redirected to the "Dashboard" page

@regression
Scenario: Navigate back to the dashboard from Allocation Report
	Given I navigate to the "Work Allocation Report" page
	When I click the "Back" button.
	Then I should be redirected to the "Dashboard" page

@regression
Scenario: Validate error message for Timesheet Report when required fields are empty
	Given I navigate to the "Timesheet Report" page
	When I click the "Generate Report" button without selecting an employee or month
	Then I should see an error message "Please enter an employee or month for the desired report"

@regression
Scenario: Validate error message for Allocation Report when required fields are empty
	Given I navigate to the "Work Allocation Report" page
	When I click the "Generate Report" button without selecting an employee or date range
	Then I should see an error message "Please select an employee and start-end dates."
