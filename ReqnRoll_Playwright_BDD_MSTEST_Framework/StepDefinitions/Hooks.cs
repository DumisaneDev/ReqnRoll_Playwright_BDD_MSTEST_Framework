using System;
using System.IO;
using System.Threading.Tasks;
using Reqnroll;
using Reqnroll.BoDi;
using ReqnRoll_Playwright_BDD_MSTEST_Framework.Utils;
using Serilog;

namespace ReqnRoll_Playwright_BDD_MSTEST_Framework.StepDefinitions
{
    [Binding]
    public class Hooks
    {
        private readonly IObjectContainer _container;
        private readonly IReqnrollOutputHelper _outputHelper;
        private readonly ScenarioContext _scenarioContext;
        private readonly PlaywrightManager _playwrightManager;

        public static string ScreenshotsPath => PlaywrightManager.ScreenshotsPath;
        public static string TracesPath => PlaywrightManager.TracesPath;

        public Hooks(IObjectContainer container, IReqnrollOutputHelper outputHelper, ScenarioContext scenarioContext)
        {
            _container = container;
            _outputHelper = outputHelper;
            _scenarioContext = scenarioContext;
            _playwrightManager = new PlaywrightManager();
        }

        #region Test Lifecycle Hooks

        [BeforeTestRun]
        public static void GlobalSetup()
        {
            LoggerManager.InitializeLogger();
            WorkerManager.InitializeWorkerPool();
        }

        [AfterTestRun]
        public static void GlobalTeardown()
        {
            Log.Information("Generating Worker Summary Report...");
            ReportManager.GenerateWorkerSummaryReport();
            LoggerManager.CloseLogger();
        }

        #endregion

        #region Scenario Lifecycle Hooks

        [BeforeScenario(Order = 0)]
        public void SetupWorkerContext()
        {
            if (WorkerManager.ShouldLogWorkerCount())
            {
                _outputHelper.WriteLine($"Number of Workers Employed = {WorkerManager.TotalWorkersDetected}");
                _outputHelper.WriteLine("-------------------------------------------");
            }

            WorkerManager.LeaseWorkerId();
            _outputHelper.WriteLine($"Worker ID: Worker-{WorkerManager.AssignedWorkerId}");
        }

        [BeforeScenario(Order = 1)]
        public async Task SetupPlaywright()
        {
            await _playwrightManager.InitializeAsync();

            _container.RegisterInstanceAs(_playwrightManager.Page);
            _container.RegisterInstanceAs(_playwrightManager.DialogState);
        }

        [AfterScenario]
        public async Task TeardownScenario()
        {
            WorkerManager.IncrementTestCount();

            await HandleTracingOnFailure();
            await _playwrightManager.DisposeAsync();
            
            WorkerManager.ReleaseWorkerId();
        }

        #endregion

        #region Step Lifecycle Hooks

        [BeforeStep]
        public void CaptureStepContext()
        {
            ExceptionTranslator.CurrentStep.Value = _scenarioContext.StepContext.StepInfo.Text;
        }

        #endregion

        #region Tagged Hooks

        [BeforeScenario("@cleanup_register_user")]
        public async Task CleanupRegisterUserData()
        {
            var testEmail = ConfigReader.getValue("RegisterUserEmail");

            // Database Cleanup
            var db = new DatabaseRepository();
            await db.DeleteAsync("Register", $"EmailAddress = '{testEmail}'");

            // Mailsac Cleanup
            var mailsac = new MailsacClient();
            await mailsac.DeleteAllMessages(testEmail);
        }

        #endregion

        #region Private Helper Methods

        private async Task HandleTracingOnFailure()
        {
            if (_scenarioContext.TestError != null)
            {
                await _playwrightManager.SaveTraceOnFailureAsync(_scenarioContext.ScenarioInfo.Title);
            }
        }
        #endregion
    }
}
