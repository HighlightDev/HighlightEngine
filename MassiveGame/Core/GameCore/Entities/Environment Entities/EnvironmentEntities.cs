using OpenTK;
using OpenTK.Graphics.OpenGL;
using TextureLoader;
using MassiveGame.Settings;
using MassiveGame.API.ResourcePool.PoolHandling;
using MassiveGame.API.ResourcePool.Policies;
using MassiveGame.API.ResourcePool;

namespace MassiveGame.Core.GameCore.Entities.EnvironmentEntities
{
    public sealed class EnvironmentEntities : Entity
    {
        #region Definitions

        private ITexture _envMap;
        private EnvironmentEntitiesShader _shader;

        #endregion

        #region Render

        public void render(BaseCamera camera, ref Matrix4 projectionMatrix)
        {
            postConstructor();

            Matrix4 modelMatrix = Matrix4.Identity;

            _shader.startProgram();
            _texture.BindTexture(TextureUnit.Texture0);
            _envMap.BindTexture(TextureUnit.Texture1);
            _shader.setUniformValues(ref modelMatrix, camera.GetViewMatrix(), ref projectionMatrix, camera.GetEyeVector(), 0, 1);
            _model.Buffer.RenderVAO(PrimitiveType.Triangles);
            _shader.stopProgram();
        }

        #endregion

        #region Constructor

        private void postConstructor()
        {
            if (bPostConstructor)
            {
                
                bPostConstructor = !bPostConstructor;
            }
        }
        public EnvironmentEntities(string modelPath, string texturePath, string normalMapPath, string specularMapPath, string[] cubemapEnvMap,
            Vector3 translation = new Vector3(), Vector3 rotation = new Vector3(), Vector3 scale = new Vector3())
            : base(modelPath, texturePath, normalMapPath, specularMapPath, translation, rotation, scale)
        {
            _shader = PoolProxy.GetResource<ObtainShaderPool, ShaderAllocationPolicy<EnvironmentEntitiesShader>, string, EnvironmentEntitiesShader>(ProjectFolders.ShadersPath + "envVS.glsl" + "," + ProjectFolders.ShadersPath + "envFS.glsl");
            this._envMap = PoolProxy.GetResource<ObtainTexturePool, TextureAllocationPolicy, string, ITexture>(cubemapEnvMap[0] + "," + cubemapEnvMap[1] + "," + cubemapEnvMap[2] + "," +
                cubemapEnvMap[3] + "," + cubemapEnvMap[4] + "," + cubemapEnvMap[5]);
            this.bPostConstructor = true;
        }

        #endregion

        #region Cleaning

        public override void cleanUp()
        {
            base.cleanUp();
            PoolProxy.FreeResourceMemoryByValue<ObtainTexturePool, TextureAllocationPolicy, string, ITexture>(_texture);
            PoolProxy.FreeResourceMemoryByValue<ObtainShaderPool, ShaderAllocationPolicy<EnvironmentEntitiesShader>, string, EnvironmentEntitiesShader>(_shader);
        }

        #endregion
    }
}
