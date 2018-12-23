using System;
using System.Drawing;
using System.Windows.Forms;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using MassiveGame.Core.GameCore;
using MassiveGame.API.ResourcePool.PoolHandling;
using MassiveGame.API.ResourcePool.Pools;
using MassiveGame.API.ResourcePool;
using MassiveGame.EngineEditor.Core.UiContent;
using MassiveGame.UiWindow;
using MassiveGame.EngineEditor.Core.Entities;
using MassiveGame.API.MouseObjectDetector;
using MassiveGame.Core.MathCore;
using MassiveGame.Core.MathCore.MathTypes;
using MassiveGame.Engine;
using MassiveGame.Core.DebugCore;

namespace MassiveGame.UI
{
    public partial class EditorWindow : Form
    {
        private EngineCore m_engineCore = null;
        private bool bOpenglControlMousePressed = false;

        #region Constructors

        private EditorWindow(EngineCore engineCore)
        {
            m_engineCore = engineCore;
            Application.EnableVisualStyles();
            InitializeComponent();

            GLControl.MouseEnter += 
                (sender, e) => { GLControl.Focus(); };
            m_engineCore.PreConstructor();
        }

        public EditorWindow(Int32 width, Int32 height, EngineCore engineCore)
            : this(engineCore)
        {
            this.Width = width;
            this.Height = height;
            //ToggleFullscreenMode();
        }

        private void ToggleFullscreenMode()
        {
            if (WindowState == FormWindowState.Maximized)
            {
                //FormBorderStyle = FormBorderStyle.Fixed3D;
                WindowState = FormWindowState.Normal;
            }
            else
            {
                //FormBorderStyle = FormBorderStyle.None;
                WindowState = FormWindowState.Maximized;
            }
        }

        #endregion

        #region FormLoad & GLControlPaint events

        private void OnRender(object sender, PaintEventArgs e)
        {
            m_engineCore.EngineRender(this.Location);

            if (previewEntity != null && GameWorld.GetWorldInstance().GetLevel() != null && GameWorld.GetWorldInstance().GetLevel().Camera != null && previewEntity.IsOwnedByEntity())
            {
                previewEntity.Render(GameWorld.GetWorldInstance().GetLevel().Camera, ref EngineStatics.ProjectionMatrix);
            }

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

        PreviewEntity previewEntity;
        MousePicker mousePicker;

        public static Point MOUSE_POSITION;

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (GameWorld.GetWorldInstance().GetLevel() == null)
                return;

            BaseCamera camera = GameWorld.GetWorldInstance().GetLevel().Camera;

            if (mousePicker == null && camera != null)
            {
                mousePicker = new MousePicker(EngineStatics.ProjectionMatrix, camera);
            }
            if (mousePicker != null)
            {
                mousePicker.Tick(0);
            }

            if (previewEntity != null && camera != null)
            {
                Point location = e.Location;
                MOUSE_POSITION = location;
                FRay MouseRay = new FRay(camera.GetEyeVector(), mousePicker.currentRay);
                var intersectionPosition = GeometryMath.GetIntersectionRayPlane(new FPlane(new Vector3(0, 0, 0), new Vector3(0, 1, 0)), MouseRay);
                previewEntity.SetTranslation(ref intersectionPosition);
            }

            if (bOpenglControlMousePressed)
            {
                Point mouseLocation = UiMouseEventHandlerHalper.GetChildLocationOffsetAtWindow(GLControl, this);
                mouseLocation.Offset(e.Location);
                m_engineCore.EngineMouseMove(mouseLocation, GLControl.Size);

                GLControl.Update(); // need to update frame after invalidation to redraw changes
            }
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    {
                        
                        break;
                    }
                case MouseButtons.Right:
                    {
                        bOpenglControlMousePressed = true;
                        Cursor.Hide();
                        break;
                    }
            }

            m_engineCore.EngineMouseDown(e.Button);
        }

        private void GLControl_MouseUp(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    {
                       
                        break;
                    }
                case MouseButtons.Right:
                    {
                        bOpenglControlMousePressed = false;
                        Cursor.Show();
                        Cursor = Cursors.Arrow;
                        break;
                    }
            }

            m_engineCore.EngineMouseUp(e.Button);
        }

        private void OnMouseWheel(object sender, MouseEventArgs e)
        {
            m_engineCore.EngineMouseWheel(e.Delta);
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
            m_engineCore.EngineKeyboardKeyDown(e.KeyCode);
        }

        private void OnKeyPress(object sender, System.Windows.Forms.KeyPressEventArgs args)
        {
            m_engineCore.EngineKeyboardKeyPress(args.KeyChar);
        }

        private void OnKeyUp(object sender, KeyEventArgs args)
        {
            m_engineCore.EngineKeyboadKeyUp(args.KeyCode);
        }

        private void elementHost1_ChildChanged(object sender, System.Windows.Forms.Integration.ChildChangedEventArgs e)
        {
            EngineObjectEntryView engineView = new EngineObjectEntryView();
            engineView.TestCreateEntries();

            //engineEntityListBox.DataContext = engineView;
            //engineEntityListBox.EntryStackPanelMouseDownEventFire += new Action<string>(MeshListBoxEventCatched);
        }

        private void MeshListBoxEventCatched(string entity)
        {
            if (previewEntity == null)
                previewEntity = new PreviewEntity(true);
        }

        #endregion

        #region Closing events

        private void OnClosing(object sender, FormClosingEventArgs e)
        {
            base.OnClosing(e);
            m_engineCore.CleanEverythingUp();
            Log.AddToFileStreamLog(String.Format("\nTime elapsed : {0}", DateTime.Now - EngineStatics.ElapsedTime));
        }

        #endregion

    }
}
