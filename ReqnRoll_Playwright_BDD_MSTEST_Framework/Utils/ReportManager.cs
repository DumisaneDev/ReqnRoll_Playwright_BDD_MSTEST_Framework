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
            try
            {
                var summaryHtml = BuildSummaryHtml();
                string summaryPath = Path.Combine(ResultsPath, "worker-summary.html");
                
                // Ensure directory exists
                if (!Directory.Exists(ResultsPath)) Directory.CreateDirectory(ResultsPath);
                
                File.WriteAllText(summaryPath, summaryHtml);
                Log.Information("Worker summary data saved for post-processing at: {SummaryPath}", summaryPath);

                // Still attempt direct injection as a courtesy
                string livingDocPath = Path.Combine(ResultsPath, "LivingDoc.html");
                if (File.Exists(livingDocPath))
                {
                    InjectIntoLivingDoc(livingDocPath, summaryHtml);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to generate worker summary report.");
            }
        }

        private static string BuildSummaryHtml()
        {
            var summaryHtml = new StringBuilder();
            summaryHtml.AppendLine("<!-- WORKER_SUMMARY_START -->");
            summaryHtml.AppendLine("<div id='worker-summary' style='margin: 20px; padding: 20px; border: 1px solid #ddd; border-radius: 8px; font-family: sans-serif; background-color: #f9f9f9; box-shadow: 0 4px 6px rgba(0,0,0,0.1); clear: both;'>");
            summaryHtml.AppendLine("    <h2 style='color: #333; border-bottom: 2px solid #ED7725; padding-bottom: 10px; margin-top: 0;'>Parallel Execution Summary</h2>");
            summaryHtml.AppendLine($"    <p><strong>Total Workers Employed:</strong> <span style='color: #ED7725; font-weight: bold;'>{WorkerManager.TotalWorkersDetected}</span></p>");
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
            summaryHtml.AppendLine("<!-- WORKER_SUMMARY_END -->");
            return summaryHtml.ToString();
        }

        private static void InjectIntoLivingDoc(string path, string summaryHtml)
        {
            try
            {
                string content = File.ReadAllText(path);
                if (content.Contains("WORKER_SUMMARY_START")) return;

                var bodyMatch = System.Text.RegularExpressions.Regex.Match(content, @"<body[^>]*>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                if (bodyMatch.Success)
                {
                    int insertIndex = bodyMatch.Index + bodyMatch.Length;
                    content = content.Insert(insertIndex, "\n" + summaryHtml);
                    File.WriteAllText(path, content);
                    Log.Information("Worker summary injected into LivingDoc.html");
                }
            }
            catch { /* Ignore errors during direct injection */ }
        }
    }
}
