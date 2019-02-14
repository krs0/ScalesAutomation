using log4net;
using log4net.Appender;
using log4net.Repository.Hierarchy;
using System;
using System.IO;
using System.Reflection;

namespace ScalesAutomation
{
    static class Misc
    {
        public static void StartNewLogFile(this ILog log, string newFileName)
        {
            Logger logger = (Logger)log.Logger;

            while (logger != null)
            {
                foreach (IAppender appender in logger.Appenders)
                {
                    FileAppender fileAppender = appender as FileAppender;
                    if (fileAppender != null)
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

    }
}