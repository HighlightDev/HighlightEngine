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
            EngineStatics.Player?.Tick(deltaTime);

            EngineStatics.Enemy?.Tick(deltaTime);

            if (EngineStatics.City != null)
            {
                foreach (var item in EngineStatics.City)
                    item.Tick(deltaTime);
            }

            EngineStatics.SkeletalMesh?.Tick(deltaTime);
            EngineStatics.Water?.Tick(deltaTime);
            EngineStatics.Plant1?.Tick(deltaTime);
            EngineStatics.Grass?.Tick(deltaTime);
            EngineStatics.Skybox?.Tick(deltaTime);
            EngineStatics.SunReplica?.Tick(deltaTime);
            EngineStatics.Picker?.Tick(deltaTime);
            EngineStatics.Mist?.Tick(deltaTime);
            EngineStatics.DayCycle?.Tick(deltaTime);
        }

        private void ThreadExecution(object target)
        {
            // Forbid parallel game thread execution
            lock (lockGameThread)
            {
                float deltaTime = (float)gameTickTime.Elapsed.TotalSeconds;
                gameTickTime.Restart();

                EngineStatics.Camera.CameraTick(deltaTime);
                TickEntities(deltaTime);

                EngineStatics.playerController.InvokeBindings();
            }
        }
    }
}
