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

        static void Main(string[] args)
        {
            string centralizatorFilePath, measurementsFileName;

            log.Info($"Starting Metrology Reader...");

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

            log.Info($"Finished Metrology Reader!");
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
                $"\tMeasurements File Name '{measurementsFileName}'");

            if(!File.Exists(centralizatorFilePath))
            {
                log.Error($"Centralizator File does not exist!");
                throw new Exception();
            }
        }
    }
}