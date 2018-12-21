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

namespace MassiveGame.UI
{
#if DEBUG
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
            m_engineCore.EngineWindowResized(new Point(0, 0), GLControl.Size);
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

            if (GameWorld.GetWorldInstance().GetLevel() != null)
            {
                BaseCamera camera = GameWorld.GetWorldInstance().GetLevel().Camera;
                if (camera != null && camera.SwitchCamera)
                    Cursor.Hide();
                else
                    Cursor.Show();
            }

            m_engineCore.EngineMouseMove(childOffset, GLControl.Size);

            GLControl.Update(); // need to update frame after invalidation to redraw changes
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    {
                        //mist.fade(this.RenderTime, 10000, FadeType.LINEARLY, 0.0f);
                        //PlantUnit plant = new PlantUnit(TerrainIntersaction.getIntersactionPoint(EngineSingleton.Map, EngineSingleton.Picker.currentRay, EngineSingleton.Camera.getPosition()), new Vector3(), new Vector3(10), 0, null);
                        //EngineSingleton.Grass.add(plant, EngineSingleton.Map);

                        break;
                    }
                case MouseButtons.Right:
                    {
                        //mist.aEngineSingleton.PostProcear(this.RenderTime, 10000, FadeType.LINEARLY, 0.016f);
                        GameWorld.GetWorldInstance().GetLevel().Camera.SwitchCamera = !GameWorld.GetWorldInstance().GetLevel().Camera.SwitchCamera;
                        break;
                    }
            }
        }

        private void OnMouseWheel(object sender, MouseEventArgs e)
        {
            //if (EngineStatics.DayCycle != null)
            //{
            //    if (e.Delta > 0)
            //    {
            //        EngineStatics.DayCycle.TimeFlow += 0.1f;
            //    }
            //    else if (e.Delta < 0 && EngineStatics.DayCycle.TimeFlow > 0)
            //    {
            //        EngineStatics.DayCycle.TimeFlow -= 0.1f;
            //    }
            //    else if (EngineStatics.DayCycle.TimeFlow < 0)
            //    {
            //        EngineStatics.DayCycle.TimeFlow = 0.0f;
            //    }
            //}
            if (e.Delta > 0)
            {
                (GameWorld.GetWorldInstance().GetLevel().Camera as ThirdPersonCamera).MaxDistanceFromTargetToCamera += 5;
            }
            else if (e.Delta < 0)
            {
                (GameWorld.GetWorldInstance().GetLevel().Camera as ThirdPersonCamera).MaxDistanceFromTargetToCamera -= 5;
            }
            GLControl.Update();
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
                        if (EngineStatics.Mode == PrimitiveType.Triangles)
                        {
                            EngineStatics.Mode = PrimitiveType.Lines;
                        }
                        else
                        {
                            EngineStatics.Mode = PrimitiveType.Triangles;
                        }
                        break;
                    }
                case Keys.Escape: this.Close(); break;//Exit
                case Keys.Add:
                    {
                        GameWorld.GetWorldInstance().GetLevel().Water.GetData().WaveSpeed += 0.1f;
                        GameWorld.GetWorldInstance().GetLevel().Water.GetData().WaveStrength += 0.1f;
                        break;
                    }
                case Keys.Subtract:
                    {
                        GameWorld.GetWorldInstance().GetLevel().Water.GetData().WaveSpeed -= 0.1f;
                        GameWorld.GetWorldInstance().GetLevel().Water.GetData().WaveStrength -= 0.1f;
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
            Keys key = (Keys)char.ToUpper(args.KeyChar);
            GameWorld.GetWorldInstance().GetLevel().PlayerController.GetKeyboardHandler().KeyPress(key);
        }

        private void OnKeyUp(object sender, KeyEventArgs args)
        {
            if (GameWorld.GetWorldInstance() != null && GameWorld.GetWorldInstance().GetLevel() != null)
                GameWorld.GetWorldInstance().GetLevel().PlayerController.GetKeyboardHandler().KeyRelease(args.KeyData);
        }

        #endregion

        #region Closing events

        private void OnClosing(object sender, FormClosingEventArgs e)
        {
            base.OnClosing(e);
            m_engineCore.CleanEverythingUp();
            Debug.Log.AddToFileStreamLog(String.Format("\nTime elapsed : {0}", DateTime.Now - EngineStatics.ElapsedTime));
            Environment.Exit(0);
        }

        #endregion

    }
#endif
}

