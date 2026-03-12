using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using ReqnRoll_Playwright_BDD_MSTEST_Framework.StepDefinitions;
using ReqnRoll_Playwright_BDD_MSTEST_Framework.Utils;
using static Microsoft.Playwright.Assertions;

namespace ReqnRoll_Playwright_BDD_MSTEST_Framework.PageObjects
{
    public class BasePage
    {
        protected IPage _page;
        protected readonly  ExceptionTranslator _errorTranslator = new ExceptionTranslator();
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
                await Expect(locator).ToHaveValueAsync(valueToInsert);

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

                await locator.SelectOptionAsync(new SelectOptionValue { Label = valueToSelect });
                await Expect(locator).ToHaveValueAsync(valueToSelect);
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

    }
}
