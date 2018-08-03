using FramebufferAPI;
using TextureLoader;

using OpenTK.Graphics.OpenGL;
using MassiveGame.API.ResourcePool.PoolHandling;
using MassiveGame.API.ResourcePool.Policies;
using MassiveGame.API.ResourcePool;

namespace MassiveGame.Core.RenderCore.PostFX.DepthOfField
{
    public class DepthOfFieldFramebufferObject : Framebuffer
    {
        #region Definitions

        public ITexture VerticalBlurTexture;
        public ITexture HorizontalBlurTexture;
        public ITexture DepthOfFieldResultTexture;

        #endregion

        #region Setters

        protected override void setTextures()
        {
            var verticalBlurRederTargetParams = new TextureParameters(TextureTarget.Texture2D, TextureMagFilter.Linear, TextureMinFilter.Linear,
               0, PixelInternalFormat.Rgb, EngineStatics.globalSettings.DomainFramebufferRezolution.X / 10,
               EngineStatics.globalSettings.DomainFramebufferRezolution.Y / 10, PixelFormat.Rgb, PixelType.UnsignedByte, TextureWrapMode.Repeat);

            var horizontalBlurRenderTargetParams = new TextureParameters(TextureTarget.Texture2D, TextureMagFilter.Linear, TextureMinFilter.Linear,
             0, PixelInternalFormat.Rgb, EngineStatics.globalSettings.DomainFramebufferRezolution.X / 10,
             EngineStatics.globalSettings.DomainFramebufferRezolution.Y / 10, PixelFormat.Rgb, PixelType.UnsignedByte, TextureWrapMode.Repeat);

            var depthOfFieldRenderTargetPrams = new TextureParameters(TextureTarget.Texture2D, TextureMagFilter.Nearest, TextureMinFilter.Nearest, 0, PixelInternalFormat.Rgb,
               EngineStatics.globalSettings.DomainFramebufferRezolution.X, EngineStatics.globalSettings.DomainFramebufferRezolution.Y, PixelFormat.Rgb, PixelType.UnsignedByte, TextureWrapMode.Repeat);

            VerticalBlurTexture = PoolProxy.GetResource<ObtainRenderTargetPool, RenderTargetAllocationPolicy, TextureParameters, ITexture>(verticalBlurRederTargetParams);
            HorizontalBlurTexture = PoolProxy.GetResource<ObtainRenderTargetPool, RenderTargetAllocationPolicy, TextureParameters, ITexture>(horizontalBlurRenderTargetParams);
            DepthOfFieldResultTexture = PoolProxy.GetResource<ObtainRenderTargetPool, RenderTargetAllocationPolicy, TextureParameters, ITexture>(depthOfFieldRenderTargetPrams);
        }

        protected override void setFramebuffers()
        {
            genFramebuffers(3);

            bindFramebuffer(1);
            attachTextureToFramebuffer(FramebufferAttachment.ColorAttachment0, VerticalBlurTexture.GetTextureDescriptor());
            bindFramebuffer(2);
            attachTextureToFramebuffer(FramebufferAttachment.ColorAttachment0, HorizontalBlurTexture.GetTextureDescriptor());
            bindFramebuffer(3);
            attachTextureToFramebuffer(FramebufferAttachment.ColorAttachment0, DepthOfFieldResultTexture.GetTextureDescriptor());
            unbindFramebuffer();
        }

        protected override void setRenderbuffers()
        {
        }

        #endregion

        public override void cleanUp()
        {
            PoolProxy.FreeResourceMemory<ObtainRenderTargetPool, RenderTargetAllocationPolicy, TextureParameters, ITexture>(VerticalBlurTexture);
            PoolProxy.FreeResourceMemory<ObtainRenderTargetPool, RenderTargetAllocationPolicy, TextureParameters, ITexture>(HorizontalBlurTexture);
            PoolProxy.FreeResourceMemory<ObtainRenderTargetPool, RenderTargetAllocationPolicy, TextureParameters, ITexture>(DepthOfFieldResultTexture);
            base.cleanUp();
        }

        #region Constructor

        public DepthOfFieldFramebufferObject()
            : base()
        {
        }

        #endregion
    }
}
