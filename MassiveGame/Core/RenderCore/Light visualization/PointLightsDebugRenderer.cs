using System.Collections.Generic;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using TextureLoader;
using GpuGraphics;
using MassiveGame.API.Collector;
using MassiveGame.Core.GameCore;
using MassiveGame.Core.RenderCore.Lights;
using MassiveGame.Settings;
using VBO;

namespace MassiveGame.Core.RenderCore.Light_visualization
{
    public class PointLightsDebugRenderer
    {
        private ITexture _texture;
        private PointLightDebugShader _shader;
        private bool _postConstructor;
        private VertexArrayObject _buffer;
        private List<PointLight> _lamps;

        public void Render(BaseCamera camera, Matrix4 projectionMatrix)
        {
            postConstructor();

            foreach (PointLight lamp in _lamps)
            {
                Matrix4 modelMatrix = Matrix4.CreateTranslation(lamp.Position.Xyz);
                _shader.startProgram();
                _texture.BindTexture(TextureUnit.Texture0);
                _shader.setUniformValues(modelMatrix, camera.GetViewMatrix(), projectionMatrix, 0);
                _buffer.RenderVAO(PrimitiveType.Points);
                _shader.stopProgram();
            }
        }

        private void postConstructor()
        {
            if (_postConstructor)
            {
                this._shader = ResourcePool.GetShaderProgram<PointLightDebugShader>(ProjectFolders.ShadersPath + "lampVS.glsl",
                    ProjectFolders.ShadersPath + "lampFS.glsl", ProjectFolders.ShadersPath + "lampGS.glsl");

                float[,] vertices = new float[1, 3];

                VertexBufferObject<float> verticesVBO = new VertexBufferObject<float>(vertices, BufferTarget.ArrayBuffer, 0, 3, VertexBufferObjectBase.DataCarryFlag.Invalidate);
                _buffer = new VertexArrayObject();
                _buffer.AddVBO(verticesVBO);
                _buffer.BindVbosToVao();
                _postConstructor = false;
            }
        }

        public PointLightsDebugRenderer(string LampTexture, List<PointLight> lamps)
        {
            this._texture = ResourcePool.GetTexture(LampTexture);
            this._lamps = lamps;
            this._postConstructor = true;
        }

        public void cleanUp()
        {
            this._shader.cleanUp();
            ResourcePool.ReleaseTexture(_texture);
            _buffer.CleanUp();
        }
    }
}
