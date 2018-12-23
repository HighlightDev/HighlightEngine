using System;
using System.Text;
using System.IO;
using MassiveGame.Core.SettingsCore;

namespace MassiveGame.Core.DebugCore
{
    public static class Log
    {
        private const string LOG_NAME = "EngineTrace.txt";

        #region Writer
        private static void writeLogsToFile(string message)
        {
            using (StreamWriter writer = new StreamWriter(ProjectFolders.GetRootFolder() + "trace/"
            + LOG_NAME, false, Encoding.Default))
            {
                writer.WriteLine(message);
            }
        }

        #endregion

        static Log()
        {
            writeLogsToFile(string.Format("Session started at : {0}", DateTime.Now));
        }

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
