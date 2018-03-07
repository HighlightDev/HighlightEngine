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
        public Vector2 LBCoordinates { set; get; }
        public Vector2 RTCoordinates { set; get; }
        public float ZCoordinate { set; get; }
        public int ID { set; get; }
        public Vector4 Color { set; get; }
        private float[,] _renderCoordinates;

        public void renderQuad(Matrix4 modelViewMatrix, ref Matrix4 projectionMatrix)
        {
            _renderCoordinates[0, 0] = LBCoordinates.X; _renderCoordinates[0, 1] = LBCoordinates.Y; _renderCoordinates[0, 2] = ZCoordinate;
            _renderCoordinates[1, 0] = RTCoordinates.X; _renderCoordinates[1, 1] = LBCoordinates.Y; _renderCoordinates[1, 2] = ZCoordinate;
            _renderCoordinates[2, 0] = RTCoordinates.X; _renderCoordinates[2, 1] = RTCoordinates.Y; _renderCoordinates[2, 2] = ZCoordinate;
            _renderCoordinates[3, 0] = LBCoordinates.X; _renderCoordinates[3, 1] = RTCoordinates.Y; _renderCoordinates[3, 2] = ZCoordinate;

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

        public void synchronizeCoordinates(float XL, float XR, float YB, float YT, float Z)
        {
            LBCoordinates = new Vector2(XL, YB);
            RTCoordinates = new Vector2(XR, YT);
            this.ZCoordinate = Z;
        }

        public Vector3 getCenter()
        {
            return new Vector3(((RTCoordinates.X - LBCoordinates.X) / 2) + LBCoordinates.X,
                ((RTCoordinates.Y - LBCoordinates.Y) / 2) + LBCoordinates.Y,
                ZCoordinate);
        }

        public float getRadius()
        {
            return (RTCoordinates.X - LBCoordinates.X) / 2;
        }

        public CollisionQuad(float XL, float XR, float YB, float YT, float Z, int ID)
        {
            this.ID = ID;
            LBCoordinates = new Vector2(XL, YB);
            RTCoordinates = new Vector2(XR, YT);
            ZCoordinate = Z;
            _renderCoordinates = new float[4, 3];
            Color = new Vector4(1.0f, 0.0f, 0.0f, 1.0f);
        }
    }
}
