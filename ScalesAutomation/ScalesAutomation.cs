using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using log4net;
using Microsoft.VisualBasic;
using ScalesAutomation.Properties;
using Timer = System.Windows.Forms.Timer;

namespace ScalesAutomation
{
    public partial class ScalesAutomation : Form
    {
        public SynchronizedCollection<Measurement> Measurements;

        public static volatile bool stopPressed = false; // will also be used to stop the Write Thread in simulation mode

        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private Timer timer;
        private readonly DataTable dataTable = new DataTable();

        private MySerialReader readPort;
        private MySerialWriter writePort;
        private Thread writeThread;

        private readonly bool simulationEnabled;
        private CsvHelper csvHelper;
        private readonly string logFolderPath = Path.Combine(Misc.AssemblyPath, Settings.Default.LogFolderPath);
        private string logFilePath = "";

        private int zeroThreshold;
        private int measurementTolerance;
        private int netWeight;

        private LotInfo lotInfo;
        private NextLotData nextLotData;

        public ScalesAutomation()
        {
            InitializeComponent();

            simulationEnabled = Settings.Default.SimulationEnabled;

            Measurements = new SynchronizedCollection<Measurement>();

            CreateTimer();
        }

        public bool IsStopPressed()
        {
            return stopPressed;
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

                if (stopPressed)
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
            stopPressed = false;
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

                log.ChangeLoggingFile(logFilePath);
                lotInfo.AppendToLot = true;

                log.Info("Button Start Clicked" + Environment.NewLine);
                log.Info("### Lot " + lotInfo.Id + " Resumed on " + lotInfo.Date + " ###");
            }
            else
            {
                logFilePath = logFolderPath + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "_" + lotInfo.Id + ".log";
                log.ChangeLoggingFile(logFilePath);
                lotInfo.AppendToLot = false;

                log.Info("Button Start Clicked" + Environment.NewLine);
                LogLotInfo(lotInfo);
            }

            var CSVOutputFolderPath = Path.Combine(Misc.AssemblyPath, Settings.Default.CSVOutputPath);
            csvHelper = new CsvHelper();
            csvHelper.PrepareOutputFile(CSVOutputFolderPath, lotInfo);

            readPort?.Dispose();
            readPort = new MySerialReader(Measurements, zeroThreshold, lotInfo.Package.Tare, lotInfo.TareIsSet);
            if(!readPort.Initialize())
            {
                MessageBox.Show($"Nu a putut fi creata o legatura cu cantarul.{Environment.NewLine}" +
                    $"Verificati setarea ReadCOMport din ScalesAutomation.dll.config, si restartati aplicatia.", "Initializare Esuata", MessageBoxButtons.OK);
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

            var lotId = uctlLotData.LotInfo.Id; // save Lot ID for later (display in result message box)

            InitializeInputControls();
            uctlLotData.EnableInputControls();

            // parse logs only for Bilanciai, for Constalaris we log exactly what's in the GUI
            if(!(Settings.Default.ScaleType == "Constalaris"))
                LogHelper.ParseLog(logFilePath);

            csvHelper.BackupOutputFile(Settings.Default.CSVBackupPath);
            csvHelper.CopyOutputFileToServer(Settings.Default.CSVServerFolderPath);

            var result = LogHelper.GetMetrologyResults(CsvHelper.OutputFileFullName);

            // display dialog with results
            if(result == "Lot Acceptat")
                MessageBox.Show($"Lot Acceptat.{Environment.NewLine}{Environment.NewLine}{lotId}", "Rezultat Metrologie", MessageBoxButtons.OK, MessageBoxIcon.None);
            else
                MessageBox.Show($"Lot Neacceptat!{Environment.NewLine}{Environment.NewLine}{lotId}", "Rezultat Metrologie", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            log.Info("### Lot Info ###");
            log.Info("Lot: " + lotInfo.Lot);
            log.Info("Product Name: " + lotInfo.ProductName);
            log.Info("Package: " + lotInfo.Package.Type);
            log.Info("Net Weight: " + lotInfo.Package.NetWeight);
            log.Info("Tare: " + lotInfo.Package.Tare);
            log.Info("Zero Threshold: " + zeroThreshold);
            log.Info("Date: " + lotInfo.Date + Environment.NewLine);
        }

        private void AddValidMeasurementToDataTableAndExcel(List<Measurement> validMeasurements)
        {
            var nrOfRowsInDataTable = dataTable.Rows.Count;
            for (int i = nrOfRowsInDataTable, j = 0; i < nrOfRowsInDataTable + validMeasurements.Count; i++, j++)
            {
                var row = dataTable.NewRow();
                row["#"] = i+1;
                row["Weight"] = validMeasurements[j].TotalWeight;
                row["TimeStamp"] = DateTime.Now.ToString("HH:mm:ss");
                dataTable.Rows.Add(row);

                // Add row to excel: - do not write in output file when using parser (Bilanciai), because it will be overwritten by parser anyway
                if(Settings.Default.ScaleType == "Constalaris")
                    csvHelper.WriteLineToOutputFile(row, dataTable.Columns.Count);

                log.Info("Measurement Detected: " + row["#"] + " - Weight: " + row["Weight"] + " - at: " + row["TimeStamp"]);
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