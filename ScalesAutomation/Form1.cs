using System;
using System.Windows.Forms;
using System.IO.Ports;
using log4net;
using System.Reflection;
using System.Threading;

namespace ScalesAutomation
{
    public partial class ScalesAutomation : Form
    {
        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ScalesAutomation()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            log.Debug(System.Environment.NewLine);
            log.Debug("Button OK Clicked" + Environment.NewLine);

            Thread writeThread = new Thread(new ThreadStart(WriteThread));
            Thread readThread = new Thread(new ThreadStart(ReadThread));
            readThread.Start();

            Thread.Sleep(100);
            writeThread = new Thread(new ThreadStart(WriteThread));
            writeThread.Start();

        }

        private void ReadThread()
        {
            var port = new MySerialReader();
        }

        private void WriteThread()
        {
            var port = new MySerialWriter();
        }

    }
}