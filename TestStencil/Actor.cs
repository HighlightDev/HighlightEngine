using CParser;
using GpuGraphics;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestStencil
{
    public class Actor
    {
        private BaseShader shader;
        private VAO buffer;

        public Vector3 Scale { set; get; }
        public Vector3 Translation { set; get; }

        public void Render(ref Matrix4 projectionMatrix, ref Matrix4 viewMatrix, ref Vector3 color)
        {
            Matrix4 worldMatrix = Matrix4.Identity;
            worldMatrix *= Matrix4.CreateScale(Scale);
            worldMatrix *= Matrix4.CreateTranslation(Translation);

            shader.startProgram();
            shader.SetColor(color);
            shader.SetTransformationMatrices(ref worldMatrix, ref viewMatrix, ref projectionMatrix);
            VAOManager.renderBuffers(buffer, OpenTK.Graphics.OpenGL.PrimitiveType.Triangles);
            shader.stopProgram();
        }

        public Actor(string modelPath)
        {
            ModelLoader loader = new ModelLoader(modelPath);
            VBOArrayF array = new VBOArrayF(loader.Verts, loader.T_Verts, null);
            buffer = new VAO(array);
            VAOManager.genVAO(buffer);
            VAOManager.setBufferData(OpenTK.Graphics.OpenGL.BufferTarget.ArrayBuffer, buffer);
            shader = new BaseShader(@"C:\Users\dzinovev\Desktop\Универчик\project\MassiveGame\MassiveGame\MassiveGame\TestStencil\Shaders\basicVS.glsl",
                @"C:\Users\dzinovev\Desktop\Универчик\project\MassiveGame\MassiveGame\MassiveGame\TestStencil\Shaders\basicFS.glsl");
            Scale = new Vector3();
            Translation = new Vector3();
        }
    }
}
