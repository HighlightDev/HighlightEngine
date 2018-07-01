using OpenTK;
using System;
using System.Linq;
using System.Threading;

namespace MassiveGame.Engine
{
    public class GameThread
    {
        private System.Threading.Timer gameThreadTimer;
        private object lockGameThread = new object();

        public GameThread(Int32 delay, Int32 period)
        {
            gameThreadTimer = new System.Threading.Timer(new System.Threading.TimerCallback(ThreadExecution));
            gameThreadTimer.Change(delay, period);
        }

        public void SuspendGameThread()
        {
            gameThreadTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public void ContinueGameThread()
        {
            gameThreadTimer.Change(0, 20);
        }

        private void TickEntities()
        {
            Matrix4 viewMatrix = DOUEngine.Camera.GetViewMatrix();

            if (DOUEngine.Player != null)
            {
                DOUEngine.Player.Tick(ref DOUEngine.ProjectionMatrix, ref viewMatrix);
            }

            if (!Object.Equals(DOUEngine.Enemy, null))
            {
                DOUEngine.Enemy.Tick(ref DOUEngine.ProjectionMatrix, ref viewMatrix);
            }

            if (DOUEngine.City != null)
            {
                foreach (var item in DOUEngine.City)
                    item.Tick(ref DOUEngine.ProjectionMatrix, ref viewMatrix);
            }
        }

        private void ThreadExecution(object target)
        {
            // Forbid parallel game thread execution
            lock (lockGameThread)
            {
                TickEntities();

                DOUEngine.Picker.Update();
                DOUEngine.Mist.Update();
                //DOUEngine.Camera.Update(DOUEngine.terrain);
                DOUEngine.DayCycle.UpdateTimeFlow();

                // Do smth better (PlayerController)
                if (DOUEngine.keyboardMask.GetWASDKeysMask().Any<bool>((key) => key == true))
                {
                    var previousPosition = DOUEngine.Player.ComponentTranslation;

                    if (DOUEngine.Player != null)
                    {
                        DOUEngine.Player.MoveActor();
                    }
                    //DOUEngine.Camera.UpdateHeight(previousPosition);
                }

                if (DOUEngine.SunReplica != null)
                    DOUEngine.SunReplica.UpdateFrustumCullingInfo();

                if (DOUEngine.Skybox != null)
                    DOUEngine.Skybox.UpdateAnimation(Convert.ToSingle(DOUEngine.RENDER_TIME));
            }
        }
    }
}
