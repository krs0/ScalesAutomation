using System;
using Excel = Microsoft.Office.Interop.Excel;

class Program
{
    static void Main(string[] args)
    {
        if(args.Length == 0)
        {
            Console.WriteLine("Please provide a filepath as an argument.");
            return;
        }

        string measurementsFilePath = args[0];
        if(!File.Exists(measurementsFilePath))
        {
            Console.WriteLine("The file does not exist.");
            return;
        }

        // Open Excel in non-visible mode
        Excel.Application excelApp = new Excel.Application
        {
            Visible = false
        };

        Excel.Workbook workbook = excelApp.Workbooks.Open(measurementsFilePath);

        Excel.Worksheet worksheet = (Excel.Worksheet)workbook.Worksheets["Metrologie"];

        string? value = worksheet.Range["A2"].Text.ToString();

        Console.WriteLine("Value of cell A2 in worksheet 'Metrologie': " + value);

        // Close the workbook and Excel application
        workbook.Close(false);
        excelApp.Quit();

        // Release the Excel objects
        System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);
        System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
        System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
    }
}