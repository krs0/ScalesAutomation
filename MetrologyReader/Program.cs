using log4net;
using log4net.Appender;
using System;
using System.IO;
using System.Reflection;

namespace MetrologyReaderNS
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger("generalLog");
        private static readonly ILog logFile = LogManager.GetLogger("measurementLog");

        static void Main(string[] args)
        {
            log4net.Util.LogLog.InternalDebugging = true;

            string centralizatorFilePath, measurementsFileName;

            log.Info($"Starting Metrology Reader...{Environment.NewLine}");

            try
            {
                ParseArgs(args, out centralizatorFilePath, out measurementsFileName);

                MetrologyReader metrologyReader = new MetrologyReader();

                metrologyReader.InitializeExcel(centralizatorFilePath);

                metrologyReader.GetMetrologyResult(measurementsFileName);

                metrologyReader.Dispose();
            }
            catch(Exception ex)
            {
                log.Error($"Error: {ex.Message}");
            }

            log.Info($"Finished Metrology Reader!{Environment.NewLine}");
            logFile.ChangeLoggingFile("./Logs/b.log");
            logFile.Info($"log in B");
            log.Info($"Log in rolling");
        }

        private static void ParseArgs(string[] args, out string centralizatorFilePath, out string measurementsFileName)
        {
            if(args.Length < 2)
            {
                log.Error($"Please provide the filepath for centralizator masuratori and the desired measurements file name to be interrogated as arguments.");
                throw new Exception();
            }

            centralizatorFilePath = args[0];
            measurementsFileName = args[1];
            log.Info($"The following arguments were provided:{Environment.NewLine}" +
                $"\tCentralizator Masuratori File Path '{centralizatorFilePath}'{Environment.NewLine}" +
                $"\tMeasurements File Name '{measurementsFileName}'{Environment.NewLine}");

            if(!File.Exists(centralizatorFilePath))
            {
                log.Error($"Centralizator File does not exist!");
                throw new Exception();
            }
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