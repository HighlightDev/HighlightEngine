using FramebufferAPI;
using MassiveGame.API.Collector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextureLoader;

using OpenTK.Graphics.OpenGL;

namespace MassiveGame.PostFX.DepthOfField
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
               0, PixelInternalFormat.Rgb, DOUEngine.globalSettings.DomainFramebufferRezolution.X / 10,
               DOUEngine.globalSettings.DomainFramebufferRezolution.Y / 10, PixelFormat.Rgb, PixelType.UnsignedByte, TextureWrapMode.Repeat);

            var horizontalBlurRenderTargetParams = new TextureParameters(TextureTarget.Texture2D, TextureMagFilter.Linear, TextureMinFilter.Linear,
             0, PixelInternalFormat.Rgb, DOUEngine.globalSettings.DomainFramebufferRezolution.X / 10,
             DOUEngine.globalSettings.DomainFramebufferRezolution.Y / 10, PixelFormat.Rgb, PixelType.UnsignedByte, TextureWrapMode.Repeat);

            var depthOfFieldRenderTargetPrams = new TextureParameters(TextureTarget.Texture2D, TextureMagFilter.Nearest, TextureMinFilter.Nearest, 0, PixelInternalFormat.Rgb,
               DOUEngine.globalSettings.DomainFramebufferRezolution.X, DOUEngine.globalSettings.DomainFramebufferRezolution.Y, PixelFormat.Rgb, PixelType.UnsignedByte, TextureWrapMode.Repeat);

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
