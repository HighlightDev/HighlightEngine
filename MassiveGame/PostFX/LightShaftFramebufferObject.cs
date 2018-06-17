using System;
using OpenTK.Graphics.OpenGL;
using FramebufferAPI;
using TextureLoader;

namespace MassiveGame
{
    public class LightShaftFramebufferObject : Framebuffer
    {
        #region Definitions 

        public ITexture FrameTexture { private set; get; }
        public ITexture RadialBlurAppliedTexture { private set; get; }
        public ITexture LightShaftsResultTexture { private set; get; }

        #endregion

        #region Setter

        protected override void setTextures()
        {
            LightShaftsResultTexture = new Texture2D(DOUEngine.domainFramebufferRezolution.X, DOUEngine.domainFramebufferRezolution.Y, PixelInternalFormat.Rgb, PixelFormat.Rgb, PixelType.UnsignedByte);
            FrameTexture = new Texture2D(DOUEngine.domainFramebufferRezolution.X, DOUEngine.domainFramebufferRezolution.Y, PixelInternalFormat.Rgb, PixelFormat.Rgb, PixelType.UnsignedByte);
            RadialBlurAppliedTexture = new Texture2D(DOUEngine.domainFramebufferRezolution.X / 10, DOUEngine.domainFramebufferRezolution.Y / 10, PixelInternalFormat.Rgb, PixelFormat.Rgb, PixelType.UnsignedByte, (Int32)All.Linear);
        }

        protected override void setFramebuffers()
        {
            base.genFramebuffers(3);

            base.bindFramebuffer(3);
            attachTextureToFramebuffer(FramebufferAttachment.ColorAttachment0, LightShaftsResultTexture.GetTextureDescriptor());
           
            base.bindFramebuffer(2);
            base.attachTextureToFramebuffer(FramebufferAttachment.ColorAttachment0, RadialBlurAppliedTexture.GetTextureDescriptor());

            base.bindFramebuffer(1);
            base.attachTextureToFramebuffer(FramebufferAttachment.ColorAttachment0, FrameTexture.GetTextureDescriptor());
        }

        protected override void setRenderbuffers()
        {
            base.genRenderbuffers(2);

            base.bindFramebuffer(1);
            base.bindRenderbuffer(1);
            base.renderbufferStorage(RenderbufferStorage.DepthComponent24, FrameTexture.GetTextureRezolution().X,
                 FrameTexture.GetTextureRezolution().Y);
            base.attachRenderbufferToFramebuffer(FramebufferAttachment.DepthAttachment, 1);

            base.bindFramebuffer(2);
            base.bindRenderbuffer(2);
            base.renderbufferStorage(RenderbufferStorage.DepthComponent24, RadialBlurAppliedTexture.GetTextureRezolution().X,
                 RadialBlurAppliedTexture.GetTextureRezolution().Y);
            base.attachRenderbufferToFramebuffer(FramebufferAttachment.DepthAttachment, 2);
        }

        #endregion

        public override void cleanUp()
        {
            FrameTexture.CleanUp();
            RadialBlurAppliedTexture.CleanUp();
            LightShaftsResultTexture.CleanUp();
            base.cleanUp();
        }

        #region Constructor

        public LightShaftFramebufferObject() : base()
        {
            Debug.Log.addToLog(DateTime.Now.ToString() + "  " + "GodRays Framebuffer 1 : " + base.getFramebufferLog());
            base.unbindFramebuffer();
        }

        #endregion 
    }
}
