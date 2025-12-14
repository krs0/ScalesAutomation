using log4net;
using ScalesAutomation.Properties;

namespace ScalesAutomation;

static class Program
{
    private static readonly ILog log = LogManager.GetLogger("generalLog");
    static Mutex mutex = new Mutex(true, "{8F6F0AC4-B9A1-45fd-A8CF-72F04E6BDE8F}");

    [STAThread]
    static void Main()
    {
        try
        {
            log.Info($"Starting Scales Automation...");

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
                log.Info($"Error: Application already open!");
                MessageBox.Show("Doar o singura instanta a aplicatiei poate rula la un moment dat!");
            }
        }
        catch(Exception ex)
        {
            log.Error($"Error: {ex.Message}");
        }
        finally
        {
            log.Info($"Finished Scales Automation!");
        }
    }
}
