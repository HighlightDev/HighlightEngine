using System;
using TextureLoader;
using FramebufferAPI;
using OpenTK.Graphics.OpenGL;
using MassiveGame.API.ResourcePool;
using MassiveGame.API.ResourcePool.PoolHandling;
using MassiveGame.API.ResourcePool.Policies;

namespace MassiveGame.Core.GameCore.Water
{
    public class WaterFBO : Framebuffer
    {
        #region Definitions

        public ITexture ReflectionTexture { set; get; }
        public ITexture RefractionTexture { set; get; }
        public ITexture DepthTexture { set; get; }

        #endregion

        #region Setter

        protected override void setTextures()
        {
            /*Create 3 textures : 
             1 - for reflection 
             2 - for refraction
             3 - for depth*/

            ReflectionTexture = PoolProxy.GetResource<ObtainRenderTargetPool, RenderTargetAllocationPolicy, TextureParameters, ITexture>(new TextureParameters(TextureTarget.Texture2D, TextureMagFilter.Nearest, TextureMinFilter.Nearest, 0, PixelInternalFormat.Rgb, (Int32)(EngineStatics.globalSettings.DomainFramebufferRezolution.X / 1.5), (Int32)(EngineStatics.globalSettings.DomainFramebufferRezolution.Y / 1.5), PixelFormat.Rgb, PixelType.UnsignedByte, TextureWrapMode.Repeat));
            RefractionTexture = PoolProxy.GetResource<ObtainRenderTargetPool, RenderTargetAllocationPolicy, TextureParameters, ITexture>(new TextureParameters(TextureTarget.Texture2D, TextureMagFilter.Nearest, TextureMinFilter.Nearest, 0, PixelInternalFormat.Rgb, (Int32)(EngineStatics.globalSettings.DomainFramebufferRezolution.X / 1.5), (Int32)(EngineStatics.globalSettings.DomainFramebufferRezolution.Y / 1.5), PixelFormat.Rgb, PixelType.UnsignedByte, TextureWrapMode.Repeat));
            DepthTexture = PoolProxy.GetResource<ObtainRenderTargetPool, RenderTargetAllocationPolicy, TextureParameters, ITexture>(new TextureParameters(TextureTarget.Texture2D, TextureMagFilter.Nearest, TextureMinFilter.Nearest, 0, PixelInternalFormat.Depth24Stencil8,
                (Int32)(EngineStatics.globalSettings.DomainFramebufferRezolution.X / 1.5f), (Int32)(EngineStatics.globalSettings.DomainFramebufferRezolution.Y / 1.5f), PixelFormat.DepthComponent, PixelType.Float, TextureWrapMode.Repeat));
        }

        protected override void setFramebuffers()
        {
            /*Create 2 framebuffers :
             1 - for reflection
             2 - for refraction*/
            base.genFramebuffers(2);
            base.bindFramebuffer(1);
            base.attachTextureToFramebuffer(FramebufferAttachment.ColorAttachment0, ReflectionTexture.GetTextureDescriptor());
            base.bindFramebuffer(2);
            base.attachTextureToFramebuffer(FramebufferAttachment.ColorAttachment0, RefractionTexture.GetTextureDescriptor());
            base.attachTextureToFramebuffer(FramebufferAttachment.DepthStencilAttachment, DepthTexture.GetTextureDescriptor());
        }

        protected override void setRenderbuffers()
        {
            /*Attach 1 depthbuffer*/
            base.genRenderbuffers(1);
            base.bindFramebuffer(1);
            base.bindRenderbuffer(1);
            base.renderbufferStorage(RenderbufferStorage.Depth24Stencil8,
                ReflectionTexture.GetTextureRezolution());
            base.attachRenderbufferToFramebuffer(FramebufferAttachment.DepthStencilAttachment);
        }

        #endregion

        public override void cleanUp()
        {
            PoolProxy.FreeResourceMemoryByValue<ObtainRenderTargetPool, RenderTargetAllocationPolicy, TextureParameters, ITexture>(ReflectionTexture);
            PoolProxy.FreeResourceMemoryByValue<ObtainRenderTargetPool, RenderTargetAllocationPolicy, TextureParameters, ITexture>(RefractionTexture);
            PoolProxy.FreeResourceMemoryByValue<ObtainRenderTargetPool, RenderTargetAllocationPolicy, TextureParameters, ITexture>(DepthTexture);
            base.cleanUp();
        }

        #region Constructor

        public WaterFBO()
            : base()
        {
        }

        #endregion
    }
}
