using OpenTK;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace MassiveGame.Engine
{
    public class GameThread
    {
        private System.Threading.Timer gameThreadTimer;
        private object lockGameThread = new object();
        private Stopwatch gameTickTime;

        public GameThread(Int32 delay, Int32 period)
        {
            gameThreadTimer = new System.Threading.Timer(new System.Threading.TimerCallback(ThreadExecution));
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
            Matrix4 viewMatrix = EngineStatics.Camera.GetViewMatrix();

            if (EngineStatics.Player != null)
            {
                EngineStatics.Player.Tick(ref EngineStatics.ProjectionMatrix, ref viewMatrix);
            }

            if (!Object.Equals(EngineStatics.Enemy, null))
            {
                EngineStatics.Enemy.Tick(ref EngineStatics.ProjectionMatrix, ref viewMatrix);
            }

            if (EngineStatics.City != null)
            {
                foreach (var item in EngineStatics.City)
                    item.Tick(ref EngineStatics.ProjectionMatrix, ref viewMatrix);
            }
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

                EngineStatics.Picker.Update();
                EngineStatics.Mist.Update();
                EngineStatics.DayCycle.UpdateTimeFlow();

                // Do smth better (PlayerController)
                EngineStatics.playerController.InvokeBindings();

                if (EngineStatics.SunReplica != null)
                    EngineStatics.SunReplica.UpdateFrustumCullingInfo();

                if (EngineStatics.Skybox != null)
                    EngineStatics.Skybox.UpdateAnimation(Convert.ToSingle(EngineStatics.RENDER_TIME));
            }
        }
    }
}
