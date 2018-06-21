using System;
using TextureLoader;
using OpenTK.Graphics.OpenGL;
using FramebufferAPI;
using MassiveGame.API.Collector;

namespace MassiveGame.PostFX.LensFlare
{
    public class LensFlareFramebufferObject : Framebuffer
    {
        #region Definitions

        public ITexture verticalBlurTexture { private set; get; }
        public ITexture horizontalBlurTexture { private set; get; }
        public ITexture frameTextureLowRezolution { private set; get; }
        public ITexture lensFlareResultTexture { private set; get; }

        #endregion

        #region Setters

        protected override void setTextures()
        {
            /* Img 1 - vertical blur stage image;
             * Img 2 - horizontal blur stage image;
             * Img 3 - texture with low rezolution for bright parts of sun;
             * Img 4 - lens flare result texture */

            verticalBlurTexture = ResourcePool.GetRenderTarget(new TextureParameters(TextureTarget.Texture2D, TextureMagFilter.Linear, TextureMinFilter.Linear,
               0, PixelInternalFormat.Rgb, DOUEngine.globalSettings.DomainFramebufferRezolution.X / 10,
               DOUEngine.globalSettings.DomainFramebufferRezolution.Y / 10, PixelFormat.Rgb, PixelType.UnsignedByte));

            horizontalBlurTexture = ResourcePool.GetRenderTarget(new TextureParameters(TextureTarget.Texture2D, TextureMagFilter.Linear, TextureMinFilter.Linear,
               0, PixelInternalFormat.Rgb, DOUEngine.globalSettings.DomainFramebufferRezolution.X / 10,
               DOUEngine.globalSettings.DomainFramebufferRezolution.Y / 10, PixelFormat.Rgb, PixelType.UnsignedByte));

            frameTextureLowRezolution = ResourcePool.GetRenderTarget(new TextureParameters(TextureTarget.Texture2D, TextureMagFilter.Linear, TextureMinFilter.Linear, 0, PixelInternalFormat.Rgb,
               DOUEngine.globalSettings.DomainFramebufferRezolution.X / 10, DOUEngine.globalSettings.DomainFramebufferRezolution.Y / 10, PixelFormat.Rgb, PixelType.UnsignedByte));

            lensFlareResultTexture = ResourcePool.GetRenderTarget(new TextureParameters(TextureTarget.Texture2D, TextureMagFilter.Nearest, TextureMinFilter.Nearest, 0, PixelInternalFormat.Rgb,
               DOUEngine.globalSettings.DomainFramebufferRezolution.X, DOUEngine.globalSettings.DomainFramebufferRezolution.Y, PixelFormat.Rgb, PixelType.UnsignedByte));
        }

        protected override void setFramebuffers()
        {
            base.genFramebuffers(4);

            base.bindFramebuffer(1);
            base.attachTextureToFramebuffer(FramebufferAttachment.ColorAttachment0, verticalBlurTexture.GetTextureDescriptor());

            base.bindFramebuffer(2);
            base.attachTextureToFramebuffer(FramebufferAttachment.ColorAttachment0, horizontalBlurTexture.GetTextureDescriptor());

            base.bindFramebuffer(3);
            base.attachTextureToFramebuffer(FramebufferAttachment.ColorAttachment0, frameTextureLowRezolution.GetTextureDescriptor());

            base.bindFramebuffer(4);
            attachTextureToFramebuffer(FramebufferAttachment.ColorAttachment0, lensFlareResultTexture.GetTextureDescriptor());
        }

        protected override void setRenderbuffers()
        {
            base.bindFramebuffer(3);
            base.genRenderbuffers(1);

            base.bindFramebuffer(3);
            base.bindRenderbuffer(1);
            base.renderbufferStorage(RenderbufferStorage.DepthComponent24, frameTextureLowRezolution.GetTextureRezolution());
            base.attachRenderbufferToFramebuffer(FramebufferAttachment.DepthAttachment, 1);
        }

        #endregion

        #region Constructor

        public LensFlareFramebufferObject()
            : base()
        {
        }

        public override void cleanUp()
        {
            ResourcePool.ReleaseRenderTarget(verticalBlurTexture);
            ResourcePool.ReleaseRenderTarget(horizontalBlurTexture);
            ResourcePool.ReleaseRenderTarget(frameTextureLowRezolution);
            ResourcePool.ReleaseRenderTarget(lensFlareResultTexture);
            base.cleanUp();
        }

        #endregion

    }
}
