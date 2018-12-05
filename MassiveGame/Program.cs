using System;
using System.Drawing;
using System.Windows.Forms;

namespace MassiveGame
{
    internal class Program
    {
#if COLLISION_EDITOR
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
            m_UiWindow = new UI.EditorWindow(startScreenRezoluion.X, startScreenRezoluion.Y, core.PreConstructor, core.RenderQueue, core.CleanEverythingUp);
#elif DEBUG
            m_UiWindow = new UI.GameWindow(startScreenRezoluion.X, startScreenRezoluion.Y, core.PreConstructor, core.RenderQueue, core.CleanEverythingUp);
#endif

            m_UiWindow.Left = EngineStatics.SCREEN_POSITION_X;
            m_UiWindow.Top = EngineStatics.SCREEN_POSITION_Y;

            m_UiWindow.ShowDialog();
        }
    }
}