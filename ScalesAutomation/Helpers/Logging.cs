using log4net;
using log4net.Appender;
using log4net.Repository.Hierarchy;

namespace ScalesAutomation
{
    static class Logging
    {
        public static void StartNewFile(this ILog log, string newFileName)
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
    }
}