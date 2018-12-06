using MassiveGame.API.ResourcePool;
using MassiveGame.Core.GameCore;
using MassiveGame.Core.GameCore.Entities.StaticEntities;
using MassiveGame.Core.ioCore;
using MassiveGame.Settings;
using OpenTK;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using ConsoleCommands = MassiveGame.Core.ioCore.ConsoleCommandsManager.ConsoleCommands;

namespace MassiveGame.Engine
{
    public class EngineCore
    {
        public EngineCore() { }

#if !COLLISION_EDITOR
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
                LoadIniSettings();
                LoadKeyboardBindings();
#endif
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

        public void RenderQueue()
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

#endif
    }
}
