using log4net;
using System;
using System.Diagnostics;
using System.Reflection;

namespace ScalesAutomation
{
    public static class StartMetrologyReader
    {
        static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>Launch the Metrology Reader application and read its metrologyResult</summary>
        public static string GetMetrologyResults(string logFileName, string serverFolderPath)
        {
            string metrologyResult = "";

            var centralizatorMasuratoriFilePath = serverFolderPath + @"..\CentralizatorMasuratori.xlsm";

            var startInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
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
                metrologyResult = metrologyResult.TrimEnd();

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
