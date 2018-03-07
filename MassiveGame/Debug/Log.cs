using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace MassiveGame.Debug
{
    public static class Log
    {
        #region Definitions

        private const string LOG_NAME = "ProjectLogs.txt";

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

        private static void writeLogs(string[] messages)
        {
            using (StreamWriter writer = new StreamWriter(ProjectFolders.getFolderPath() + "Debug/"
            + LOG_NAME, true, Encoding.Default))
            {
                foreach (var item in messages)
                {
                    writer.WriteLine(item);
                }
            }
        }

        #endregion

        #region AddLog

        public static void addToLog(params string[] logMessages)
        {
            writeLogs(logMessages);
        }
        #endregion
    }
}
