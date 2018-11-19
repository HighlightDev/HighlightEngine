using MassiveGame.Core.GameCore;
using MassiveGame.Core.GameCore.Entities.StaticEntities;
using MassiveGame.Settings;
using OpenTK;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace MassiveGame.Engine
{
    public class EngineCore
    {
        // here has to be created and held game or editor window
        private Form m_UiWindow = null;

#if !COLLISION_EDITOR
        private bool bPostConstructor = true;

        private RenderThread m_renderThread;

        private GameThread m_gameThread;

        private Stopwatch m_renderTickTime;
#endif

        public EngineCore()
        {
#if !COLLISION_EDITOR
            Point startScreenRezoluion = new Point(1400, 800);
            Action preConstructorFunction = new Action(preConstructor), renderQueueFunction = new Action(RenderQueue), cleanUpFunction = new Action(cleanEverythingUp);
#endif

#if DESIGN_EDITOR
            m_UiWindow = new UI.EditorWindow(startScreenRezoluion.X, startScreenRezoluion.Y, preConstructorFunction, renderQueueFunction, cleanUpFunction);
#elif COLLISION_EDITOR
            m_UiWindow = new UI.CollisionEditorWindow();
#else
            m_UiWindow = new UI.GameWindow(startScreenRezoluion.X, startScreenRezoluion.Y, preConstructorFunction, renderQueueFunction, cleanUpFunction);
#endif
            m_UiWindow.Left = EngineStatics.SCREEN_POSITION_X;
            m_UiWindow.Top = EngineStatics.SCREEN_POSITION_Y;
        }

        public void ShowUiWindow()
        {
            m_UiWindow.ShowDialog();
        }

        public void CloseUiWindow()
        {
            m_UiWindow.Close();
        }

#if !COLLISION_EDITOR
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

        private void LoadKeyboardBindings()
        {
            KeyboardBindingsLoader bindingsLoader = new KeyboardBindingsLoader();
            bindingsLoader.SetKeyboardBindings();
        }

        private void preConstructor() { }

        private void postConstructor()
        {
            if (bPostConstructor)
            {
                // TODO -> test game level
                GameWorld.GetWorldInstance().LoadTestLevel();
                //
                SetProjectionMatrixToDefault();
                EngineStatics.PrevCursorPosition = new Point(-1, -1);
                EngineStatics.ElapsedTime = DateTime.Now;
                LoadIniSettings();
                LoadKeyboardBindings();
                m_renderTickTime = new Stopwatch();
                // Start game and render thread execution
                m_renderThread = new RenderThread();
                // Every frame capture time of draw call execution
                m_renderTickTime.Start();
                m_gameThread = new GameThread(100, 1);
            }
        }

        private void RenderQueue()
        {
            postConstructor();
            m_renderTickTime.Restart();
            m_renderThread.ThreadExecution(EngineStatics.globalSettings.ActualScreenRezolution, bPostConstructor);
            EngineStatics.RENDER_TIME = (float)m_renderTickTime.Elapsed.TotalSeconds;
            bPostConstructor = false;
        }

        #region Cleaning

        private void cleanEverythingUp()
        {
            if (GameWorld.GetWorldInstance().GetLevel().Water != null) GameWorld.GetWorldInstance().GetLevel().Water.cleanUp();
            if (GameWorld.GetWorldInstance().GetLevel().SunRenderer != null) GameWorld.GetWorldInstance().GetLevel().SunRenderer.GetData().cleanUp();
            if (GameWorld.GetWorldInstance().GetLevel().Terrain != null) GameWorld.GetWorldInstance().GetLevel().Terrain.cleanUp();
            if (GameWorld.GetWorldInstance().GetLevel().Player != null) GameWorld.GetWorldInstance().GetLevel().Player.GetData().CleanUp();
            if (GameWorld.GetWorldInstance().GetLevel().Bots != null) foreach (var bot in GameWorld.GetWorldInstance().GetLevel().Bots) { bot.CleanUp(); }
            if (GameWorld.GetWorldInstance().GetLevel().Grass != null) GameWorld.GetWorldInstance().GetLevel().Grass.cleanUp();
            if (GameWorld.GetWorldInstance().GetLevel().Plant != null) GameWorld.GetWorldInstance().GetLevel().Plant.cleanUp();
            if (GameWorld.GetWorldInstance().GetLevel().StaticMeshCollection != null) foreach (Building house in GameWorld.GetWorldInstance().GetLevel().StaticMeshCollection) { house.CleanUp(); }
            if (GameWorld.GetWorldInstance().GetLevel().Skybox != null) GameWorld.GetWorldInstance().GetLevel().Skybox.cleanUp();
        }

        #endregion
    
#endif
    }
}
