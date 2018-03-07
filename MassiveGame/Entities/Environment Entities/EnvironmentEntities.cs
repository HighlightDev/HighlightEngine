using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using GpuGraphics;
using TextureLoader;
using MassiveGame.API.Collector;

namespace MassiveGame
{
    public sealed class EnvironmentEntities : Entity
    {
        #region Definitions

        private ITexture _envMap;
        private EnvironmentEntitiesShader _shader;

        #endregion

        #region Render

        public void render(Camera camera, ref Matrix4 projectionMatrix)
        {
            postConstructor();

            Matrix4 modelMatrix = Matrix4.Identity;

            _shader.startProgram();
            _texture.BindTexture(TextureUnit.Texture0);
            _envMap.BindTexture(TextureUnit.Texture1);
            _shader.setUniformValues(ref modelMatrix, camera.getViewMatrix(), ref projectionMatrix, camera.getPosition(), 0, 1);
            VAOManager.renderBuffers(this._model.Buffer, PrimitiveType.Triangles);
            _shader.stopProgram();
        }

        #endregion

        #region Constructor

        private void postConstructor()
        {
            if (_postConstructor)
            {
                
                _postConstructor = !_postConstructor;
            }
        }
        public EnvironmentEntities(string modelPath, string texturePath, string normalMapPath, string specularMapPath, string[] cubemapEnvMap,
            Vector3 translation = new Vector3(), Vector3 rotation = new Vector3(), Vector3 scale = new Vector3())
            : base(modelPath, texturePath, normalMapPath, specularMapPath, translation, rotation, scale)
        {
            _shader = (EnvironmentEntitiesShader)ResourcePool.GetShaderProgram(ProjectFolders.ShadersPath + "envVS.glsl", ProjectFolders.ShadersPath + "envFS.glsl", "",
                    typeof(EnvironmentEntitiesShader));
            this._envMap = ResourcePool.GetTexture(cubemapEnvMap);
            this._postConstructor = true;
        }

        #endregion

        #region Cleaning

        public override void cleanUp()
        {
            base.cleanUp();
            ResourcePool.ReleaseTexture(_texture);
            ResourcePool.ReleaseShaderProgram(_shader);
        }

        #endregion
    }
}
