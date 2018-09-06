using System;

namespace MassiveGame
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Engine.EngineCore core = new Engine.EngineCore();
            core.ShowUiWindow();
        }
    }
}