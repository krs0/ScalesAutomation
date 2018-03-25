using System;
using System.Windows.Forms;
using log4net;
using System.Reflection;
using System.Threading;
using System.Collections.Generic;
using System.Data;
using ScalesAutomation.Properties;
using System.IO;
using System.Text.RegularExpressions;

namespace ScalesAutomation
{
    public partial class ScalesAutomation : Form
    {
        public SynchronizedCollection<Measurement> Measurements;

        bool stopPressed;

        readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        XmlHelper XmlHandler = new XmlHelper();

        System.Windows.Forms.Timer timer;
        DataTable dataTable = new DataTable();

        MySerialReader readPort;
        MySerialWriter writePort;
        Thread writeThread;

        bool simulationEnabled;
        CsvHelper csvHelper;

        double measurementTollerance;
        double netWeight;

        #region Properties

        LotInfo LotInfo;
        Product ProductDefinition { get; set; }
        Package PackageDefinition { get; set; }

        #endregion

        public ScalesAutomation()
        {
            InitializeComponent();

            simulationEnabled = Settings.Default.SimulationEnabled;

            XmlHandler.ReadCatalogue(Path.Combine(AssemblyPath, @Settings.Default.CatalogFilePath));
            InitializeGuiFromXml();

            Measurements = new SynchronizedCollection<Measurement>();

            CreateTimer();

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
            //this.BeginInvoke((MethodInvoker)(delegate (){

            List<Measurement> validMeasurements = new List<Measurement>();
            int measurementStartPosition;
            int measurementEndPosition = 0;
            bool endOfMeasurementFound = false;

            if (stopPressed) return; // Do not process any more measurements

            if (Measurements.Count > 1)
            {
                // Keep only one measurement between 2 consecutive "0"s
                // - A valid measurement will be the last Stable measurement before a Stable "0"
                // - Glitches should be filtered out in code above this function  

                // If no start detected insert one artificially
                if (!(Measurements[0].TotalWeight == 0))
                {
                    var item = new Measurement
                    {
                        TotalWeight = 0,
                        IsStable = true
                    };
                    Measurements.Insert(0, item);
                }

                // Identify Start and end of measurement positions
                while (Measurements.Count > 1) // Start supposed at [0]
                {
                    measurementStartPosition = 0;

                    // Look for next "0"
                    for (int i = 1; i < Measurements.Count; i++)
                    {
                        // Ignore all duplicate trailing "0"
                        if ((i == measurementStartPosition + 1) && (Measurements[i].TotalWeight == 0))
                        {
                            measurementStartPosition = i;
                        }
                        else
                        {
                            if (Measurements[i].TotalWeight == 0) // if end of measurement found
                            {
                                endOfMeasurementFound = true;
                                measurementEndPosition = i - 1;
                                break;
                            }
                        }
                    }

                    // If next "0" found, save last stable measurement and discard everything else
                    if (endOfMeasurementFound)
                    {
                        var measurement = Measurements[measurementEndPosition];
                        // Sanity Check at the end with expected value
                        if (measurement.TotalWeight > (netWeight - measurementTollerance) &&
                            measurement.TotalWeight < (netWeight + measurementTollerance))
                        {
                            validMeasurements.Add(measurement);
                        }
                        else
                            log.Error("Measurement not within tollerance: " + measurement.TotalWeight);

                        // clear Measuremetns array for the processed measurements
                        for (int i = 0; i <= measurementEndPosition; i++)
                            Measurements.RemoveAt(0);

                        endOfMeasurementFound = false;
                    }
                    else
                        break;
                }

                if (stopPressed)
                    return; // Do not process any more measurements. Stop could be pressed in midde of loop

                // Add all valid measurements to DataTable and excel
                if (validMeasurements.Count > 0)
                    AddToDataTableAndExcel(validMeasurements);
            }

            if (dataGridViewMeasurements.RowCount > 0)
                dataGridViewMeasurements.FirstDisplayedScrollingRowIndex = dataGridViewMeasurements.RowCount - 1;

            dataGridViewMeasurements.Refresh();
               
            // }));
        }

        #region Button Events

