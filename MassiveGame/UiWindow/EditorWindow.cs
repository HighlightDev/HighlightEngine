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
            m_engineCore.EngineWindowResized(new Point(0, 0), GLControl.Size);
            EngineStatics.globalSettings.ActualScreenRezolution = new Point(this.Width, this.Height);
            GLControl.Invalidate();
        }

        private void OnMove(object sender, EventArgs e)
        {
            EngineStatics.ScreenLocation.X = this.Left;
            EngineStatics.ScreenLocation.Y = this.Top;
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
        }

        private void OnMouseWheel(object sender, MouseEventArgs e)
        {
        }
        #endregion

        #region Key events

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) // arrow keys event
        {
            FirstPersonCamera firstPersonCamera = GameWorld.GetWorldInstance().GetLevel().Camera as FirstPersonCamera;
            if (firstPersonCamera != null)
            {
                switch (keyData)
                {
                    case Keys.Up: firstPersonCamera.moveCamera(CameraDirections.FORWARD); return true;
                    case Keys.Down: firstPersonCamera.moveCamera(CameraDirections.BACK); return true;
                    case Keys.Left: firstPersonCamera.moveCamera(CameraDirections.LEFT); return true;
                    case Keys.Right: firstPersonCamera.moveCamera(CameraDirections.RIGHT); return true;
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        Int32 renderTargetIndex = 0;

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                #region In-game settings
                case Keys.R:
                    {
                        break;
                    }
                case Keys.M:
                    {
                        break;
                    }
                case Keys.Escape: this.Close(); break;
                case Keys.Add:
                    {
                        break;
                    }
                case Keys.Subtract:
                    {
                        break;
                    }
                case Keys.Insert:
                    {
                        GameWorld.GetWorldInstance().GetUiFrameCreator().PushFrame((new GetRenderTargetPool().GetPool() as RenderTargetPool).GetRenderTargetAt(renderTargetIndex));
                        Int32 count = PoolProxy.GetResourceCountInPool<GetRenderTargetPool>();
                        if (renderTargetIndex + 1 >= count)
                        {
                            renderTargetIndex = 0;
                        }
                        else
                        {
                            ++renderTargetIndex;
                        }
                        break;
                    }
                    #endregion
            }
        }

        private void OnKeyPress(object sender, System.Windows.Forms.KeyPressEventArgs args)
        {
        }

        private void OnKeyUp(object sender, KeyEventArgs args)
        {
         
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
            Debug.Log.AddToFileStreamLog(String.Format("\nTime elapsed : {0}", DateTime.Now - EngineStatics.ElapsedTime));
        }

        #endregion

    }
}
