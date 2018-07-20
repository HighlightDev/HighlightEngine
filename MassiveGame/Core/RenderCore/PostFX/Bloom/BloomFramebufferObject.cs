using FramebufferAPI;
using TextureLoader;
using OpenTK.Graphics.OpenGL;
using MassiveGame.API.ResourcePool.PoolHandling;
using MassiveGame.API.ResourcePool.Policies;
using MassiveGame.API.ResourcePool;

namespace MassiveGame.Core.RenderCore.PostFX.Bloom
{
    public class BloomFramebufferObject : Framebuffer
    {

        #region Definitions

        public ITexture verticalBlurTexture;
        public ITexture horizontalBlurTexture;
        public ITexture bloomResultTexture;

        #endregion

        #region Setters

        protected override void setTextures()
        {
            verticalBlurTexture = PoolProxy.GetResource<ObtainRenderTargetPool, RenderTargetAllocationPolicy, TextureParameters, ITexture>(new TextureParameters(TextureTarget.Texture2D, TextureMagFilter.Linear, TextureMinFilter.Linear,
                0, PixelInternalFormat.Rgb, EngineStatics.globalSettings.DomainFramebufferRezolution.X / 10,
                EngineStatics.globalSettings.DomainFramebufferRezolution.Y / 10, PixelFormat.Rgb, PixelType.UnsignedByte, TextureWrapMode.Repeat));

            horizontalBlurTexture = PoolProxy.GetResource<ObtainRenderTargetPool, RenderTargetAllocationPolicy, TextureParameters, ITexture>(new TextureParameters(TextureTarget.Texture2D, TextureMagFilter.Linear, TextureMinFilter.Linear,
               0, PixelInternalFormat.Rgb, EngineStatics.globalSettings.DomainFramebufferRezolution.X / 10,
               EngineStatics.globalSettings.DomainFramebufferRezolution.Y / 10, PixelFormat.Rgb, PixelType.UnsignedByte, TextureWrapMode.Repeat));

            bloomResultTexture = PoolProxy.GetResource<ObtainRenderTargetPool, RenderTargetAllocationPolicy, TextureParameters, ITexture>(new TextureParameters(TextureTarget.Texture2D, TextureMagFilter.Nearest, TextureMinFilter.Nearest,
                0, PixelInternalFormat.Rgb, EngineStatics.globalSettings.DomainFramebufferRezolution.X,
                EngineStatics.globalSettings.DomainFramebufferRezolution.Y, PixelFormat.Rgb, PixelType.UnsignedByte, TextureWrapMode.Repeat));
        }

        protected override void setFramebuffers()
        {
            base.genFramebuffers(3);

            base.bindFramebuffer(1);
            base.attachTextureToFramebuffer(FramebufferAttachment.ColorAttachment0, verticalBlurTexture.GetTextureDescriptor());
            base.bindFramebuffer(2);
            base.attachTextureToFramebuffer(FramebufferAttachment.ColorAttachment0, horizontalBlurTexture.GetTextureDescriptor());
            base.bindFramebuffer(3);
            base.attachTextureToFramebuffer(FramebufferAttachment.ColorAttachment0, bloomResultTexture.GetTextureDescriptor());
            base.unbindFramebuffer();
        }

        protected override void setRenderbuffers()
        {
        }

        #endregion

        public override void cleanUp()
        {
            PoolProxy.FreeResourceMemoryByValue<ObtainRenderTargetPool, RenderTargetAllocationPolicy, TextureParameters, ITexture>(verticalBlurTexture);
            PoolProxy.FreeResourceMemoryByValue<ObtainRenderTargetPool, RenderTargetAllocationPolicy, TextureParameters, ITexture>(horizontalBlurTexture);
            PoolProxy.FreeResourceMemoryByValue<ObtainRenderTargetPool, RenderTargetAllocationPolicy, TextureParameters, ITexture>(bloomResultTexture);
        }

        #region Constructor

        public BloomFramebufferObject()
        {
        }

        #endregion
    }
}
