using System;
using System.Data;
using System.IO;

namespace ScalesAutomation
{
    public class CsvHelper
    {
        public string CsvFilePath;

        public CsvHelper(string csvFilePath)
        {
            CsvFilePath = csvFilePath;
        }

        public void CreateCsvFile(DataTable dataTable)
        {
            try
            {
                // Create the CSV file to which grid data will be exported.
                using (var sw = new StreamWriter(CsvFilePath, false))
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
                using (var sw = new StreamWriter(CsvFilePath, true))
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