using log4net;
using System;
using System.Diagnostics;
using System.Reflection;

namespace ScalesAutomation
{
    public static class StartLogParser
    {
        private static readonly ILog log = LogManager.GetLogger("generalLog");

        /// <summary>Launch the log parsing application with some options set.</summary>
        public static void ParseLog(string logFilePath, string outputFolderPath)
        {
            var startInfo = new ProcessStartInfo
            {
                CreateNoWindow = false,
                UseShellExecute = false,
                FileName = "LogParser.exe",
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = "\"" + logFilePath + "\" \"" + outputFolderPath + "\\\"" // add an extra \ at the end, not to escape last " when it arrives in parser
            };

            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using-statement will close.
                log.Info($"Starting parser with arguments: {startInfo.Arguments}");
                using(var parserProcess = Process.Start(startInfo))
                {
                    parserProcess?.WaitForExit();
                }
            }
            catch(Exception ex)
            {
                log.Info($"Cannot start log parser process: {ex.Message}");
            }
        }
    }
}
