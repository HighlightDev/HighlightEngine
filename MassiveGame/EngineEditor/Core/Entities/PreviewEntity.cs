using MassiveGame.API.Mesh;
using MassiveGame.API.ResourcePool;
using MassiveGame.API.ResourcePool.Policies;
using MassiveGame.API.ResourcePool.PoolHandling;
using TextureLoader;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using MassiveGame.Core.GameCore;
using MassiveGame.Settings;

namespace MassiveGame.EngineEditor.Core.Entities
{
    public class PreviewEntity
    {
        private Skin m_skin;

        private ITexture m_texture;

        private PreviewEntityShader m_shader;

        private bool m_bOwnedByEntity = false;

        private Vector3 m_translation;
        private Vector3 m_rotation;
        private Vector3 m_scale;
        private Matrix4 m_modelMatrix;

        public PreviewEntity()
        {
            m_scale = Vector3.One;
            m_shader = PoolProxy.GetResource<GetShaderPool, ShaderAllocationPolicy<PreviewEntityShader>, string, PreviewEntityShader>(
                string.Format("{0}previewEntityVS.glsl,{0}previewEntityFS.glsl", ProjectFolders.ShadersPath));
        }

        public bool IsOwnedByEntity()
        {
            return m_bOwnedByEntity;
        }

        public void UpdateModelMatrix()
        {
            m_modelMatrix = Matrix4.Identity;
            m_modelMatrix *= Matrix4.CreateScale(m_scale);
            m_modelMatrix *= Matrix4.CreateRotationX(m_rotation.X);
            m_modelMatrix *= Matrix4.CreateRotationY(m_rotation.Y);
            m_modelMatrix *= Matrix4.CreateRotationZ(m_rotation.Z);
            m_modelMatrix *= Matrix4.CreateTranslation(m_translation);
        }

        public void SetTranslation(ref Vector3 translation)
        {
            m_translation = translation;
            UpdateModelMatrix();
        }

        public void SetRotation(ref Vector3 rotation)
        {
            m_rotation = rotation;
            UpdateModelMatrix();
        }

        public void SetScale(ref Vector3 scale)
        {
            m_scale = scale;
            UpdateModelMatrix();
        }

        public void SetPreviewResources(string skinPath, string texturePath, Vector3 translation)
        {
            if (IsOwnedByEntity())
            {
                ReleasePossession();
            }

            m_texture = PoolProxy.GetResource<GetTexturePool, TextureAllocationPolicy, string, ITexture>(texturePath);
            m_skin = PoolProxy.GetResource<GetModelPool, ModelAllocationPolicy, string, Skin>(skinPath);
            m_translation = translation;
            UpdateModelMatrix();
            m_bOwnedByEntity = true;
        }

        private void ReleasePossession()
        {
            PoolProxy.FreeResourceMemory<GetTexturePool, TextureAllocationPolicy, string, ITexture>(m_texture);
            PoolProxy.FreeResourceMemory<GetModelPool, ModelAllocationPolicy, string, Skin>(m_skin);
            m_translation = Vector3.Zero;
            m_rotation = Vector3.Zero;
            m_scale = Vector3.One;
            m_bOwnedByEntity = false;
        }

        public void Render(BaseCamera baseCamera, ref Matrix4 projectionMatrix)
        {
            Matrix4 worldMatrix = m_modelMatrix;
            Matrix4 viewMatrix = baseCamera.GetViewMatrix();

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            m_shader.startProgram();
            m_texture.BindTexture(OpenTK.Graphics.OpenGL.TextureUnit.Texture0);
            m_shader.SetTransformationMatrices(ref worldMatrix, ref viewMatrix, ref projectionMatrix);
            m_shader.SetDiffuseTexSampler(0);
            m_skin.Buffer.RenderVAO(OpenTK.Graphics.OpenGL.PrimitiveType.Triangles);
            m_shader.stopProgram();
            GL.Disable(EnableCap.Blend);
        }

        #region TestVersion
        public PreviewEntity(bool TEST)
        {
            m_scale = Vector3.One;
            m_bOwnedByEntity = true;
            TestInitResources();
        }

        private void TestInitResources()
        {
            m_shader = PoolProxy.GetResource<GetShaderPool, ShaderAllocationPolicy<PreviewEntityShader>, string, PreviewEntityShader>(
                string.Format("{0}previewEntityVS.glsl,{0}previewEntityFS.glsl", ProjectFolders.ShadersPath));

            var modelPath = ProjectFolders.ModelsPath + "playerCube.obj";
            var texturePath = ProjectFolders.MultitexturesPath + "path.png";

            m_texture = PoolProxy.GetResource<GetTexturePool, TextureAllocationPolicy, string, ITexture>(texturePath);
            m_skin = PoolProxy.GetResource<GetModelPool, ModelAllocationPolicy, string, Skin>(modelPath);
        }
        #endregion
    }
}
