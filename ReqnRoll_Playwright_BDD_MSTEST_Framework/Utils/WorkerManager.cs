using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Serilog;

namespace ReqnRoll_Playwright_BDD_MSTEST_Framework.Utils
{
    public static class WorkerManager
    {
        private static int _isWorkerCountLogged = 0;
        private static string _totalWorkersDetected = "1";
        private static ConcurrentStack<int> _workerIdPool;
        private static readonly AsyncLocal<int> _assignedWorkerId = new AsyncLocal<int>();
        private static readonly ConcurrentDictionary<int, int> _workerTestCounts = new ConcurrentDictionary<int, int>();

        private static int _workerCount = 1;

        public static string TotalWorkersDetected => _totalWorkersDetected;
        public static int WorkerCount => _workerCount;
        public static int AssignedWorkerId => _assignedWorkerId.Value;
        public static IReadOnlyDictionary<int, int> WorkerTestCounts => _workerTestCounts;

        public static void InitializeWorkerPool()
        {
            string workers = DetectWorkerCount();
            _totalWorkersDetected = workers;

            if (int.TryParse(workers, out int workerCount) && workerCount > 0)
            {
                _workerCount = workerCount;
                var ids = new List<int>();
                for (int i = 1; i <= workerCount; i++) ids.Add(i);
                ids.Reverse(); // So Pop gives 1, 2, 3...
                _workerIdPool = new ConcurrentStack<int>(ids);
            }
            else
            {
                _workerCount = 1;
                _workerIdPool = new ConcurrentStack<int>(new[] { 1 });
            }

            Console.WriteLine($"Number of Workers Employed = {workers}");
            Console.WriteLine("-------------------------------------------");
            Log.Information("Number of Workers Employed = {Workers}", workers);
        }

        private static string DetectWorkerCount()
        {
            string workers = "1";
            try
            {
                // 1. Check Environment Variable
                workers = Environment.GetEnvironmentVariable("TEST_WORKERS");

                if (string.IsNullOrEmpty(workers) || workers == "Unknown")
                {
                    // 2. Check for MSTest.Parallelize.Workers=X in Command Line
                    string cmdLine = Environment.CommandLine;
                    var match = System.Text.RegularExpressions.Regex.Match(cmdLine, @"MSTest\.Parallelize\.Workers=(\d+)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        workers = match.Groups[1].Value;
                    }
                    else
                    {
                        // 3. Check for Workers=X (alternative pattern)
                        match = System.Text.RegularExpressions.Regex.Match(cmdLine, @"Workers[:=](\d+)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                        if (match.Success)
                        {
                            workers = match.Groups[1].Value;
                        }
                        else
                        {
                            // 4. Fallback: Read from .runsettings
                            string settingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", ".runsettings");
                            if (File.Exists(settingsPath))
                            {
                                var doc = new System.Xml.XmlDocument();
                                doc.Load(settingsPath);
                                var node = doc.SelectSingleNode("//Parallelize/Workers");
                                if (node != null) workers = node.InnerText;
                            }
                        }
                    }
                }
            }
            catch { /* Ignore errors */ }

            return (string.IsNullOrEmpty(workers) || workers == "Unknown") ? "1" : workers;
        }

        public static void LeaseWorkerId()
        {
            if (_workerIdPool != null && _workerIdPool.TryPop(out int id))
            {
                _assignedWorkerId.Value = id;
            }
            else
            {
                _assignedWorkerId.Value = 1;
            }

            string workerIdStr = $"Worker-{_assignedWorkerId.Value}";
            ExceptionTranslator.WorkerId.Value = workerIdStr;
        }

        public static void ReleaseWorkerId()
        {
            if (_assignedWorkerId.Value != 0 && _workerIdPool != null)
            {
                _workerIdPool.Push(_assignedWorkerId.Value);
                _assignedWorkerId.Value = 0;
            }
        }

        public static void IncrementTestCount()
        {
            int id = _assignedWorkerId.Value;
            if (id != 0)
            {
                _workerTestCounts.AddOrUpdate(id, 1, (key, oldValue) => oldValue + 1);
            }
        }

        public static bool ShouldLogWorkerCount()
        {
            return Interlocked.CompareExchange(ref _isWorkerCountLogged, 1, 0) == 0;
        }
    }
}
