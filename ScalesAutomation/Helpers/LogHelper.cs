using System.Diagnostics;
using log4net;
using System.Reflection;
using System;
using ScalesAutomation.Properties;
using System.IO;

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

        public static void GetMetrologyResults(string logFileName)
        {
            var centralizatorMasuratoriFilePath = CsvHelper.OutputFolderPath + @"..\..\Server\CentralizatorMasuratori.xlsm";

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
                // Start the process with the info we specified.
                // Call WaitForExit and then the using-statement will close.
                log.Info("Starting Metrology Reader with arguments: " + startInfo.Arguments);
                using(var metrologyReaderProcess = Process.Start(startInfo))
                {
                    // Synchronously read the standard output of Metrology Reader process.
                    StreamReader reader = metrologyReaderProcess.StandardOutput;
                    string output = reader.ReadToEnd();

                    // Write the redirected output to this application's window.
                    log.Info(output);

                    metrologyReaderProcess?.WaitForExit();
                }
            }
            catch(Exception ex)
            {
                log.Info($"Cannot start Metrology Reader process: {ex.Message}");
            }
        }

    }
}