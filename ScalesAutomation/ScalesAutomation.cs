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

        bool stopPressed;

        readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        System.Windows.Forms.Timer timer;
        DataTable dataTable = new DataTable();

        MySerialReader readPort;
        MySerialWriter writePort;
        Thread writeThread;

        bool simulationEnabled;
        CsvHelper csvHelper;

        double zeroThreshold;
        double measurementTollerance;
        double netWeight;

        LotInfo lotInfo;
        NextLotData nextLotData = null;

        public ScalesAutomation()
        {
            InitializeComponent();

            simulationEnabled = Settings.Default.SimulationEnabled;

            Measurements = new SynchronizedCollection<Measurement>();

            CreateTimer();

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

                // If no start detected insert one zero measurement artificially
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
                while (Measurements.Count > 1) // Start supposed at [0]
                {
                    measurementStartPosition = 0;

                    // Look for the position of next "0" measurement (measurementEndPosition),
                    // ignoring all leading "0" by increasing measurementStartPosition
                    for (int i = 1; i < Measurements.Count; i++)
                    {
                        // Ignore all duplicate leading "0"
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

            if (!uctlLotData.CheckInputControls()) return;

            lotInfo = uctlLotData.LotInfo;
            lotInfo.Date = DateTime.Now.ToString("yyyy-MM-dd");

            // Calculate Tollerance
            netWeight = lotInfo.Package.NetWeight * 1000;
            measurementTollerance = (netWeight * Settings.Default.MeasurementTollerace) / 100;
            zeroThreshold = (netWeight * Settings.Default.ZeroThreshold) / 100;

            btnPause.Enabled = false;

            // for each LOT save logs in separate files. (If a log file was already created for a lot reuse it)
            var logFilePath = "";
            var logFolderPath = Misc.AssemblyPath + Settings.Default.LogFolderPath;
            if (lotInfo.AppendToLot && CsvHelper.LogAlreadyPresent(lotInfo.Lot, logFolderPath, ref logFilePath))
            {
                Misc.ChangeLoggingFile(log, logFilePath);

                log.Info("Button Start Clicked" + Environment.NewLine);
                log.Info("### Lot " + lotInfo.Lot + " Resumed on " + lotInfo.Date + " ###");
            }
            else
            {
                logFilePath = Settings.Default.LogFolderPath + DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss") + "_" + lotInfo.Lot + ".log";
                Misc.ChangeLoggingFile(log, logFilePath);

                log.Info("Button Start Clicked" + Environment.NewLine);
                log.Info("### Lot Info ###");
                log.Info("Lot: " + lotInfo.Lot);
                log.Info("Product Name: " + lotInfo.ProductName);
                log.Info("Package: " + lotInfo.Package.Type);
                log.Info("Net Weight: " + netWeight);
                log.Info("Tare: " + lotInfo.Package.Tare * 1000);
                log.Info("Zero Threshold: " + zeroThreshold + Environment.NewLine);
                log.Info("Date: " + lotInfo.Date);
            }

            var CSVOutputFolderPath = Path.Combine(Misc.AssemblyPath, Settings.Default.CSVOutputPath);
            csvHelper = new CsvHelper();
            csvHelper.PrepareFile(CSVOutputFolderPath, lotInfo);

            readPort?.Dispose();
            readPort = new MySerialReader(Measurements, zeroThreshold);

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

            InitializeInputControls();
            uctlLotData.EnableInputControls();

            csvHelper.BackupCurrentCsv();
            if (csvHelper.IsServerFolderReachable())
                csvHelper.CopyCurrentCsvToServer(Settings.Default.CSVServerFolderPath);

        }

        private void btnShowNextLotData_Click(object sender, EventArgs e)
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

        private void NextLotDataClosed()
        {
            nextLotData = null;
        }

        private void NextLotDataApplyClicked()
        {
            if (uctlLotData.InputControlsEnabled())
            {
                uctlLotData.SetLotInfo(nextLotData.GetLotInfo());
            }
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

                log.Info("Measurements Added: " + row["#"] + " - Weight: " + row["Weight"] + " - at: " + row["TimeStamp"]);
            }
        }

        void InitializeInputControls()
        {
            dataTable.Rows.Clear();
            dataGridViewMeasurements.Refresh();
            lotInfo = new LotInfo();
            uctlLotData.InitializeInputControls();
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

        void ReadThread()
        {
            readPort = new MySerialReader(Measurements, 5000);
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

        public bool IsStopPressed()
        {
            return stopPressed;
        }
    }
}