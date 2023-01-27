using log4net;
using System.Reflection;
using Excel = Microsoft.Office.Interop.Excel;

namespace MetrologyReaderNS
{
    public class MetrologyReader
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        Excel.Application excelApp;
        Excel.Workbooks workbooks;
        Excel.Workbook workbook;
        Excel.Sheets worksheets;
        Excel.Worksheet worksheet;
        Excel.Range rangeFileName, rangeStatus;

        public string GetMetrologyResult(string measurementsFileName)
        {
            excelApp.EnableEvents = true; // we need to manually enable here because by default Excel events are disabled

            // set the measurements file name in a special cell that is monitored for changed even within Excel (this event will select this file in files dropdown and run all macros).
            rangeFileName = worksheet.Range["MeasurementsFileName"];
            rangeFileName.Value2 = measurementsFileName;

            excelApp.EnableEvents = false; // disable Excel events again

            log.Debug($"Written value in MeasurementsFileName Range: {rangeFileName.Text.ToString()}");

            rangeStatus = worksheet.Range["A2"];
            string? measurementsOverallStatus = rangeStatus.Text.ToString();
            log.Info($"Measurement overall status: {measurementsOverallStatus}");

            // write in console so that calling process can catch this result
            Console.WriteLine(measurementsOverallStatus);

            return measurementsOverallStatus;
        }

        public void InitializeExcel(string centralizatorFilePath)
        {
            // Open Excel in non-visible mode
            excelApp = new Excel.Application
            {
                Visible = false,
                DisplayAlerts = false,
                AlertBeforeOverwriting = false
            };
            workbooks = excelApp.Workbooks;
            workbook = workbooks.Open(centralizatorFilePath);
            worksheets = workbook.Worksheets;
            worksheet = (Excel.Worksheet)worksheets["Metrologie"];
        }

        public void CloseExcel()
        {
            // Release the Excel objects
            System.Runtime.InteropServices.Marshal.ReleaseComObject(rangeFileName);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(rangeStatus);

            System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheets);
            workbook.Save();
            workbook.Close(true);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(workbooks);
            excelApp.Quit();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
        }

    }
}
