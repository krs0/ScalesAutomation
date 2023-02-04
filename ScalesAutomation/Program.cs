using System;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using log4net;
using Microsoft.VisualBasic.Logging;
using ScalesAutomation.Properties;

namespace ScalesAutomation
{
    static class Program
    {
        private static readonly ILog log = LogManager.GetLogger("generalLog");
        static Mutex mutex = new Mutex(true, "{8F6F0AC4-B9A1-45fd-A8CF-72F04E6BDE8F}");

        [STAThread]
        static void Main()
        {
            log4net.Util.LogLog.InternalDebugging = true;

            log.Info($"Starting Scales Automation...");
            //var savedLogFileName = log.GetLoggingFile();

            if(mutex.WaitOne(TimeSpan.Zero, true))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                if(Settings.Default.DataImporterEnabled)
                    Application.Run(new DataImporter());
                else
                    Application.Run(new ScalesAutomationForm());

                mutex.ReleaseMutex();
            }
            else
            {
                MessageBox.Show("Doar o singura instanta a aplicatiei poate rula la un moment dat!");
            }

            //log.ChangeLoggingFile(savedLogFileName);
            log.Info($"Finished Scales Automation!");

        }
    }
}
