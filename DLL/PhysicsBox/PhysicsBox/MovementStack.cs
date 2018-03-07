using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace PhysicsBox
{
    public sealed class MovementStack
    {
        public float LeftX;
        public float RightX;
        public float BottomY;
        public float TopY;
        public float NearZ;
        public float FarZ;
        public float MoveX;
        public float MoveZ;
        public float MoveY;

        public MovementStack(float LX, float RX, float BY, float TY, float NZ, float FZ)
        {
            LeftX = LX;
            RightX = RX;
            BottomY = BY;
            TopY = TY;
            NearZ = NZ;
            FarZ = FZ;
        }

        public void resetPositionValues(float MX, float MY, float MZ)
        {
            MoveX = MX;
            MoveY = MY;
            MoveZ = MZ;
        }

        public Vector3 popPositionValues()
        {
            return new Vector3(MoveX, MoveY, MoveZ);
        }
    }
}
