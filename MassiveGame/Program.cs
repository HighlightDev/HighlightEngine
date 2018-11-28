using System;
using System.Threading;

namespace MassiveGame
{
    internal class Program
    {
#if COLLISION_EDITOR
        [STAThread]
#endif
        private static void Main(string[] args)
        {
            Engine.EngineCore core = new Engine.EngineCore();

            // TODO : Console thread, not best implementation!
            Thread consoleInputThread = new Thread(new ParameterizedThreadStart(ConsoleReadInput));
            consoleInputThread.Start(core);

            core.ShowUiWindow();
        }

        private static void ConsoleReadInput(object engineCore)
        {
            Engine.EngineCore core = engineCore as Engine.EngineCore;
            while (true)
            {
                var inputString = Console.ReadLine();
                ParseConsoleInput(inputString, core);
            }
        }

        private static void ParseConsoleInput(string inputStr, Engine.EngineCore engineCore)
        {
            if (inputStr.ToLower() == "shaders_recompile")
            {
                engineCore.RecompileShaders();
            }
        }
    }
}