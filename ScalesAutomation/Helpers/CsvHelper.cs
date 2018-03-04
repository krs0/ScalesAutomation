using ScalesAutomation.Properties;
using System;
using System.Data;
using System.IO;
using log4net;
using System.Reflection;

namespace ScalesAutomation
{
    public class CsvHelper
    {
        public string CsvFileFullPath;
        public string CsvFolderPath;
        public string CsvFileFullName;

        readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private bool appendToExistingFile;

        #region Public Methods

        public void PrepareFile(string folderPath, LotInfo lotInfo, DataTable dataTable)
        {
            var productInfo = lotInfo.Lot + "_" + lotInfo.ProductName + "_" + lotInfo.Package.Type;
            productInfo = productInfo.Replace(" ", ""); // No spaces in file names

            CalculatePaths(folderPath, productInfo);

            if (!appendToExistingFile)
                CreateFile(lotInfo, dataTable);
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
                            sw.Write(",");
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

        public void BackupCurrentCsv()
        {
            FileCopy(CsvFolderPath, Settings.Default.CSVBackupPath, CsvFileFullName);
        }

        public bool IsServerFolderReachable()
        {
            return DirectoryExists(CsvFolderPath);
        }

        #endregion

        #region Private Methods

        private void CalculatePaths(string folderPath, string productInfo)
        {
            try
            {
                CsvFolderPath = folderPath;

                var dirInfo = new DirectoryInfo(CsvFolderPath);
                var files = dirInfo.GetFiles("*" + productInfo + ".csv");

                // if a file exists starting with same product info (LOT!!!), reuse it
                if (files.Length > 0)
                {
                    CsvFileFullName = files[0].Name;
                    CsvFileFullPath = files[0].FullName;
                    appendToExistingFile = true;
                }
                else
                {
                    CsvFileFullName = DateTime.Now.ToString("yyyy-MM-dd-hhmmss") + "_" + productInfo + ".csv";
                    CsvFileFullPath = Path.Combine(CsvFolderPath, CsvFileFullName);
                    appendToExistingFile = false;
                }
            }
            catch (Exception ex)
            {
                log.Error("Cannot calculate CSV file Path... " + CsvFileFullPath + ex.Message + Environment.NewLine);
                throw;
            }
        }

        private void CreateFile(LotInfo lotInfo, DataTable dataTable)
        {
            try
            {
                // Create the CSV file to which grid data will be exported.
                using (var sw = new StreamWriter(CsvFileFullPath, false))
                {
                    var iColCount = dataTable.Columns.Count;
                    for (var i = 0; i < iColCount; i++)
                    {
                        sw.Write(dataTable.Columns[i]);
                        if (i < iColCount - 1)
                        {
                            sw.Write(",");
                        }
                    }

                    sw.Write("," + lotInfo.Lot + "," + lotInfo.ProductName + "," + lotInfo.Package.Type + "," + lotInfo.Package.NetWeight + "," + lotInfo.Package.Tare + "," + lotInfo.Date);
                    sw.Write(sw.NewLine);
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                log.Error("Error creating CSV File... " + CsvFileFullPath + ex.Message + Environment.NewLine);
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