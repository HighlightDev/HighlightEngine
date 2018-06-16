using System;
using TextureLoader;
using FramebufferAPI;
using OpenTK.Graphics.OpenGL;

namespace MassiveGame
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

            ReflectionTexture = new Texture2Dlite((Int32)(DOUEngine.domainFramebufferRezolution.X / 1.5), (Int32)(DOUEngine.domainFramebufferRezolution.Y / 1.5), PixelInternalFormat.Rgb, PixelFormat.Rgb, PixelType.UnsignedByte);
            RefractionTexture = new Texture2Dlite((Int32)(DOUEngine.domainFramebufferRezolution.X / 1.5), (Int32)(DOUEngine.domainFramebufferRezolution.Y / 1.5), PixelInternalFormat.Rgb, PixelFormat.Rgb, PixelType.UnsignedByte);
            DepthTexture = new Texture2Dlite((Int32)(DOUEngine.domainFramebufferRezolution.X / 1.5), (Int32)(DOUEngine.domainFramebufferRezolution.Y / 1.5), PixelInternalFormat.Depth24Stencil8, PixelFormat.DepthComponent, PixelType.Float);
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

        #region Constructor

        public WaterFBO()
            : base()
        {
            base.bindFramebuffer(1);
            Debug.Log.addToLog( DateTime.Now.ToString() + "  " + "Water Framebuffer 1 : " + base.getFramebufferLog());
            base.bindFramebuffer(2);
            Debug.Log.addToLog( DateTime.Now.ToString() + "  " + "Water Framebuffer 2 : " + base.getFramebufferLog());
            base.unbindFramebuffer();
        }

        #endregion
    }
}
