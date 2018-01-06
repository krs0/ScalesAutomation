using System;
using System.Data;
using System.IO;

namespace ScalesAutomation
{
    public class CsvHelper
    {
        public string CsvFileFullPath;

        public CsvHelper(){}

        public void PrepareCsvFile(DataTable dataTable, string filePath, string productInfo)
        {
            var dirInfo = new DirectoryInfo(filePath);
            var files = dirInfo.GetFiles("*" + productInfo + ".csv");

            // if a file exists starting with same product info, reuse it
            if (files.Length > 0)
                CsvFileFullPath = files[0].FullName;
            else
                CsvFileFullPath = filePath + DateTime.Now.ToString("yyyy-MM-dd-hhmmss") + "_" + productInfo + ".csv";

            CreateCsvFile(dataTable);
        }

        public void CreateCsvFile(DataTable dataTable)
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
                throw;
            }
        }

        public void WriteOneMeasurementToCsv(DataRow row, int iColCount)
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
                throw;
            }
        }
    }
}