using System;
using System.Windows.Forms;
using log4net;
using System.Reflection;
using System.Threading;
using System.Collections.Generic;
using System.Data;
using ScalesAutomation.Properties;
using System.IO;

namespace ScalesAutomation
{
    public partial class ScalesAutomation : Form
    {
        public SynchronizedCollection<Measurement> Measurements;

        readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        XmlHelper XmlHandler = new XmlHelper();

        System.Windows.Forms.Timer timer;
        DataTable dataTable = new DataTable();

        MySerialReader readPort;
        MySerialWriter writePort;
        Thread writeThread;
        Thread readThread;

        bool simulationEnabled;
        CsvHelper csvHelper;

        #region Properties

        LotInfo LotInfo;
        Product ProductDefinition { get; set; }
        Package PackageDefinition { get; set; }

        #endregion

        public ScalesAutomation()
        {
            InitializeComponent();

            XmlHandler.ReadCatalogue(Path.Combine(AssemblyPath, @Settings.Default.CatalogFilePath));
            InitializeGuiFromXml();

            Measurements = new SynchronizedCollection<Measurement>();
            csvHelper = new CsvHelper();

            timer = new System.Windows.Forms.Timer
            {
                Interval = 500,
                Enabled = true
            };
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        public static string AssemblyPath
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        private void InitializeGuiFromXml()
        {
            foreach (var product in XmlHandler.Catalogue)
            {
                cbProduct.Items.Add(product.Name);

                // Keep for reference. Adds an array to a cb.
                //cbPackage.Items.AddRange(new object[] {"CutieCarton10Kg", "GaleataPlastic5Kg"});
            }
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
            dataTable.Columns.Add("#", typeof(int));
            dataTable.Columns.Add("Weight", typeof(string));
            dataTable.Columns.Add("TimeStamp", typeof(string));
            dataGridViewMeasurements.DataSource = dataTable;

            dataGridViewMeasurements.Columns["#"].Width = 50;
            dataGridViewMeasurements.Columns["Weight"].Width = 100;
            dataGridViewMeasurements.Columns["TimeStamp"].Width = 100;

            // Set Columns not sortable
            foreach (DataGridViewColumn column in dataGridViewMeasurements.Columns)
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
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
                    row["TimeStamp"] = DateTime.Now.ToString("HHmmss");
                    dataTable.Rows.Add(row);

                    // Add row to excel
                    csvHelper.WriteLine(row, dataTable.Columns.Count);

                    log.Debug("Measurements Added: " + row["#"] + " - Weight: " + row["Weight"] + " - at: " + row["TimeStamp"]);
                }
            }

            if (dataGridViewMeasurements.RowCount > 0)
                dataGridViewMeasurements.FirstDisplayedScrollingRowIndex = dataGridViewMeasurements.RowCount - 1;

            dataGridViewMeasurements.Refresh();
            Measurements.Clear();

        }

        void btnStart_Click(object sender, EventArgs e)
        {
            if (!CheckInputControls()) return;

            btnPause.Enabled = false;

            log.Debug(Environment.NewLine + "Button Start Clicked" + Environment.NewLine);

            simulationEnabled = chkEnableSimulation.Checked;

            var filePath = Path.Combine(AssemblyPath, Settings.Default.CSVOutputPath);
            csvHelper = new CsvHelper();
            csvHelper.PrepareFile(filePath, LotInfo, dataTable);

            readPort?.Dispose();
            readPort = new MySerialReader(Measurements);

            // Code for Start Reading in a new Thread
            //readThread?.Abort();
            //readThread = new Thread(new ThreadStart(ReadThread));
            //readThread.Start();

            Thread.Sleep(100);

            if (simulationEnabled)
            {
                writePort?.Dispose();
                writeThread?.Abort();
                writeThread = new Thread(new ThreadStart(WriteThread));
                writeThread.Start();
            }

            btnStart.Enabled = false;
            btnPause.Enabled = true;
            btnStopLot.Enabled = true;

            DisableInputControls();

        }

