using System.Diagnostics;
using log4net;
using System.Reflection;
using System;
using System.IO;

namespace ScalesAutomation
{
    public static class LogHelper
    {
        static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

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

        /// <summary>Launch the Metrology Reader application and read its metrologyResult</summary>
        public static string GetMetrologyResults(string logFileName, string serverFolderPath)
        {
            string metrologyResult = "";

            var centralizatorMasuratoriFilePath = serverFolderPath + @"..\CentralizatorMasuratori.xlsm";

            var startInfo = new ProcessStartInfo
            {
                CreateNoWindow = false,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                FileName = "MetrologyReader.exe",
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = "\"" + centralizatorMasuratoriFilePath + "\" \"" + logFileName + "\""
            };

            try
            {
                // Start the process with the info we specified, read from its std metrologyResult and WaitForExit
                log.Info("Starting Metrology Reader with arguments: " + startInfo.Arguments);

                using var metrologyReaderProcess = Process.Start(startInfo);

                // Synchronously read the standard metrologyResult of Metrology Reader process.
                StreamReader reader = metrologyReaderProcess.StandardOutput;
                metrologyResult = reader.ReadToEnd();

                // Write the redirected metrologyResult to this application's window.
                log.Info($"Metrology Result: {metrologyResult}");

                metrologyReaderProcess?.WaitForExit();
            }
            catch(Exception ex)
            {
                log.Info($"Cannot start Metrology Reader process: {ex.Message}");
            }

            return metrologyResult;
        }

    }
}