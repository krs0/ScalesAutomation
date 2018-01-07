using System;
using System.Data;
using System.IO;
using System.Windows.Forms.VisualStyles;

namespace ScalesAutomation
{
    public class CsvHelper
    {
        public string CsvFileFullPath;

        private bool appendToExistingFile;

        public void PrepareFile(DataTable dataTable, string filePath, string productInfo)
        {
            CsvFileFullPath = DetermineFullFilePath(filePath, productInfo);
            if (!appendToExistingFile)
                CreateFile(dataTable);
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
                throw ex;
            }
        }

        private void CreateFile(DataTable dataTable)
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
                    sw.Write(sw.NewLine);
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string DetermineFullFilePath(string filePath, string productInfo)
        {
            string fileFullPath;
            var dirInfo = new DirectoryInfo(filePath);
            var files = dirInfo.GetFiles("*" + productInfo + ".csv");

            // if a file exists starting with same product info, reuse it
            if (files.Length > 0)
            {
                fileFullPath = files[0].FullName;
                appendToExistingFile = true;
            }
            else
            {
                fileFullPath = filePath + DateTime.Now.ToString("yyyy-MM-dd-hhmmss") + "_" + productInfo + ".csv";
                appendToExistingFile = false;
            }

            return fileFullPath;
        }
    }
}