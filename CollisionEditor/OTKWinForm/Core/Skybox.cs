using GpuGraphics;
using OpenTK;
using OTKWinForm.IOCore;
using OTKWinForm.RenderCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextureLoader;

namespace OTKWinForm.Core
{
    public class Skybox
    {
        private const float SKYBOX_SIZE = 200;
        private RawModel model;
        private bool bPostConstructor = true;
        private VBOArrayF attribs = null;
        private SkyboxShader shader;
        private ITexture cubemap;

        public void Render(Matrix4 viewMatrix, Matrix4 projectionMatrix)
        {
            if (bPostConstructor)
            {
                model = new RawModel(attribs);
                shader = new SkyboxShader(Environment.CurrentDirectory + "/../../Shaders/skyboxVS.glsl", Environment.CurrentDirectory + "/../../Shaders/skyboxFS.glsl");
                cubemap = ProxyTextureLoader.LoadCubemap(new string[] {
            Environment.CurrentDirectory + "/../../Textures/Right.bmp",
            Environment.CurrentDirectory + "/../../Textures/Left.bmp",
            Environment.CurrentDirectory + "/../../Textures/top.bmp",
            Environment.CurrentDirectory + "/../../Textures/bottom.bmp",
            Environment.CurrentDirectory + "/../../Textures/Back.bmp",
            Environment.CurrentDirectory + "/../../Textures/Front.bmp"
            });
                bPostConstructor = false;
            }

            shader.startProgram();
            cubemap.BindTexture(OpenTK.Graphics.OpenGL.TextureUnit.Texture0);
            shader.SetTransformationMatrices(viewMatrix, projectionMatrix);
            shader.SetCubemap(0);
            VAOManager.renderBuffers(model.Buffer, OpenTK.Graphics.OpenGL.PrimitiveType.Triangles);
            shader.stopProgram();
        }

        public Skybox()
        {
            attribs = new VBOArrayF(new float[6 * 6, 3] { { -SKYBOX_SIZE, SKYBOX_SIZE, -SKYBOX_SIZE }, { -SKYBOX_SIZE, -SKYBOX_SIZE, -SKYBOX_SIZE }, { SKYBOX_SIZE, -SKYBOX_SIZE, -SKYBOX_SIZE },
            { SKYBOX_SIZE, -SKYBOX_SIZE, -SKYBOX_SIZE },{ SKYBOX_SIZE, SKYBOX_SIZE, -SKYBOX_SIZE },{ -SKYBOX_SIZE, SKYBOX_SIZE, -SKYBOX_SIZE },{ -SKYBOX_SIZE, -SKYBOX_SIZE, SKYBOX_SIZE },
            { -SKYBOX_SIZE, -SKYBOX_SIZE, -SKYBOX_SIZE },{ -SKYBOX_SIZE, SKYBOX_SIZE, -SKYBOX_SIZE },{ -SKYBOX_SIZE, SKYBOX_SIZE, -SKYBOX_SIZE },{ -SKYBOX_SIZE, SKYBOX_SIZE, SKYBOX_SIZE },
            { -SKYBOX_SIZE, -SKYBOX_SIZE, SKYBOX_SIZE },{ SKYBOX_SIZE, -SKYBOX_SIZE, -SKYBOX_SIZE },{ SKYBOX_SIZE, -SKYBOX_SIZE, SKYBOX_SIZE },{ SKYBOX_SIZE, SKYBOX_SIZE, SKYBOX_SIZE },
            { SKYBOX_SIZE, SKYBOX_SIZE, SKYBOX_SIZE },{ SKYBOX_SIZE, SKYBOX_SIZE, -SKYBOX_SIZE },{ SKYBOX_SIZE, -SKYBOX_SIZE, -SKYBOX_SIZE },{ -SKYBOX_SIZE, -SKYBOX_SIZE, SKYBOX_SIZE },
            { -SKYBOX_SIZE, SKYBOX_SIZE, SKYBOX_SIZE },{ SKYBOX_SIZE, SKYBOX_SIZE, SKYBOX_SIZE },{ SKYBOX_SIZE, SKYBOX_SIZE, SKYBOX_SIZE },{ SKYBOX_SIZE, -SKYBOX_SIZE, SKYBOX_SIZE },
            { -SKYBOX_SIZE, -SKYBOX_SIZE, SKYBOX_SIZE },{ -SKYBOX_SIZE, SKYBOX_SIZE, -SKYBOX_SIZE },{ SKYBOX_SIZE, SKYBOX_SIZE, -SKYBOX_SIZE },{ SKYBOX_SIZE, SKYBOX_SIZE, SKYBOX_SIZE },
            { SKYBOX_SIZE, SKYBOX_SIZE, SKYBOX_SIZE },{ -SKYBOX_SIZE, SKYBOX_SIZE, SKYBOX_SIZE },{ -SKYBOX_SIZE, SKYBOX_SIZE, -SKYBOX_SIZE },{ -SKYBOX_SIZE, -SKYBOX_SIZE, -SKYBOX_SIZE },
            { -SKYBOX_SIZE, -SKYBOX_SIZE, SKYBOX_SIZE },{ SKYBOX_SIZE, -SKYBOX_SIZE, -SKYBOX_SIZE },{ SKYBOX_SIZE, -SKYBOX_SIZE, -SKYBOX_SIZE },{ -SKYBOX_SIZE, -SKYBOX_SIZE, SKYBOX_SIZE },
            { SKYBOX_SIZE, -SKYBOX_SIZE, SKYBOX_SIZE }});
           
        }

        public void CleanUp()
        {
            shader.cleanUp();
        }

    }
}
