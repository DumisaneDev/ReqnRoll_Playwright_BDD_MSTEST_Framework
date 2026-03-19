using System;
using System.Text.RegularExpressions;
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
        public void Translate(Exception ex)
        {
            Log.Error($"[TECHNICAL ERROR]: {ex.GetType().Name}\nMessage: {ex.Message}");

            string friendlyStatus = "[Test Failure]";
            string insight = "The application did not behave as expected. This could be due to a bug or a change in requirements.";
            string expectedValue = "N/A";
            string actualValue = "N/A";

            string msg = ex.Message;

            // 1. Element Not Found / Timeout specific handling
            if (msg.Contains("element(s) not found") || msg.Contains("waiting for Locator"))
            {
                actualValue = "Element not found or hidden";
            }

            // 2. Expected Value Extraction
            // Improved regex to skip technical Playwright phrasing like "to have text matching regex"
            var expectedMatch = Regex.Match(msg, @"expected.*?(?:""|'|/)(.*?)(?:""|'|/)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (expectedMatch.Success && expectedMatch.Groups.Count > 1)
            {
                expectedValue = expectedMatch.Groups[1].Value.Trim();
                
                // If the regex accidentally captured the "to have text..." part because of nesting
                // we strip common Playwright assertion prefixes
                expectedValue = Regex.Replace(expectedValue, @"^(?:to have|to contain|to be|text matching regex|string|value)\s+", "", RegexOptions.IgnoreCase).Trim();
            }
            else 
            {
                // Fallback for simpler formats
                var expectedFallback = Regex.Match(msg, @"expected\s*(?:string|value)?\s*[:\s-]*\s*(.*?)(\r|\n|but was|received|$)", RegexOptions.IgnoreCase);
                if (expectedFallback.Success) 
                {
                    expectedValue = expectedFallback.Groups[1].Value.Trim(' ', '"', '\'', ':');
                    expectedValue = Regex.Replace(expectedValue, @"^(?:to have|to contain|to be|text matching regex)\s+", "", RegexOptions.IgnoreCase).Trim();
                }
            }

            // 3. Actual Value Extraction
            var actualMatch = Regex.Match(msg, @"(?:but was|received|actual)\s*[:\s-]*\s*(?:""|'|/)(.*?)(?:""|'|/)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (actualMatch.Success && actualMatch.Groups.Count > 1)
            {
                actualValue = actualMatch.Groups[1].Value.Trim();
                if (string.IsNullOrEmpty(actualValue) || actualValue == "null") actualValue = "empty/null";
            }
            else if (msg.Contains("But was: 'null'"))
            {
                actualValue = "empty/null";
            }

            // Identify the failure type
            if (msg.Contains("ToHaveURL")) 
            {
                friendlyStatus = "[Navigation Error]";
                insight = "The website did not navigate to the expected page URL.";
            }
            else if (msg.Contains("ToBeVisible")) 
            {
                friendlyStatus = "[Visibility Error]";
                insight = "A required element was not found on the screen.";
            }
            else if (msg.Contains("ToHaveText") || msg.Contains("ToContainText"))
            {
                friendlyStatus = "[Content Error]";
                insight = "The text displayed on the page is incorrect.";
            }
            else if (msg.Contains("Timeout") || ex is TimeoutException)
            {
                friendlyStatus = "[System Timeout]";
                insight = "The application took too long to respond.";
            }

            string reportOutput = 
                $"\n============================================================\n" +
                $"                TEST FAILURE SUMMARY                        \n" +
                $"============================================================\n" +
                $"STATUS   : {friendlyStatus}\n" +
                $"INSIGHT  : {insight}\n" +
                $"------------------------------------------------------------\n" +
                $"EXPECTED : {expectedValue}\n" +
                $"ACTUAL   : {actualValue}\n" +
                $"============================================================\n" +
                $"(Detailed technical logs are available for the engineering team.)";

            throw new StakeholderFriendlyException(reportOutput);
        }
    }
}
