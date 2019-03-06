using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LogParser
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application. Args[0] - logFilePath, Args[1] - outputFolderPath
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length == 0)
                Application.Run(new LogParser());
            else
            {
                var logParser = new LogParser();
                
                logParser.Initialize(Path.GetDirectoryName(args[0]), args[1]);
                logParser.ParseLog(args[0]);
            }
        }
    }
}
