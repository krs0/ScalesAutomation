﻿using log4net;
using log4net.Appender;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ScalesAutomation
{
    static class Misc
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
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
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
                var currentMeasurement = "";
                initialIndex++; // new index should be greater than last measurements index

                // read all csv file contents and split it in a list
                string contents = File.ReadAllText(inputFilePath);
                contents = contents.Replace(Environment.NewLine, "");
                var fileAsList = contents.Split(';');

                // modify each line to include index and date
                for (var i = 0; i < fileAsList.Length; i++)
                {
                    currentMeasurement = fileAsList[i];
                    fileAsList[i] = (i + initialIndex).ToString() + ';' + currentMeasurement + ';' + "";
                }

                // write new list to file
                using (var outputFile = new StreamWriter(outputFilePath))
                {
                    foreach (var line in fileAsList)
                        outputFile.WriteLine(line);
                }

            }
            catch (Exception ex)
            {
                // log.Error("Error creating CSV File... " + CsvFileFullPath + ex.Message + Environment.NewLine);
                throw;
            }
        }
    }
}