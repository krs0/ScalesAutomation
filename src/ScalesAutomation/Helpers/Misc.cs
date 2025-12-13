using log4net;
using log4net.Appender;
using log4net.Repository.Hierarchy;
using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace ScalesAutomation
{
    public static class Misc
    {
        /// <summary>// remove trailing kg from string </summary>
        public static string RemoveTrailingKg(string rawValue)
        {

            if (!string.IsNullOrEmpty(rawValue) && rawValue.Contains("Kg"))
                rawValue = rawValue.Remove(rawValue.Length - 2);

            return rawValue;
        }

        /// <summary> We will never have a non integer value. Grams is the lowest unit.</summary>
        public static int GetValueInGrams(string value)
        {
            int outputValue;
            double valueInKg;

            if (value.Contains("Kg"))
            {
                value = value.Replace(",", "."); // only Kg can have "."
                value = RemoveTrailingKg(value);
                double.TryParse(value, out valueInKg);
                outputValue = (int)(valueInKg * 1000);
            }
            else
                int.TryParse(value, out outputValue);

            return outputValue;
        }

        /// <summary>Deprecated. Used for transforming log from an old format to the current one.</summary>
        public static void MakeTemporaryFileWithStandardizedContents(string inputFilePath, string outputFilePath, string dateTime, int initialIndex)
        {
            try
            {
                // read all csv file contents and split it in a list
                string contents = File.ReadAllText(inputFilePath);
                contents = contents.TrimEnd('\r', '\n');
                var fileAsList = contents.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

                // modify each line to include index and date
                for(var i = 0; i < fileAsList.Length; i++)
                {
                    var currentMeasurement = fileAsList[i];
                    fileAsList[i] = (i + initialIndex).ToString() + ';' + currentMeasurement + ';' + "";
                }

                // write new list to file
                using(var outputFile = new StreamWriter(outputFilePath))
                {
                    foreach(var line in fileAsList)
                        outputFile.WriteLine(line);
                }

            }
            catch(Exception)
            {
                MessageBox.Show("Nu se poate deschide fisierul .csv: " + inputFilePath + Environment.NewLine + "Inchideti fisierul deschis in Excel si mai incercati odata.");
                throw;
            }
        }
    }
}