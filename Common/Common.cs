﻿using log4net;
using log4net.Appender;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ScalesAutomation
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
    }
}