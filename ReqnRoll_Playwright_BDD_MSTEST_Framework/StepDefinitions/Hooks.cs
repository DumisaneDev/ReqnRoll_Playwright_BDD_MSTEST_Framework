using System;
using System.IO;
using System.Threading.Tasks;
using Io.Cucumber.Messages.Types;
using Reqnroll;
using Reqnroll.BoDi;
using ReqnRoll_Playwright_BDD_MSTEST_Framework.PageObjects;
using ReqnRoll_Playwright_BDD_MSTEST_Framework.Utils;

namespace ReqnRoll_Playwright_BDD_MSTEST_Framework.StepDefinitions
{
    [Binding]
    public class Hooks
    {
        private readonly IObjectContainer _container;
        private IPlaywright _playwright;
        private IBrowser _browser;
        private IBrowserContext _context;
        private IPage page;

        private static readonly string ResultsPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "TestResults"));
        private static readonly string ScreenshotsPath = Path.Combine(ResultsPath, "Screenshots");
        private static readonly string VideosPath = Path.Combine(ResultsPath, "Videos");
        private static readonly string TracesPath = Path.Combine(ResultsPath, "Traces");

        public Hooks(IObjectContainer container)
        {
            _container = container;
        }

        [BeforeScenario]
        public async Task FirstBeforeScenario()
        {
            // Ensure directories exist
            Directory.CreateDirectory(ScreenshotsPath);
            Directory.CreateDirectory(VideosPath);
            Directory.CreateDirectory(TracesPath);

            Boolean isHeadless = Boolean.Parse(ConfigReader.getValue("Headless"));

            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = isHeadless, // Set to true for headless mode
            });

            _context = await _browser.NewContextAsync(new BrowserNewContextOptions
            {
                RecordVideoDir = VideosPath,
                ViewportSize = new ViewportSize { Width = 1280, Height = 720 }
            });

            // Start Tracing
            await _context.Tracing.StartAsync(new TracingStartOptions
            {
                Screenshots = true,
                Snapshots = true,
                Sources = true
            });

            page = await _context.NewPageAsync();

            var dialogState = new Utils.DialogState();
            page.Dialog += async (_, dialog) =>
            {
                dialogState.LastMessage = dialog.Message;
                await dialog.AcceptAsync();
            };

            // Register instances for Dependency Injection
            _container.RegisterInstanceAs(page);
            _container.RegisterInstanceAs(dialogState);
        }

        [AfterStep]
        public async Task TakeScreenshotAfterStep(ScenarioContext scenarioContext)
        {
            if (scenarioContext.TestError != null) // Only take screenshot if the step has failed
            {
                string scenarioName = scenarioContext.ScenarioInfo.Title.Replace(" ", "_");
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");

                string screenshotFileName = $"{scenarioName}_{timestamp}.png";
                string screenshotFilePath = Path.Combine(ScreenshotsPath, screenshotFileName);
                await page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotFilePath });
                Console.WriteLine($"Screenshot saved to: {screenshotFilePath}");
            }
        }



        [AfterScenario]
        public async Task AfterScenario(ScenarioContext scenarioContext)
        {
            string scenarioName = scenarioContext.ScenarioInfo.Title.Replace(" ", "_");
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string traceFileName = $"{scenarioName}_{timestamp}.zip";
            string traceFilePath = Path.Combine(TracesPath, traceFileName);

            await _context.Tracing.StopAsync(new TracingStopOptions { Path = traceFilePath });
            Console.WriteLine($"Trace saved to: {traceFilePath}");

            await page.CloseAsync();
            await _context.CloseAsync();
            await HandleVideoName(scenarioContext);
            await _browser.CloseAsync();
            _playwright.Dispose();
        }

        public async Task HandleVideoName(ScenarioContext scenarioContext)
        {
            string scenarioName = scenarioContext.ScenarioInfo.Title.Replace(" ", "_");
            string timestamp = DateTime.Now.ToString("yyyyMMdd_[HH-mm-ss]");
            string videoFileName = $"{scenarioName}_{timestamp}.webm";
            string videoFilePath = Path.Combine(VideosPath, videoFileName);

            // Accessing the video from the page context
            var video = page.Video;

            if (video != null)
            {
                await video.SaveAsAsync(videoFilePath);
                Console.WriteLine($"Video saved to: {videoFilePath}");
            }
        }
    }
}
