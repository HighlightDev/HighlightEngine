using System;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using MassiveGame.UI;

namespace MassiveGame
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            MainUI form = new MainUI(DOUEngine.SCREEN_WIDTH, DOUEngine.SCREEN_HEIGHT);
            form.Left = DOUEngine.SCREEN_POSITION_X;
            form.Top = DOUEngine.SCREEN_POSITION_Y;
            form.ShowDialog();
        }
    }
}