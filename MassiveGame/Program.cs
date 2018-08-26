using System;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using MassiveGame.UI;
using System.Windows.Forms;

namespace MassiveGame
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Form uiWindow = null;
#if DESIGN_EDITOR
            uiWindow = new UI.EditorWindow();
#else
            uiWindow = new UI.GameWindow();
#endif
            uiWindow.Left = EngineStatics.SCREEN_POSITION_X;
            uiWindow.Top = EngineStatics.SCREEN_POSITION_Y;
            uiWindow.ShowDialog();
        }
    }
}