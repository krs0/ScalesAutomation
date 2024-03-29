using System;
using System.IO;
using log4net;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace CommonNS
{
    public class PathHelper
    {
        private static readonly ILog log = LogManager.GetLogger("generalLog");

        private static HashSet<char> invalidCharacters = new HashSet<char>(Path.GetInvalidPathChars());

        #region Public Methods

        public static bool GetFilePath(string lotId, string folderPath, ref string filePath, string fileExtension)
        {
            var result = false;

            try
            {
                var dirInfo = new DirectoryInfo(folderPath);
                var files = dirInfo.GetFiles("*" + lotId + fileExtension);

                if(files.Length > 0)
                {
                    filePath = files[0].FullName;
                    result = true;
                }
            }
            catch(Exception ex)
            {
                log.Error($"GetFilePath error: '{lotId}' '{folderPath}'{Environment.NewLine}{ex.Message}");
                throw;
            }

            return result;
        }

        public static bool GetLogFilePath(string lotId, string logFolderPath, ref string logFilePath)
        {
            return GetFilePath(lotId, logFolderPath, ref logFilePath, ".log");
        }

        public static bool GetOutputFilePath(string lotId, string outputFolderPath, ref string outputFilePath)
        {
            return GetFilePath(lotId, outputFolderPath, ref outputFilePath, ".csv");
        }

        public static void FileCopy(string sourceFolderPath, string destinationFolderPath, string fileName)
        {
            try
            {
                string sourceFilePath = Path.Combine(sourceFolderPath, fileName);
                string destinationFilePath = Path.Combine(destinationFolderPath, fileName);

                File.Copy(sourceFilePath, destinationFilePath, true);
            }
            catch(Exception ex)
            {
                log.Error($"Cannot copy file: '{fileName}' from '{sourceFolderPath}' to '{destinationFolderPath}'" +
                    $"{Environment.NewLine}{ex.Message}");
                throw;
            }
        }

        public static bool DirectoryExists(string folderPath)
        {
            var exists = false;

            try
            {
                if(Directory.Exists(folderPath))
                    exists = true;
            }
            catch(Exception ex)
            {
                log.Error($"Folder does not exist or is unreachable: '{folderPath}'{Environment.NewLine}{ex.Message}");
                throw;
            }

            return exists;
        }

        public static bool IsPathValid(string filePath)
        {
            char[] charactersToAdd = { '*', '/', ':', '<', '>', '?', '\\', '|' };

            // Add the characters to the invalidCharacters set
            foreach(char c in charactersToAdd)
                invalidCharacters.Add(c);

            return !string.IsNullOrEmpty(filePath) && !filePath.Any(pc => invalidCharacters.Contains(pc));
        }

        #endregion
    }
}