using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Playwright;
using ReqnRoll_Playwright_BDD_MSTEST_Framework.PageObjects;
using ReqnRoll_Playwright_BDD_MSTEST_Framework.Utils;

namespace ReqnRoll_Playwright_BDD_MSTEST_Framework.Utils
{
    public class PlaywrightManager
    {
        private IPlaywright _playwright;
        private IBrowser _browser;
        private IBrowserContext _context;
        private IPage _page;
        private DialogState _dialogState;

        private static readonly string ResultsPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "TestResults"));
        public static readonly string ScreenshotsPath = Path.Combine(ResultsPath, "Screenshots");
        public static readonly string TracesPath = Path.Combine(ResultsPath, "Traces");

        public IPage Page => _page;
        public IBrowserContext Context => _context;
        public DialogState DialogState => _dialogState;

        public async Task InitializeAsync()
        {
            EnsureDirectoriesExist();

            bool isHeadless = bool.Parse(ConfigReader.getValue("Headless"));

            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = isHeadless,
            });

            _context = await _browser.NewContextAsync(new BrowserNewContextOptions
            {
                ViewportSize = new ViewportSize { Width = 1280, Height = 720 }
            });

            await _context.Tracing.StartAsync(new TracingStartOptions
            {
                Screenshots = true,
                Snapshots = true,
                Sources = true
            });

            _page = await _context.NewPageAsync();
            _dialogState = new DialogState();
            _page.Dialog += async (_, dialog) =>
            {
                _dialogState.LastMessage = dialog.Message;
                await dialog.AcceptAsync();
            };
        }

        private void EnsureDirectoriesExist()
        {
            Directory.CreateDirectory(ScreenshotsPath);
            Directory.CreateDirectory(TracesPath);
        }

        public async Task SaveTraceOnFailureAsync(string scenarioName)
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string traceFileName = $"{scenarioName.Replace(" ", "_")}_{timestamp}.zip";
            string traceFilePath = Path.Combine(TracesPath, traceFileName);

            await _context.Tracing.StopAsync(new TracingStopOptions { Path = traceFilePath });
            Console.WriteLine($"Trace saved to: {traceFilePath}");

            // Attach trace to ReportPortal
            try
            {
                ReportPortal.Shared.Context.Current.Log.Info($"Trace: {traceFileName}", "application/zip", File.ReadAllBytes(traceFilePath));
            }
            catch (Exception rpEx)
            {
                Console.WriteLine($"Failed to attach trace to ReportPortal: {rpEx.Message}");
            }
        }

        public async Task DisposeAsync()
        {
            if (_page != null) await _page.CloseAsync();
            if (_context != null) await _context.CloseAsync();
            if (_browser != null) await _browser.CloseAsync();
            _playwright?.Dispose();
        }
    }
}
