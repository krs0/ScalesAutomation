using Microsoft.Office.Interop.Excel;
using System;
using Excel = Microsoft.Office.Interop.Excel;
//using System.Windows.Forms;

class Program
{
    static void Main(string[] args)
    {
        if(args.Length < 2)
        {
            Console.WriteLine("Please provide the filepath for centralizator masuratori and the desired outputFilename to be interrogated as arguments.");
            return;
        }

        string centralizatorFilePath = args[0];
        string measurementsFileName = args[1];
        Console.WriteLine(measurementsFileName);

        if(!File.Exists(centralizatorFilePath))
        {
            Console.WriteLine("Centralizator File does not exist.");
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

        excelApp.EnableEvents = true; // disable events

        worksheet.Range["MeasurementsFileName"].Value2 = measurementsFileName;

        excelApp.EnableEvents = false; // re-enable events

        string? value = worksheet.Range["MeasurementsFileName"].Text.ToString();
        string? value2 = worksheet.Range["A2"].Text.ToString();

        Console.WriteLine("Written value in MeasurementsFileName: " + value);
        Console.WriteLine("Lot Status: " + value2);

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