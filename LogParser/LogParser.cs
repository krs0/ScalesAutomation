using System;
using System.Collections.Generic;
using log4net;
using System.Reflection;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using LogParser.Properties;
using ScalesAutomation;

namespace LogParser
{
    public partial class LogParser : Form
    {
        readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        // TODO: CrLa - Add code to save to .csv

        class MeasurementInfo
        {
            public int Position;
            public string Time;
            public bool IsStable;
            public double Measurement;

            public MeasurementInfo(int position, string time, bool isStable, double measurement)
            {
                Position = position;
                Time = time;
                IsStable = isStable;
                Measurement = measurement;
            }

            public MeasurementInfo()
            {
                Position = 0;
                Time = "";
                IsStable = false;
                Measurement = 0;
            }

            public override string ToString()
            {
                return Position + ";" + Measurement + ";" + Time;
            }
        }

        LotInfo lotInfo;

        string logFolderPath = Settings.Default.LogFolderPath;
        string outputFolderPath = Settings.Default.LogFolderPath;
        string outputFileName = "";
        string outputFilePath = "";
        string normalizedMeasurementsFilePath = "";
        private int startingMeasurementIndex = 1; // 1 if new file, will be recalculated if output file already exists

        public LogParser()
        {
            InitializeComponent();
            lotInfo = new LotInfo();
        }

        public void Initialize(string logFolderPath, string outputFolderPath)
        {
            this.logFolderPath = logFolderPath;
            this.outputFolderPath = outputFolderPath;
        }

        public void ParseLog(string logFilePath)
        {
            lotInfo = lotInfo.ReadLotInfoFromLog(logFilePath);

            // we rewrite whole logs. no appending
            if (!CsvHelper.OutputAlreadyPresent(lotInfo.GetUniqueLotId(), outputFolderPath, ref outputFilePath))
            {
                var logFileName = Path.GetFileNameWithoutExtension(logFilePath);
                outputFilePath = Path.Combine(outputFolderPath, logFileName + ".csv");
            }
            else
            {
                //startingMeasurementIndex = int.Parse(CsvHelper.GetLastMeasurementIndex(outputFilePath));
            }

            CsvHelper.InitializeOutputFileContents(outputFilePath, lotInfo.MakeMeasurementFileHeader());

            var normalizedMeasurements = ReadAndNormalizeMeasurements(logFilePath, lotInfo.ZeroThreshold);
            RemoveFakeMeasurements(normalizedMeasurements);
            // normalizedMeasurementsFilePath = Path.Combine(outputFolderPath, logFileName + ".out");
            // SaveListToFile(normalizedMeasurements, normalizedMeasurementsFilePath); // Generic list save does not work

            var finalMeasurements = ExtractFinalMeasurements(normalizedMeasurements, lotInfo.Package.NetWeight);
            AddPositionToEachMeasurement(finalMeasurements, startingMeasurementIndex);
            SaveListToFile(finalMeasurements, outputFilePath);

        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            var logFilePaths = GetListOfFiles(logFolderPath, "*.log");

            foreach (var logFilePath in logFilePaths)
            {
                ParseLog(logFilePath);
            }
        }

        private static List<MeasurementInfo> ExtractFinalMeasurements(List<MeasurementInfo> normalizedMeasurements, double netWeight)
        {
            var measurementsDetected = false;
            var stableMeasurementFound = false;
            var finalMeasurements = new List<MeasurementInfo>();

            for (var i = normalizedMeasurements.Count - 1; i >= 0; i--)
            {
                var l = normalizedMeasurements[i];

                if (l.Measurement == 0)
                {
                    if (measurementsDetected && !stableMeasurementFound)
                    {
                        finalMeasurements.Add(new MeasurementInfo(finalMeasurements.Count + 1, "", true, netWeight)); // Invent an appropriate unstable measurement
                    }

                    measurementsDetected = false;
                    stableMeasurementFound = false;
                }
                else
                {
                    if (stableMeasurementFound) continue;

                    if (!measurementsDetected)
                        measurementsDetected = true;

                    // find the "first" aka last 4 stable measurements
                    if (!l.IsStable || i <= 3) continue;

                    // find if stable more than 4
                    if (normalizedMeasurements[i - 1].IsStable && normalizedMeasurements[i - 2].IsStable && normalizedMeasurements[i - 3].IsStable)
                    {
                        stableMeasurementFound = true;
                        finalMeasurements.Add(normalizedMeasurements[i - 1]);
                    }
                }
            }

            finalMeasurements.Reverse();

            return finalMeasurements;
        }

        // find fake intervals (less than 20 measurements > 0)
        private void RemoveFakeMeasurements(List<MeasurementInfo> normalizedMeasurements)
        {
            var consecutives = 0;
            var index = 0;
            var startingIndex = 0; // index where a non zero measurement is detected
            var deletedItems = 0; // because we delete from measurements list, we need to manually keep track of current index.

            var clonedList = new List<MeasurementInfo>(normalizedMeasurements); // use temp so we can change list in-place in foreach
            foreach (var element in clonedList)
            {
                // we ignore everything until we find some positive measurements
                if (element.Measurement > 0)
                {
                    if (consecutives == 0)
                        startingIndex = index; // save starting index of first non zero measurement

                    consecutives++;
                }
                else
                {
                    if (consecutives > 0) // if we detected some consecutive measurements
                    {
                        if (consecutives < 20) // but not a minimum required number, we have a glitch (a few nozero measurements) and we need to delete it
                        {
                            normalizedMeasurements.RemoveRange(startingIndex - deletedItems, consecutives);
                            deletedItems += consecutives;

                            consecutives = 0;
                        }
                        else
                        {
                            if (IsZeroGlitch(normalizedMeasurements, index - deletedItems))
                            {
                                normalizedMeasurements.RemoveRange(index - deletedItems, 1); // remove current Zero Measurement
                                deletedItems++;
                            }
                            else // a real zero region is coming, reset measurement counters
                            {
                                consecutives = 0;
                                startingIndex = 0;
                            }
                        }
                    }
                }

                index++;
            }
        }

