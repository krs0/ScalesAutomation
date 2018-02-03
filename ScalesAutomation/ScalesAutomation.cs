using System;
using System.Windows.Forms;
using log4net;
using System.Reflection;
using System.Threading;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace ScalesAutomation
{
    // TODO: get this list from a config file
    enum PackageType
    {
        CutieCarton10Kg, GaleataPlastic10Kg, GaleataPlastic5Kg
    }

    public struct Measurement
    {
        public bool isStable;
        public int weight;
    }

    public partial class ScalesAutomation : Form
    {
        readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public SynchronizedCollection<Measurement> Measurements;

        MySerialReader readPort;
        MySerialWriter writePort;
        Thread writeThread;
        Thread readThread;

        String Product { get; set; }
        String Lot { get; set; }
        String NominalWeight { get; set; }
        PackageType Package { get; set; }
        String PackageTare { get; set; }

        bool simulationEnabled;
        readonly DataTable dataTable = new DataTable();

        CsvHelper csvHelper;
        readonly System.Windows.Forms.Timer timer;

        public ScalesAutomation()
        {
            InitializeComponent();

            cbPackage.DataSource = Enum.GetValues(typeof(PackageType));
            Measurements = new SynchronizedCollection<Measurement>();
            csvHelper = new CsvHelper();

            timer = new System.Windows.Forms.Timer
            {
                Interval = (1000) * (2),
                Enabled = true
            };
            timer.Tick += new EventHandler(timer_Tick);
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
                    csvHelper.WriteLine(row, dataTable.Columns.Count);

                    log.Debug("Measurements Added: " + row["#"] + " - Weight: " + row["Weight"]);
                }
            }

            dataGridViewMeasurements.Refresh();
            Measurements.Clear();

        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            btnPause.Enabled = false;

            log.Debug(System.Environment.NewLine);
            log.Debug("Button OK Clicked" + Environment.NewLine);

            simulationEnabled = chkEnableSimulation.Checked;

            dataTable.Columns.Add("#", typeof(int));
            dataTable.Columns.Add("Weight", typeof(string));
            dataGridViewMeasurements.DataSource = dataTable;

            // TODO: Use here a network drive provided in a config file
            var filePath = "D:\\";
            var productInfo = Product + "_" + Lot + "_" + NominalWeight + "_" + Package + "_" + PackageTare;

            csvHelper = new CsvHelper();
            csvHelper.PrepareFile(dataTable, filePath, productInfo);

            readPort?.Dispose();
            readThread?.Abort();
            readThread = new Thread(new ThreadStart(ReadThread));
            readThread.Start();
            
            Thread.Sleep(100);

            if (simulationEnabled)
            {
                writePort?.Dispose();
                writeThread?.Abort();
                writeThread = new Thread(new ThreadStart(WriteThread));
                writeThread.Start();
            }

            // btnPause.Enabled = true;
            btnStart.Enabled = false;

        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            log.Debug(System.Environment.NewLine);
            log.Debug("Button Stop Clicked" + Environment.NewLine);

            readPort.Dispose();

            if (simulationEnabled)
            {
                writePort?.Dispose();
                writeThread?.Abort();
            }

            // btnStart.Enabled = true;
            btnPause.Enabled = true;
        }

        private void ReadThread()
        {
            readPort = new MySerialReader(Measurements);
        }

        private void WriteThread()
        {
            writePort = new MySerialWriter();
        }

        #region "Events"

        private void chkEnableSimulation_CheckedChanged(object sender, EventArgs e)
            {
                simulationEnabled = chkEnableSimulation.Checked;
            }

            private void txtProduct_Validated(object sender, EventArgs e)
            {
                Product = txtProduct.Text;
            }

            private void txtLot_Validated(object sender, EventArgs e)
            {
                Lot = txtLot.Text;
            }

            private void txtNominalWeight_Validated(object sender, EventArgs e)
            {
                NominalWeight = txtNominalWeight.Text;
            }

            private void txtPackageTare_Validated(object sender, EventArgs e)
            {
                PackageTare = txtPackageTare.Text;
            }

        #endregion

        private void ScalesAutomation_Load(object sender, EventArgs e)
        {
            btnPause.Enabled = false;
        }
    }
}