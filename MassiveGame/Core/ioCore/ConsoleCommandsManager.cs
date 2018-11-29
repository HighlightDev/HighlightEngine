using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using COMMAND = MassiveGame.Core.ioCore.ConsoleCommandsManager.ConsoleCommands;

namespace MassiveGame.Core.ioCore
{
    public class ConsoleCommandsManager
    {
        private Thread m_consoleInputThread;
        private Queue<COMMAND> m_commandBuffer;
        private static bool bConsoleIsReading = true;

        public enum ConsoleCommands
        {
            UNDEFINED,
            INFO,
            SHADERS_RECOMPILE
        }

        public ConsoleCommandsManager()
        {
            m_commandBuffer = new Queue<COMMAND>();
            m_consoleInputThread = new Thread(new ThreadStart(ConsoleReadInput));
            m_consoleInputThread.Start();
        }

        public bool IsQueueEmpty()
        {
            return m_commandBuffer.Count <= 0;
        }

        public bool GetIsConsoleReading()
        {
            return bConsoleIsReading;
        }

        public void ToggleConsoleReading()
        {
            bConsoleIsReading = !bConsoleIsReading;
        }

       
        public void PushCommand(COMMAND inCommand)
        {
            m_commandBuffer.Enqueue(inCommand);
        }

        public COMMAND PopCommand()
        {
            return m_commandBuffer.Dequeue();
        }

        private void ConsoleReadInput()
        {
            while (bConsoleIsReading)
            {
                string commandStr = Console.ReadLine();
                Console.Clear();
                PushCommand(ParseCommandString(commandStr));
            }
        }

        private COMMAND ParseCommandString(string inCommand)
        {
            COMMAND resultCommand = COMMAND.UNDEFINED;
            string lowerCommand = inCommand.ToLower();
            if (lowerCommand == "shaders_recompile")
            {
                resultCommand = COMMAND.SHADERS_RECOMPILE;
            }
            else if (lowerCommand == "info")
            {
                resultCommand = COMMAND.INFO;
            }

            return resultCommand;
        }

    }
}
