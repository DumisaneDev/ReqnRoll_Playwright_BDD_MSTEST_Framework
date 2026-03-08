using System;
using System.Collections.Generic;
using System.Text;
using static Microsoft.Playwright.Assertions;

namespace ReqnRoll_Playwright_BDD_MSTEST_Framework.PageObjects
{
    public class BasePage
    {
        protected IPage _page;

        protected BasePage(IPage page)
        {
            this._page = page;
        }

        public async Task goToPage(String testURL) => await _page.GotoAsync(testURL);


        public async Task populateInputField(ILocator locator, String valueToInsert) => 
            await locator.FillAsync(valueToInsert);

        public async Task selectDropdownValue(ILocator locator, String valueToSelect) =>
            await locator.SelectOptionAsync(new SelectOptionValue { Label = valueToSelect });


        public async Task clickElement(ILocator locator) => await locator.ClickAsync();

        public async Task<string> getPageTitle()
        {
            return await _page.TitleAsync();
        }

        public async Task<string> getPageURL()
        {
            return _page.Url;
        }

        public async Task<string> getElementContent(ILocator locator)
        {
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
