﻿using System;
using System.Collections.Generic;
using log4net;
using System.Reflection;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Windows.Forms.VisualStyles;
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
            public int position;
            public string time;
            public bool isStable;
            public double measurement;

            public MeasurementInfo(int position, string time, bool isStable, double measurement)
            {
                this.position = position;
                this.time = time;
                this.isStable = isStable;
                this.measurement = measurement;
            }

            public MeasurementInfo()
            {
                position = 0;
                time = "";
                isStable = false;
                measurement = 0;
            }

            public override string ToString()
            {
                return position + ";" + measurement + ";" + time;
            }
        }

        LotInfo lotInfo;

        string logFolderPath = Settings.Default.LogFolderPath;
        string outputFolderPath = Settings.Default.OutputFolderPath;
        string outFileName = "";
        string outMeasurementsFilePath = "";
        string normalizedMeasurementsFilePath = "";


        public LogParser()
        {
            InitializeComponent();
            lotInfo = new LotInfo();
        }

        public void Initialize(string logFolderPath)
        {
            this.logFolderPath = logFolderPath;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            var logFilePaths = GetListOfFiles(logFolderPath, "*.log");

            foreach (var logFilePath in logFilePaths)
            {
                ParseLog(logFilePath);
            }
        }

        private void ParseLog(string logFilePath)
        {
            lotInfo = lotInfo.ReadLotInfoFromLog(logFilePath);

            var logFileName = Path.GetFileNameWithoutExtension(logFilePath);

            normalizedMeasurementsFilePath = Path.Combine(logFolderPath, logFileName + ".out");
            outMeasurementsFilePath = Path.Combine(logFolderPath, logFileName + ".meas");

            CsvHelper.InitializeOutputFileContents(outMeasurementsFilePath, lotInfo.MakeMeasurementFileHeader());

            var normalizedMeasurements = ReadAndNormalizeMeasurements(logFilePath, lotInfo.ZeroThreshold);
            RemoveFakeMeasurements(normalizedMeasurements);
            // SaveListToFile(normalizedMeasurements, normalizedMeasurementsFilePath); // Generic list save does not work

            var finalMeasurements = ExtractFinalMeasurements(normalizedMeasurements, lotInfo.Package.NetWeight);
            SaveListToFile(finalMeasurements, outMeasurementsFilePath);

        }

        private static List<MeasurementInfo> ExtractFinalMeasurements(List<MeasurementInfo> normalizedMeasurements, double netWeight)
        {
            var measurementsDetected = false;
            var stableMeasurementFound = false;
            var finalMeasurements = new List<MeasurementInfo>();

            for (var i = normalizedMeasurements.Count - 1; i >= 0; i--)
            {
                var l = normalizedMeasurements[i];

                if (l.measurement == 0)
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
                    if (!l.isStable || i <= 3) continue;

                    // find if stable more than 4
                    if (normalizedMeasurements[i - 1].isStable && normalizedMeasurements[i - 2].isStable && normalizedMeasurements[i - 3].isStable)
                    {
                        stableMeasurementFound = true;
                        finalMeasurements.Add(normalizedMeasurements[i - 1]);
                    }
                }
            }

            finalMeasurements.Reverse();
            for (int i = 0; i < finalMeasurements.Count; i++)
            {
                finalMeasurements[i].position = i + 1;
            }

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
                if (element.measurement > 0)
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
                    measurementInfo.time = splitLine[1];
                    measurementInfo.isStable = (splitLine[5] == "T");

                    int.TryParse(splitLine[8], out var measurement);
                    if (measurement < zeroThreshold)
                        measurement = 0;
                    measurementInfo.measurement = measurement;

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
                if (lines[i].measurement > 0)
                {
                    isZeroGlitch = true;

                    break;
                }
            }

            return isZeroGlitch;
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
    }
}
