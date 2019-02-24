using System;
using System.Data;
using System.IO;
using log4net;
using System.Reflection;
using System.Windows.Forms.VisualStyles;

namespace ScalesAutomation
{
    public class CsvHelper
    {
        public static string CsvFileFullPath;
        public static string CsvFolderPath;
        public static string CsvFileFullName;

        readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private bool appendToExistingOutputFile;
       
        #region Public Methods

        public void PrepareFile(string folderPath, LotInfo lotInfo)
        {
            var productInfo = MakeProductInfo(lotInfo);

            CalculatePaths(folderPath, productInfo, lotInfo.AppendToLot);

            if (!appendToExistingOutputFile)
            {
                var fileHeader = CreateMeasurementFileHeader(lotInfo);
                InitializeOutputFile(folderPath, fileHeader);
            }
        }

        public static string MakeProductInfo(LotInfo lotInfo)
        {
            var productInfo = lotInfo.Lot + "_" + lotInfo.ProductName + "_" + lotInfo.Package.Type;
            productInfo = productInfo.Replace(" ", ""); // No spaces in file names

            return productInfo;
        }

        public static string CreateMeasurementFileHeader(LotInfo lotInfo)
        {
            var fileHeader = "#;Cantitatea Cantarita [Cc];Ora;" +
                         lotInfo.Lot + ";" +
                         lotInfo.ProductName + ";" +
                         lotInfo.Package.Type + ";" +
                         lotInfo.Package.NetWeight + "Kg" + ";" +
                         lotInfo.Package.Tare + "Kg" + ";" +
                         lotInfo.Date;

            return fileHeader;
        }

        public static LotInfo ReadMeasurementFileHeader(string CSVOutputFilePath)
        {
            var lotInfo = new LotInfo();
            using (var outputFile = new StreamReader(CSVOutputFilePath))
            {
                var line = outputFile.ReadLine() ?? throw new Exception("Empty output file. No header found.");
                {
                    if (line.Contains("#;Cantitatea Cantarita [Cc];Ora")) // Header line
                    {
                        var splitLine = line.Split(';');
                        lotInfo.Lot = splitLine[3];
                        lotInfo.ProductName = splitLine[4];
                        lotInfo.Package.Type = splitLine[5];
                        var netWeight = splitLine[6].Remove(splitLine[6].Length - 2); // remove trailing kg from string
                        double.TryParse(netWeight, out lotInfo.Package.NetWeight);
                        var tare = splitLine[7].Remove(splitLine[7].Length - 2); // remove trailing kg from string
                        double.TryParse(tare, out lotInfo.Package.Tare);
                        lotInfo.Date = splitLine[8];
                    }
                }
            }

            return lotInfo;
        }

