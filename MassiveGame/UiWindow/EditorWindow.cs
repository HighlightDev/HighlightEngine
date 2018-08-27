using System;
using System.Drawing;
using System.Windows.Forms;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using MassiveGame.Core.GameCore;
using MassiveGame.API.ResourcePool.PoolHandling;
using MassiveGame.API.ResourcePool.Pools;
using MassiveGame.API.ResourcePool;

namespace MassiveGame.UI
{
    public partial class EditorWindow : Form
    {
        private Action m_renderQueueFunction, m_cleanUpFunction;

        #region Constructors

        private EditorWindow(Action preConstructorFunction, Action renderQueueFunction, Action cleanUpFunction)
        {
            m_renderQueueFunction = renderQueueFunction;
            m_cleanUpFunction = cleanUpFunction;

            Application.EnableVisualStyles();
            InitializeComponent();
            preConstructorFunction();
        }

        public EditorWindow(Int32 width, Int32 height, Action preConstructorFunction, Action renderQueueFunction, Action cleanUpFunction)
            : this(preConstructorFunction, renderQueueFunction, cleanUpFunction)
        {
            this.Width = width;
            this.Height = height;
        }

        #endregion

        #region FormLoad & GLControlPaint events

        private void OnRender(object sender, PaintEventArgs e)
        {
            // Maybe somehow I can remove this trick
            AdjustMouseCursor();

            m_renderQueueFunction();
            GLControl.SwapBuffers();
            GLControl.Invalidate();
        }

        #endregion

        #region Form Move&Resize events

        private void OnResize(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, this.Width, this.Height);
            EngineStatics.globalSettings.ActualScreenRezolution = new Point(this.Width, this.Height);
            GLControl.Invalidate();
        }

        private void OnMove(object sender, EventArgs e)
        {
            EngineStatics.SCREEN_POSITION_X = this.Left;
            EngineStatics.SCREEN_POSITION_Y = this.Top;
        }
        #endregion

        #region Mouse events

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (EngineStatics.Camera.SwitchCamera)
            {
                EngineStatics.Camera.Rotate(e.X, e.Y, new Point(Width, GLControl.Height));
                Cursor.Hide();

                if ((EngineStatics.PrevCursorPosition.X != -1) && (EngineStatics.PrevCursorPosition.Y != -1)) // need to calculate delta of mouse position
                {
                    Int32 xDelta = e.X - EngineStatics.PrevCursorPosition.X;
                    Int32 yDelta = e.Y - EngineStatics.PrevCursorPosition.Y;
                }

                EngineStatics.PrevCursorPosition = e.Location;
            }
            else
            {
                Cursor.Show();
                Cursor.Draw(this.CreateGraphics(),
                    new Rectangle(this.Location.X, this.Location.Y, this.Size.Width, this.Size.Height));
            }

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
                        EngineStatics.Camera.SwitchCamera = !EngineStatics.Camera.SwitchCamera;
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
            //        EngineStatics.DayCycle.TimeFlow += 0.01f;
            //    }
            //    else if (e.Delta < 0 && EngineStatics.DayCycle.TimeFlow > 0)
            //    {
            //        EngineStatics.DayCycle.TimeFlow -= 0.01f;
            //    }
            //    else if (EngineStatics.DayCycle.TimeFlow < 0)
            //    {
            //        EngineStatics.DayCycle.TimeFlow = 0.0f;
            //    }
            //}
            if (e.Delta > 0)
            {
                (EngineStatics.Camera as ThirdPersonCamera).SeekDistanceFromTargetToCamera += 5;
            }
            else if (e.Delta < 0)
            {
                (EngineStatics.Camera as ThirdPersonCamera).SeekDistanceFromTargetToCamera -= 5;
            }

        }
        #endregion

        private void AdjustMouseCursor()
        {
            EngineStatics.SCREEN_POSITION_X = this.Location.X + 8;
            EngineStatics.SCREEN_POSITION_Y = this.Location.Y + 8;
            //для корректной работы камеры с учетом рамки
            //+ 8 из - за того, что при открытии на полный экран, смещение стартовой позиции окна = -8
            EngineStatics.SCREEN_POSITION_X = ((EngineStatics.WINDOW_BORDER != WindowBorder.Hidden) && (EngineStatics.WINDOW_STATE != OpenTK.WindowState.Fullscreen))
                ? this.Location.X + 8 : this.Location.X;
            EngineStatics.SCREEN_POSITION_Y = ((EngineStatics.WINDOW_BORDER != WindowBorder.Hidden) && (EngineStatics.WINDOW_STATE != OpenTK.WindowState.Fullscreen))
                ? this.Location.Y + 8 : this.Location.Y;
        }

        #region Key events

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) // arrow keys event
        {
            FirstPersonCamera firstPersonCamera = EngineStatics.Camera as FirstPersonCamera;
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
                case Keys.N: EngineStatics.NormalMapTrigger = !EngineStatics.NormalMapTrigger; break;
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
                        EngineStatics.Water.WaveSpeed += 0.1f;
                        EngineStatics.Water.WaveStrength += 0.1f;
                        break;
                    }
                case Keys.Subtract:
                    {
                        EngineStatics.Water.WaveSpeed -= 0.1f;
                        EngineStatics.Water.WaveStrength -= 0.1f;
                        break;
                    }
                case Keys.Insert:
                    {
                        EngineStatics.uiFrameCreator.PushFrame((new ObtainRenderTargetPool().GetPool() as RenderTargetPool).GetRenderTargetAt(renderTargetIndex));
                        Int32 count = PoolProxy.GetResourceCountInPool<ObtainRenderTargetPool>();
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
            EngineStatics.playerController.GetKeyboardHandler().KeyPress(key);
        }

        private void OnKeyUp(object sender, KeyEventArgs args)
        {
            EngineStatics.playerController.GetKeyboardHandler().KeyRelease(args.KeyData);
        }

        #endregion

        #region Closing events

        private void OnClosing(object sender, FormClosingEventArgs e)
        {
            base.OnClosing(e);
            m_cleanUpFunction();
            Debug.Log.AddToFileStreamLog(String.Format("\nTime elapsed : {0}", DateTime.Now - EngineStatics.ElapsedTime));
            Environment.Exit(0);
        }

        #endregion

    }
}
