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
        public static readonly string ScreenshotsPath = Path.Combine(ResultsPath, "Screenshots");
        //private static readonly string VideosPath = Path.Combine(ResultsPath, "Videos");
        private static readonly string TracesPath = Path.Combine(ResultsPath, "Traces");

        [BeforeTestRun]
        public static void InitializeLogger()
        {
            if (!Directory.Exists(ResultsPath))
            {
                Directory.CreateDirectory(ResultsPath);
            }

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File(Path.Combine(ResultsPath, "Script_logs.txt"), rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Log.Information("Serilog initialized. Results path: {ResultsPath}", ResultsPath);
        }

        [AfterTestRun]
        public static void CloseLogger()
        {
            Log.CloseAndFlush();
        }

        public Hooks(IObjectContainer container)
        {
            _container = container;
        }

        [BeforeScenario("@cleanup_register_user")]
        public async Task CleanupRegisterUser()
        {
            var testEmail = ConfigReader.getValue("RegisterUserEmail");

            // Database Cleanup
            var db = new DatabaseRepository();
            await db.DeleteAsync("Register", $"EmailAddress = '{testEmail}'");

            // Mailsac Cleanup
            var mailsac = new MailsacClient();
            await mailsac.DeleteAllMessages(testEmail);
        }

        [BeforeScenario]
        public async Task FirstBeforeScenario()
        {
            // Ensure directories exist
            Directory.CreateDirectory(ScreenshotsPath);
            //Directory.CreateDirectory(VideosPath);
            Directory.CreateDirectory(TracesPath);

            Boolean isHeadless = Boolean.Parse(ConfigReader.getValue("Headless"));

            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = isHeadless, // Set to true for headless mode
            });

            _context = await _browser.NewContextAsync(new BrowserNewContextOptions
            {
                //RecordVideoDir = VideosPath,
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
        public async Task SaveTraceAfterFailedTest(ScenarioContext scenarioContext)
        {
            if (scenarioContext.TestError != null) 
            {
                string scenarioName = scenarioContext.ScenarioInfo.Title.Replace(" ", "_");
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string traceFileName = $"{scenarioName}_{timestamp}.zip";
                string traceFilePath = Path.Combine(TracesPath, traceFileName);

                await _context.Tracing.StopAsync(new TracingStopOptions { Path = traceFilePath });
                Console.WriteLine($"Trace saved to: {traceFilePath}");
            }
        }

        [AfterScenario]
        public async Task AfterScenario(ScenarioContext scenarioContext)
        {
            await SaveTraceAfterFailedTest(scenarioContext);
            await page.CloseAsync();
            await _context.CloseAsync();
            //await HandleVideoName(scenarioContext);
            await _browser.CloseAsync();
            _playwright.Dispose();
        }

        #region Redundent Code due to framework requirement change - Kept for reference
        //public async Task HandleVideoName(ScenarioContext scenarioContext)
        //{
        //    string scenarioName = scenarioContext.ScenarioInfo.Title.Replace(" ", "_");
        //    string timestamp = DateTime.Now.ToString("yyyyMMdd_[HH-mm-ss]");
        //    string videoFileName = $"{scenarioName}_{timestamp}.webm";
        //    string videoFilePath = Path.Combine(VideosPath, videoFileName);

        //    // Accessing the video from the page context
        //    var video = page.Video;

        //    if (video != null)
        //    {
        //        await video.SaveAsAsync(videoFilePath);
        //        Console.WriteLine($"Video saved to: {videoFilePath}");
        //    }
        //}
        #endregion
    }
}
