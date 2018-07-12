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

        private static string LOG_MESSAGE = String.Empty;

        #endregion

        static Log()
        {
            using (StreamWriter writer = new StreamWriter(ProjectFolders.getFolderPath() + "Debug/"
                + LOG_NAME, true, Encoding.Default))
            {
                writer.WriteLine("Session started at : {0}", DateTime.Now);
            }
        }

        #region Writer

        private static void writeLogs(string message)
        {
            using (StreamWriter writer = new StreamWriter(ProjectFolders.getFolderPath() + "Debug/"
            + LOG_NAME, false, Encoding.Default))
            {
                writer.WriteLine(message);
            }
        }

        #endregion

        #region AddLog

        public static void addToLog(params string[] logMessages)
        {
            foreach (string messageLine in logMessages)
            {
                if (messageLine != null)
                    LOG_MESSAGE += messageLine + System.Environment.NewLine;
            }
            writeLogs(LOG_MESSAGE);
        }
        #endregion
    }
}