        void btnPause_Click(object sender, EventArgs e)
        {
            log.Debug(Environment.NewLine + "Button Pause Clicked" + Environment.NewLine);

            btnPause.Enabled = false;
            btnStart.Enabled = true;
            btnStopLot.Enabled = true;

            CloseSerialCommunication();

        }
    
        private void btnStopLot_Click(object sender, EventArgs e)
        {
            log.Debug(Environment.NewLine + "Button Stop Clicked" + Environment.NewLine);

            btnStart.Enabled = true;
            btnPause.Enabled = false;
            btnStopLot.Enabled = false;

            CloseSerialCommunication();

            LotInfo = new LotInfo();
            InitializeInputControls();
            EnableInputControls();

              csvHelper.BackupCurrentCsv();
            // test if folder exists
            if (csvHelper.IsServerFolderReachable())
            {
                csvHelper.CopyCurrentCsvToServer(Settings.Default.CSVServerFolderPath);
            }
        }

        void chkEnableSimulation_CheckedChanged(object sender, EventArgs e)
        {
            simulationEnabled = chkEnableSimulation.Checked;
        }

        #region "Events For Input Controls"

        void txtLot_Validated(object sender, EventArgs e)
        {
            LotInfo = new LotInfo
            {
                Lot = txtLot.Text
            };
        }

        private void cbProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbProduct.SelectedIndex == -1) return;

            LotInfo.ProductName = cbProduct.Text;
            ProductDefinition = XmlHandler.Catalogue.Find(x => x.Name == LotInfo.ProductName);

            cbPackage.Items.Clear();
            foreach (var package in ProductDefinition.PackageDetails)
            {
                cbPackage.Items.Add(package.Type);
            }

            cbPackage.SelectedIndex = -1;
            txtPackageTare.Text = "";
            txtNominalWeight.Text = "";
        }

        private void cbPackage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbPackage.SelectedIndex == -1) return;

            // TODO: Separate type and netweight by _
            LotInfo.Package.Type = cbPackage.Text;

            PackageDefinition = ProductDefinition.PackageDetails.Find(x => x.Type == LotInfo.Package.Type);
            LotInfo.Package.Tare = PackageDefinition.Tare;
            LotInfo.Package.NetWeight = PackageDefinition.NetWeight;

            txtPackageTare.Text = LotInfo.Package.Tare;
            txtNominalWeight.Text = LotInfo.Package.NetWeight;
        }

        void txtNominalWeight_Validated(object sender, EventArgs e)
        {
            LotInfo.Package.NetWeight = txtNominalWeight.Text;
        }

        void txtPackageTare_Validated(object sender, EventArgs e)
        {
            LotInfo.Package.Tare = txtPackageTare.Text;
        }

        #endregion

        #endregion

        #region Methods

        private bool CheckInputControls()
        {
            bool inputsAreValid = true;

            if ((txtLot.Text == "") || (cbProduct.SelectedIndex == -1) || (cbPackage.SelectedIndex == -1) || (txtPackageTare.Text == "") || (txtNominalWeight.Text == ""))
            {
                log.Debug("Invalid Lot configuration detected!" + Environment.NewLine);
                MessageBox.Show("Invalid Lot configuration detected. Please make sure all fields are filled and that they have the rigt values",
                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                inputsAreValid = false;
            }

            return inputsAreValid;
        }

        void EnableInputControls()
        {
            txtLot.Enabled = true;
            cbProduct.Enabled = true;
            cbPackage.Enabled = true;
            txtNominalWeight.Enabled = false;
            txtPackageTare.Enabled = false;
        }

        void DisableInputControls()
        {
            txtLot.Enabled = false;
            cbProduct.Enabled = false;
            cbPackage.Enabled = false;
            txtNominalWeight.Enabled = false;
            txtPackageTare.Enabled = false;
        }

        void InitializeInputControls()
        {
            txtLot.Text = "";
            cbProduct.SelectedIndex = -1;
            cbPackage.SelectedIndex = -1;
            txtNominalWeight.Text = "";
            txtPackageTare.Text = "";
        }

        void CloseSerialCommunication()
        {
            readPort.Dispose();

            if (simulationEnabled)
            {
                writePort?.Dispose();
                writeThread?.Abort();
            }
        }

        #endregion  
    }
}