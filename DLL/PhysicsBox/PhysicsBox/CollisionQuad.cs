using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace PhysicsBox
{
    public sealed class CollisionQuad
    {
        public Vector3 LBNCoordinates { set; get; }
        public Vector3 RTFCoordinates { set; get; }
        public int ID { set; get; }
        public Vector4 Color { set; get; }
        private float[,] _renderCoordinates;

        public void renderQuad(Matrix4 modelViewMatrix, ref Matrix4 projectionMatrix)
        {
            _renderCoordinates[0, 0] = LBNCoordinates.X; _renderCoordinates[0, 1] = LBNCoordinates.Y; _renderCoordinates[0, 2] = LBNCoordinates.Z;
            _renderCoordinates[1, 0] = RTFCoordinates.X; _renderCoordinates[1, 1] = LBNCoordinates.Y; _renderCoordinates[1, 2] = RTFCoordinates.Z;
            _renderCoordinates[2, 0] = RTFCoordinates.X; _renderCoordinates[2, 1] = RTFCoordinates.Y; _renderCoordinates[2, 2] = RTFCoordinates.Z;
            _renderCoordinates[3, 0] = LBNCoordinates.X; _renderCoordinates[3, 1] = RTFCoordinates.Y; _renderCoordinates[3, 2] = LBNCoordinates.Z;

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

        public Vector3 getCenter()
        {
            return new Vector3(((RTFCoordinates.X - LBNCoordinates.X) / 2) + LBNCoordinates.X,
                ((RTFCoordinates.Y - LBNCoordinates.Y) / 2) + LBNCoordinates.Y,
                ((RTFCoordinates.Z - LBNCoordinates.Z) / 2) + LBNCoordinates.Z);
        }

        public float getRadius()
        {
            return (RTFCoordinates.X - LBNCoordinates.X) / 2;
        }

        public CollisionQuad(float XL, float XR, float YB, float YT, float ZN, float ZF)
        {
            this.ID = ID;
            LBNCoordinates = new Vector3(XL, YB, ZN);
            RTFCoordinates = new Vector3(XR, YT, ZF);
            _renderCoordinates = new float[4, 3];
            Color = new Vector4(1.0f, 0.0f, 0.0f, 1.0f);
        }
    }
}
