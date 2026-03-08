@all 
@smoke
@regression
Feature: Login Fuctionality for CoreOps Web Application

As a Lubanzi Employee,
I want to securely log in to the CoreOps Employee Management system,
So that I can access my operational dashboard and perform my job functions efficiently.

As a Lubanzi Admin User,
I want to securely log in to the CoreOps Employee Management system,
So i can aceess my admin-privallaged functions to perform my operationals functions effciently

| Users        |
| Employee     |
| admin        |

Scenario: Successful Login as An Employee User
	Given I am on the login page of the test system,
	When I enter a valid email address,
	And I enter a valid password,
	And I click the login button,
	Then I should see a url change to contain "/Dashboard",
	And see a welcome message of "Welcome To Our Dashboard" on the page.
	And see the tabs for for my role "Dashboard", "Project", "Employee", "Timesheet", "Room Booking", "Permissions" and Logout.

Scenario: Successful Login as An Admin User
	Given I am on the login page of the test system,
	When I enter a admin email address,
	And I enter an admin password,
	And I click the login button,
	Then I should see a url change to contain "/Dashboard",
	And see a welcome message of "Welcome To Our Dashboard" on the page.
	And see the tabs for for my role DashBoard, Project, Employee, Clients, Timesheet, Room Booking, Permissions, Work Allocation, Leave, Report, Logout.


Scenario: Unsuccessful Login Attempts with Invalid Credentials
	Given I am on the login page of the test system,
	When I enter an invalid email address "whrishchenko8@multiply.com",
	And I enter an invalid password "rR5$WX0<58i",
	And I click the login button,
	Then I should remain on the login page indicated by the url remaining the same,
	And I should see a popup message with the message "Incorrect credentials"


Scenario Outline: Unsuccessful Login Attempts as any user
	Given I am on the login page of the test system,
	When I enter an invalid email address "<email>",
	And I enter an invalid password "<password>",
	And I click the login button,
	Then I should remain on the login page indicated by the url remaining the same,
	And I should see an error message or browser alert "<Expected_message>" detailing the reason for the failed login attempt.

Examples:
	| email                      | password    | Expected_message               |
	|                            | rR5$WX0<58i | Please enter a valid email.    |
	|                            |             | Please enter a valid email.    |
	| whrishchenko8@multiply.com |             | Please enter a valid password. |