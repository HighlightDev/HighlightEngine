using OpenTK.Graphics.OpenGL;
using FramebufferAPI;
using TextureLoader;
using MassiveGame.API.ResourcePool.PoolHandling;
using MassiveGame.API.ResourcePool.Policies;
using MassiveGame.API.ResourcePool;

namespace MassiveGame.Core.RenderCore.PostFX.LightShafts
{
    public class LightShaftFramebufferObject : Framebuffer
    {
        #region Definitions 

        public ITexture RadialBlurAppliedTexture { private set; get; }
        public ITexture LightShaftsResultTexture { private set; get; }

        #endregion

        #region Setter

        protected override void setTextures()
        {
            RadialBlurAppliedTexture = PoolProxy.GetResource<GetRenderTargetPool, RenderTargetAllocationPolicy, TextureParameters, ITexture>(new TextureParameters(TextureTarget.Texture2D, TextureMagFilter.Linear, TextureMinFilter.Linear,
             0, PixelInternalFormat.Rgb, EngineStatics.globalSettings.DomainFramebufferRezolution.X / 10,
             EngineStatics.globalSettings.DomainFramebufferRezolution.Y / 10, PixelFormat.Rgb, PixelType.UnsignedByte, TextureWrapMode.Repeat));

            LightShaftsResultTexture = PoolProxy.GetResource<GetRenderTargetPool, RenderTargetAllocationPolicy, TextureParameters, ITexture>(new TextureParameters(TextureTarget.Texture2D, TextureMagFilter.Nearest, TextureMinFilter.Nearest, 0, PixelInternalFormat.Rgb,
               EngineStatics.globalSettings.DomainFramebufferRezolution.X, EngineStatics.globalSettings.DomainFramebufferRezolution.Y, PixelFormat.Rgb, PixelType.UnsignedByte, TextureWrapMode.Repeat));
        }

        protected override void setFramebuffers()
        {
            base.genFramebuffers(2);

            base.bindFramebuffer(2);
            attachTextureToFramebuffer(FramebufferAttachment.ColorAttachment0, LightShaftsResultTexture.GetTextureDescriptor());
           
            base.bindFramebuffer(1);
            base.attachTextureToFramebuffer(FramebufferAttachment.ColorAttachment0, RadialBlurAppliedTexture.GetTextureDescriptor());
        }

        protected override void setRenderbuffers()
        {
            base.genRenderbuffers(1);

            base.bindFramebuffer(1);
            base.bindRenderbuffer(1);
            base.renderbufferStorage(RenderbufferStorage.DepthComponent24, RadialBlurAppliedTexture.GetTextureRezolution().X,
                 RadialBlurAppliedTexture.GetTextureRezolution().Y);
            base.attachRenderbufferToFramebuffer(FramebufferAttachment.DepthAttachment, 1);
        }

        #endregion

        public override void cleanUp()
        {
            PoolProxy.FreeResourceMemory<GetRenderTargetPool, RenderTargetAllocationPolicy, TextureParameters, ITexture>(RadialBlurAppliedTexture);
            PoolProxy.FreeResourceMemory<GetRenderTargetPool, RenderTargetAllocationPolicy, TextureParameters, ITexture>(LightShaftsResultTexture);
            base.cleanUp();
        }

        #region Constructor

        public LightShaftFramebufferObject() : base()
        {
        }

        #endregion 
    }
}
