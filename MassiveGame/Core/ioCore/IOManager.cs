using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CONSOLE_COMMAND = MassiveGame.Core.ioCore.ConsoleCommandsManager.ConsoleCommands;

namespace MassiveGame.Core.ioCore
{
    public class IOManager
    {
        private static ConsoleCommandsManager m_consoleManager;
        private static ConsoleCommandFactory consoleCommandFactory;

        public void ProcessConsoleCommands()
        {
            consoleCommandFactory.ProceedConsoleCommands(this);
        }

        public List<CONSOLE_COMMAND> GetConsoleUnhandledCommands()
        {
            List<CONSOLE_COMMAND> result = null;
            if (!m_consoleManager.IsQueueEmpty())
            {
                result = new List<CONSOLE_COMMAND>();

                while (!m_consoleManager.IsQueueEmpty())
                    result.Add(m_consoleManager.PopCommand());
            }
            return result;
        }

        public string GetConsoleCommandsInfo()
        {
            StringBuilder result = new StringBuilder();

            result.AppendLine("info");
            result.AppendLine("shaders_recompile");

            return result.ToString();
        }

        public IOManager()
        {
            if (m_consoleManager == null)
            {
                m_consoleManager = new ConsoleCommandsManager();
            }
            if (consoleCommandFactory == null)
            {
                consoleCommandFactory = new ConsoleCommandFactory();
            }
        }
    }
}
