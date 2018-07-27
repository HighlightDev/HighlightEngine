using System.Collections.Generic;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using TextureLoader;
using MassiveGame.Core.GameCore;
using MassiveGame.Core.RenderCore.Lights;
using MassiveGame.Settings;
using VBO;
using MassiveGame.API.ResourcePool.PoolHandling;
using MassiveGame.API.ResourcePool.Policies;
using MassiveGame.API.ResourcePool;

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
                this._shader = PoolProxy.GetResource<ObtainShaderPool, ShaderAllocationPolicy<PointLightDebugShader>, string, PointLightDebugShader>(ProjectFolders.ShadersPath + "lampVS.glsl" + "," + ProjectFolders.ShadersPath + "lampFS.glsl" + "," + ProjectFolders.ShadersPath + "lampGS.glsl");

                float[,] vertices = new float[1, 3];

                VertexBufferObjectTwoDimension<float> verticesVBO = new VertexBufferObjectTwoDimension<float>(vertices, BufferTarget.ArrayBuffer, 0, 3, VertexBufferObjectBase.DataCarryFlag.Invalidate);
                _buffer = new VertexArrayObject();
                _buffer.AddVBO(verticesVBO);
                _buffer.BindBuffersToVao();
                _postConstructor = false;
            }
        }

        public PointLightsDebugRenderer(string LampTexture, List<PointLight> lamps)
        {
            this._texture = PoolProxy.GetResource<ObtainTexturePool, TextureAllocationPolicy, string, ITexture>(LampTexture);
            this._lamps = lamps;
            this._postConstructor = true;
        }

        public void cleanUp()
        {
            PoolProxy.FreeResourceMemoryByValue<ObtainShaderPool, ShaderAllocationPolicy<PointLightDebugShader>, string, PointLightDebugShader>(_shader);
            PoolProxy.FreeResourceMemoryByValue<ObtainTexturePool, TextureAllocationPolicy, string, ITexture>(_texture);
            _buffer.CleanUp();
        }
    }
}
