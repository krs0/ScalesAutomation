using System;
using System.Windows.Forms;
using log4net;
using System.Reflection;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;

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
        private CsvHelper csvHelper;

        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        
        public struct measurement
        {
            public bool isStable;
            public int weight;
        }

        public SynchronizedCollection<ScalesAutomation.measurement> Measurements = new SynchronizedCollection<ScalesAutomation.measurement>();

        public DataTable dataTable = new DataTable();

        // private BindingList<measurement> bindingList;
        // private BindingSource source;

        public ScalesAutomation()
        {
            InitializeComponent();

            dataTable.Columns.Add("#", typeof(int));
            dataTable.Columns.Add("Weight", typeof(string));

            // bindingList = new BindingList<measurement>(Measurements);
            // source = new BindingSource(bindingList, null);
            // dataGridViewMain.DataSource = bindingList;
            // dataGridViewMain.DataBind();
            dataGridViewMain.DataSource = dataTable;

            csvHelper = new CsvHelper("D:\\a.csv");
            csvHelper.CreateCsvFile(dataTable);

            timer.Tick += new EventHandler(timer_Tick); // Everytime timer ticks, timer_Tick will be called
            timer.Interval = (1000) * (2);              // Timer will tick evert 5 second
            timer.Enabled = true;                       // Enable the timer
            timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {

            if (Measurements.Count > 0)
            {
                var nrOfRowsInDataTable = dataTable.Rows.Count;
                for (int i = nrOfRowsInDataTable, j = 0; i < nrOfRowsInDataTable + Measurements.Count; i++, j++)
                {
                    var row = dataTable.NewRow();
                    row["#"] = i;
                    row["Weight"] = Measurements[j].weight;
                    dataTable.Rows.Add(row);

                    // Add row to excel
                    csvHelper.WriteOneMeasurementToCsv(row, dataTable.Columns.Count);

                    log.Debug("Measurements Added: " + row["#"] + " - Weight: " + row["Weight"]);
                }
            }

            // bindingList = new BindingList<measurement>(Measurements);
            //dataGridViewMain.DataSource = bindingList;
            dataGridViewMain.Refresh();
            Measurements.Clear();
            //dataGridViewMain.DataSource = dataTable;
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
            readPort = new MySerialReader(Measurements);
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