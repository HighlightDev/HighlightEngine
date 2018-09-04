using System;

namespace MassiveGame
{
    internal class Program
    {
        public class A
        {
            public void foo()
            { Console.WriteLine("A"); }
        }

        private static void Main(string[] args)
        {
            Engine.EngineCore core = new Engine.EngineCore();
            core.ShowUiWindow();
        }
    }
}