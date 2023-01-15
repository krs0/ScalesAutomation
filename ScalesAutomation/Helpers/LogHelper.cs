using System.Diagnostics;
using log4net;
using System.Reflection;
using System;

namespace ScalesAutomation
{
    public static class LogHelper
    {
        static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>Launch the log parsing application with some options set.</summary>
        public static void ParseLog(string logFilePath)
        {
            var startInfo = new ProcessStartInfo
            {
                CreateNoWindow = false,
                UseShellExecute = false,
                FileName = "LogParser.exe",
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = "\"" + logFilePath + "\" \"" + CsvHelper.OutputFolderPath + "\\\"" // add an extra \ at the end, not to escape last " when it arrives in parser
            };

            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using-statement will close.
                log.Info("Starting parser with arguments: " + startInfo.Arguments);
                using(var parserProcess = Process.Start(startInfo))
                {
                    parserProcess?.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                log.Info($"Cannot start log parser process: {ex.Message}");
            }
        }

    }
}