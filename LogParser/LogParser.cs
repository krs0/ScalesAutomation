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
        string outputFilePath = "";
        int startingMeasurementIndex = 0;

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
            RemoveLastMeasurementIfNotInTolerance(finalMeasurements);
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

        /// <summary>
        /// This method goes through normalized measurements from back to front and
        /// detects / decides what were the measurements for each interval between consecutive zeroes.
        /// Zero measurements are usually triggers for reseting variables (if beginning) or
        /// adding a measurement to the final list (if ending a series of measurements).
        /// There are 3 things detected:
        /// Stable measurements within tolerance. Once this is detected we add it to list and noting more is done on that interval.
        /// Stable measurements not within tolerance. First potential candidate: a stable measurement close to scale unloading, but does not fit within tolerance.
        /// Best Guesses: Continously, independent if Stable or not we compare each measurement to the desired net weght. This is used when nothing stable is found.
        /// </summary>
        private static List<MeasurementInfo> ExtractFinalMeasurements(List<MeasurementInfo> normalizedMeasurements, double netWeight)
        {
            var measurementsDetected = false;
            var stableMeasurementFound = false;
            var finalMeasurements = new List<MeasurementInfo>();
            var potentialMeasurementIndex = 0;
            var firstNonZeroIndex = 0;
            MeasurementInfo bestFit = new MeasurementInfo();

            var tolerance = Settings.Default.ConfidenceLevel;

            for (var i = normalizedMeasurements.Count - 1; i >= 0; i--)
            {
                var currentMeasurement = normalizedMeasurements[i];

                if (currentMeasurement.Measurement == 0)
                {
                    if (measurementsDetected && !stableMeasurementFound)
                    {
                        if (potentialMeasurementIndex > 0)
                        {
                            finalMeasurements.Add(normalizedMeasurements[potentialMeasurementIndex]);
                            potentialMeasurementIndex = 0;
                        }
                        else
                        {
                            // mark measurement as not stable by chainging milliseconds time to 42
                            bestFit.Time = bestFit.Time.Split(',')[0] + ",42";
                            finalMeasurements.Add(bestFit);
                        }

                        bestFit = new MeasurementInfo();
                    }

                    measurementsDetected = false;
                    stableMeasurementFound = false;
                }
                else
                {
                    if (stableMeasurementFound) continue;

                    if (IsBetterFit(currentMeasurement, bestFit, netWeight))
                    {
                        bestFit = currentMeasurement;
                    }
                    

                    if (!measurementsDetected)
                    {
                        measurementsDetected = true;
                        firstNonZeroIndex = i;
                    }

                    // find the "first" aka last 4 stable measurements
                    if (!currentMeasurement.IsStable || i <= 4) continue;

                    // find if stable more than 3
                    if (normalizedMeasurements[i - 1].IsStable && normalizedMeasurements[i - 2].IsStable)
                    {
                        if (potentialMeasurementIndex == 0)
                        {
                            if (normalizedMeasurements[i - 3].IsStable && normalizedMeasurements[i - 4].IsStable)
                            {
                                if (firstNonZeroIndex - i < 10)
                                    potentialMeasurementIndex = i - 2 ;
                                else if (firstNonZeroIndex - i < 15)
                                    potentialMeasurementIndex = i - 2;
                                else if (firstNonZeroIndex - i < 20)
                                    potentialMeasurementIndex = i - 2;
                            }
                        }

                        if (IsWithinSkewedTolerance(currentMeasurement.Measurement, netWeight, tolerance))
                        {
                            stableMeasurementFound = true;
                            finalMeasurements.Add(normalizedMeasurements[i - 1]);
                            potentialMeasurementIndex = 0;
                            bestFit = new MeasurementInfo();
                        }
                    }
                }
            }

            finalMeasurements.Reverse();

            return finalMeasurements;
        }

        private static bool IsWithinTolerance(double measuredValue, double nominalValue, int toleranceInPercentage)
        {
            var toleranceInterval = nominalValue - nominalValue * toleranceInPercentage / 100;
            var toleranceHigh = nominalValue + toleranceInterval;
            var toleranceLow = nominalValue - toleranceInterval;

            return ((measuredValue > toleranceLow) && (measuredValue < toleranceHigh));
        }

        private static bool IsWithinSkewedTolerance(double measuredValue, double nominalValue, int toleranceInPercentage)
        {
            var toleranceHigh = nominalValue + (nominalValue - nominalValue * toleranceInPercentage / 100);
            var toleranceLow = nominalValue - (nominalValue - nominalValue * (toleranceInPercentage + 1) / 100); // skewed to catch more lower

            return ((measuredValue > toleranceLow) && (measuredValue < toleranceHigh));
        }

        private static bool IsBetterFit(MeasurementInfo currentMeasurement, MeasurementInfo bestFit, double netWeight)
        {
            if (Math.Abs(currentMeasurement.Measurement - netWeight) < Math.Abs(bestFit.Measurement - netWeight))
            {
                return true;
            }

            return false;
        }

        private void RemoveLastMeasurementIfNotInTolerance(List<MeasurementInfo> finalMeasurements)
        {
            if (!IsWithinSkewedTolerance(finalMeasurements.Last().Measurement, lotInfo.Package.NetWeight, Settings.Default.ConfidenceLevel))
            {
                finalMeasurements.RemoveAt(finalMeasurements.Count - 1);
            }
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
                    measurementInfo.Time = splitLine[0];
                    measurementInfo.IsStable = (splitLine[4] == "T");

                    int.TryParse(splitLine[7], out var measurement);
                    if (measurement < zeroThreshold)
                        measurement = 0;
                    measurementInfo.Measurement = measurement;

                    normalizedMeasurements.Add(measurementInfo);
                }
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
                    file.WriteLine(element);
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
                var line = csvFile.ReadLine();
                var splitLine = line?.Split(';');

                lotInfoHeader.Add(splitLine[8] + " INFO  ### Lot Info ###");
                lotInfoHeader.Add(splitLine[8] + " INFO  Lot: " + splitLine[3]);
                lotInfoHeader.Add(splitLine[8] + " INFO  Product Name: " + splitLine[4]);
                lotInfoHeader.Add(splitLine[8] + " INFO  Package: " + splitLine[5]);
                lotInfoHeader.Add(splitLine[8] + " INFO  Net Weight: " + Misc.GetValueInGrams(splitLine[6]));
                lotInfoHeader.Add(splitLine[8] + " INFO  Tare: " + Misc.GetValueInGrams(splitLine[7]));
                lotInfoHeader.Add(splitLine[8] + " INFO  Zero Threshold: ");
                lotInfoHeader.Add(splitLine[8] + " INFO  Date: " + splitLine[8]);

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
            var productName = fileHeader[2].Split(new[] {"Product Name: "}, StringSplitOptions.None)[1];
            var package = fileHeader[3].Split(new[] {"Package: "}, StringSplitOptions.None)[1];

            var outputFileName = Path.GetFileNameWithoutExtension(outputFilePath);
            var newOutputFileName = outputFileName + "_" + productName + "_" + package;
            var newOutputFilePath = outputFilePath.Replace(outputFileName, newOutputFileName);

            File.Move(outputFilePath, newOutputFilePath);
        }

        #endregion

        #region "Temporary code for converting old date format to new shorter format"

        private void btnRemoveDate_Click(object sender, EventArgs e)
        {
            var logFilePaths = GetListOfFiles(logFolderPath, "*.log");

            foreach (var logFilePath in logFilePaths)
            {
                RemoveDate(logFilePath);
            }

        }

        void RemoveDate(string logFilePath)
        {
            var fileAsList = new List<string>();

            using (var logFile = new StreamReader(logFilePath))
            {
                string line;
                while ((line = logFile.ReadLine()) != null)
                {
                    if (line.StartsWith("2019-"))
                        line = line.Substring(11);

                    fileAsList.Add(line);
                }
            }

            using (var logFile = new StreamWriter(logFilePath))
            {
                foreach (var line in fileAsList)
                    logFile.WriteLine(line);
            }

            #endregion
        }
    }
}
