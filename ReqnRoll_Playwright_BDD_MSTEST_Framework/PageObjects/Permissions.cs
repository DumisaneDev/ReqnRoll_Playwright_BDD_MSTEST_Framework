using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReqnRoll_Playwright_BDD_MSTEST_Framework.Utils;
using static Microsoft.Playwright.Assertions;

namespace ReqnRoll_Playwright_BDD_MSTEST_Framework.PageObjects
{
    public class Permissions : BasePage
    {
        public Permissions(IPage page) : base(page)
        {
        }

        // Locators based on the provided DOM
        private ILocator loc_txtSearch => _page.Locator("xpath=//input[@id='searchInput']");
        private ILocator loc_tblEmployees => _page.Locator("xpath=//table[contains(@class, 'datatables')]");
        private ILocator loc_btnSortName => _page.Locator("xpath=//th[contains(., 'First Name')]//button[contains(@class, 'btnSort')]");
        
        // Dynamic locators for Edit - Unique identification by name and email
        private ILocator getEditButton(string employeeName, string email = null) 
        {
            if (!string.IsNullOrEmpty(email))
            {
                // Find row containing the specific email and click its edit button
                return _page.Locator($"xpath=//tr[td[contains(., '{email.Split('@')[0]}')]]//button[@title='Edit']").First;
            }

            var parts = employeeName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2)
            {
                return _page.Locator($"xpath=//tr[td[contains(., '{parts[0]}')] and td[contains(., '{parts[1]}')]]//button[@title='Edit']").First;
            }
            return _page.Locator($"xpath=//tr[contains(., '{employeeName}')]//button[@title='Edit']").First;
        }

        // Web Actions
        public async Task searchEmployee(string employeeName, string scenarioTitle)
        {
            await populateInputField(loc_txtSearch, employeeName, scenarioTitle);
            await Task.Delay(1000); 
        }

        public async Task sortByColumn(string columnName, string scenarioTitle)
        {
            if (columnName.Equals("Name", StringComparison.OrdinalIgnoreCase) || columnName.Equals("First Name", StringComparison.OrdinalIgnoreCase))
            {
                await clickElement(loc_btnSortName, scenarioTitle);
            }
            else
            {
                var header = loc_tblEmployees.Locator($"xpath=//th[contains(., '{columnName}')]//button");
                await clickElement(header, scenarioTitle);
            }
        }

        public async Task clickEditForEmployee(string employeeName, string scenarioTitle, string email = null)
        {
            await clickElement(getEditButton(employeeName, email), scenarioTitle);
        }

        // Validation Methods
        public async Task verifyTableSorted(string columnName, string order)
        {
            bool descending = order.Equals("descending", StringComparison.OrdinalIgnoreCase) || order.Equals("reverse", StringComparison.OrdinalIgnoreCase);
            string targetColumn = columnName.Equals("Name", StringComparison.OrdinalIgnoreCase) ? "First Name" : columnName;
            
            bool isSorted = await IsTableSortedAsync(loc_tblEmployees, targetColumn, descending);
            Assert.IsTrue(isSorted, $"Table should be sorted by {columnName} in {order} order.");
        }

        public async Task verifyEmployeeVisible(string employeeName)
        {
            var parts = employeeName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            ILocator cell;
            if (parts.Length >= 2)
            {
                cell = _page.Locator($"xpath=//table[contains(@class, 'datatables')]//tr[td[contains(., '{parts[0]}')] and td[contains(., '{parts[1]}')]]").First;
            }
            else
            {
                cell = _page.Locator($"xpath=//table[contains(@class, 'datatables')]//td[contains(., '{employeeName}')]").First;
            }
            await Expect(cell).ToBeVisibleAsync();
        }

        public async Task verifyTabVisibility(string tabName, bool visible)
        {
            // Sidebar tabs check - matching by text content precisely
            var tab = _page.Locator($"xpath=//div[contains(@class, 'menu-item')]//a[normalize-space()='{tabName}' or contains(., '{tabName}')]");
            if (visible)
                await Expect(tab.First).ToBeVisibleAsync();
            else
                await Expect(tab.First).ToBeHiddenAsync();
        }
    }
}
