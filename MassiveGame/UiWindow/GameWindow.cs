using System;
using System.Drawing;
using System.Windows.Forms;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using MassiveGame.Core.GameCore;
using MassiveGame.API.ResourcePool.PoolHandling;
using MassiveGame.API.ResourcePool.Pools;
using MassiveGame.API.ResourcePool;
using MassiveGame.UiWindow;
using MassiveGame.Engine;
using MassiveGame.Core.DebugCore;

namespace MassiveGame.UI
{
    public partial class GameWindow : Form
    {
        private EngineCore m_engineCore = null;

        #region Constructors

        private GameWindow(EngineCore engineCore)
        {
            m_engineCore = engineCore;

            Application.EnableVisualStyles();
            InitializeComponent();

            m_engineCore.PreConstructor();
            Cursor.Hide();
        }

        public GameWindow(Int32 width, Int32 height, EngineCore engineCore)
            : this(engineCore)
        {
            this.Width = width;
            this.Height = height;
        }

        #endregion

        #region FormLoad & GLControlPaint events

        private void OnRender(object sender, PaintEventArgs e)
        {
            m_engineCore.EngineRender(this.Location);

            GLControl.SwapBuffers();
            GLControl.Invalidate();
        }

        #endregion

        #region Form Move&Resize events

        private void OnResize(object sender, EventArgs e)
        {
            m_engineCore.EngineWindowResized(new Point(0, 0), GLControl.Size, this.Size);
            GLControl.Invalidate();
        }

        private void OnMove(object sender, EventArgs e)
        {
            m_engineCore.EngineWindowLocationChanged(this.Location);
        }
        #endregion

        #region Mouse events

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            Point childOffset = UiMouseEventHandlerHalper.GetChildLocationOffsetAtWindow(GLControl, this);
            childOffset.Offset(e.Location);

            m_engineCore.EngineMouseMove(childOffset, GLControl.Size);

            GLControl.Update();
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            m_engineCore.EngineMouseDown(e.Button);
        }

        private void OnMouseWheel(object sender, MouseEventArgs e)
        {
            m_engineCore.EngineMouseWheel(e.Delta);
            GLControl.Update();
        }
        #endregion

        #region Key events

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) // arrow keys event
        {
            m_engineCore.EngineCmdKeyboardKeyDown(keyData);
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
            else
            {
                m_engineCore.EngineKeyboardKeyDown(e.KeyCode);
            }
        }

        private void OnKeyPress(object sender, System.Windows.Forms.KeyPressEventArgs args)
        {
            m_engineCore.EngineKeyboardKeyPress(args.KeyChar);
        }

        private void OnKeyUp(object sender, KeyEventArgs args)
        {
            m_engineCore.EngineKeyboadKeyUp(args.KeyCode);
        }

        #endregion

        #region Closing events

        private void OnClosing(object sender, FormClosingEventArgs e)
        {
            base.OnClosing(e);
            m_engineCore.CleanEverythingUp();
            Log.AddToFileStreamLog(String.Format("\nTime elapsed : {0}", DateTime.Now - EngineStatics.ElapsedTime));
            Environment.Exit(0);
        }

#endregion

    }
}

