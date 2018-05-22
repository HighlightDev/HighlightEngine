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
        public static Vector3 G = new Vector3(0, -9.8f, 0);

        public static Vector3 UpdateFreeFallPosition(Vector3 Position, float time, Vector3 Velocity)
        {
            return Position + Velocity * time + 0.5f * time * time * G;
        }

        public static Vector3 UpdateFreeFallVelocity(Vector3 Velocity, float time)
        {
            return Velocity + G * time;
        }

    }
}
