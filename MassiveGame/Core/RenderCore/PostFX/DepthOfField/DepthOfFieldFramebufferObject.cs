using FramebufferAPI;
using MassiveGame.API.Collector;
using TextureLoader;

using OpenTK.Graphics.OpenGL;

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

            VerticalBlurTexture = ResourcePool.GetRenderTarget(verticalBlurRederTargetParams);
            HorizontalBlurTexture = ResourcePool.GetRenderTarget(horizontalBlurRenderTargetParams);
            DepthOfFieldResultTexture = ResourcePool.GetRenderTarget(depthOfFieldRenderTargetPrams);
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
            VerticalBlurTexture.CleanUp();
            HorizontalBlurTexture.CleanUp();
            DepthOfFieldResultTexture.CleanUp();
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
