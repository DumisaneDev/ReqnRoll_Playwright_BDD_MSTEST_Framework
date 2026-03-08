using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ReqnRoll_Playwright_BDD_MSTEST_Framework.Utils
{
    public class ConfigReader
    {
        private static IConfiguration _config;

        static ConfigReader() 
        {
            _config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("Test Data/testdata.json", optional: false, reloadOnChange: true)
                .Build();
        }

        public static string getValue(string key) => _config[$"TestSetting:{key}"];
    }
}
