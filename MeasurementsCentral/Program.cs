using log4net;
using log4net.Appender;
using System;
using System.Reflection;
using System.Windows.Forms;

namespace MeasurementsCentral
{
    internal static class Program
    {
        private static readonly ILog log = LogManager.GetLogger("generalLog");
        private static readonly ILog logFile = LogManager.GetLogger("measurementLog");

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            log4net.Util.LogLog.InternalDebugging = true;

            log.Info($"Starting Measurements Central...{Environment.NewLine}");

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new MeasurementsCentral());

            log.Info($"log in Rolling");
            logFile.Info($"log in A");
            logFile.ChangeLoggingFile("./Logs/b.log");
            logFile.Info($"log in B");
            log.Info($"Finished Measurements Central!{Environment.NewLine}");

        }
    }
    public static class UtilityForLogging
    {
        public static void ChangeLoggingFile(this ILog localLog, string logFileName)
        {
            var rootRepository = log4net.LogManager.GetRepository();
            foreach(var appender in rootRepository.GetAppenders())
            {
                if(appender.Name.Equals("LogToFile") && appender is FileAppender)
                {
                    var fileAppender = appender as FileAppender;
                    fileAppender.File = logFileName;
                    fileAppender.ActivateOptions();
                    break;  // Appender found and name changed to NewFilename
                }
            }

            localLog.Info($"Measurements logging will be done to: '{logFileName}'");
        }
    }
}