        public void WriteLine(DataRow row, int iColCount)
        {
            try
            {
                using (var sw = new StreamWriter(CsvFileFullPath, true))
                {
                    for (int i = 0; i < iColCount; i++)
                    {
                        if (!Convert.IsDBNull(row[i]))
                        {
                            sw.Write(row[i].ToString());
                        }

                        if (i < iColCount - 1)
                        {
                            sw.Write(";");
                        }
                    }

                    sw.Write(sw.NewLine);
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                log.Error("Cannot write measurement to csv file... " + CsvFileFullPath + ex.Message + Environment.NewLine);
                throw;
            }
        }

        public void CopyCurrentCsvToServer(string serverFolderPath)
        {
            try
            {
                string destinationFilePath = Path.Combine(serverFolderPath, CsvFileFullName);

                File.Copy(CsvFileFullPath, destinationFilePath, true);
            }
            catch (Exception ex)
            {
                log.Error("Cannot copy file to server:" + CsvFileFullName + " From " + CsvFileFullPath + " to " + serverFolderPath + ex.Message + Environment.NewLine);
                throw;
            }
        }

        public void BackupCurrentCsv(string bckFolderPath)
        {
            FileCopy(CsvFolderPath, bckFolderPath, CsvFileFullName);
        }

        public bool IsServerFolderReachable()
        {
            return DirectoryExists(CsvFolderPath);
        }

        public static bool OutputAlreadyPresent(string lotInfo, string outputFolderPath, ref string lotOutputFileName)
        {
            var result = false;

            try
            {
                var dirInfo = new DirectoryInfo(outputFolderPath);
                var files = dirInfo.GetFiles("*" + lotInfo + ".csv");

                if (files.Length > 0)
                {
                    lotOutputFileName = files[0].FullName;
                    result = true;
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return result;
        }

        public static bool LogAlreadyPresent(string lotNumber, string logFolderPath, ref string lotLogFileName)
        {
            var result = false;

            try
            {
                var dirInfo = new DirectoryInfo(logFolderPath);
                var files = dirInfo.GetFiles("*" + lotNumber + ".log");

                if (files.Length > 0)
                {
                    lotLogFileName = files[0].FullName;
                    result = true;
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return result;
        }

        public static bool LotAlreadyPresent(string lotNumber, string outputFolderPath, ref string lotLogFileName)
        {
            var result = false;

            try
            {
                var dirInfo = new DirectoryInfo(outputFolderPath);
                var files = dirInfo.GetFiles("*_" + lotNumber + "_*.csv");

                if (files.Length > 0)
                {
                    lotLogFileName = files[0].FullName;
                    result = true;
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return result;
        }

        public static string GetExistingLotOutputFileName(string lotNumber, string outputFolderPath)
        {
            var outputFileName = "";

            LotAlreadyPresent(lotNumber, outputFolderPath, ref outputFileName);

            return outputFileName;
        }

        #endregion

        #region Private Methods

        private void CalculatePaths(string folderPath, string productInfo, bool appendToExistingLogFile)
        {
            try
            {
                CsvFolderPath = folderPath;

                if (appendToExistingLogFile && OutputAlreadyPresent(productInfo, folderPath, ref CsvFileFullPath))
                {
                    CsvFileFullName = Path.GetFileName(CsvFileFullPath);
                    appendToExistingOutputFile = true;
                }
                else
                {
                    MakeOutputFilePath(folderPath, DateTime.Now, productInfo);
                    appendToExistingOutputFile = false;
                }
            }
            catch (Exception ex)
            {
                log.Error("Cannot calculate CSV file Path... " + CsvFileFullPath + ex.Message + Environment.NewLine);
                throw;
            }
        }

        public static string MakeOutputFilePath(string outputFolderPath, DateTime date, string productInfo)
        {
            CsvFileFullName = date.ToString("yyyy-MM-dd-hhmmss") + "_" + productInfo + ".csv";
            CsvFileFullPath = Path.Combine(outputFolderPath, CsvFileFullName);

            return CsvFileFullPath;
        }

        public static void InitializeOutputFile(string outputFilePath, string headerRow)
        {
            try
            {
                // Create the CSV file to which measured data will be saved
                using (var csvFile = new StreamWriter(outputFilePath, false))
                {
                    csvFile.Write(headerRow + csvFile.NewLine);
                }
            }
            catch (Exception ex)
            {
                // log.Error("Error creating CSV File... " + CsvFileFullPath + ex.Message + Environment.NewLine);
                throw;
            }
        }

        private void FileCopy(string sourceFolderPath, string destinationFolderPath, string fileName)
        {
            try
            {
                string sourceFilePath = Path.Combine(sourceFolderPath, fileName);
                string destinationFilePath = Path.Combine(destinationFolderPath, fileName);

                File.Copy(sourceFilePath, destinationFilePath, true);
            }
            catch (Exception ex)
            {
                log.Error("Cannot copy file:" + fileName + " From " + sourceFolderPath + " to " + destinationFolderPath + ex.Message + Environment.NewLine);
                throw;
            }
        }

        private bool DirectoryExists(string folderPath)
        {
            bool exists = false;

            try
            {
                if (Directory.Exists(folderPath))
                    exists = true;
            }
            catch (Exception ex)
            {
                log.Error("Folder does not exist:" + folderPath + ex.Message + Environment.NewLine);
                throw;
            }

            return exists;
        }

        #endregion

    }
}