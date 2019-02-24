﻿using System;
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

        struct MeasurementInfo
        {
            public string time;
            public bool isStable;
            public int measurement;
        }

        LotInfo lotInfo;

        string logFolderPath = Settings.Default.LogFolderPath;
        string outFileName = "";
        string outMeasurementsFilePath = "";
        string normalizedMeasurementsFilePath = "";


        public LogParser()
        {
            InitializeComponent();
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
                ParseOneFile(logFilePath);
            }
        }

        private void ParseOneFile(string logFilePath)
        {
            lotInfo = lotInfo.ReadLotInfo(logFilePath);

            var logFileName = Path.GetFileNameWithoutExtension(logFilePath);

            normalizedMeasurementsFilePath = Path.Combine(logFolderPath, logFileName + ".out");
            outMeasurementsFilePath = Path.Combine(logFolderPath, logFileName + ".meas");

            var normalizedMeasurements = ReadAndNormalizeMeasurements(logFilePath, lotInfo.ZeroThreshold);
            RemoveFakeMeasurements(normalizedMeasurements);
            SaveListToFile(normalizedMeasurements, normalizedMeasurementsFilePath);

            var finalMeasurements = ExtractFinalMeasurements(normalizedMeasurements, lotInfo.Package.NetWeight);
            SaveListToFile(finalMeasurements, outMeasurementsFilePath);

        }

        private static List<int> ExtractFinalMeasurements(List<MeasurementInfo> normalizedMeasurements, double netWeight)
        {
            var measurementsDetected = false;
            var stableMeasurementFound = false;
            var finalMeasurements = new List<int>();

            for (var i = normalizedMeasurements.Count - 1; i >= 0; i--)
            {
                var l = normalizedMeasurements[i];

                if (l.measurement == 0)
                {
                    if (measurementsDetected && !stableMeasurementFound)
                    {
                        finalMeasurements.Add((int)netWeight); // Invent an appropriate unstable measurement
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

                    // find if stable more than 2
                    if (normalizedMeasurements[i - 1].isStable && normalizedMeasurements[i - 2].isStable && normalizedMeasurements[i - 3].isStable)
                    {
                        stableMeasurementFound = true;
                        finalMeasurements.Add(normalizedMeasurements[i - 1].measurement);
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
                            if (IsZeroGlitch(normalizedMeasurements, index))
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

            for (var i = startingIndex; i < startingIndex + 20; i++)
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
            using (var file = new StreamWriter(filePath))
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

        #region Temporary Code for processing log headers

        // will replace the .csv file contents with lot info header!!!!!!!!!!!!
        private void btnMakeLotInfoHeader_Click(object sender, EventArgs e)
        {

            var csvFilePaths = GetListOfFiles(Settings.Default.CSVFolderPath, "*.csv");

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
