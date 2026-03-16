@regression
Feature: Permissions feature for the coreops web application

As an admin user, 
I want to be able to manage permissions for users in the coreops web application,
So that i can easily assign and revoke permission to users based on their roles and responsibilities.

Background:
	Given I am on the login page of the test system,
	When i login to the testsite as an admin user,
	Then I should see a url change to contain "/Dashboard",
	And see a welcome message of "Welcome To Our Dashboard" on the page.
	And see the tabs for for my role DashBoard, Project, Employee, Clients, Timesheet, Room Booking, Permissions, Work Allocation, Leave, Report, Logout.

Scenario: Admin can search for an employee in the employee navigation table
	Given I click on the "Permissions" tab
	When I search for an employee named "Bruce Wayne" in the employee table
	Then the employee "Bruce Wayne" should be visible in the search results

Scenario: Admin can sort the employee table by name
	Given I click on the "Permissions" tab
	When I sort the employee table by "Name" in "ascending" order
	Then the employee table should be sorted by "Name" correctly

Scenario Outline: Admin can revoke user access to a specific system part
	Given I click on the "Permissions" tab
	And I click the "Edit" button for employee "<employee>" with email "<email>"
	When I revoke access to the "<permission>" tab for the user
	And I save the permission changes
	Then the "<permission>" tab should not be visible when user with email "<email>" logs in
Examples:
	| employee    | email                | permission   |
	| Bruce Wayne | employee@mailsac.com | Project      |
	| Bruce Wayne | employee@mailsac.com | Timesheet    |
	| Bruce Wayne | employee@mailsac.com | Room Booking |


Scenario Outline: Admin can grant user access to a specific system part
	Given I click on the Permissions tab
	And I click the "Edit" button for employee "<employee>" with email "<email>"
	When I grant access to the "<permission>" tab for the user
	And I save the permission changes
	Then the "<permission>" tab should be visible when user with email "<email>" logs in
	Examples: 
	| employee    | email                | permission   |
	| Bruce Wayne | employee@mailsac.com | Project      |
	| Bruce Wayne | employee@mailsac.com | Timesheet    |
	| Bruce Wayne | employee@mailsac.com | Room Booking |
