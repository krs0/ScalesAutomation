using System;
using System.Threading;
using System.Windows.Forms;

namespace ScalesAutomation
{
    static class Program
    {
        static Mutex mutex = new Mutex(true, "{8F6F0AC4-B9A1-45fd-A8CF-72F04E6BDE8F}");

        [STAThread]
        static void Main()
        {
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new ScalesAutomation());
                mutex.ReleaseMutex();
            }
            else
            {
                MessageBox.Show("Doar o singura instanta a aplicatiei poate rula la un moment dat!");
            }
        }
    }
}
