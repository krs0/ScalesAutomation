using log4net;
using log4net.Appender;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CommonNS
{
    public static class Common
    {
        private static readonly ILog log = LogManager.GetLogger("generalLog");

        public static string AssemblyPath
        {
            get
            {
                return Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
            }
        }

        public static void ChangeLoggingFile(this ILog localLog, string logFilePath)
        {
            var rootRepository = log4net.LogManager.GetRepository();
            foreach(var appender in rootRepository.GetAppenders())
            {
                if(appender.Name.Equals("LogToFile") && appender is FileAppender)
                {
                    var fileAppender = appender as FileAppender;
                    fileAppender.File = logFilePath;
                    fileAppender.ActivateOptions();
                    break;  // Appender found and name changed to NewFilename
                }
            }
        }

        public static string GetLoggingFile(this ILog log)
        {
            string logFileName = "";
            var logger = (Logger)log.Logger;

            while(logger != null)
            {
                foreach(var appender in logger.Appenders)
                {
                    if(appender is FileAppender fileAppender)
                        logFileName = fileAppender.File;
                }
                logger = logger.Parent;
            }

            return logFileName;
        }

        public static void AppendFiles(string inputFilePath, string outputFilePath)
        {
            using(Stream input = File.OpenRead(inputFilePath))
            using(Stream output = new FileStream(outputFilePath, FileMode.Append, FileAccess.Write, FileShare.None))
            {
                input.CopyTo(output); // Using .NET 4
            }
        }

        public static bool IsAbsolutePath(string filePath)
        {
            return filePath.Contains(':');
        }

        /// <summary>Transforms a relative path in a absolute path relative to AssemblyPath</summary>
        public static string TransformToAbsolutePath(string folderPath)
        {
            var transformedFolderPath = folderPath;

            if(!IsAbsolutePath(transformedFolderPath))
                transformedFolderPath = Path.Combine(Common.AssemblyPath, transformedFolderPath);

            return transformedFolderPath;
        }

        public static int[] TransformIEnumerableByteToIntArray(IEnumerable<byte> package, ref byte[] packageAsByteArray)
        {
            packageAsByteArray = package.ToArray();
            var packageAsCharArray = Array.ConvertAll(packageAsByteArray, element => (System.Text.Encoding.ASCII.GetChars(new[] { element })[0]));
            var packageAsIntArray = Array.ConvertAll(packageAsCharArray, element => (int)char.GetNumericValue(element));

            return packageAsIntArray;
        }

        /// <summary>
        ///             // ENT in %,g sau ml will be calculated according to the following table

        // 5 <= Qn <= 50 9%
        // 50 <= Qn <= 100 4.5g
        // 100 <= Qn <= 200 4.5%
        // 200 <= Qn <= 300 9g
        // 300 <= Qn <= 500 3%
        // 500 <= Qn <= 1000 15g
        // 1000 <= Qn <= 10000 1.5%
        /// </summary>
        /// <param name="nominalQuantity">Nominal Quantity</param>
        /// <param name="unit">optional measurement unit: Kg, g. Default is in grams</param>
        public static double CalculateNegativeToleratedError(int nominalQuantity, string unit = "g")
        {
            double ENT; // NegativeToleratedError
            double Qn; // local Nominal Quantity
            var unitDivider = 1; // default for grams

            if(unit == "Kg")
                unitDivider = 1000;

            Qn = Convert.ToDouble(nominalQuantity) * unitDivider; // first we transform everything in grams

            if(Qn >= 5 && Qn < 50)
                ENT = Qn * 9 / 100;
            else if(Qn >= 50 && Qn < 100)
                ENT = 4.5;
            else if(Qn >= 100 && Qn < 200)
                ENT = Qn * 4.5 / 100;
            else if(Qn >= 200 && Qn < 300)
                ENT = 9;
            else if(Qn >= 300 && Qn < 500)
                ENT = Qn * 3 / 100;
            else if(Qn >= 500 && Qn < 1000)
                ENT = 15;
            else if(Qn >= 1000 && Qn < 10000)
                ENT = Qn * 1.5 / 100;
            else
                ENT = Qn * 1.5 / 100;

            return ENT / unitDivider; // we transform ENT in same unit as input Qn: Kg or g
        }
    }
}