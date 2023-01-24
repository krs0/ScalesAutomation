using log4net;
using System.Reflection;
using Excel = Microsoft.Office.Interop.Excel;

class MetrologyReader
{
    readonly static ILog log = LogManager.GetLogger(typeof(MetrologyReader));

    static void Main(string[] args)
    {
        if(args.Length < 2)
        {
            log.Error($"Please provide the filepath for centralizator masuratori and the desired measurements file name to be interrogated as arguments.");
            return;
        }

        string centralizatorFilePath = args[0];
        string measurementsFileName = args[1];
        log.Info($"Metrology Reader Started with the following arguments:{System.Environment.NewLine}" +
            $"Centralizator Masuratori File Path = {centralizatorFilePath}{System.Environment.NewLine}" +
            $"Measurements File Name = {measurementsFileName}{System.Environment.NewLine}");

        if(!File.Exists(centralizatorFilePath))
        {
            log.Error($"Centralizator File does not exist!");
            return;
        }

        // Open Excel in non-visible mode
        Excel.Application excelApp = new Excel.Application
        {
            Visible = false,
            DisplayAlerts = false
        };

        Excel.Workbook workbook = excelApp.Workbooks.Open(centralizatorFilePath);
        Excel.Sheets worksheets = (Excel.Sheets)workbook.Worksheets;
        Excel.Worksheet worksheet = (Excel.Worksheet)workbook.Worksheets["Metrologie"];

        excelApp.EnableEvents = true; // we need to manually enable here because by default Excel events are disabled

        // set the measurements file name in a special cell that is monitored for changed even within Excel (this event will select this file in files dropdown and run all macros).
        worksheet.Range["MeasurementsFileName"].Value2 = measurementsFileName;

        excelApp.EnableEvents = false; // disable Excel events again

        log.Debug($"Written value in MeasurementsFileName Range: {worksheet.Range["MeasurementsFileName"].Text.ToString()}");

        string? measurementsOverallStatus = worksheet.Range["A2"].Text.ToString();
        log.Info($"Measurement overall status: {measurementsOverallStatus}");

        // write in console so that calling process can catch this result
        Console.WriteLine(measurementsOverallStatus);

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