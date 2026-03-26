using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReqnRoll_Playwright_BDD_MSTEST_Framework.Utils
{
    public class ScreenshotManager
    {
        private readonly IPage _page;
        public ScreenshotManager(IPage page) 
        {
            _page = page;
        }

        public async Task captureScreenshot(string scenarioTitle, string baseScreenshotPath, string actionTaken)
        {
            try
            {
                string datePrefix = DateTime.Now.ToString("yyyy-MM-dd");
                string timestamp = DateTime.Now.ToString("HHmmss");

                // Sanitize scenario title for folder name
                string sanitizedScenarioFolder = SanitizeFileName(scenarioTitle);
                
                // Construct scenario-specific directory: TestResults/Screenshots/yyyy-MM-dd/Scenario_Name/
                string scenarioDirectory = Path.Combine(baseScreenshotPath, datePrefix, sanitizedScenarioFolder);
                
                if (!Directory.Exists(scenarioDirectory))
                {
                    Directory.CreateDirectory(scenarioDirectory);
                }

                // Sanitize actionTaken for filename
                string sanitizedAction = SanitizeFileName(actionTaken);
                
                // Filename: HHmmss_Action_Taken.png
                string screenshotFileName = $"{timestamp}_{sanitizedAction}.png";
                string fullScreenshotPath = Path.Combine(scenarioDirectory, screenshotFileName);

                await _page.ScreenshotAsync(new PageScreenshotOptions { Path = fullScreenshotPath });
                Log.Information($"Screenshot captured: {fullScreenshotPath}");

                // Attach to ReportPortal
                try
                {
                    ReportPortal.Shared.Context.Current.Log.Info(sanitizedAction, "image/png", File.ReadAllBytes(fullScreenshotPath));
                }
                catch (Exception rpEx)
                {
                    Log.Warning($"Failed to attach screenshot to ReportPortal: {rpEx.Message}");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to capture screenshot for '{scenarioTitle}' during '{actionTaken}': {ex.Message}");
            }
        }

        private string SanitizeFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return "unnamed_action";

            // Replace invalid characters with underscores
            char[] invalidChars = Path.GetInvalidFileNameChars();
            string sanitized = new string(fileName.Select(ch => invalidChars.Contains(ch) || ch == ':' || ch == '/' || ch == '\\' ? '_' : ch).ToArray());
            
            // Further clean up: replace multiple underscores with a single one and trim
            sanitized = System.Text.RegularExpressions.Regex.Replace(sanitized, @"_+", "_").Trim('_');
           
            if (sanitized.Length > 100)
            {
                sanitized = sanitized.Substring(0, 100);
            }

            return sanitized;
        }
    }
}
