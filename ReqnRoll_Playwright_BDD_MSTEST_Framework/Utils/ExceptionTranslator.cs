using System;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ReqnRoll_Playwright_BDD_MSTEST_Framework.Utils
{
    public class StakeholderFriendlyException : AssertFailedException
    {
        public override string StackTrace => ""; 
        public StakeholderFriendlyException(string message) : base(message) { }
    }

    public class ExceptionTranslator
    {
        // Thread-safe way to store the current step text during execution
        public static AsyncLocal<string> CurrentStep = new AsyncLocal<string>();
        public static AsyncLocal<string> WorkerId = new AsyncLocal<string>();

        public void Translate(Exception ex, string stepDetail = null)
        {
            string currentStep = stepDetail ?? CurrentStep.Value ?? "Unknown Step";
            string workerId = WorkerId.Value ?? "Unknown Worker";
            Log.Error($"[TECHNICAL ERROR]: {ex.GetType().Name}\nAction: {currentStep}\nWorker: {workerId}\nMessage: {ex.Message}");

            string friendlyStatus = "[Test Failure]";
            string insight = "The application did not behave as expected. This could be due to a bug or a change in requirements.";
            string expectedValue = "N/A";
            string actualValue = "N/A";
            string category = "Application Defect"; // Default to Application Defect

            string msg = ex.Message;
            string stepLower = currentStep.ToLower();

            // 0. Determine Category (Application vs Script Defect)
            // Technical errors are always Script Defects
            bool isTechnicalError = 
                ex is ArgumentException || 
                ex is ArgumentNullException || 
                ex is NullReferenceException ||
                ex is InvalidOperationException ||
                ex.GetType().Name.Contains("TargetClosedException") ||
                msg.Contains("Target closed") ||
                msg.Contains("context closed") ||
                msg.Contains("is not a valid selector") ||
                msg.Contains("Syntax error") ||
                msg.Contains("Unexpected token");

            // Assertion steps/messages imply Application Defect
            bool isAssertionStep = 
                stepLower.Contains("should") || 
                stepLower.Contains("verify") || 
                stepLower.Contains("expect") || 
                stepLower.Contains("see") ||
                stepLower.Contains("check") ||
                stepLower.Contains("validate") ||
                stepLower.Contains("must");

            bool isAssertionMessage = 
                msg.Contains("expected to") || 
                msg.Contains("ToHave") || 
                msg.Contains("ToBe") || 
                msg.Contains("ToContain") ||
                msg.Contains("ToHaveValue") ||
                ex is AssertFailedException;

            if (isTechnicalError)
            {
                category = "Script Defect";
                insight = "The test script encountered a technical error (e.g., null reference, invalid argument). This requires automation team attention.";
            }
            else if (msg.Contains("waiting for Locator") || msg.Contains("element(s) not found") || msg.Contains("Timeout") || ex is TimeoutException)
            {
                // If it's a timeout/not found error:
                // It's an Application Defect if the step is an assertion OR the message is an assertion.
                // Otherwise (e.g. failing to click a button during setup/action), it's a Script Defect.
                if (isAssertionStep || isAssertionMessage)
                {
                    category = "Application Defect";
                }
                else
                {
                    category = "Script Defect";
                    insight = "The test script could not find an element to interact with during an action step. This usually indicates a broken selector or a changed UI element.";
                }
            }

            // 1. Element Not Found / Timeout specific handling
            if (msg.Contains("element(s) not found") || msg.Contains("waiting for Locator"))
            {
                actualValue = "Element not found or hidden";
            }

            // 2. Expected Value Extraction
            // Look for patterns like expected: <Value>, expected "Value", expected 'Value', etc.
            var expectedMatch = Regex.Match(msg, @"expected.*?(?:""|'|/|<)(.*?)(?:""|'|/|>)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (expectedMatch.Success && expectedMatch.Groups.Count > 1)
            {
                expectedValue = expectedMatch.Groups[1].Value.Trim();
                expectedValue = Regex.Replace(expectedValue, @"^(?:to have|to contain|to be|text matching regex|string|value)\s+", "", RegexOptions.IgnoreCase).Trim();
            }
            else 
            {
                // Fallback for simpler formats or MSTest Assert.AreEqual
                var expectedFallback = Regex.Match(msg, @"expected\s*(?:string|value)?\s*[:\s-]*\s*(.*?)(\r|\n|but was|received|$)", RegexOptions.IgnoreCase);
                if (expectedFallback.Success) 
                {
                    expectedValue = expectedFallback.Groups[1].Value.Trim(' ', '"', '\'', ':', '<', '>');
                    expectedValue = Regex.Replace(expectedValue, @"^(?:to have|to contain|to be|text matching regex)\s+", "", RegexOptions.IgnoreCase).Trim();
                }
            }

            // 3. Actual Value Extraction (Enhanced)
            var actualMatch = Regex.Match(msg, @"(?:but was|received|actual)\s*[:\s-]*\s*(?:""|'|/|<)(.*?)(?:""|'|/|>)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (actualMatch.Success && actualMatch.Groups.Count > 1)
            {
                actualValue = actualMatch.Groups[1].Value.Trim();
                if (string.IsNullOrEmpty(actualValue) || actualValue == "null") actualValue = "empty/null";
            }
            else if (msg.Contains("But was: 'null'") || msg.Contains("Actual: <null>"))
            {
                actualValue = "empty/null";
            }

            // 4. Fallback to Step Text for Expected Value
            // If we still don't have a good expected value, try to extract it from the Gherkin step text
            if ((expectedValue == "N/A" || string.IsNullOrEmpty(expectedValue)) && !string.IsNullOrEmpty(currentStep))
            {
                var stepMatch = Regex.Match(currentStep, @"""(.*?)""", RegexOptions.IgnoreCase);
                if (stepMatch.Success)
                {
                    expectedValue = stepMatch.Groups[1].Value;
                }
            }

            // Identify the failure type and refine Expected/Actual values
            if (msg.Contains("element(s) not found") || msg.Contains("waiting for Locator") || msg.Contains("ToBeVisible")) 
            {
                friendlyStatus = "[Visibility Error]";
                insight = "A required element was not found on the screen.";
                if (expectedValue == "N/A" || expectedValue.Contains("ToBeVisible")) 
                    expectedValue = "Visible";
                
                if (actualValue == "N/A") actualValue = "Not visible or hidden";
            }
            else if (msg.Contains("ToHaveURL")) 
            {
                friendlyStatus = "[Navigation Error]";
                insight = "The website did not navigate to the expected page URL.";
                if (expectedValue == "N/A" || expectedValue.Contains("ToHaveURL")) 
                    expectedValue = "Target URL (Check technical logs)";
            }
            else if (msg.Contains("ToBeHidden"))
            {
                friendlyStatus = "[Visibility Error]";
                insight = "An element that should be hidden is still visible.";
                expectedValue = "Hidden";
                actualValue = "Visible";
            }
            else if (msg.Contains("ToHaveText") || msg.Contains("ToContainText"))
            {
                friendlyStatus = "[Content Error]";
                insight = "The text displayed on the page is incorrect.";
            }
            else if (msg.Contains("ToHaveValue"))
            {
                friendlyStatus = "[Input Error]";
                insight = "The value in the input field is not what was expected.";
            }
            else if (msg.Contains("ToBeEnabled"))
            {
                friendlyStatus = "[Interaction Error]";
                insight = "The element (button/input) is disabled and cannot be used.";
                expectedValue = "Enabled";
                actualValue = "Disabled";
            }
            else if (msg.Contains("ToBeDisabled"))
            {
                friendlyStatus = "[Interaction Error]";
                insight = "The element (button/input) is enabled when it should be disabled.";
                expectedValue = "Disabled";
                actualValue = "Enabled";
            }
            else if (msg.Contains("ToBeChecked"))
            {
                friendlyStatus = "[Selection Error]";
                insight = "The checkbox or radio button was not selected.";
                expectedValue = "Checked/Selected";
                actualValue = "Unchecked/Not Selected";
            }
            else if (msg.Contains("Assert.IsTrue failed") || msg.Contains("Assert.IsFalse failed"))
            {
                friendlyStatus = "[Assertion Error]";
                insight = "A logical check in the test failed.";
                expectedValue = msg.Contains("IsTrue") ? "True" : "False";
                actualValue = msg.Contains("IsTrue") ? "False" : "True";
            }
            else if (msg.Contains("Timeout") || ex is TimeoutException)
            {
                friendlyStatus = "[System Timeout]";
                insight = "The application took too long to respond.";
                if (actualValue == "N/A") actualValue = "Process timed out";
            }

            // Final Cleanup: If any technical jargon remains in expected/actual, strip it
            expectedValue = CleanTechnicalJargon(expectedValue);
            actualValue = CleanTechnicalJargon(actualValue);

            string reportOutput = 
                $"\n============================================================\n" +
                $"                TEST FAILURE SUMMARY                        \n" +
                $"============================================================\n" +
                $"STEP     : {currentStep}\n" +
                $"WORKER   : {workerId}\n" +
                $"CATEGORY : {category}\n" +
                $"STATUS   : {friendlyStatus}\n" +
                $"INSIGHT  : {insight}\n" +
                $"------------------------------------------------------------\n" +
                $"EXPECTED : {expectedValue}\n" +
                $"ACTUAL   : {actualValue}\n" +
                $"============================================================\n" +
                $"(Detailed technical logs are available for the engineering team.)";

            throw new StakeholderFriendlyException(reportOutput);
        }

        private string CleanTechnicalJargon(string value)
        {
            if (string.IsNullOrEmpty(value) || value == "N/A") return value;

            // Remove common technical suffixes and prefixes
            string cleaned = Regex.Replace(value, @"Async$", "", RegexOptions.IgnoreCase);
            cleaned = Regex.Replace(cleaned, @"^Expect", "", RegexOptions.IgnoreCase);
            cleaned = Regex.Replace(cleaned, @"^To(?:Have|Be|Contain)", "", RegexOptions.IgnoreCase);
            
            // Handle method-like names
            if (cleaned.EndsWith("()")) cleaned = cleaned.Substring(0, cleaned.Length - 2);

            return cleaned.Trim();
        }
    }
}
