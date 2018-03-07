using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace PhysicsBox
{
    public sealed class CollisionSphereBox
    {
        public Vector3 LBNCoordinates { set; get; }
        public Vector3 RTFCoordinates { set; get; }
        public float Radius { set; get; }
        public int ID { set; get; }
        public Vector4 Color { set; get; }
        private float[,] _renderCoordinates;

        public void renderBox(Matrix4 modelViewMatrix, ref Matrix4 projectionMatrix)
        {
            _renderCoordinates[0, 0] = LBNCoordinates.X; _renderCoordinates[0, 1] = LBNCoordinates.Y; _renderCoordinates[0, 2] = LBNCoordinates.Z;
            _renderCoordinates[1, 0] = RTFCoordinates.X; _renderCoordinates[1, 1] = LBNCoordinates.Y; _renderCoordinates[1, 2] = LBNCoordinates.Z;
            _renderCoordinates[2, 0] = RTFCoordinates.X; _renderCoordinates[2, 1] = RTFCoordinates.Y; _renderCoordinates[2, 2] = LBNCoordinates.Z;
            _renderCoordinates[3, 0] = LBNCoordinates.X; _renderCoordinates[3, 1] = RTFCoordinates.Y; _renderCoordinates[3, 2] = LBNCoordinates.Z;

            _renderCoordinates[4, 0] = RTFCoordinates.X; _renderCoordinates[4, 1] = LBNCoordinates.Y; _renderCoordinates[4, 2] = LBNCoordinates.Z;
            _renderCoordinates[5, 0] = RTFCoordinates.X; _renderCoordinates[5, 1] = LBNCoordinates.Y; _renderCoordinates[5, 2] = RTFCoordinates.Z;
            _renderCoordinates[6, 0] = RTFCoordinates.X; _renderCoordinates[6, 1] = RTFCoordinates.Y; _renderCoordinates[6, 2] = RTFCoordinates.Z;
            _renderCoordinates[7, 0] = RTFCoordinates.X; _renderCoordinates[7, 1] = RTFCoordinates.Y; _renderCoordinates[7, 2] = LBNCoordinates.Z;

            _renderCoordinates[8, 0] = RTFCoordinates.X; _renderCoordinates[8, 1] = LBNCoordinates.Y; _renderCoordinates[8, 2] = RTFCoordinates.Z;
            _renderCoordinates[9, 0] = LBNCoordinates.X; _renderCoordinates[9, 1] = LBNCoordinates.Y; _renderCoordinates[9, 2] = RTFCoordinates.Z;
            _renderCoordinates[10, 0] = LBNCoordinates.X; _renderCoordinates[10, 1] = RTFCoordinates.Y; _renderCoordinates[10, 2] = RTFCoordinates.Z;
            _renderCoordinates[11, 0] = RTFCoordinates.X; _renderCoordinates[11, 1] = RTFCoordinates.Y; _renderCoordinates[11, 2] = RTFCoordinates.Z;

            _renderCoordinates[12, 0] = LBNCoordinates.X; _renderCoordinates[12, 1] = LBNCoordinates.Y; _renderCoordinates[12, 2] = RTFCoordinates.Z;
            _renderCoordinates[13, 0] = LBNCoordinates.X; _renderCoordinates[13, 1] = LBNCoordinates.Y; _renderCoordinates[13, 2] = LBNCoordinates.Z;
            _renderCoordinates[14, 0] = LBNCoordinates.X; _renderCoordinates[14, 1] = RTFCoordinates.Y; _renderCoordinates[14, 2] = LBNCoordinates.Z;
            _renderCoordinates[15, 0] = LBNCoordinates.X; _renderCoordinates[15, 1] = RTFCoordinates.Y; _renderCoordinates[15, 2] = RTFCoordinates.Z;

            _renderCoordinates[16, 0] = LBNCoordinates.X; _renderCoordinates[16, 1] = LBNCoordinates.Y; _renderCoordinates[16, 2] = LBNCoordinates.Z;
            _renderCoordinates[17, 0] = RTFCoordinates.X; _renderCoordinates[17, 1] = LBNCoordinates.Y; _renderCoordinates[17, 2] = LBNCoordinates.Z;
            _renderCoordinates[18, 0] = RTFCoordinates.X; _renderCoordinates[18, 1] = LBNCoordinates.Y; _renderCoordinates[18, 2] = RTFCoordinates.Z;
            _renderCoordinates[19, 0] = LBNCoordinates.X; _renderCoordinates[19, 1] = LBNCoordinates.Y; _renderCoordinates[19, 2] = RTFCoordinates.Z;

            _renderCoordinates[20, 0] = LBNCoordinates.X; _renderCoordinates[20, 1] = RTFCoordinates.Y; _renderCoordinates[20, 2] = LBNCoordinates.Z;
            _renderCoordinates[21, 0] = RTFCoordinates.X; _renderCoordinates[21, 1] = RTFCoordinates.Y; _renderCoordinates[21, 2] = LBNCoordinates.Z;
            _renderCoordinates[22, 0] = RTFCoordinates.X; _renderCoordinates[22, 1] = RTFCoordinates.Y; _renderCoordinates[22, 2] = RTFCoordinates.Z;
            _renderCoordinates[23, 0] = LBNCoordinates.X; _renderCoordinates[23, 1] = RTFCoordinates.Y; _renderCoordinates[23, 2] = RTFCoordinates.Z;

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projectionMatrix);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelViewMatrix);
            GL.Color4(Color);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.VertexPointer(3, VertexPointerType.Float, 0, _renderCoordinates);
            GL.DrawArrays(PrimitiveType.LineStrip, 0, _renderCoordinates.Length / 3);
            GL.DisableClientState(ArrayCap.VertexArray);
        }

        public void synchronizeCoordinates(float XL, float XR, float YB, float YT, float ZN, float ZF)
        {
            LBNCoordinates = new Vector3(XL, YB, ZN);
            RTFCoordinates = new Vector3(XR, YT, ZF);
        }

        public void synchronizeCoordinates(Vector3 lbn, Vector3 rtf)
        {
            LBNCoordinates = lbn;
            RTFCoordinates = rtf;
        }

        public Vector3 getCenter()
        {
            return new Vector3(((RTFCoordinates.X - LBNCoordinates.X) / 2) + LBNCoordinates.X,
                ((RTFCoordinates.Y - LBNCoordinates.Y) / 2) + LBNCoordinates.Y,
                ((RTFCoordinates.Z - LBNCoordinates.Z) / 2) + LBNCoordinates.Z);
        }

        private float getRadius()
        {
            return NestedMath.Max3((RTFCoordinates.X - LBNCoordinates.X) / 2,
                (RTFCoordinates.Y - LBNCoordinates.Y) / 2,
                (RTFCoordinates.Z - LBNCoordinates.Z) / 2);
        }

        private void Init()
        {
            _renderCoordinates = new float[24, 3];
            Color = new Vector4(1.0f, 0.0f, 0.0f, 1.0f);
            this.Radius = getRadius();
        }

        public CollisionSphereBox(float XL, float XR, float YB, float YT, float ZN, float ZF, int ID)
        {
            this.ID = ID;
            LBNCoordinates = new Vector3(XL, YB, ZN);
            RTFCoordinates = new Vector3(XR, YT, ZF);
            Init();
        }

        public CollisionSphereBox(Vector3 lbn, Vector3 rtf, int ID)
        {
            this.ID = ID;
            LBNCoordinates = lbn;
            RTFCoordinates = rtf;
            Init();
        }
     
        private static class NestedMath
        {
            public static float Max(float left, float right)
            {
                return (left > right ? left : right);
            }

            public static float Max3(float left, float middle, float right)
            {
                return (left > middle ? left > right ? left : right : middle > right ? middle : right);
            }
        }

    }
}
