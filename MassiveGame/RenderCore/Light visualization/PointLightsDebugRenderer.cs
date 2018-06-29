using System.Collections.Generic;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using TextureLoader;
using GpuGraphics;
using MassiveGame.API.Collector;
using MassiveGame.RenderCore.Lights;
using MassiveGame.Core;

namespace MassiveGame.Light_visualization
{
    public class PointLightsDebugRenderer
    {
        private ITexture _texture;
        private PointLightDebugShader _shader;
        private bool _postConstructor;
        private VBOArrayF _attributes;
        private VAO _buffer;
        private List<PointLight> _lamps;

        public void Render(Camera camera, Matrix4 projectionMatrix)
        {
            postConstructor();

            foreach (PointLight lamp in _lamps)
            {
                Matrix4 modelMatrix = Matrix4.CreateTranslation(lamp.Position.Xyz);
                _shader.startProgram();
                _texture.BindTexture(TextureUnit.Texture0);
                _shader.setUniformValues(modelMatrix, camera.getViewMatrix(), projectionMatrix, 0);
                VAOManager.renderBuffers(_buffer, PrimitiveType.Points);
                _shader.stopProgram();
            }
        }

        private void postConstructor()
        {
            if (_postConstructor)
            {
                this._shader = (PointLightDebugShader)ResourcePool.GetShaderProgram(ProjectFolders.ShadersPath + "lampVS.glsl",
                    ProjectFolders.ShadersPath + "lampFS.glsl", ProjectFolders.ShadersPath + "lampGS.glsl", typeof(PointLightDebugShader));
                this._buffer = new VAO(_attributes);
                VAOManager.genVAO(_buffer);
                VAOManager.setBufferData(BufferTarget.ArrayBuffer, _buffer);
                _postConstructor = false;
            }
        }

        public PointLightsDebugRenderer(string LampTexture, List<PointLight> lamps)
        {
            this._texture = ResourcePool.GetTexture(LampTexture);
            this._lamps = lamps;
            this._attributes = new VBOArrayF(new float[1, 3]);
            this._postConstructor = true;
        }

        public void cleanUp()
        {
            this._shader.cleanUp();
            ResourcePool.ReleaseTexture(_texture);
            VAOManager.cleanUp(_buffer);
        }
    }
}
