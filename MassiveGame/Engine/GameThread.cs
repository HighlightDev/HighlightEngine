using MassiveGame.Core.GameCore;
using OpenTK;
using System;
using System.Diagnostics;
using System.Threading;

namespace MassiveGame.Engine
{
    public class GameThread
    {
        private Timer gameThreadTimer;
        private object lockGameThread = new object();
        private Stopwatch gameTickTime;

        public GameThread(Int32 delay, Int32 period)
        {
            gameThreadTimer = new Timer(new TimerCallback(ThreadExecution));
            gameThreadTimer.Change(delay, period);
            gameTickTime = new Stopwatch();
            gameTickTime.Start();
        }

        public void SuspendGameThread()
        {
            gameThreadTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public void ContinueGameThread()
        {
            gameThreadTimer.Change(0, 20);
        }

        private void TickEntities(float deltaTime)
        {
            GameWorld.GetWorldInstance().GetLevel().Player?.Tick(deltaTime);

            if (GameWorld.GetWorldInstance().GetLevel().Bots != null)
            {
                foreach (var bot in GameWorld.GetWorldInstance().GetLevel().Bots)
                {
                    bot.Tick(deltaTime);
                }
            }

            if (GameWorld.GetWorldInstance().GetLevel().StaticMeshCollection != null)
            {
                foreach (var item in GameWorld.GetWorldInstance().GetLevel().StaticMeshCollection)
                    item.Tick(deltaTime);
            }

            GameWorld.GetWorldInstance().GetLevel().SkeletalMesh?.Tick(deltaTime);
            GameWorld.GetWorldInstance().GetLevel().Water?.Tick(deltaTime);
            GameWorld.GetWorldInstance().GetLevel().Plant?.Tick(deltaTime);
            GameWorld.GetWorldInstance().GetLevel().Grass?.Tick(deltaTime);
            GameWorld.GetWorldInstance().GetLevel().Skybox?.Tick(deltaTime);
            GameWorld.GetWorldInstance().GetLevel().SunRenderer?.Tick(deltaTime);
#if DESIGN_EDITOR
            GameWorld.GetWorldInstance().GetLevel().Picker?.Tick(deltaTime);
#endif
            GameWorld.GetWorldInstance().GetLevel().Mist?.Tick(deltaTime);
            GameWorld.GetWorldInstance().GetLevel().DayCycle?.Tick(deltaTime);
        }

        private void ThreadExecution(object target)
        {
            // Forbid parallel game thread execution
            lock (lockGameThread)
            {
                float deltaTime = (float)gameTickTime.Elapsed.TotalSeconds;
                gameTickTime.Restart();

                GameWorld.GetWorldInstance().GetLevel().Camera.CameraTick(deltaTime);
                TickEntities(deltaTime);

                GameWorld.GetWorldInstance().GetLevel().PlayerController.InvokeBindings();
            }
        }
    }
}
