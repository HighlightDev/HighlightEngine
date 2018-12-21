using System;
using System.Drawing;
using System.Windows.Forms;

namespace MassiveGame
{
    internal class Program
    {
#if COLLISION_EDITOR || ENGINE_EDITOR
        [STAThread]
#endif
        private static void Main(string[] args)
        {
            Form m_UiWindow = null;

            Engine.EngineCore core = new Engine.EngineCore();
            Point startScreenRezoluion = new Point(1400, 800);

#if COLLISION_EDITOR
            m_UiWindow = new UI.CollisionEditorWindow(startScreenRezoluion.X, startScreenRezoluion.Y);
#elif ENGINE_EDITOR
            m_UiWindow = new UI.EditorWindow(startScreenRezoluion.X, startScreenRezoluion.Y, core);
#elif DEBUG
            m_UiWindow = new UI.GameWindow(startScreenRezoluion.X, startScreenRezoluion.Y, core);
#endif

            m_UiWindow.Location = EngineStatics.ScreenLocation;

            m_UiWindow.ShowDialog();
        }
    }
}