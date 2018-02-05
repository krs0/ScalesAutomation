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
        XmlHelper XmlHandler = new XmlHelper();

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

        Product ProductDetails { get; set; }
        PackageDetails PackageDetails { get; set; }
        String Product { get; set; }
        String Lot { get; set; }
        String NominalWeight { get; set; }
        String PackageType { get; set; }
        String PackageTare { get; set; }

        #endregion

        public ScalesAutomation()
        {
            InitializeComponent();

            XmlHandler.Read(@"c:\Home\Krs\Work\Cantar\ScalesAutomation\ScalesAutomation\bin\Debug\CatalogProduse.xml");
            InitializeGuiFromXml();

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

        private void InitializeGuiFromXml()
        {
            // TODO: There should be no spaces because it will be used to make file name
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
            dataGridViewMeasurements.DataSource = dataTable;

            dataGridViewMeasurements.Columns["#"].Width = 50;
            dataGridViewMeasurements.Columns["Weight"].Width = 100;

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
                    dataTable.Rows.Add(row);

                    // Add row to excel
                    csvHelper.WriteLine(row, dataTable.Columns.Count);

                    log.Debug("Measurements Added: " + row["#"] + " - Weight: " + row["Weight"]);
                }
            }

            if (dataGridViewMeasurements.RowCount > 0)
                dataGridViewMeasurements.FirstDisplayedScrollingRowIndex = dataGridViewMeasurements.RowCount - 1;

            dataGridViewMeasurements.Refresh();
            Measurements.Clear();

        }

        void btnStart_Click(object sender, EventArgs e)
        {
            btnPause.Enabled = false;

            log.Debug(System.Environment.NewLine);
            log.Debug("Button OK Clicked" + Environment.NewLine);

            simulationEnabled = chkEnableSimulation.Checked;

            // TODO: Use here a network drive provided in a config file
            var filePath = "D:\\";
            var productInfo = Lot + "_" + Product + "_" + PackageDetails.Type;
            productInfo = productInfo.Replace(" ", "");

            csvHelper = new CsvHelper();
            csvHelper.PrepareFile(dataTable, filePath, productInfo);

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

            btnPause.Enabled = true;
            btnStart.Enabled = false;

        }

        void btnPause_Click(object sender, EventArgs e)
        {
            log.Debug(System.Environment.NewLine + "Button Stop Clicked" + Environment.NewLine);

            readPort.Dispose();

            if (simulationEnabled)
            {
                writePort?.Dispose();
                writeThread?.Abort();
            }

            btnPause.Enabled = false;
            Thread.Sleep(100);
            btnStart.Enabled = true;

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

        private void cbPackage_SelectedIndexChanged(object sender, EventArgs e)
        {
            // TODO: Separate type and netweight by _
            PackageType = cbPackage.Text;

            PackageDetails = ProductDetails.PackageDetails.Find(x => x.Type == PackageType);

            txtPackageTare.Text = PackageDetails.Tare;
            txtNominalWeight.Text = PackageDetails.NetWeight;
        }

        private void cbProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            Product = cbProduct.Text;
            ProductDetails = XmlHandler.Catalogue.Find(x => x.Name == Product);

            cbPackage.Items.Clear();
            foreach (var package in ProductDetails.PackageDetails)
            {
                cbPackage.Items.Add(package.Type);
            }

            cbPackage.SelectedIndex = -1;
            txtPackageTare.Text = "";
            txtNominalWeight.Text = "";
        }

        #endregion

        #endregion


    }
}