using System;
using System.IO;
using Serilog;
using ReportPortal.Serilog;

namespace ReqnRoll_Playwright_BDD_MSTEST_Framework.Utils
{
    public static class LoggerManager
    {
        private static readonly string ResultsPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "TestResults"));

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
                .WriteTo.ReportPortal()
                .CreateLogger();

            Log.Information("Serilog initialized. Results path: {ResultsPath}", ResultsPath);
        }

        public static void CloseLogger()
        {
            Log.CloseAndFlush();
        }
    }
}
