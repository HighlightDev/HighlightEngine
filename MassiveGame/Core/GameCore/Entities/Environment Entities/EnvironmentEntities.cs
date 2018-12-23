using OpenTK;
using OpenTK.Graphics.OpenGL;
using TextureLoader;
using MassiveGame.Core.SettingsCore;
using MassiveGame.API.ResourcePool.PoolHandling;
using MassiveGame.API.ResourcePool.Policies;
using MassiveGame.API.ResourcePool;

namespace MassiveGame.Core.GameCore.Entities.EnvironmentEntities
{
    public sealed class EnvironmentEntities : Entity
    {
        #region Definitions

        private ITexture m_envMap;

        #endregion

        #region Render

        public void render(BaseCamera camera, ref Matrix4 projectionMatrix)
        {
            postConstructor();

            Matrix4 modelMatrix = Matrix4.Identity;

            GetShader().startProgram();
            m_texture.BindTexture(TextureUnit.Texture0);
            m_envMap.BindTexture(TextureUnit.Texture1);
            GetShader().setUniformValues(ref modelMatrix, camera.GetViewMatrix(), ref projectionMatrix, camera.GetEyeVector(), 0, 1);
            m_skin.Buffer.RenderVAO(PrimitiveType.Triangles);
            GetShader().stopProgram();
        }

        #endregion

        #region Constructor

        private void postConstructor()
        {
            if (bPostConstructor)
                bPostConstructor = false;
        }
        public EnvironmentEntities(string modelPath, string texturePath, string normalMapPath, string specularMapPath, string[] cubemapEnvMap,
            Vector3 translation = new Vector3(), Vector3 rotation = new Vector3(), Vector3 scale = new Vector3())
            : base(modelPath, texturePath, normalMapPath, specularMapPath, translation, rotation, scale)
        { 
            this.m_envMap = PoolProxy.GetResource<GetTexturePool, TextureAllocationPolicy, string, ITexture>(cubemapEnvMap[0] + "," + cubemapEnvMap[1] + "," + cubemapEnvMap[2] + "," +
                cubemapEnvMap[3] + "," + cubemapEnvMap[4] + "," + cubemapEnvMap[5]);
        }

        #endregion
        
        private EnvironmentEntitiesShader GetShader()
        {
            return m_shader as EnvironmentEntitiesShader;
        }

        #region Cleaning

        public override void CleanUp()
        {
            base.CleanUp();
            PoolProxy.FreeResourceMemory<GetTexturePool, TextureAllocationPolicy, string, ITexture>(m_texture);
            PoolProxy.FreeResourceMemory<GetTexturePool, TextureAllocationPolicy, string, ITexture>(m_envMap);
        }

        protected override void InitShader()
        {
            m_shader = PoolProxy.GetResource<GetShaderPool, ShaderAllocationPolicy<EnvironmentEntitiesShader>, string, EnvironmentEntitiesShader>(ProjectFolders.ShadersPath + "envVS.glsl" + "," + ProjectFolders.ShadersPath + "envFS.glsl");
        }

        protected override void FreeShader()
        {
            PoolProxy.FreeResourceMemory<GetShaderPool, ShaderAllocationPolicy<EnvironmentEntitiesShader>, string, EnvironmentEntitiesShader>(GetShader());
        }

        #endregion
    }
}
