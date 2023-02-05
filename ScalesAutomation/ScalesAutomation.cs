using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using CommonNS;
using log4net;
using ScalesAutomation.Properties;
using Timer = System.Windows.Forms.Timer;

namespace ScalesAutomation
{
    public partial class ScalesAutomationForm : Form
    {
        public SynchronizedCollection<Measurement> Measurements;

        public static volatile bool stopTransmission = false; // will also be used to stop the Write Thread in simulation mode

        private static readonly ILog log = LogManager.GetLogger("generalLog");
        private static readonly ILog logM = LogManager.GetLogger("measurementLog");

        private Timer timer;
        private readonly DataTable dataTable = new DataTable();

        private MySerialReader readPort;
        private MySerialWriter writePort;
        private Thread writeThread;

        private readonly bool simulationEnabled;
        private CsvHelper csvHelper;
        private readonly string csvServerFolderPath = Common.TransformToAbsolutePath(Settings.Default.CSVServerFolderPath);
        private readonly string logFolderPath = Common.TransformToAbsolutePath(Settings.Default.LogFolderPath);
        private string logFilePath = "";

        private int zeroThreshold;
        private int measurementTolerance;
        private int netWeight;

        private LotInfo lotInfo;
        private NextLotData nextLotData;

        public ScalesAutomationForm()
        {
            InitializeComponent();

            simulationEnabled = Settings.Default.SimulationEnabled;

            Measurements = new SynchronizedCollection<Measurement>();

            CreateTimer();
        }

        #region "Events"

