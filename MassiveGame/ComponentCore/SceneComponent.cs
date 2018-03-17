using GpuGraphics;
using MassiveGame.API.Collector;
using OpenTK;
using PhysicsBox.ComponentCore;
using PhysicsBox.MathTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassiveGame.ComponentCore
{
    public class SceneComponent : Component
    {
        public void RenderBound(Matrix4 viewMatrix, ref Matrix4 projectionMatrix)
        {
            float[,] renderCoordinates = new float[24, 3];
            if (Bound is AABB)
            {

            }
            else if (Bound is OBB)
            {

            }

            //renderCoordinates[0, 0] = LBNCoordinates.X; _renderCoordinates[0, 1] = LBNCoordinates.Y; _renderCoordinates[0, 2] = LBNCoordinates.Z;
            //renderCoordinates[1, 0] = RTFCoordinates.X; _renderCoordinates[1, 1] = LBNCoordinates.Y; _renderCoordinates[1, 2] = LBNCoordinates.Z;
            //renderCoordinates[2, 0] = RTFCoordinates.X; _renderCoordinates[2, 1] = RTFCoordinates.Y; _renderCoordinates[2, 2] = LBNCoordinates.Z;
            //renderCoordinates[3, 0] = LBNCoordinates.X; _renderCoordinates[3, 1] = RTFCoordinates.Y; _renderCoordinates[3, 2] = LBNCoordinates.Z;

            //renderCoordinates[4, 0] = RTFCoordinates.X; _renderCoordinates[4, 1] = LBNCoordinates.Y; _renderCoordinates[4, 2] = LBNCoordinates.Z;
            //renderCoordinates[5, 0] = RTFCoordinates.X; _renderCoordinates[5, 1] = LBNCoordinates.Y; _renderCoordinates[5, 2] = RTFCoordinates.Z;
            //renderCoordinates[6, 0] = RTFCoordinates.X; _renderCoordinates[6, 1] = RTFCoordinates.Y; _renderCoordinates[6, 2] = RTFCoordinates.Z;
            //renderCoordinates[7, 0] = RTFCoordinates.X; _renderCoordinates[7, 1] = RTFCoordinates.Y; _renderCoordinates[7, 2] = LBNCoordinates.Z;

            //renderCoordinates[8, 0] = RTFCoordinates.X; _renderCoordinates[8, 1] = LBNCoordinates.Y; _renderCoordinates[8, 2] = RTFCoordinates.Z;
            //renderCoordinates[9, 0] = LBNCoordinates.X; _renderCoordinates[9, 1] = LBNCoordinates.Y; _renderCoordinates[9, 2] = RTFCoordinates.Z;
            //renderCoordinates[10, 0] = LBNCoordinates.X; _renderCoordinates[10, 1] = RTFCoordinates.Y; _renderCoordinates[10, 2] = RTFCoordinates.Z;
            //renderCoordinates[11, 0] = RTFCoordinates.X; _renderCoordinates[11, 1] = RTFCoordinates.Y; _renderCoordinates[11, 2] = RTFCoordinates.Z;

            //renderCoordinates[12, 0] = LBNCoordinates.X; _renderCoordinates[12, 1] = LBNCoordinates.Y; _renderCoordinates[12, 2] = RTFCoordinates.Z;
            //renderCoordinates[13, 0] = LBNCoordinates.X; _renderCoordinates[13, 1] = LBNCoordinates.Y; _renderCoordinates[13, 2] = LBNCoordinates.Z;
            //renderCoordinates[14, 0] = LBNCoordinates.X; _renderCoordinates[14, 1] = RTFCoordinates.Y; _renderCoordinates[14, 2] = LBNCoordinates.Z;
            //renderCoordinates[15, 0] = LBNCoordinates.X; _renderCoordinates[15, 1] = RTFCoordinates.Y; _renderCoordinates[15, 2] = RTFCoordinates.Z;

            //renderCoordinates[16, 0] = LBNCoordinates.X; _renderCoordinates[16, 1] = LBNCoordinates.Y; _renderCoordinates[16, 2] = LBNCoordinates.Z;
            //renderCoordinates[17, 0] = RTFCoordinates.X; _renderCoordinates[17, 1] = LBNCoordinates.Y; _renderCoordinates[17, 2] = LBNCoordinates.Z;
            //renderCoordinates[18, 0] = RTFCoordinates.X; _renderCoordinates[18, 1] = LBNCoordinates.Y; _renderCoordinates[18, 2] = RTFCoordinates.Z;
            //renderCoordinates[19, 0] = LBNCoordinates.X; _renderCoordinates[19, 1] = LBNCoordinates.Y; _renderCoordinates[19, 2] = RTFCoordinates.Z;

            //renderCoordinates[20, 0] = LBNCoordinates.X; _renderCoordinates[20, 1] = RTFCoordinates.Y; _renderCoordinates[20, 2] = LBNCoordinates.Z;
            //renderCoordinates[21, 0] = RTFCoordinates.X; _renderCoordinates[21, 1] = RTFCoordinates.Y; _renderCoordinates[21, 2] = LBNCoordinates.Z;
            //renderCoordinates[22, 0] = RTFCoordinates.X; _renderCoordinates[22, 1] = RTFCoordinates.Y; _renderCoordinates[22, 2] = RTFCoordinates.Z;
            //renderCoordinates[23, 0] = LBNCoordinates.X; _renderCoordinates[23, 1] = RTFCoordinates.Y; _renderCoordinates[23, 2] = RTFCoordinates.Z;

            //GL.MatrixMode(MatrixMode.Projection);
            //GL.LoadMatrix(ref projectionMatrix);
            //GL.MatrixMode(MatrixMode.Modelview);
            //GL.LoadMatrix(ref modelViewMatrix);
            //GL.Color4(Color);
            //GL.EnableClientState(ArrayCap.VertexArray);
            //GL.VertexPointer(3, VertexPointerType.Float, 0, _renderCoordinates);
            //GL.DrawArrays(PrimitiveType.LineStrip, 0, _renderCoordinates.Length / 3);
            //GL.DisableClientState(ArrayCap.VertexArray);
        }

        public SceneComponent()
        {

        }
    }
}
