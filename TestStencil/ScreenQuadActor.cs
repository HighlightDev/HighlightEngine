using GpuGraphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace TestStencil
{
    public class ScreenQuadActor
    {
        private VAO buffer;
        private VBOArrayF quadVertices;
        private ScreenQuadShader shader;

        public void Render(Int32 textureHandler)
        {
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, textureHandler);

            shader.startProgram();
            shader.SetTexture(0);
            VAOManager.renderBuffers(buffer, OpenTK.Graphics.OpenGL.PrimitiveType.Triangles);
            shader.stopProgram();
        }

        public ScreenQuadActor()
        {
            shader = new ScreenQuadShader(@"C:\Users\dzinovev\Desktop\Универчик\project\MassiveGame\MassiveGame\MassiveGame\TestStencil\Shaders\screenQuadVS.glsl",
                @"C:\Users\dzinovev\Desktop\Универчик\project\MassiveGame\MassiveGame\MassiveGame\TestStencil\Shaders\screenQuadFS.glsl");

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

            buffer = new VAO(quadVertices);
            VAOManager.genVAO(buffer);
            VAOManager.setBufferData(OpenTK.Graphics.OpenGL.BufferTarget.ArrayBuffer, buffer);
        }
    }
}
