using log4net;
using System.Reflection;
using Excel = Microsoft.Office.Interop.Excel;

namespace MetrologyReader
{
    public class Metrology
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        Excel.Application excelApp;
        Excel.Workbook workbook;
        Excel.Sheets worksheets;
        Excel.Worksheet worksheet;

        public void GetMetrologyResult(string measurementsFileName)
        {
            excelApp.EnableEvents = true; // we need to manually enable here because by default Excel events are disabled

            // set the measurements file name in a special cell that is monitored for changed even within Excel (this event will select this file in files dropdown and run all macros).
            worksheet.Range["MeasurementsFileName"].Value2 = measurementsFileName;

            excelApp.EnableEvents = false; // disable Excel events again

            log.Debug($"Written value in MeasurementsFileName Range: {worksheet.Range["MeasurementsFileName"].Text.ToString()}");

            string? measurementsOverallStatus = worksheet.Range["A2"].Text.ToString();
            log.Info($"Measurement overall status: {measurementsOverallStatus}");

            // write in console so that calling process can catch this result
            Console.WriteLine(measurementsOverallStatus);
        }

        public void InitializeExcel(string centralizatorFilePath)
        {
            // Open Excel in non-visible mode
            excelApp = new Excel.Application
            {
                Visible = false,
                DisplayAlerts = false
            };
            workbook = excelApp.Workbooks.Open(centralizatorFilePath);
            worksheets = (Excel.Sheets)workbook.Worksheets;
            worksheet = (Excel.Worksheet)workbook.Worksheets["Metrologie"];
        }

        public void CloseExcel()
        {
            // Close the workbook and Excel application
            workbook.Close(true);
            excelApp.Quit();

            // Release the Excel objects
            System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheets);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
        }

    }
}
