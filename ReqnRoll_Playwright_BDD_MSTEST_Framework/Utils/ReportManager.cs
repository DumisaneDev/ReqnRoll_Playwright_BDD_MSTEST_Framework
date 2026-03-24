using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Serilog;

namespace ReqnRoll_Playwright_BDD_MSTEST_Framework.Utils
{
    public static class ReportManager
    {
        private static readonly string ResultsPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "TestResults"));

        public static void GenerateWorkerSummaryReport()
        {
            if (WorkerManager.WorkerCount <= 1)
            {
                Log.Information("Single worker detected. Skipping parallel execution summary report.");
                return;
            }

            try
            {
                string livingDocPath = Path.Combine(ResultsPath, "LivingDoc.html");
                bool fileReady = WaitForLivingDoc(livingDocPath);

                var summaryHtml = BuildSummaryHtml();

                if (fileReady)
                {
                    InjectIntoLivingDoc(livingDocPath, summaryHtml);
                }
                else
                {
                    GenerateStandaloneReport(summaryHtml);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to generate worker summary report.");
            }
        }

        private static bool WaitForLivingDoc(string path)
        {
            int attempts = 0;
            while (attempts < 20)
            {
                try
                {
                    if (File.Exists(path))
                    {
                        using (FileStream stream = File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                        {
                            return true;
                        }
                    }
                }
                catch (IOException) { }
                Thread.Sleep(500);
                attempts++;
            }
            return false;
        }

        private static string BuildSummaryHtml()
        {
            var summaryHtml = new StringBuilder();
            summaryHtml.AppendLine("<div id='worker-summary' style='margin-top: 50px; padding: 20px; border: 1px solid #ddd; border-radius: 8px; font-family: sans-serif; background-color: #f9f9f9;'>");
            summaryHtml.AppendLine("    <h2 style='color: #333; border-bottom: 2px solid #ED7725; padding-bottom: 10px;'>Parallel Execution Summary</h2>");
            summaryHtml.AppendLine($"    <p><strong>Total Workers Employed:</strong> {WorkerManager.TotalWorkersDetected}</p>");
            summaryHtml.AppendLine("    <table style='width: 100%; border-collapse: collapse; margin-top: 20px;'>");
            summaryHtml.AppendLine("        <thead>");
            summaryHtml.AppendLine("            <tr style='background-color: #ED7725; color: white;'>");
            summaryHtml.AppendLine("                <th style='padding: 12px; text-align: left; border: 1px solid #ddd;'>Worker ID</th>");
            summaryHtml.AppendLine("                <th style='padding: 12px; text-align: left; border: 1px solid #ddd;'>Tests Executed</th>");
            summaryHtml.AppendLine("                <th style='padding: 12px; text-align: left; border: 1px solid #ddd;'>Workload Distribution</th>");
            summaryHtml.AppendLine("            </tr>");
            summaryHtml.AppendLine("        </thead>");
            summaryHtml.AppendLine("        <tbody>");

            int totalTests = WorkerManager.WorkerTestCounts.Values.Sum();

            foreach (var entry in WorkerManager.WorkerTestCounts.OrderBy(e => e.Key))
            {
                double percentage = totalTests > 0 ? (double)entry.Value / totalTests * 100 : 0;
                summaryHtml.AppendLine("            <tr>");
                summaryHtml.AppendLine($"                <td style='padding: 10px; border: 1px solid #ddd;'>Worker {entry.Key}</td>");
                summaryHtml.AppendLine($"                <td style='padding: 10px; border: 1px solid #ddd;'>{entry.Value}</td>");
                summaryHtml.AppendLine("                <td style='padding: 10px; border: 1px solid #ddd;'>");
                summaryHtml.AppendLine($"                    <div style='background-color: #e0e0e0; width: 100%; height: 20px; border-radius: 10px; overflow: hidden;'>");
                summaryHtml.AppendLine($"                        <div style='background-color: #4caf50; width: {percentage:F1}%; height: 100%; text-align: center; color: white; font-size: 12px; line-height: 20px;'>{percentage:F0}%</div>");
                summaryHtml.AppendLine("                    </div>");
                summaryHtml.AppendLine("                </td>");
                summaryHtml.AppendLine("            </tr>");
            }

            summaryHtml.AppendLine("        </tbody>");
            summaryHtml.AppendLine("    </table>");
            summaryHtml.AppendLine("</div>");
            return summaryHtml.ToString();
        }

        private static void InjectIntoLivingDoc(string path, string summaryHtml)
        {
            string content = File.ReadAllText(path);
            if (content.Contains("</body>"))
            {
                content = content.Replace("</body>", summaryHtml + "</body>");
                File.WriteAllText(path, content);
                Log.Information("Worker summary injected into LivingDoc.html");
            }
            else
            {
                File.AppendAllText(path, summaryHtml);
                Log.Information("Worker summary appended to LivingDoc.html");
            }
        }

        private static void GenerateStandaloneReport(string summaryHtml)
        {
            string summaryPath = Path.Combine(ResultsPath, "WorkerSummary.html");
            string fullHtml = $"<html><head><title>Worker Summary</title></head><body style='padding: 20px;'>{summaryHtml}</body></html>";
            File.WriteAllText(summaryPath, fullHtml);
            Log.Information("Standalone WorkerSummary.html generated at: {SummaryPath}", summaryPath);
        }
    }
}