        void btnStart_Click(object sender, EventArgs e)
        {
            stopPressed = false;

            if (!CheckInputControls()) return;

            // Calculate Tollerance
            netWeight = LotInfo.Package.NetWeight * 1000;
            measurementTollerance = (netWeight * Settings.Default.MeasurementTollerace) / 100;

            LotInfo.Date = DateTime.Now.ToString("yyyy-MM-dd");

            btnPause.Enabled = false;

            log.Info("Button Start Clicked" + Environment.NewLine);

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
            log.Info("Button Pause Clicked" + Environment.NewLine);

            btnPause.Enabled = false;
            btnStart.Enabled = true;
            btnStopLot.Enabled = true;

            CloseSerialCommunication();

        }

        void btnStopLot_Click(object sender, EventArgs e)
        {
            log.Info("Button Stop Clicked" + Environment.NewLine);

            btnStart.Enabled = true;
            btnPause.Enabled = false;
            btnStopLot.Enabled = false;

            stopPressed = true;

            CloseSerialCommunication();

            Thread.Sleep(500);

            LotInfo = new LotInfo();
            InitializeInputControls();
            EnableInputControls();

            csvHelper.BackupCurrentCsv();
            if (csvHelper.IsServerFolderReachable())
                csvHelper.CopyCurrentCsvToServer(Settings.Default.CSVServerFolderPath);

        }

        #endregion

        #region "Events For Input Controls"

        void txtLot_Validated(object sender, EventArgs e)
        {
            LotInfo = new LotInfo
            {
                Lot = txtLot.Text
            };
        }

        void txtLot_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
                this.SelectNextControl(this.ActiveControl, true, true, true, true);
        }

        void cbProduct_SelectedIndexChanged(object sender, EventArgs e)
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

        void cbPackage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbPackage.SelectedIndex == -1) return;

            // TODO: Separate type and netweight by _
            LotInfo.Package.Type = cbPackage.Text;

            PackageDefinition = ProductDefinition.PackageDetails.Find(x => x.Type == LotInfo.Package.Type);
            LotInfo.Package.Tare = PackageDefinition.Tare;
            LotInfo.Package.NetWeight = PackageDefinition.NetWeight;
            LotInfo.Package.TotalWeight = PackageDefinition.TotalWeight;

            txtPackageTare.Text = LotInfo.Package.Tare.ToString() + "Kg";
            txtNominalWeight.Text = LotInfo.Package.TotalWeight.ToString() + "Kg";
        }

        void txtPackageTare_Validated(object sender, EventArgs e)
        {
            if (txtPackageTare.Text.IndexOf("Kg", StringComparison.InvariantCultureIgnoreCase) == -1)
            {
                MessageBox.Show("Tara trebuie sa fie urmata de unitatea de masura. Ex: 20.5Kg" + Environment.NewLine + "Doar Kg sunt suportate ca unitate de masura!");
                return;
            }

            Double.TryParse(Regex.Replace(txtPackageTare.Text, "Kg", "", RegexOptions.IgnoreCase), out LotInfo.Package.Tare);
            LotInfo.Package.TotalWeight = LotInfo.Package.NetWeight + LotInfo.Package.Tare;
            txtNominalWeight.Text = LotInfo.Package.TotalWeight.ToString() + "Kg";

        }

        void txtPackageTare_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
                this.SelectNextControl(this.ActiveControl, true, true, true, true);
        }

        #endregion

        #endregion

        #region Private Methods

        void AddToDataTableAndExcel(List<Measurement> validMeasurements)
        {
            var nrOfRowsInDataTable = dataTable.Rows.Count;
            for (int i = nrOfRowsInDataTable, j = 0; i < nrOfRowsInDataTable + validMeasurements.Count; i++, j++)
            {
                var row = dataTable.NewRow();
                row["#"] = i+1;
                row["Weight"] = validMeasurements[j].TotalWeight;
                row["TimeStamp"] = DateTime.Now.ToString("HH:mm:ss");
                dataTable.Rows.Add(row);

                // Add row to excel
                csvHelper.WriteLine(row, dataTable.Columns.Count);

                log.Debug("Measurements Added: " + row["#"] + " - Weight: " + row["Weight"] + " - at: " + row["TimeStamp"]);
            }
        }

        bool CheckInputControls()
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
            txtPackageTare.Enabled = true;
            txtNominalWeight.Enabled = false;
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
            dataTable.Rows.Clear();
            dataGridViewMeasurements.Refresh();
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

        void InitializeGuiFromXml()
        {
            foreach (var product in XmlHandler.Catalogue)
            {
                cbProduct.Items.Add(product.Name);
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

        void CreateTimer()
        {
            timer = new System.Windows.Forms.Timer
            {
                Interval = 500,
                Enabled = true
            };
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        #endregion

    }
}