using CommonNS.Properties;
using log4net;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace ScalesAutomation
{
    public static class OpenExcel
    {
        static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>Launch Excel application for the selected xls file.</summary>
        public static void OpenWorkbook(string xlsFilePath)
        {
            var startInfo = new ProcessStartInfo
            {
                CreateNoWindow = false,
                UseShellExecute = false,
                FileName = $"{Settings.Default.ExcelExePath}",
                Arguments = $"\"{xlsFilePath}\""
            };

            try
            {
                log.Info($"Starting excel app with arguments: {xlsFilePath}");
                using var parserProcess = Process.Start(startInfo);

            }
            catch(Exception ex)
            {
                log.Info($"Cannot start Excel app: {ex.Message}");
            }
        }

        /// <summary> Kill no name EXCEL processes. Detect if Centralizator Masuratori Open. </summary>
        public static bool CheckIfExcelIsOpen()
        {
            bool result = false;

            Process[] excelProcesses = Process.GetProcessesByName("EXCEL");
            if(excelProcesses.Length > 0)
            {
                foreach(Process process in excelProcesses)
                {
                    if (process.MainWindowTitle == "")
                        process.Kill(); // root couse of orphan excel processes not found. safe to just kill them when detected.

                    if(process.MainWindowTitle.Contains("CentralizatorMasuratori"))
                    {
                        var dlgResult = MessageBox.Show("Nu se poate continua cu o aplicatie CentralizatorMasuratori.xls deja deschisa. Doriti sa o inchidem automat?",
                            "Aplicatia CentralizatorMasuratori.xls deschisa", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                        if (dlgResult == DialogResult.Yes)
                            process.Kill();
                        else
                            result = true; // we return that its still open
                    }
                }
            }

            return result;
        }
    }
}
