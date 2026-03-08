using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;

namespace ReqnRoll_Playwright_BDD_MSTEST_Framework.Utils
{
    public class MailsacClient
    {
        private readonly HttpClient _httpClient;
        private string ApiKey = ConfigReader.getValue("MailsacApiKey");

        public MailsacClient()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Mailsac-Key", ApiKey);
        }

        public async Task<string> GetLatestEmailBody(string email, int timeoutSecounds = 20)
        {
            var uri = $"https://mailsac.com/api/addresses/{email}/messages";
            var startTime = DateTime.Now;

            while ((DateTime.Now - startTime).TotalSeconds < timeoutSecounds)
            { 
                var response = await _httpClient.GetFromJsonAsync<JsonElement[]>(uri);

                if (response != null && response.Length > 0) 
                {
                    var latestEmailId = response[0].GetProperty("_id").GetString();

                    return await _httpClient.GetStringAsync($"https://mailsac.com/api/body/{email}/{latestEmailId}");
                } 
                await Task.Delay(1000); // Wait for 1 second before checking again
            }
            return null; // Return null if no email is received within the timeout period
        }
    }
}
