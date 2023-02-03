using log4net;
using System;
using System.Reflection;
using System.Windows.Forms;

namespace MeasurementsCentral
{
    internal static class Program
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            log.Info($"Starting Measurements Central...{Environment.NewLine}");

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new MeasurementsCentral());

            log.Info($"Finished Measurements Central!{Environment.NewLine}");
        }
    }
}