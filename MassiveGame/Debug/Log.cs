using System;
using System.Text;
using System.IO;
using MassiveGame.Settings;

namespace MassiveGame.Debug
{
    public static class Log
    {
        #region Definitions

        private const string LOG_NAME = "ProjectLogs.txt";
        #endregion

        static Log()
        {
            using (StreamWriter writer = new StreamWriter(ProjectFolders.GetRootFolder() + "Debug/"
                + LOG_NAME, true, Encoding.Default))
            {
                writer.WriteLine("Session started at : {0}", DateTime.Now);
            }
        }

        #region Writer

        private static void writeLogsToFile(string message)
        {
            using (StreamWriter writer = new StreamWriter(ProjectFolders.GetRootFolder() + "Debug/"
            + LOG_NAME, false, Encoding.Default))
            {
                writer.WriteLine(message);
            }
        }

        #endregion

        #region AddLog

        public static void AddToConsoleStreamLog(params string[] logMessages)
        {
            string resultMessage = String.Empty;
            foreach (string messageLine in logMessages)
            {
                if (messageLine != null)
                    resultMessage += messageLine + System.Environment.NewLine;
            }
            Console.WriteLine(resultMessage);
        }

        public static void AddToFileStreamLog(params string[] logMessages)
        {
            string resultMessage = String.Empty;
            foreach (string messageLine in logMessages)
            {
                if (messageLine != null)
                    resultMessage += messageLine + System.Environment.NewLine;
            }
            writeLogsToFile(resultMessage);
        }
        #endregion
    }
}
