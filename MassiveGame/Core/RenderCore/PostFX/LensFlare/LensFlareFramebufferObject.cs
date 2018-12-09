using TextureLoader;
using OpenTK.Graphics.OpenGL;
using FramebufferAPI;
using MassiveGame.API.ResourcePool.PoolHandling;
using MassiveGame.API.ResourcePool.Policies;
using MassiveGame.API.ResourcePool;

namespace MassiveGame.Core.RenderCore.PostFX.LensFlare
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

            verticalBlurTexture = PoolProxy.GetResource<GetRenderTargetPool, RenderTargetAllocationPolicy, TextureParameters, ITexture>(new TextureParameters(TextureTarget.Texture2D, TextureMagFilter.Linear, TextureMinFilter.Linear,
               0, PixelInternalFormat.Rgb, EngineStatics.globalSettings.DomainFramebufferRezolution.X / 10,
               EngineStatics.globalSettings.DomainFramebufferRezolution.Y / 10, PixelFormat.Rgb, PixelType.UnsignedByte, TextureWrapMode.Repeat));

            horizontalBlurTexture = PoolProxy.GetResource<GetRenderTargetPool, RenderTargetAllocationPolicy, TextureParameters, ITexture>(new TextureParameters(TextureTarget.Texture2D, TextureMagFilter.Linear, TextureMinFilter.Linear,
               0, PixelInternalFormat.Rgb, EngineStatics.globalSettings.DomainFramebufferRezolution.X / 10,
               EngineStatics.globalSettings.DomainFramebufferRezolution.Y / 10, PixelFormat.Rgb, PixelType.UnsignedByte, TextureWrapMode.Repeat));

            frameTextureLowRezolution = PoolProxy.GetResource<GetRenderTargetPool, RenderTargetAllocationPolicy, TextureParameters, ITexture>(new TextureParameters(TextureTarget.Texture2D, TextureMagFilter.Linear, TextureMinFilter.Linear, 0, PixelInternalFormat.Rgb,
               EngineStatics.globalSettings.DomainFramebufferRezolution.X / 10, EngineStatics.globalSettings.DomainFramebufferRezolution.Y / 10, PixelFormat.Rgb, PixelType.UnsignedByte, TextureWrapMode.Repeat));

            lensFlareResultTexture = PoolProxy.GetResource<GetRenderTargetPool, RenderTargetAllocationPolicy, TextureParameters, ITexture>(new TextureParameters(TextureTarget.Texture2D, TextureMagFilter.Nearest, TextureMinFilter.Nearest, 0, PixelInternalFormat.Rgb,
               EngineStatics.globalSettings.DomainFramebufferRezolution.X, EngineStatics.globalSettings.DomainFramebufferRezolution.Y, PixelFormat.Rgb, PixelType.UnsignedByte, TextureWrapMode.Repeat));
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

        #endregion

        public override void cleanUp()
        {
            PoolProxy.FreeResourceMemory<GetRenderTargetPool, RenderTargetAllocationPolicy, TextureParameters, ITexture>(verticalBlurTexture);
            PoolProxy.FreeResourceMemory<GetRenderTargetPool, RenderTargetAllocationPolicy, TextureParameters, ITexture>(horizontalBlurTexture);
            PoolProxy.FreeResourceMemory<GetRenderTargetPool, RenderTargetAllocationPolicy, TextureParameters, ITexture>(frameTextureLowRezolution);
            PoolProxy.FreeResourceMemory<GetRenderTargetPool, RenderTargetAllocationPolicy, TextureParameters, ITexture>(lensFlareResultTexture);
            base.cleanUp();
        }

    }
}
