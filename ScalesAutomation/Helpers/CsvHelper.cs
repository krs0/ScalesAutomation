using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using log4net;
using System.Reflection;
using System.Windows.Forms.VisualStyles;

namespace ScalesAutomation
{
    public class CsvHelper
    {
        public static string OutputFilePath;
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
            InitializeOutputFileContents(OutputFilePath, fileHeader);
        }

        public static string CalculateOutputFilePath(string outputFolderPath, DateTime date, string lotId)
        {
            OutputFileFullName = date.ToString("yyyy-MM-dd-HH-mm-ss") + "_" + lotId + ".csv";
            OutputFilePath = Path.Combine(outputFolderPath, OutputFileFullName);

            return OutputFilePath;
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
            catch (Exception)
            {
                // log.Error("Error creating CSV File... " + CsvFileFullPath + ex.Message + Environment.NewLine);
                throw;
            }
        }

        public void WriteLineToOutputFile(DataRow row, int iColCount)
        {
            try
            {
                using (var sw = new StreamWriter(OutputFilePath, true))
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
                }
            }
            catch (Exception ex)
            {
                log.Error("Cannot write measurement to csv file... " + OutputFilePath + ex.Message + Environment.NewLine);
                throw;
            }
        }

        public void CopyCurrentCsvToServer(string serverFolderPath)
        {
            try
            {
                if (!IsServerFolderReachable(serverFolderPath)) return;

                var serverOutputFilePath = Path.Combine(serverFolderPath, OutputFileFullName);

                File.Copy(OutputFilePath, serverOutputFilePath, true);
            }
            catch (Exception ex)
            {
                log.Error("Cannot copy file to server:" + OutputFileFullName + " From " + OutputFilePath + " to " + serverFolderPath + ex.Message + Environment.NewLine);
                throw;
            }
        }

        public void BackupCurrentCsv(string bckFolderPath)
        {
            var backupFileName = OutputFileFullName;

            // For the moment we overwrite our backups, since data is anyway appended and not rewritten. So following lines are commented.

            //if (File.Exists(Path.Combine(bckFolderPath, OutputFileFullName)))
            //    backupFileName = Path.GetFileNameWithoutExtension(OutputFileFullName) + "_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".csv";
                                 
            FileCopy(OutputFolderPath, bckFolderPath, backupFileName);
        }

        /// <summary>Launch the legacy application with some options set.</summary>
        public void ParseCurrentLog(string logFilePath)
        {
            var startInfo = new ProcessStartInfo
            {
                CreateNoWindow = false,
                UseShellExecute = false,
                FileName = "LogParser.exe",
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = "\"" + logFilePath + "\" \"" + OutputFolderPath + "\\\"" // add an extra \ at the end, not to escape last " when it arrives in parser
            };

            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using-statement will close.
                log.Info("Starting parser with arguments: " + startInfo.Arguments );
                using (var parserProcess = Process.Start(startInfo))
                {
                    parserProcess?.WaitForExit();
                }
            }
            catch
            {
                // Log error.
            }
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
            catch (Exception)
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

        public static string GetLastMeasurementIndex(string outputFilePath)
        {
            var lastLine = File.ReadLines(outputFilePath).Last();
            var splitLine = lastLine.Split(';');
            var lastMeasurementIndex = splitLine[0];
            return lastMeasurementIndex;
        }

        #endregion

        #region Private Methods

        private void CalculatePaths(string folderPath, string lotId, bool appendToExistingLogFile)
        {
            try
            {
                OutputFolderPath = folderPath;

                if (appendToExistingLogFile && OutputAlreadyPresent(lotId, folderPath, ref OutputFilePath))
                {
                    OutputFileFullName = Path.GetFileName(OutputFilePath);
                    appendToExistingOutputFile = true;
                }
                else
                {
                    CalculateOutputFilePath(folderPath, DateTime.Now, lotId);
                    appendToExistingOutputFile = false;
                }
            }
            catch (Exception ex)
            {
                log.Error("Cannot calculate CSV output file Path... " + OutputFilePath + ex.Message + Environment.NewLine);
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