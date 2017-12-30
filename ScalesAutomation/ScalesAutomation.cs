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
        private MySerialReader readPort;
        private MySerialWriter writePort;
        private Thread writeThread;
        private Thread readThread;
        private String code;

        public ScalesAutomation()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            log.Debug(System.Environment.NewLine);
            log.Debug("Button OK Clicked" + Environment.NewLine);

            if (readPort != null)
                readPort.Dispose();

            if (writePort != null)
                writePort.Dispose();

            if (writeThread != null)
                writeThread.Abort();

            if (readThread != null)
                readThread.Abort();

            writeThread = new Thread(new ThreadStart(WriteThread));
            readThread = new Thread(new ThreadStart(ReadThread));

            readThread.Start();

            Thread.Sleep(100);
            writeThread.Start();
        }

        private void ReadThread()
        {
            readPort = new MySerialReader();
        }

        private void WriteThread()
        {
            writePort = new MySerialWriter();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            log.Debug(System.Environment.NewLine);
            log.Debug("Button Stop Clicked" + Environment.NewLine);

            readPort.Dispose();
            writePort.Dispose();
        }

        private void txtCode_Validated(object sender, EventArgs e)
        {
            code = txtCode.Text;
        }
    }
}