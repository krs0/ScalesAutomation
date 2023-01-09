using log4net;
using log4net.Appender;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace ScalesAutomation
{
    public static class Misc
    {
        public static void ChangeLoggingFile(this ILog log, string newFileName)
        {
            var logger = (Logger)log.Logger;

            while (logger != null)
            {
                foreach (var appender in logger.Appenders)
                {
                    if (appender is FileAppender fileAppender)
                    {
                        fileAppender.File = newFileName;
                        fileAppender.ActivateOptions();
                    }
                }
                logger = logger.Parent;
            }
        }

        public static string AssemblyPath
        {
            get
            {
                return Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
            }
        }

        public static void AppendOneFileToAnother(string inputFilePath, string outputFilePath)
        {
            using (Stream input = File.OpenRead(inputFilePath))
            using (Stream output = new FileStream(outputFilePath, FileMode.Append, FileAccess.Write, FileShare.None))
            {
                input.CopyTo(output); // Using .NET 4
            }
        }

        public static void MakeTemporaryFileWithStandardizedContents(string inputFilePath, string outputFilePath, string dateTime, int initialIndex)
        {
            try
            {
                // read all csv file contents and split it in a list
                string contents = File.ReadAllText(inputFilePath);
                contents = contents.TrimEnd('\r', '\n');
                var fileAsList = contents.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

                // modify each line to include index and date
                for (var i = 0; i < fileAsList.Length; i++)
                {
                    var currentMeasurement = fileAsList[i];
                    fileAsList[i] = (i + initialIndex).ToString() + ';' + currentMeasurement + ';' + "";
                }

                // write new list to file
                using (var outputFile = new StreamWriter(outputFilePath))
                {
                    foreach (var line in fileAsList)
                        outputFile.WriteLine(line);
                }

            }
            catch (Exception)
            {
                MessageBox.Show("Nu se poate deschide fisierul .csv: " + inputFilePath + Environment.NewLine + "Inchideti fisierul deschis in Excel si mai incercati odata.");
                throw;
            }
        }

        /// <summary>// remove trailing kg from string </summary>
        public static string RemoveTrailingKg(string rawValue)
        {

            if (!string.IsNullOrEmpty(rawValue) && rawValue.Contains("Kg"))
                rawValue = rawValue.Remove(rawValue.Length - 2);

            return rawValue;
        }

        public static double GetValueInGrams(string value)
        {
            double outputValue;

            if (value.Contains("Kg"))
            {
                value = RemoveTrailingKg(value);
                double.TryParse(value, out outputValue);
                outputValue *= 1000;
            }
            else
                double.TryParse(value, out outputValue);

            return outputValue;
        }
    }
}