using System;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using MassiveGame.RenderCore;

namespace MassiveGame.UI
{
    public partial class MainUI : Form
    {
        private Stopwatch renderTime = new Stopwatch();

        #region FormLoad & GLControlPaint events

        private void OnLoad(object sender, EventArgs e)
        {
            renderTime.Start();
        }

        private void OnRender(object sender, PaintEventArgs e)
        {
            AdjustMouseCursor();
            renderTime.Restart();
            RenderFrame(renderTime);
            GLControl.Invalidate();
        }

        #endregion

        #region Form Move&Resize events

        private void OnResize(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, this.Width, this.Height);
            defaultMatrixSettings();
            GLControl.Invalidate();
            if (defaultFB != null)
            {
                defaultFB.CleanUp();
            }
            defaultFB = new DefaultFrameBuffer(DOUEngine.ScreenRezolution.X, DOUEngine.ScreenRezolution.Y);
        }

        private void OnMove(object sender, EventArgs e)
        {
            DOUEngine.SCREEN_POSITION_X = this.Left;
            DOUEngine.SCREEN_POSITION_Y = this.Top;
        }
        #endregion

        #region Mouse events
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (DOUEngine.Camera.SwitchCamera)
            {
                DOUEngine.Camera.RotateCameraByMouse(e.X, e.Y, GLControl.Width, GLControl.Height);
                //Cursor.Hide();

                if ((DOUEngine.PrevCursorPosition.X != -1) && (DOUEngine.PrevCursorPosition.Y != -1)) // need to calculate delta of mouse position
                {
                    int xDelta = e.X - DOUEngine.PrevCursorPosition.X;
                    int yDelta = e.Y - DOUEngine.PrevCursorPosition.Y;

                    /*True - enable blur
                    False - do nothing*/
                    if ((xDelta > 40 || yDelta > 40) || (xDelta < -40 || yDelta < -40))
                    {
                        blurEnable = true;
                    }
                }

                DOUEngine.PrevCursorPosition = e.Location;
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
                        DOUEngine.Camera.SwitchCamera = !DOUEngine.Camera.SwitchCamera;
                        break;
                    }
            }
        }

        private void OnMouseWheel(object sender, MouseEventArgs e)
        {
            //if (DOUEngine.PostProc != null)
            //{
            //    if (e.Delta > 0)
            //    {
            //        DOUEngine.PostProc.BlurWidth--;
            //        DOUEngine.PostProc.BloomThreshold -= 0.1f;
            //    }
            //    if (e.Delta < 0)
            //    {
            //        DOUEngine.PostProc.BlurWidth++;
            //        DOUEngine.PostProc.BloomThreshold += 0.1f;
            //    }
            //}
            //else
            //{
            //    if (e.Delta > 0)
            //    {
            //        DOUEngine.Camera.setThirdPersonZoom(-1);
            //    }
            //    if (e.Delta < 0)
            //    {
            //        DOUEngine.Camera.setThirdPersonZoom(1);
            //    }
            //}

            if (DOUEngine.DayCycle != null)
            {
                if (e.Delta > 0)
                {
                    DOUEngine.DayCycle.TimeFlow += 0.05f;
                }
                if (e.Delta < 0)
                {
                    DOUEngine.DayCycle.TimeFlow -= 0.05f;
                }
            }
        }
        #endregion

        #region Key events
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) // arrow keys event
        {
            switch (keyData)
            {
                case Keys.Up: DOUEngine.Camera.moveCamera(CAMERA_DIRECTIONS.FORWARD); return true;
                case Keys.Down: DOUEngine.Camera.moveCamera(CAMERA_DIRECTIONS.BACK); return true;
                case Keys.Left: DOUEngine.Camera.moveCamera(CAMERA_DIRECTIONS.LEFT); return true;
                case Keys.Right: DOUEngine.Camera.moveCamera(CAMERA_DIRECTIONS.RIGHT); return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                #region In-game settings
                case Keys.Back: DOUEngine.CollisionBoxRender = !DOUEngine.CollisionBoxRender; break;
                case Keys.R:
                    {

                        break;
                    }
                case Keys.N: DOUEngine.NormalMapTrigger = !DOUEngine.NormalMapTrigger; break;
                case Keys.F:
                    {
                        DOUEngine.FirstPersonCameraTrigger = !DOUEngine.FirstPersonCameraTrigger;

                        // TODO : Это пизда 
                        // -> правки нахуй

                        if (DOUEngine.FirstPersonCameraTrigger)
                        {
                            DOUEngine.Camera.SetFirstPerson();
                        }
                        else
                        {
                            DOUEngine.Camera.SetThirdPerson(DOUEngine.Player);
                            DOUEngine.Camera.movePosition(0f, 10f); // need to move in right direction
                        }

                        break;
                    }
                case Keys.M:   //Меняем типы полигонов
                    {
                        DOUEngine.ShowLightSource = !DOUEngine.ShowLightSource;
                        if (DOUEngine.Mode == PrimitiveType.Triangles)
                        {
                            DOUEngine.Mode = PrimitiveType.Lines;
                        }
                        else
                        {
                            DOUEngine.Mode = PrimitiveType.Triangles;
                        }
                        break;
                    }
                case Keys.Escape: this.Close(); break;//Exit
                case Keys.Add:
                    {
                        DOUEngine.Water.WaveSpeed += 0.1f;
                        DOUEngine.Water.WaveStrength += 0.1f;
                        break;
                    }
                case Keys.Subtract:
                    {
                        DOUEngine.Water.WaveSpeed -= 0.1f;
                        DOUEngine.Water.WaveStrength -= 0.1f;
                        break;
                    }
                    #endregion
            }
        }

        private void OnKeyPress(object sender, System.Windows.Forms.KeyPressEventArgs args)
        {
            if      (args.KeyChar == 'W' || args.KeyChar == 'w')
            { DOUEngine.keyboardMask[0] = true; }
            else if (args.KeyChar == 'A' || args.KeyChar == 'a')
            { DOUEngine.keyboardMask[1] = true; }
            else if (args.KeyChar == 'S' || args.KeyChar == 's')
            { DOUEngine.keyboardMask[2] = true; }
            else if (args.KeyChar == 'D' || args.KeyChar == 'd')
            { DOUEngine.keyboardMask[3] = true; }
            else if (args.KeyChar == ' ')
            {
                DOUEngine.keyboardMask[4] = true;
            }
        }

        private void OnKeyUp(object sender, KeyEventArgs args)
        {
            switch (args.KeyData)
            {
                case Keys.W: { DOUEngine.keyboardMask[0] = false; break; }
                case Keys.A: { DOUEngine.keyboardMask[1] = false; break; }
                case Keys.S: { DOUEngine.keyboardMask[2] = false; break; }
                case Keys.D: { DOUEngine.keyboardMask[3] = false; break; }
                case Keys.Space: { DOUEngine.keyboardMask[4] = false; break; }
            }
        }

        #endregion

        #region Closing events
        private void OnClosing(object sender, FormClosingEventArgs e)
        {
            base.OnClosing(e);
            unsubscribeCollisions();
            cleanEverythingUp();
            Debug.Log.addToLog(String.Format("\nTime elapsed : {0}", DateTime.Now - DOUEngine.ElapsedTime));
            Environment.Exit(0);
        }
        #endregion
    }
}
