using OpenTK;
using OpenTK.Graphics.OpenGL;
using TextureLoader;
using MassiveGame.Core.ComponentCore;
using MassiveGame.Core.RenderCore.Lights;
using MassiveGame.Core.GameCore;
using System;
using System.Runtime.Serialization;
using MassiveGame.API.ResourcePool.PoolHandling;
using MassiveGame.API.ResourcePool;
using System.Collections.Generic;
using MassiveGame.API.ResourcePool.Policies;
using MassiveGame.Settings;

namespace MassiveGame.Core.RenderCore.Shadows
{
    [Serializable]
    public class DirectionalLightWithShadow : DirectionalLight,
        ISerializable
    {
        #region Fields

        private ShadowMapCache m_shadowMapCache;

        private ShadowOrthoBuilder m_shadowOrthographicProjectionBuilder;

        [NonSerialized]
        private BaseCamera m_viewerCamera;

        private ITexture m_shadowMapTexture;

        private Int32 m_framebufferHandler;

        private BasicShadowShader m_shadowShader;

        #endregion

        public DirectionalLightWithShadow(BaseCamera viewerCamera, TextureParameters ShadowMapSettings, Vector3 Direction, 
            Vector4 Ambient, Vector4 Diffuse, Vector4 Specular) : base(Direction, Ambient, Diffuse, Specular)
        {
            m_viewerCamera = viewerCamera;
            m_shadowOrthographicProjectionBuilder = new ShadowOrthoBuilder();
            m_shadowMapCache = null;
            InitResources(ShadowMapSettings);
        }

        #region Serialization

        public void PostDeserializePass(BaseCamera camera)
        {
            m_viewerCamera = camera;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            TextureParameters shadowmapTextureParameters = m_shadowMapTexture.GetTextureParameters();
            info.AddValue("shadowMapTextureParameters", shadowmapTextureParameters, typeof(TextureParameters));
        }

        protected DirectionalLightWithShadow(SerializationInfo info, StreamingContext context) 
            : base(info ,context)
        {
            var shadowMapSettings = info.GetValue("shadowMapTextureParameters", typeof(TextureParameters)) as TextureParameters;
            InitResources(shadowMapSettings);
            m_shadowOrthographicProjectionBuilder = new ShadowOrthoBuilder();
            m_shadowMapCache = null;
        }

        #endregion

        public ITexture GetShadowMapTexture()
        {
            return m_shadowMapTexture;
        }

        private Vector3 GetTargetOriginPosition()
        {
            ThirdPersonCamera thirdPersonCamera = m_viewerCamera as ThirdPersonCamera;

            if (thirdPersonCamera != null)
                return (thirdPersonCamera.GetThirdPersonTarget() as Component).ComponentTranslation;
            else
                return m_viewerCamera.GetEyeVector();
        }

        public Matrix4 GetShadowViewMatrix()
        {
            Vector3 normLightDir = Direction.Normalized();

            Vector3 targetPositon = GetTargetOriginPosition();
            Vector3 shadowCastPosition = new Vector3(targetPositon.X, Position.Y, targetPositon.Z);
            Vector3 lightTranslatedPosition = normLightDir * 300;
            var lightEye = new Vector3(shadowCastPosition.X - lightTranslatedPosition.X, shadowCastPosition.Y + lightTranslatedPosition.Y, shadowCastPosition.Z - lightTranslatedPosition.Z);
            var lightTarget = shadowCastPosition + (Direction * 100);
            return Matrix4.LookAt(lightEye, lightTarget, new Vector3(0, 1, 0));
        }

        public Matrix4 GetShadowProjectionMatrix(ref Matrix4 projectionMatrix)
        {
            return Matrix4.CreateOrthographic(400, 400, 1, 400);
        }

        private void PrepareRenderTarget()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, m_framebufferHandler);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, m_shadowMapTexture.GetTextureDescriptor(), 0);
            GL.DrawBuffer(DrawBufferMode.None);
            GL.ReadBuffer(ReadBufferMode.None);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void CreateShadowMapCache()
        {
            m_shadowMapCache = new ShadowMapCache();
        }

        public override bool GetHasShadow()
        {
            return true;
        }

        public Matrix4 GetShadowMatrix(ref Matrix4 ModelMatrix, ref Matrix4 ProjectionMatrix)
        {
            Matrix4 ShadowBiasMatrix = new Matrix4(
                0.5f, 0, 0, 0,
                0, 0.5f, 0, 0,
                0, 0, 0.5f, 0,
                0.5f, 0.5f, 0.5f, 1
                );

            Matrix4 result = ModelMatrix;
            result *= GetShadowViewMatrix();
            result *= GetShadowProjectionMatrix(ref ProjectionMatrix);
            result *= ShadowBiasMatrix;
            return result;
        }

        public void AllocateRenderTarget(TextureParameters shadowMapSettings)
        {
            m_shadowMapTexture = PoolProxy.GetResource<GetRenderTargetPool, RenderTargetAllocationPolicy, TextureParameters, ITexture>(shadowMapSettings);
            m_framebufferHandler = GL.GenFramebuffer();
        }

        public void DeallocateRenderTarget()
        {
            PoolProxy.FreeResourceMemory<GetRenderTargetPool, RenderTargetAllocationPolicy, TextureParameters, ITexture>(m_shadowMapTexture);
            GL.DeleteFramebuffer(m_framebufferHandler);
        }

        public void WriteDepth(IList<IDrawable> CastingShadowActors, ref Matrix4 ProjectionMatrix)
        {
            GL.Viewport(0, 0, m_shadowMapTexture.GetTextureRezolution().X, m_shadowMapTexture.GetTextureRezolution().Y);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, m_framebufferHandler);
            GL.Clear(ClearBufferMask.DepthBufferBit);

            Matrix4 shadowProjectionMatrix = GetShadowProjectionMatrix(ref ProjectionMatrix);
            Matrix4 shadowViewMatrix = GetShadowViewMatrix();

            m_shadowShader.startProgram();
            foreach (IDrawable actor in CastingShadowActors)
            {
                if (actor != null)
                {
                    m_shadowShader.SetUniformValues(actor.GetWorldMatrix(), shadowViewMatrix, shadowProjectionMatrix);
                    actor.GetMeshVao().RenderVAO(PrimitiveType.Triangles);
                }
            }
            m_shadowShader.stopProgram();

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        private void InitResources(TextureParameters ShadowMapSettings)
        {
            m_shadowShader = PoolProxy.GetResource<GetShaderPool, ShaderAllocationPolicy<BasicShadowShader>, string, BasicShadowShader>(string.Format("{0}basicShadowVS.glsl,{0}basicShadowFS.glsl", ProjectFolders.ShadersPath));
            AllocateRenderTarget(ShadowMapSettings);
            PrepareRenderTarget();
        }

        public void CleanUp()
        {
            DeallocateRenderTarget();
        }

    }
}