        void ScalesAutomation_Load(object sender, EventArgs e)
        {
            var imageColumn = new DataGridViewImageColumn();
            imageColumn.Image = Image.FromFile("Images/ok.png");
            imageColumn.HeaderText = "";
            imageColumn.Name = "Status";
            dataGridViewMeasurements.Columns.Add(imageColumn);

            dataTable.Columns.Add("#", typeof(int));
            dataTable.Columns.Add("Weight", typeof(string));
            dataTable.Columns.Add("TimeStamp", typeof(string));
            dataGridViewMeasurements.DataSource = dataTable;

            dataGridViewMeasurements.Columns["Status"].Width = 32;
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

            if (stopTransmission) return; // Do not process any more measurements

            if (Measurements.Count > 1)
            {
                // Keep only one measurement between 2 consecutive "0"s
                // - A valid measurement will be the last Stable measurement before a Stable "0"
                // - Glitches should be filtered out in code above this function  

                // If no start from stable 0 detected insert one zero measurement artificially
                if (!(Measurements[0].TotalWeight == 0))
                {
                    var item = new Measurement
                    {
                        TotalWeight = 0,
                        IsStable = true
                    };
                    Measurements.Insert(0, item);
                }

                // Identify Start and end of measurement positions. (Measurements[] contains only "stable" measurements )
                while (Measurements.Count > 1) // Start always present at [0]
                {
                    measurementStartPosition = 0;

                    // Look for the position of next "0" measurement (measurementEndPosition),
                    // ignoring all leading "0" by increasing measurementStartPosition
                    for (int i = 1; i < Measurements.Count; i++)
                    {
                        // Ignore all duplicate leading "0"
                        if ((i == measurementStartPosition + 1) && (Measurements[i].TotalWeight == 0))
                            measurementStartPosition = i;
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
                        if (measurement.TotalWeight > (netWeight - measurementTolerance) &&
                            measurement.TotalWeight < (netWeight + measurementTolerance))
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

                if (stopTransmission)
                    return; // Do not process any more measurements. Stop could be pressed in middle of loop

                // Add all valid measurements to DataTable and excel
                if (validMeasurements.Count > 0)
                    AddValidMeasurementToDataTableAndExcel(validMeasurements);
            }

            if (dataGridViewMeasurements.RowCount > 0)
                dataGridViewMeasurements.FirstDisplayedScrollingRowIndex = dataGridViewMeasurements.RowCount - 1;

            dataGridViewMeasurements.Refresh();
               
            // }));
        }

        #region Button Events

        void btnStart_Click(object sender, EventArgs e)
        {
            log.Info($"Button Start Clicked");

            stopTransmission = false;
            btnPause.Enabled = false;

            if (!uctlLotData.AreInputControlsValid()) return;

            lotInfo = uctlLotData.LotInfo;
            lotInfo.Date = DateTime.Now.ToString("yyyy-MM-dd");

            netWeight = lotInfo.Package.NetWeight;
            measurementTolerance = (int)(netWeight * Settings.Default.MeasurementTollerace) / 100;
            zeroThreshold = (int)(netWeight * Settings.Default.ZeroThreshold) / 100;

            // for each LOT save logs in separate files. (If a log file was already created for a lot reuse it)
            if (PathHelper.GetLogFilePath(lotInfo.Id, logFolderPath, ref logFilePath))
            {
                DialogResult result = MessageBox.Show("Pentru lotul selectat exista deja masuratori. Noile masuratori se vor adauga celor existente. Doriti sa Continuati?", "Continuare Lot", MessageBoxButtons.YesNo);
                if (result != DialogResult.Yes)
                    return;

                logM.ChangeLoggingFile(logFilePath);
                lotInfo.AppendToLot = true;

                logM.Info($"### Lot {lotInfo.Id} resumed on {lotInfo.Date} ###{Environment.NewLine}");
            }
            else
            {
                logFilePath = logFolderPath + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "_" + lotInfo.Id + ".log";
                logM.ChangeLoggingFile(logFilePath);
                lotInfo.AppendToLot = false;

                LogLotInfo(lotInfo);
            }

            log.Info($"Measurements logging will be done to: '{logFilePath}'");

            var CSVOutputFolderPath = Path.Combine(Common.AssemblyPath, Settings.Default.CSVOutputPath);
            csvHelper = new CsvHelper();
            csvHelper.PrepareOutputFile(CSVOutputFolderPath, lotInfo);

            readPort?.Dispose();
            readPort = new MySerialReader(Measurements, zeroThreshold, lotInfo.Package.Tare, lotInfo.TareIsSet);
            if(!readPort.Initialize())
            {
                MessageBox.Show($"Nu a putut fi creata o legatura cu cantarul.{Environment.NewLine}" +
                    $"Verificati setarea ReadCOMport din ScalesAutomationForm.dll.config, si restartati aplicatia.", "Initializare Esuata", MessageBoxButtons.OK);
                return;
            }

            Thread.Sleep(100);

            if (simulationEnabled)
            {
                writeThread = new Thread(WriteThread);
                writeThread.Start();
            }

            btnStart.Enabled = false;
            btnPause.Enabled = true;
            btnStopLot.Enabled = true;

            uctlLotData.DisableInputControls();
        }

        void btnPause_Click(object sender, EventArgs e)
        {
            log.Info("Button Pause Clicked");

            btnPause.Enabled = false;
            btnStart.Enabled = true;
            btnStopLot.Enabled = true;

            stopTransmission = true;

            CloseSerialCommunication();
        }

        void btnStopLot_Click(object sender, EventArgs e)
        {
            log.Info("Button Stop Clicked");

            btnStart.Enabled = true;
            btnPause.Enabled = false;
            btnStopLot.Enabled = false;

            stopTransmission = true;

            CloseSerialCommunication();

            Thread.Sleep(500);

            // parse logs only for Bilanciai, for Constalaris we log exactly what's in the GUI
            if(!(Settings.Default.ScaleType == "Constalaris"))
                StartLogParser.ParseLog(logFilePath, CsvHelper.OutputFolderPath);

            csvHelper.BackupOutputFile(Settings.Default.CSVBackupPath);
            csvHelper.CopyOutputFileToServer(csvServerFolderPath);

            var metrologyResult = StartMetrologyReader.GetMetrologyResults(CsvHelper.OutputFileFullName, csvServerFolderPath);

            // display dialog with results
            var lotId = uctlLotData.LotInfo.Id;
            if(metrologyResult == "Lot Acceptat")
                MessageBox.Show($"Lot Acceptat.{Environment.NewLine}{Environment.NewLine}{lotId}", "Rezultat Metrologie", MessageBoxButtons.OK, MessageBoxIcon.None);
            else
                MessageBox.Show($"Lot Respins!{Environment.NewLine}{Environment.NewLine}{lotId}", "Rezultat Metrologie", MessageBoxButtons.OK, MessageBoxIcon.Error);

            InitializeInputControls();
            uctlLotData.EnableInputControls();
        }

        void btnShowNextLotData_Click(object sender, EventArgs e)
        {
            if (nextLotData == null)
            {
                nextLotData = new NextLotData();
                nextLotData.WindowClosed += NextLotDataClosed;
                nextLotData.ApplyClicked += NextLotDataApplyClicked;
                nextLotData.Show();
            }
            else
            {
                nextLotData.BringToFront();
            }
        }

        void NextLotDataClosed()
        {
            nextLotData = null;
        }

        void NextLotDataApplyClicked()
        {
            if (uctlLotData.InputControlsEnabled())
            {
                uctlLotData.SetLotInfo(nextLotData.GetLotInfo());
            }
        }

        #endregion

        #endregion

        #region Private Methods

        private void InitializeInputControls()
        {
            dataTable.Rows.Clear();
            dataGridViewMeasurements.Refresh();
            lotInfo = new LotInfo();
            uctlLotData.InitializeInputControls();
        }

        private void LogLotInfo(LotInfo lotInfo)
        {
            logM.Info("### Lot Info ###");
            logM.Info("Lot: " + lotInfo.Lot);
            logM.Info("Product Name: " + lotInfo.ProductName);
            logM.Info("Package: " + lotInfo.Package.Type);
            logM.Info("Net Weight: " + lotInfo.Package.NetWeight);
            logM.Info("Tare: " + lotInfo.Package.Tare);
            logM.Info("Zero Threshold: " + zeroThreshold);
            logM.Info("Date: " + lotInfo.Date + Environment.NewLine);
        }

        private void AddValidMeasurementToDataTableAndExcel(List<Measurement> validMeasurements)
        {
            var nrOfRowsInDataTable = dataTable.Rows.Count;
            for (int i = nrOfRowsInDataTable, j = 0; i < nrOfRowsInDataTable + validMeasurements.Count; i++, j++)
            {
                var measurement = validMeasurements[j].TotalWeight;

                // Add measurement info to dataTable
                var row = dataTable.NewRow();
                row["#"] = i+1;
                row["Weight"] = measurement;
                row["TimeStamp"] = DateTime.Now.ToString("HH:mm:ss");
                dataTable.Rows.Add(row);

                // Add icon according to Status
                Image image;
                if(measurement > 960) // ToDo: CrLa - Crude aproximation here. Replace with something meaningful.
                    image =  Image.FromFile("Images/ok_24.png");
                else
                    image = Image.FromFile("Images/x_24.png");

                dataGridViewMeasurements.Rows[i].Cells["Status"].Value = image;


                // Add row to excel: - do not write in output file when using parser (Bilanciai), because it will be overwritten by parser anyway
                if(Settings.Default.ScaleType == "Constalaris")
                    csvHelper.WriteLineToOutputFile(row, dataTable.Columns.Count);

                logM.Info("Measurement Detected: " + row["#"] + " - Weight: " + row["Weight"] + " - at: " + row["TimeStamp"]);
            }
        }

        private void CloseSerialCommunication()
        {
            readPort.Dispose();

            if (simulationEnabled)
            {
                try
                {
                    writePort?.Dispose();
                }
                catch(Exception)
                {
                }
            }
        }

        private void WriteThread()
        {
            writePort = new MySerialWriter();
        }

        private void CreateTimer()
        {
            timer = new Timer
            {
                Interval = 500,
                Enabled = true
            };
            timer.Tick += timer_Tick;
            timer.Start();
        }

        #endregion

        private void ScalesAutomation_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(!btnStart.Enabled)
                btnStopLot_Click(sender, e);
        }
    }
}