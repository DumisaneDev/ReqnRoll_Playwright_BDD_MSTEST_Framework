using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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

        public async Task<string> GetLatestEmailBody(string email, int timeoutSeconds = 45)
        {
            var uri = $"https://mailsac.com/api/addresses/{email}/messages";
            var startTime = DateTime.Now;

            while ((DateTime.Now - startTime).TotalSeconds < timeoutSeconds)
            { 
                var response = await _httpClient.GetFromJsonAsync<JsonElement[]>(uri);

                if (response != null && response.Length > 0) 
                {
                    // Sort by received date if possible, but usually [0] is latest
                    var latestEmailId = response[0].GetProperty("_id").GetString();

                    // Try to get plain text body for easier regex matching
                    var bodyResponse = await _httpClient.GetAsync($"https://mailsac.com/api/text/{email}/{latestEmailId}");
                    if (bodyResponse.IsSuccessStatusCode)
                    {
                        return await bodyResponse.Content.ReadAsStringAsync();
                    }
                    
                    // Fallback to original body endpoint
                    return await _httpClient.GetStringAsync($"https://mailsac.com/api/body/{email}/{latestEmailId}");
                } 
                await Task.Delay(3000); // Increased delay for stability
            }
            return null;
        }

        public async Task DeleteAllMessages(string email)
        {
            var uri = $"https://mailsac.com/api/addresses/{email}/messages";
            var response = await _httpClient.DeleteAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                Log.Information($"Successfully deleted all messages for {email}");
            }
            else
            {
                Log.Warning($"Failed to delete messages for {email}. Status: {response.StatusCode}");
            }
        }
    }
}
