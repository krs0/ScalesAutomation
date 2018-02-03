using System;
using System.Windows.Forms;
using log4net;
using System.Reflection;
using System.Threading;
using System.Collections.Generic;
using System.Data;

namespace ScalesAutomation
{
    public struct Measurement
    {
        public bool isStable;
        public int weight;
    }

    public partial class ScalesAutomation : Form
    {
        public SynchronizedCollection<Measurement> Measurements;

        System.Windows.Forms.Timer timer;
        DataTable dataTable = new DataTable();
        readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        MySerialReader readPort;
        MySerialWriter writePort;
        Thread writeThread;
        Thread readThread;

        bool simulationEnabled;
        CsvHelper csvHelper;

        #region Properties

        String Product { get; set; }
        String Lot { get; set; }
        String NominalWeight { get; set; }
        String Package { get; set; }
        String PackageTare { get; set; }

        #endregion

        public ScalesAutomation()
        {
            InitializeComponent();

            InitializeComponentCustom();

            var xmlHandler = new XmlHelper();
            xmlHandler.Read(@"c:\Home\Krs\Work\Cantar\ScalesAutomation\ScalesAutomation\bin\Debug\CatalogProduse.xml");

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

        private void InitializeComponentCustom()
        {
            // TODO: There should be no spaces because it will be used to make file name
            cbPackage.Items.AddRange(new object[] {
                "CutieCarton10Kg",
                "GaleataPlastic10Kg",
                "GaleataPlastic5Kg"});
        }

        void ReadThread()
        {
            readPort = new MySerialReader(Measurements);
        }

        void WriteThread()
        {
            writePort = new MySerialWriter();
        }

        #region "Events"

        void ScalesAutomation_Load(object sender, EventArgs e)
        {
            btnPause.Enabled = false;
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

        void btnStart_Click(object sender, EventArgs e)
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

            btnPause.Enabled = true;
            btnStart.Enabled = false;

        }

        void btnPause_Click(object sender, EventArgs e)
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
            btnPause.Enabled = false;
        }

        void chkEnableSimulation_CheckedChanged(object sender, EventArgs e)
        {
            simulationEnabled = chkEnableSimulation.Checked;
        }

        #region "Events For Input Controls"

        void txtLot_Validated(object sender, EventArgs e)
        {
            Lot = txtLot.Text;
        }

        void txtNominalWeight_Validated(object sender, EventArgs e)
        {
            NominalWeight = txtNominalWeight.Text;
        }

        void txtPackageTare_Validated(object sender, EventArgs e)
        {
            PackageTare = txtPackageTare.Text;
        }

        #endregion

        #endregion

        private void cbPackage_SelectedIndexChanged(object sender, EventArgs e)
        {
            Package = cbPackage.Text;
            PackageTare = "10g";
            NominalWeight = "20Kg";

            txtPackageTare.Text = PackageTare;
            txtNominalWeight.Text = NominalWeight;
            
        }

        private void cbProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbPackage.SelectedIndex = -1;
            txtPackageTare.Text = "";
            txtNominalWeight.Text = "";
        }
    }
}