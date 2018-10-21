using OpenTK;
using System;
using System.Collections.Generic;
using TextureLoader;
using OpenTK.Graphics.OpenGL;
using MassiveGame.Settings;
using MassiveGame.API.ResourcePool.PoolHandling;
using MassiveGame.API.ResourcePool.Policies;
using MassiveGame.API.ResourcePool;
using System.Runtime.Serialization;

namespace MassiveGame.Core.RenderCore.Shadows
{
    [Serializable]
    public abstract class ShadowBase : ISerializable
    {
        public abstract ShadowTypes GetShadowType();
        public abstract Matrix4 GetShadowProjectionMatrix(ref Matrix4 projectionMatrix);
        public abstract Matrix4 GetShadowViewMatrix();
        protected abstract void PrepareRenderTarget();

        [NonSerialized]
        protected ITexture ShadowMapTexture;

        [NonSerialized]
        protected Int32 FramebufferHandler;

        [NonSerialized]
        protected BasicShadowShader shadowShader;

        #region Serialization

        protected ShadowBase(SerializationInfo info, StreamingContext context)
        {
            var shadowMapSettings = (TextureParameters)info.GetValue("shadowMapTextureRezolution", typeof(TextureParameters));
            AllocateRenderTarget(shadowMapSettings);
            PrepareRenderTarget();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            var shadowMapTextureRezolution = PoolProxy.GetResourceKey<ObtainRenderTargetPool, TextureParameters, ITexture>(ShadowMapTexture);
            info.AddValue("shadowMapTextureRezolution", shadowMapTextureRezolution, typeof(TextureParameters));
        }

        #endregion

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
            ShadowMapTexture = PoolProxy.GetResource<ObtainRenderTargetPool, RenderTargetAllocationPolicy, TextureParameters, ITexture>(shadowMapSettings);
            FramebufferHandler = GL.GenFramebuffer();
        }

        public void DeallocateRenderTarget()
        {
            PoolProxy.FreeResourceMemory<ObtainRenderTargetPool, RenderTargetAllocationPolicy, TextureParameters, ITexture>(ShadowMapTexture);
            GL.DeleteFramebuffer(FramebufferHandler);
        }

        public void WriteDepth(IList<IDrawable> CastingShadowActors, ref Matrix4 ProjectionMatrix)
        {
            GL.Viewport(0, 0, ShadowMapTexture.GetTextureRezolution().X, ShadowMapTexture.GetTextureRezolution().Y);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FramebufferHandler);
            GL.Clear(ClearBufferMask.DepthBufferBit);

            Matrix4 shadowProjectionMatrix = GetShadowProjectionMatrix(ref ProjectionMatrix);
            Matrix4 shadowViewMatrix = GetShadowViewMatrix();

            //GL.Enable(EnableCap.CullFace);
            //GL.CullFace(CullFaceMode.Front);
            shadowShader.startProgram();
            foreach (IDrawable actor in CastingShadowActors)
            {
                if (actor != null)
                {
                    shadowShader.SetUniformValues(actor.GetWorldMatrix(), shadowViewMatrix, shadowProjectionMatrix);
                    actor.GetMeshVao().RenderVAO(PrimitiveType.Triangles);
                }
            }
            shadowShader.stopProgram();
            //GL.Disable(EnableCap.CullFace);
            //GL.CullFace(CullFaceMode.Back);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        private void InitResources(TextureParameters ShadowMapSettings)
        {
            shadowShader = PoolProxy.GetResource<ObtainShaderPool, ShaderAllocationPolicy<BasicShadowShader>, string, BasicShadowShader>(ProjectFolders.ShadersPath + "basicShadowVS.glsl" + "," + ProjectFolders.ShadersPath + "basicShadowFS.glsl");
            AllocateRenderTarget(ShadowMapSettings);
            PrepareRenderTarget();
        }

        public ShadowBase(TextureParameters ShadowMapSettings)
        {
            InitResources(ShadowMapSettings);
        }

        public void CleanUp()
        {
            DeallocateRenderTarget();
        }

        public ITexture GetShadowMapTexture()
        {
            return ShadowMapTexture;
        }

    }
}
