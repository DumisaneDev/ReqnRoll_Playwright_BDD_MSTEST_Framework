using System;
using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ReqnRoll_Playwright_BDD_MSTEST_Framework.Utils
{
    /// <summary>
    /// A custom exception that hides the technical stack trace from reports 
    /// while maintaining the failure status in MSTest.
    /// </summary>
    public class StakeholderFriendlyException : AssertFailedException
    {
        public override string StackTrace => ""; 
        public StakeholderFriendlyException(string message) : base(message) { }
    }

    public class ExceptionTranslator
    {
        public void Translate(Exception ex)
        {
            // Log the full TECHNICAL detail for developers in the Serilog log file
            Log.Error($"[TECHNICAL ERROR]: {ex.GetType().Name}\nMessage: {ex.Message}");

            string friendlyStatus = "[Test Failure]";
            string insight = "The application did not behave as expected. This could be due to a bug or a change in requirements.";
            string expectedValue = "N/A";
            string actualValue = "N/A";

            string msg = ex.Message;

            // 2. Refined Parsing Logic
            // Playwright .NET outputs typically look like: 
            // - Locator expected text matching regex 'Expected Text'
            // - But was: 'Actual Text'
            // OR
            // - expected string "Expected Text"
            // - received string "Actual Text"
            
            // Expected Value Extraction
            var expectedMatch = Regex.Match(msg, @"(?:expected|expected string|expected pattern|expected value|expected text matching regex)\s*[:\s-]*\s*(?:""|/|')?(.*?)(?:""|/|')?(?:\r|\n|$)", RegexOptions.IgnoreCase);
            if (expectedMatch.Success && expectedMatch.Groups.Count > 1)
            {
                expectedValue = expectedMatch.Groups[1].Value.Trim(' ', '"', '\r', '\n', '/', '\'');
                // Specifically remove 'text matching regex' if it leaked into the captured group (though regex above should handle most cases)
                expectedValue = Regex.Replace(expectedValue, @"^text matching regex\s+", "", RegexOptions.IgnoreCase).Trim('\'', ' ');
            }

            // Actual Value Extraction
            // We look for 'But was:', 'received:', 'actual:', etc. 
            // For 'But was:', it can be multi-line, so we look for the content between quotes.
            var actualMatch = Regex.Match(msg, @"(?:but was|received|actual|received string|actual value|received value)\s*[:\s-]*\s*(?:""|/|')\s*(.*?)\s*(?:""|/|')\s*(?:\r|\n|Call log:|$)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            
            if (actualMatch.Success && actualMatch.Groups.Count > 1)
            {
                actualValue = actualMatch.Groups[1].Value.Trim();
            }
            else
            {
                // Fallback for simpler single-line formats
                var actualMatchSimple = Regex.Match(msg, @"(?:received|actual|received string|actual value|received value)\s*[:\s-]*\s*(?:""|/|')?(.*?)(?:""|/|')?(?:\r|\n|$)", RegexOptions.IgnoreCase);
                if (actualMatchSimple.Success && actualMatchSimple.Groups.Count > 1)
                {
                    actualValue = actualMatchSimple.Groups[1].Value.Trim(' ', '"', '\r', '\n', '/', '\'');
                }
            }

            // Identify the failure type and provide non-technical insight
            if (msg.Contains("Expected") || msg.Contains("Received") || msg.Contains("Actual") || msg.Contains("PlaywrightAssertionException"))
            {
                friendlyStatus = "[Validation Failed]";
                insight = "The information on the screen does not match what the test expected.";

                if (msg.Contains("ToHaveURL")) 
                {
                    friendlyStatus = "[Navigation Error]";
                    insight = "The website did not navigate to the expected page URL.";
                }
                else if (msg.Contains("ToBeVisible")) 
                {
                    friendlyStatus = "[Visibility Error]";
                    insight = "A required element was not found on the screen. It might be missing or hidden.";
                }
                else if (msg.Contains("ToHaveText") || msg.Contains("ToContainText"))
                {
                    friendlyStatus = "[Content Error]";
                    insight = "The text message or label displayed on the page is incorrect.";
                }
            }
            else if (msg.Contains("Timeout") || ex is TimeoutException)
            {
                friendlyStatus = "[System Timeout]";
                insight = "The application took too long to respond. The test waited but the element/page never appeared.";
            }

            // Construct the clean, boxed summary for the LivingDoc
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

            // Throw the custom exception to hide the stack trace in the LivingDoc report
            throw new StakeholderFriendlyException(reportOutput);
        }
    }
}
