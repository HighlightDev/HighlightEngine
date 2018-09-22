using System;

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
            core.ShowUiWindow();
        }
    }
}