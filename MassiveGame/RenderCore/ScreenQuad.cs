using GpuGraphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassiveGame.RenderCore
{
    public static class ScreenQuad
    {
        private static VBOArrayF quadVertices;
        private static VAO quadBuffer;

        public static VAO GetScreenQuadBuffer()
        {
            return quadBuffer;
        }

        static ScreenQuad()
        {
            /*Screen fill quad*/
            quadVertices = new VBOArrayF(
                new float[6, 3] { { -1.0f, -1.0f, 0.0f },
                { 1.0f, -1.0f, 0.0f },
                { 1.0f, 1.0f, 0.0f },
                { 1.0f, 1.0f, 0.0f },
                { -1.0f, 1.0f, 0.0f },
                { -1.0f, -1.0f, 0.0f} },
                new float[6, 2] { { 0, 1 },
                { 1, 1 },
                { 1, 0 },
                { 1, 0 },
                { 0, 0 },
                { 0, 1 } },
                null
                );

            quadBuffer = new VAO(quadVertices);
            VAOManager.genVAO(quadBuffer);
            VAOManager.setBufferData(OpenTK.Graphics.OpenGL.BufferTarget.ArrayBuffer, quadBuffer);
        }
        
    }
}
