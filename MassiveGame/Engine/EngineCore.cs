using MassiveGame.Core.GameCore;
using MassiveGame.Core.GameCore.Entities.StaticEntities;
using MassiveGame.Core.ioCore;
using MassiveGame.Core.SettingsCore;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace MassiveGame.Engine
{
    public class EngineCore
    {
        public EngineCore() { }

        private IOManager m_ioManager;

        private bool bPostConstructor = true;

        private RenderThread m_renderThread;

        private GameThread m_gameThread;

        private Stopwatch m_renderTickTime;

        public void ProcessIO()
        {
            m_ioManager.ProcessConsoleCommands();
        }

        private void SetProjectionMatrixToDefault()
        {
            // create projection matrix
            EngineStatics.ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(EngineStatics.FoV), EngineStatics.SCREEN_ASPECT_RATIO,
                EngineStatics.NEAR_CLIPPING_PLANE, EngineStatics.FAR_CLIPPING_PLANE);
        }

        private void LoadIniSettings()
        {
            SettingsLoader settingsLoader = new SettingsLoader();
            settingsLoader.SetGlobalSettings();
        }

#if DEBUG
        private void LoadKeyboardBindings()
        {
            KeyboardBindingsLoader bindingsLoader = new KeyboardBindingsLoader();
            bindingsLoader.SetKeyboardBindings();
        }
#endif

        public void PreConstructor() { }

        private void PostConstructor()
        {
            if (bPostConstructor)
            {
#if ENGINE_EDITOR
                GameWorld.GetWorldInstance().CreateEditorDefaultLevel();
#elif DEBUG
                // TODO -> test game level
                GameWorld.GetWorldInstance().LoadTestLevel();
                //
                LoadKeyboardBindings();
#endif
                LoadIniSettings();
                GameWorld.GetWorldInstance().PostInit();
                SetProjectionMatrixToDefault();
                EngineStatics.PrevCursorPosition = new Point(-1, -1);
                EngineStatics.ElapsedTime = DateTime.Now;

                m_renderTickTime = new Stopwatch();
                // Start game and render thread execution
                m_renderThread = new RenderThread();
                // Every frame capture time of draw call execution
                m_renderTickTime.Start();
                m_gameThread = new GameThread(100, 1);
                m_ioManager = new IOManager();

            }
        }

        private void RenderQueue()
        {
            PostConstructor();
            ProcessIO();
            m_renderTickTime.Restart();
            m_renderThread.ThreadExecution(EngineStatics.globalSettings.ActualScreenRezolution, bPostConstructor);
            EngineStatics.RENDER_TIME = (float)m_renderTickTime.Elapsed.TotalSeconds;
            bPostConstructor = false;
        }

        #region Cleaning

        public void CleanEverythingUp()
        {
            GameWorld.GetWorldInstance().GetLevel().Water.GetData()?.CleanUp();
            GameWorld.GetWorldInstance().GetLevel().SunRenderer.GetData()?.cleanUp();
            GameWorld.GetWorldInstance().GetLevel().Terrain.GetData()?.cleanUp();
            GameWorld.GetWorldInstance().GetLevel().Player.GetData()?.CleanUp();
            foreach (Core.GameCore.Entities.MoveEntities.MovableMeshEntity bot in GameWorld.GetWorldInstance().GetLevel().Bots) { bot.CleanUp(); }
            if (GameWorld.GetWorldInstance().GetLevel().Grass != null) GameWorld.GetWorldInstance().GetLevel().Grass.cleanUp();
            if (GameWorld.GetWorldInstance().GetLevel().Plant != null) GameWorld.GetWorldInstance().GetLevel().Plant.cleanUp();
            if (GameWorld.GetWorldInstance().GetLevel().StaticMeshCollection != null)
            {
                foreach (Building house in GameWorld.GetWorldInstance().GetLevel().StaticMeshCollection) { house.CleanUp(); }
            }

            if (GameWorld.GetWorldInstance().GetLevel().Skybox != null) GameWorld.GetWorldInstance().GetLevel().Skybox.cleanUp();
        }

        #endregion

        private void AdjustMouseCursor(ref Point actualScreenLocation)
        {
            if ((EngineStatics.WINDOW_BORDER != WindowBorder.Hidden) && (EngineStatics.WINDOW_STATE != OpenTK.WindowState.Fullscreen))
                EngineStatics.ScreenLocation = new Point(actualScreenLocation.X + 8, actualScreenLocation.Y + 8);
            else
                EngineStatics.ScreenLocation = actualScreenLocation;
        }

        #region Methods accessors for window

        public void EngineWindowResized(Point windowLocation, Size newGLWindowSize, Size newWindowSize)
        {
            GL.Viewport(windowLocation, newGLWindowSize);
            EngineStatics.globalSettings.ActualScreenRezolution = new Point(newWindowSize.Width, newWindowSize.Height);
        }

        public void EngineWindowLocationChanged(Point newLocation)
        {

        }

        public void EngineRender(Point actualScreenLocation)
        {
            // Maybe somehow I can remove this trick
            AdjustMouseCursor(ref actualScreenLocation);
            RenderQueue();
        }

        public void EngineMouseDown(MouseButtons mouseButton)
        {
            switch (mouseButton)
            {
                case MouseButtons.Left:
                    {
                        break;
                    }
                case MouseButtons.Right:
                    {
                        break;
                    }
            }
        }

        public void EngineMouseUp(MouseButtons mouseButton)
        {

        }

        public void EngineMouseMove(Point mouseLocation, Size glWindowSize)
        {
            if (GameWorld.GetWorldInstance().GetLevel() != null && GameWorld.GetWorldInstance().GetLevel().Camera != null)
            {
                if (EngineStatics.PrevCursorPosition != mouseLocation)
                {
                    EngineStatics.PrevCursorPosition = mouseLocation;
                    GameWorld.GetWorldInstance().GetLevel().Camera.RotateFacade(mouseLocation, glWindowSize);
                }
            }
        }

        public void EngineMouseWheel(float wheelDelta)
        {
#if DEBUG
            if (wheelDelta > 0)
            {
                (GameWorld.GetWorldInstance().GetLevel().Camera as ThirdPersonCamera).MaxDistanceFromTargetToCamera += 5;
            }
            else if (wheelDelta < 0)
            {
                (GameWorld.GetWorldInstance().GetLevel().Camera as ThirdPersonCamera).MaxDistanceFromTargetToCamera -= 5;
            }
#endif
        }

        public void EngineKeyboardKeyDown(Keys code)
        {
            switch (code)
            {
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
#if DEBUG || ENGINE_EDITOR || COLLISION_EDITOR
                        GameWorld.GetWorldInstance().GetUiFrameCreator().PushDebugRenderTarget();
#endif
                        break;
                    }
            }
        }

        public void EngineCmdKeyboardKeyDown(Keys cmdKey)
        {
#if ENGINE_EDITOR
            FirstPersonCamera firstPersonCamera = GameWorld.GetWorldInstance().GetLevel().Camera as FirstPersonCamera;
            if (firstPersonCamera != null)
            {
                switch (cmdKey)
                {
                    case Keys.Up: firstPersonCamera.moveCamera(CameraDirections.FORWARD); break;
                    case Keys.Down: firstPersonCamera.moveCamera(CameraDirections.BACK); break;
                    case Keys.Left: firstPersonCamera.moveCamera(CameraDirections.LEFT); break;
                    case Keys.Right: firstPersonCamera.moveCamera(CameraDirections.RIGHT); break;
                }
            }
#endif
        }

        public void EngineKeyboardKeyPress(char keyCharCode)
        {
            Keys key = (Keys)char.ToUpper(keyCharCode);
#if DEBUG
            GameWorld.GetWorldInstance().GetLevel().PlayerController.GetKeyboardHandler().KeyPress(key);
#endif
        }

        public void EngineKeyboadKeyUp(Keys keyCode)
        {
#if DEBUG
            if (GameWorld.GetWorldInstance() != null && GameWorld.GetWorldInstance().GetLevel() != null)
                GameWorld.GetWorldInstance().GetLevel().PlayerController.GetKeyboardHandler().KeyRelease(keyCode);
#endif

        }

#endregion
    }
}
