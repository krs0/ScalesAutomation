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
        public static string OutputFileFullPath;
        public static string OutputFolderPath;
        public static string OutputFileFullName;

        readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private bool appendToExistingOutputFile;
       
        #region Public Methods

        public void PrepareOutputFile(string folderPath, LotInfo lotInfo)
        {
            CalculatePaths(folderPath, lotInfo.Id, lotInfo.AppendToLot);

            if (appendToExistingOutputFile) return;

            var fileHeader = lotInfo.MakeMeasurementFileHeader();
            InitializeOutputFileContents(folderPath, fileHeader);
        }

        public static string CalculateOutputFilePath(string outputFolderPath, DateTime date, string productInfo)
        {
            OutputFileFullName = date.ToString("yyyy-MM-dd-hh-mm-ss") + "_" + productInfo + ".csv";
            OutputFileFullPath = Path.Combine(outputFolderPath, OutputFileFullName);

            return OutputFileFullPath;
        }

        public static void InitializeOutputFileContents(string outputFilePath, string headerRow)
        {
            try
            {
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

        public void WriteLineToOutputFile(DataRow row, int iColCount)
        {
            try
            {
                using (var sw = new StreamWriter(OutputFileFullPath, true))
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
                log.Error("Cannot write measurement to csv file... " + OutputFileFullPath + ex.Message + Environment.NewLine);
                throw;
            }
        }

        public void CopyCurrentCsvToServer(string serverFolderPath)
        {
            try
            {
                if (IsServerFolderReachable(serverFolderPath))
                {

                    var destinationFilePath = Path.Combine(serverFolderPath, OutputFileFullName);

                    File.Copy(OutputFileFullPath, destinationFilePath, true);
                }
            }
            catch (Exception ex)
            {
                log.Error("Cannot copy file to server:" + OutputFileFullName + " From " + OutputFileFullPath + " to " + serverFolderPath + ex.Message + Environment.NewLine);
                throw;
            }
        }

        public void BackupCurrentCsv(string bckFolderPath)
        {
            FileCopy(OutputFolderPath, bckFolderPath, OutputFileFullName);
        }

        public bool IsServerFolderReachable(string serverFolderPath)
        {
            return DirectoryExists(serverFolderPath);
        }

        public static bool IsAbsolutePath(string filePath)
        {
            return filePath.Contains(":");
        }

        public static bool LogAlreadyPresent(string lotId, string logFolderPath, ref string logFilePath)
        {
            return FileAlreadyPresent(lotId, logFolderPath, ref logFilePath, ".log");
        }

        public static bool OutputAlreadyPresent(string lotId, string outputFolderPath, ref string outputFilePath)
        {
            return FileAlreadyPresent(lotId, outputFolderPath, ref outputFilePath, ".csv");
        }

        private static bool FileAlreadyPresent(string lotId, string folderPath, ref string filePath, string fileExtension)
        {
            var result = false;

            try
            {
                var dirInfo = new DirectoryInfo(folderPath);
                var files = dirInfo.GetFiles("*" + lotId + fileExtension);

                if (files.Length > 0)
                {
                    filePath = files[0].FullName;
                    result = true;
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return result;
        }

        public static string GetExistingOutputFileName(string lotId, string outputFolderPath)
        {
            var outputFileName = "";

            OutputAlreadyPresent(lotId, outputFolderPath, ref outputFileName);

            return outputFileName;
        }

        public static string GetExistingLogFileName(string lotId, string logFolderPath)
        {
            var logFileName = "";

            LogAlreadyPresent(lotId, logFolderPath, ref logFileName);

            return logFileName;
        }

        #endregion

        #region Private Methods

        private void CalculatePaths(string folderPath, string productInfo, bool appendToExistingLogFile)
        {
            try
            {
                OutputFolderPath = folderPath;

                if (appendToExistingLogFile && OutputAlreadyPresent(productInfo, folderPath, ref OutputFileFullPath))
                {
                    OutputFileFullName = Path.GetFileName(OutputFileFullPath);
                    appendToExistingOutputFile = true;
                }
                else
                {
                    CalculateOutputFilePath(folderPath, DateTime.Now, productInfo);
                    appendToExistingOutputFile = false;
                }
            }
            catch (Exception ex)
            {
                log.Error("Cannot calculate CSV output file Path... " + OutputFileFullPath + ex.Message + Environment.NewLine);
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
            var exists = false;

            try
            {
                if (Directory.Exists(folderPath))
                    exists = true;
            }
            catch (Exception ex)
            {
                log.Error("Folder does not exist or is unreachable:" + folderPath + ex.Message + Environment.NewLine);
                throw;
            }

            return exists;
        }

        #endregion

    }
}