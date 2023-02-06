using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LogParser
{
    static class Program
    {
        private static readonly ILog log = LogManager.GetLogger("generalLog");

        /// <summary>
        /// The main entry point for the application. Args[0] - logFilePath, Args[1] - outputFolderPath
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                log.Info($"Starting Log Parser...");

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                if(args.Length == 0)
                    Application.Run(new LogParser());
                else
                {
                    var logParser = new LogParser();

                    logParser.Initialize(Path.GetDirectoryName(args[0]), Path.GetDirectoryName(args[1]));
                    logParser.ParseLog(args[0]);
                }
            }
            catch(Exception ex)
            {
                log.Error($"Error: {ex.Message}");
            }
            finally
            {
                log.Info($"Finished Log Parser");
            } 
            
        }
    }
}
