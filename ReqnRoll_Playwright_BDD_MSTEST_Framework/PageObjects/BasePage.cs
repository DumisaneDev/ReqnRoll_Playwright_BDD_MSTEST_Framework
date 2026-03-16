using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Playwright;
using ReqnRoll_Playwright_BDD_MSTEST_Framework.StepDefinitions;
using ReqnRoll_Playwright_BDD_MSTEST_Framework.Utils;
using static System.Net.WebRequestMethods;
using static Microsoft.Playwright.Assertions;

namespace ReqnRoll_Playwright_BDD_MSTEST_Framework.PageObjects
{
    public class BasePage
    {
        protected IPage _page;
        protected readonly ExceptionTranslator _errorTranslator = new ExceptionTranslator();
        protected ScreenshotManager _screenshotManager;
        protected readonly string _screenshotPath;

        protected BasePage(IPage page)
        {
            this._page = page;
            _screenshotManager = new ScreenshotManager(page);
        }

        public async Task goToPage(String testURL, string scenarioTitle)
        {
            try
            {
                await _page.GotoAsync(testURL);
                await Expect(_page).ToHaveURLAsync(new System.Text.RegularExpressions.Regex(testURL));
                await _screenshotManager.captureScreenshot(scenarioTitle, Hooks.ScreenshotsPath, $"Navigating_to_{testURL}_page");
            }
            catch (Exception ex)
            {
                Log.Information($"\nException hit when navigating to page...{ex.Message}");
                _errorTranslator.Translate(ex);
            }
        }


        public async Task populateInputField(ILocator locator, String valueToInsert, string scenarioTitle)
        {
            try
            {
                await Expect(locator).ToBeVisibleAsync();
                string elementText = await locator.GetAttributeAsync("placeholder") ?? await locator.InnerTextAsync();
                if (string.IsNullOrWhiteSpace(elementText)) elementText = "input_field";

                await locator.FillAsync(valueToInsert);
                
                try 
                {
                    await Expect(locator).ToHaveValueAsync(valueToInsert);
                }
                catch (Exception)
                {
                    if (!string.IsNullOrEmpty(valueToInsert)) throw;
                    Log.Information("Warning: Could not verify empty value in input field.");
                }

                if (valueToInsert == string.Empty) valueToInsert = "Empty";

                await _screenshotManager.captureScreenshot(scenarioTitle, Hooks.ScreenshotsPath, $"Inputing_{valueToInsert}_into_{elementText}");
            }
            catch (Exception ex)
            {
                Log.Information($"Exception hit when populating input field... {ex.Message}");
                _errorTranslator.Translate(ex);
            }
        }


        public async Task selectDropdownValue(ILocator locator, String valueToSelect, string scenarioTitle)
        {
            try
            {
                await Expect(locator).ToBeVisibleAsync();
                string elementText = await locator.InnerTextAsync();
                if (string.IsNullOrWhiteSpace(elementText)) elementText = "dropdown";

                if (!string.IsNullOrEmpty(valueToSelect))
                {
                    await locator.SelectOptionAsync(new SelectOptionValue { Label = valueToSelect });
                    await Expect(locator).ToHaveValueAsync(valueToSelect);
                }
                
                await _screenshotManager.captureScreenshot(scenarioTitle, Hooks.ScreenshotsPath, $"Selecting_{valueToSelect}_in_{elementText}");
            }
            catch (Exception ex)
            {
                Log.Information($"Exception hit when selecting dropdown value... {ex.Message}");
                _errorTranslator.Translate(ex);
            }
        }


        public async Task clickElement(ILocator locator, string scenarioTitle)
        {
            try
            {
                await Expect(locator).ToBeVisibleAsync();
                // Get text or title/aria-label for a better description
                string elementText = await locator.InnerTextAsync();
                if (string.IsNullOrWhiteSpace(elementText)) elementText = await locator.GetAttributeAsync("value");
                if (string.IsNullOrWhiteSpace(elementText)) elementText = await locator.GetAttributeAsync("aria-label");
                if (string.IsNullOrWhiteSpace(elementText)) elementText = "button";

                await locator.ClickAsync();
                await _screenshotManager.captureScreenshot(scenarioTitle, Hooks.ScreenshotsPath, $"Clicking_{elementText}");
            }
            catch (Exception ex)
            {
                Log.Information($"Exception hit when clicking element...{ex.Message}");
                _errorTranslator.Translate(ex);
            }
        }

        public async Task<string> getPageTitle() => await _page.TitleAsync();


        public async Task<string> getPageURL()
        {
            return _page.Url;
        }

        public async Task<string> getElementContent(ILocator locator)
        {
            try
            {
                await Expect(locator).ToBeVisibleAsync();
            }
            catch (Exception ex)
            {
                Log.Information($"Exception hit when getting element content...{ex.Message}");
                _errorTranslator.Translate(ex);
            }
            return await locator.InnerTextAsync();
        }

        public async Task handlePopupModel(ILocator locator, string popupText, string btnName)
        {
            var model = locator;
            await Expect(model).ToBeVisibleAsync();
            await Expect(model.GetByText(popupText)).ToHaveValueAsync(popupText);
            await model.GetByRole(AriaRole.Button, new() { Name = btnName }).ClickAsync();
            await Expect(model).ToBeHiddenAsync();
        }

        public async Task<string> retriveOTPCodeFromEmail(string email, string RegExpPattern)
        {
            MailsacClient mailsacClient = new MailsacClient();

            string emailBody = await mailsacClient.GetLatestEmailBody(email, 30);

            if (string.IsNullOrEmpty(emailBody)) return "";

            Match match = Regex.Match(emailBody, RegExpPattern, RegexOptions.IgnoreCase);

            return match.Value;
        }

        public async Task<bool> IsTableSortedAsync(ILocator tableLocator, string columnName, bool descending = false)
        {
            try
            {
                // Find column index based on header text
                var headers = tableLocator.Locator("thead th");
                var headerTexts = await headers.AllInnerTextsAsync();
                int columnIndex = -1;

                for (int i = 0; i < headerTexts.Count; i++)
                {
                    if (headerTexts[i].Contains(columnName, StringComparison.OrdinalIgnoreCase))
                    {
                        columnIndex = i;
                        break;
                    }
                }

                if (columnIndex == -1)
                {
                    Log.Error($"Column '{columnName}' not found in table headers.");
                    return false;
                }

                // Extract texts from the identified column (nth-child is 1-based)
                var columnCells = tableLocator.Locator($"tbody tr td:nth-child({columnIndex + 1})");
                var cellTexts = await columnCells.AllInnerTextsAsync();
                var cleanedTexts = cellTexts.Select(t => t.Trim()).ToList();

                var sorted = new List<string>(cleanedTexts);
                if (descending)
                    sorted.Sort((a, b) => string.Compare(b, a, StringComparison.OrdinalIgnoreCase));
                else
                    sorted.Sort(StringComparer.OrdinalIgnoreCase);

                bool isSorted = cleanedTexts.SequenceEqual(sorted);

                if (isSorted)
                    Log.Information($"Table column '{columnName}' is correctly sorted in {(descending ? "descending" : "ascending")} order.");
                else
                    Log.Information($"Table column '{columnName}' sorting check failed. Expected {(descending ? "descending" : "ascending")} order.");

                return isSorted;
            }
            catch (Exception ex)
            {
                Log.Error($"Error checking table sorting: {ex.Message}");
                return false;
            }
        }
    }
}
