# 🎭 Playwright BDD Test Suite

A simple, human-readable automated testing suite for our web applications. This project uses "Gherkin" (Given/When/Then) so that anyone can understand what is being tested.

## 📋 Prerequisites
Before you begin, ensure you have the following installed:
* **.NET SDK 10.0** (The engine that runs the tests).
* **Git** (To download and update the test project).

---

## 🚀 3-Step Guide to Running Tests Locally

1. **Prepare the Environment**  
   Right-click the file `dependencies-install.ps1` and select **Run with PowerShell**. This installs the necessary "browsers" (Chrome/Firefox) and tools needed for the tests to work.

2. **Run the Tests**  
   Open a terminal and run the test script. You can choose which tests to run:
   * **Smoke Tests (Fast):** `.\run-tests.ps1 -Category "smoke"`
   * **All Tests:** `.\run-tests.ps1 -Category "all"`
   * **Parallel (Speed up):** `.\run-tests.ps1 -Parallel -Workers 4`

3. **Check the Results**  
   Once finished, a summary will appear in your terminal. For a detailed view, open the `TestResults` folder created in the project root.

---

## 📂 Where are the Tests?
If you want to read or add new test scenarios, look in:
* `ReqnRoll_Playwright_BDD_MSTEST_Framework/Features/`
* Inside, you will find `.feature` files (like `Login.feature`) written in plain English.

---

## 📊 How to Read Visual Reports

After a test run, navigate to the `TestResults` folder:

* **HTML Living Doc:** Open `LivingDoc.html`. This is your main dashboard showing green (passed) and red (failed) results.
* **Screenshots:** Found in `TestResults/Screenshots`. These show exactly what the app looked like at the moment of failure.
* **Traces:** Found in `TestResults/Traces`. You can get a full snapshot (play-by-play) of the test to see exactly how it behaved on test execution failure.
* **Workers HTML Report:** Found when parallel execution is enabled, It shows the workers used and how the work(Test Scenarios) are distributed between them.

---

## 🛠️ About the Test Script (`run-tests.ps1`)

The `run-tests.ps1` script is the "Remote Control" for this project. It handles all the technical heavy lifting:
1. **Cleans Up:** Automatically deletes old reports so you only see the latest results.
2. **Organizes:** Creates a clean folder structure for Screenshots, Videos, and Traces.
3. **Executes:** Commands the browsers to follow the steps defined in our Feature files.
4. **Reports:** Generates the technical data used by Azure DevOps to display results.

---

## 💡 Best Practices
* **Don't touch `testdata.json`**: This file contains the "brain" of the test data. Changing it manually might cause tests to fail.
* **Pull before you Run**: Always run `git pull` to make sure you have the latest test steps from the team.
