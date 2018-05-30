using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassiveGame.Physics
{
    public static class BodyMechanics
    {
        public static float FallTime = 0.05f;
        public static Vector3 G = new Vector3(0, -9.8f, 0);

        public static float COS_MAX_REACHABLE_INCLINE = 0.266f;

        public static float GetFreeFallDistanceInVelocity(Vector3 Velocity)
        {
            return (Velocity * FallTime + 0.5f * FallTime * FallTime * G).Length;
        }

        public static Vector3 UpdateFreeFallPosition(Vector3 Position, Vector3 Velocity)
        {
            return Position + Velocity * FallTime + 0.5f * FallTime * FallTime * G;
        }

        public static Vector3 GetFreeFallVelocity(Vector3 Velocity)
        {
            return Velocity * FallTime + 0.5f * FallTime * FallTime * G;
        }

        public static Vector3 UpdateFreeFallVelocity(Vector3 Velocity)
        {
            return Velocity + G * FallTime;
        }

    }
}
