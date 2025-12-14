using log4net;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;

namespace MetrologyReaderNS;

public class MetrologyReader : IDisposable
{
    private static readonly ILog log = LogManager.GetLogger("generalLog");

    private bool disposed = false; // flag to indicate whether the resource has already been disposed 

    // When true, Dispose/Close will not quit Excel � used to "detach" the workbook so user can inspect it.
    private bool leaveOpen = false;

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

        log.Debug($"Written value in MeasurementsFileName Range: '{rangeFileName.Text.ToString()}'");

        rangeStatus = worksheet.Range["A2"];
        string measurementsOverallStatus = rangeStatus.Text.ToString();
        log.Info($"Measurement overall status: {measurementsOverallStatus}!");

        // write in console so that calling process can catch this result
        Console.WriteLine(measurementsOverallStatus);

        return measurementsOverallStatus;
    }

    public void InitializeExcel(string centralizatorFilePath)
    {
        try
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
        catch (System.IO.FileNotFoundException ex)
        {
            throw new InvalidOperationException("Aplicatia Microsoft Office/Excel nu este instalata.", ex);
        }
        catch (System.Runtime.InteropServices.COMException ex)
        {
            throw new InvalidOperationException("Erroare la initializarea Excel. Aplicatia Microsoft Office/Excel nu este instalata sau este configurata incorect.", ex);
        }
    }

    /// <summary>
    /// Make the currently opened workbook visible and prevent this object from closing Excel on Dispose.
    /// Call this when you want the user to see / edit the workbook after calculations.
    /// </summary>
    public void DetachAndMakeVisible()
    {
        leaveOpen = true;
        if (excelApp != null)
            excelApp.Visible = true;
    }

    public void CloseExcel()
    {
        // Check this for info https://www.add-in-express.com/creating-addins-blog/2013/11/05/release-excel-com-objects/

        // Release the Excel objects
        if(rangeFileName != null) Marshal.ReleaseComObject(rangeFileName);
        if(rangeStatus != null) Marshal.ReleaseComObject(rangeStatus);

        if(worksheet != null) Marshal.ReleaseComObject(worksheet);
        if(worksheets != null) Marshal.ReleaseComObject(worksheets);
        excelApp.DisplayAlerts = false;
        workbook.Save();
        workbook.Close(true);
        excelApp.DisplayAlerts = true;
        if(workbook != null) Marshal.ReleaseComObject(workbook);
        if(workbooks != null) Marshal.ReleaseComObject(workbooks);
        excelApp.Quit();
        if(excelApp != null) Marshal.ReleaseComObject(excelApp);
    }

    ~MetrologyReader()
    {
        this.Dispose(false);
    }

    public virtual void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if(!this.disposed)
        {
            if(disposing)
            {
                // If caller requested to leave Excel open, do not call CloseExcel().
                if(!leaveOpen)
                {
                    CloseExcel();

                    // WARNING: Wait for ALL pending finalizers
                    // COM objects in other STA threads will require those threads to process messages in a timely manner.
                    // However, this is the only way to be sure GCed RCWs actually invoked the COM object's Release.
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
                else
                {
                    // We're detaching: do not release/quit Excel here. The external Excel process will remain.
                    // Avoid releasing COM objects that are still in use by the running Excel instance.
                }
            }
            this.disposed = true;
        }
    }
}
