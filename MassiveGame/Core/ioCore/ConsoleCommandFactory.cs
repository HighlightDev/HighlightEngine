using MassiveGame.API.ResourcePool;
using MassiveGame.Core.DebugCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MassiveGame.Core.ioCore.ConsoleCommandsManager;

namespace MassiveGame.Core.ioCore
{
    public class ConsoleCommandFactory
    {
        public void ProceedConsoleCommands(IOManager iOManager)
        {
            // Console commands processing
            var consoleCommands = iOManager.GetConsoleUnhandledCommands();
            if (consoleCommands == null)
                return;

            foreach (var command in consoleCommands)
            {
                switch (command)
                {
                    case ConsoleCommands.SHADERS_RECOMPILE:
                        {
                            bool bSuccess = PoolCollector.GetInstance().s_ShaderPool.RecompileAllShaders();
                            if (!bSuccess)
                            {
                                Log.AddToConsoleStreamLog("Shaders recompilation failed!");
                                Log.AddToFileStreamLog("Shaders recompilation failed!");
                            }
                            break;
                        }
                    case ConsoleCommands.INFO:
                        {
                            Log.AddToConsoleStreamLog("Available console commands: ", iOManager.GetConsoleCommandsInfo());
                            break;
                        }
                    case ConsoleCommands.UNDEFINED:
                        {
                            Log.AddToConsoleStreamLog("Unknown command, available console commands are: ", iOManager.GetConsoleCommandsInfo());
                            break;
                        }
                }
            }
        }
    }
}