        // read all lines containing measurements from the log file,
        // save info in a list of structs
        // and change all measurements below threshold to ZERO
        private static List<MeasurementInfo> ReadAndNormalizeMeasurements(string logFilePath, double zeroThreshold)
        {
            var normalizedMeasurements = new List<MeasurementInfo>();

            using (var logFile = new StreamReader(logFilePath))
            {
                string line;
                while ((line = logFile.ReadLine()) != null)
                {
                    if (!line.Contains(" - W: ")) continue;

                    var splitLine = line.Split(' ');

                    var measurementInfo = new MeasurementInfo();
                    measurementInfo.Time = splitLine[1];
                    measurementInfo.IsStable = (splitLine[5] == "T");

                    int.TryParse(splitLine[8], out var measurement);
                    if (measurement < zeroThreshold)
                        measurement = 0;
                    measurementInfo.Measurement = measurement;

                    normalizedMeasurements.Add(measurementInfo);
                }

                logFile.Close();
            }

            return normalizedMeasurements;
        }

        private bool IsZeroGlitch(List<MeasurementInfo> lines, int startingIndex)
        {
            var isZeroGlitch = false;
            var endIndex = startingIndex + 20;

            if (startingIndex >= lines.Count() - 20)
                endIndex = lines.Count();

            for (var i = startingIndex; i < endIndex; i++)
            {
                if (lines[i].Measurement > 0)
                {
                    isZeroGlitch = true;

                    break;
                }
            }

            return isZeroGlitch;
        }

        private static void AddPositionToEachMeasurement(List<MeasurementInfo> finalMeasurements, int startingIndex)
        {
            for (var i = 0; i < finalMeasurements.Count; i++)
            {
                finalMeasurements[i].Position = startingIndex + i + 1;
            }
        }

        private void SaveListToFile<T>(List<T> list, string filePath)
        {
            using (var file = new StreamWriter(filePath, append: true))
            {
                foreach (var element in list)
                {
                    file.WriteLine(element);
                }

                file.Close();
            }
        }

        private List<string> GetListOfFiles(string folder, string searchPattern)
        {
            return Directory.GetFiles(folder, searchPattern, SearchOption.TopDirectoryOnly).ToList();
        }

        #region Temporary Code for extracting csv headers

        // will replace the .csv file contents with lot info header!!!!!!!!!!!!
        private void btnMakeLotInfoHeader_Click(object sender, EventArgs e)
        {

            var csvFilePaths = GetListOfFiles(Settings.Default.OutputFolderPath, "*.csv");

            foreach (var csvFilePath in csvFilePaths)
            {
                MakeLotInfoHeader(csvFilePath);
            }
        }

        private void MakeLotInfoHeader(string csvFilePath)
        {
            var lotInfoHeader = ReadLotInfoFromFirstLine(csvFilePath);
            SaveListToFile(lotInfoHeader, csvFilePath);
        }

        private List<string> ReadLotInfoFromFirstLine(string csvFilePath)
        {
            var lotInfoHeader = new List<string>();

            using (var csvFile = new StreamReader(csvFilePath))
            {
                string line;
                line = csvFile.ReadLine();
                var splittedLine = line.Split(';');

                lotInfoHeader.Add(splittedLine[8] + " INFO  ### Lot Info ###");
                lotInfoHeader.Add(splittedLine[8] + " INFO  Lot:  " + splittedLine[3]);
                lotInfoHeader.Add(splittedLine[8] + " INFO  Product Name:  " + splittedLine[4]);
                lotInfoHeader.Add(splittedLine[8] + " INFO  Package:  " + splittedLine[5]);
                lotInfoHeader.Add(splittedLine[8] + " INFO  Net Weight:  " + splittedLine[6]);
                lotInfoHeader.Add(splittedLine[8] + " INFO  Tare:  " + splittedLine[7]);
                lotInfoHeader.Add(splittedLine[8] + " INFO  Zero Threshold:  " + "Kg");
                lotInfoHeader.Add(splittedLine[8] + " INFO  Date:  " + splittedLine[8]);

            }

            return lotInfoHeader;
        }

        #endregion

        #region "Temporary Code for batch renaming output files with lot id's"

        private void btnRenameOutput_Click(object sender, EventArgs e)
        {
            var outputFilePaths = GetListOfFiles(logFolderPath, "*.csv");

            foreach (var outputFilePath in outputFilePaths)
            {
                RenameOutputFile(outputFilePath);
            }
        }

        private void RenameOutputFile(string outputFilePath)
        {
            var fileHeader = ReadLotInfoFromFirstLine(outputFilePath);
            var productName = fileHeader[2].Split(new[] { "Product Name:  " }, StringSplitOptions.None)[1];
            var package = fileHeader[3].Split(new[] { "Package:  " }, StringSplitOptions.None)[1];

            var outputFileName = Path.GetFileNameWithoutExtension(outputFilePath);
            var newOutputFileName = outputFileName + "_" + productName + "_" + package;
            var newOutputFilePath = outputFilePath.Replace(outputFileName, newOutputFileName);

            File.Move(outputFilePath, newOutputFilePath);
        }

        #endregion

    }
}
