using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using FramebufferAPI;
using TextureLoader;

namespace MassiveGame
{
    public class GodRaysFBO : Framebuffer
    {
        #region Definitions 

        public ITexture FrameTexture { private set; get; }
        public ITexture RadialBlurAppliedTexture { private set; get; }
        public ITexture LensFlareTexture { private set; get; }

        public ITexture LightShaftsResultTexture { private set; get; }

        #endregion

        #region Setter

        protected override void setTextures()
        {
            LightShaftsResultTexture = new Texture2Dlite(DOUEngine.ScreenRezolution.X, DOUEngine.ScreenRezolution.Y, PixelInternalFormat.Rgb, PixelFormat.Rgb, PixelType.UnsignedByte);

            FrameTexture = new Texture2Dlite(DOUEngine.ScreenRezolution.X, DOUEngine.ScreenRezolution.Y, PixelInternalFormat.Rgb, PixelFormat.Rgb, PixelType.UnsignedByte);
            RadialBlurAppliedTexture = new Texture2Dlite(DOUEngine.ScreenRezolution.X / 10, DOUEngine.ScreenRezolution.Y / 10, PixelInternalFormat.Rgb, PixelFormat.Rgb, PixelType.UnsignedByte, (Int32)All.Linear);
            
            /*IF pp is disabled and lens flare is enabled*/
            if (LensFlareRenderer.LensFlareEnabled)
            {
                LensFlareTexture = new Texture2Dlite(DOUEngine.ScreenRezolution.X, DOUEngine.ScreenRezolution.Y, PixelInternalFormat.Rgb, PixelFormat.Rgb, PixelType.UnsignedByte, (Int32)All.Linear);
            }
        }

        protected override void setFramebuffers()
        {
            base.genFramebuffers(4);

            base.bindFramebuffer(4);
            attachTextureToFramebuffer(FramebufferAttachment.ColorAttachment0, LightShaftsResultTexture.GetTextureDescriptor());
           
            base.bindFramebuffer(2);
            base.attachTextureToFramebuffer(FramebufferAttachment.ColorAttachment0, RadialBlurAppliedTexture.GetTextureDescriptor());

            base.bindFramebuffer(1);
            base.attachTextureToFramebuffer(FramebufferAttachment.ColorAttachment0, FrameTexture.GetTextureDescriptor());

             /*IF pp is disabled and lens flare is enabled*/
            if (LensFlareRenderer.LensFlareEnabled)
            {
                base.bindFramebuffer(3);
                base.attachTextureToFramebuffer(FramebufferAttachment.ColorAttachment0, LensFlareTexture.GetTextureDescriptor());
            }
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
            if (LensFlareTexture != null)
                LensFlareTexture.CleanUp();

            base.cleanUp();
        }

        #region Constructor

        public GodRaysFBO() : base()
        {
            Debug.Log.addToLog(DateTime.Now.ToString() + "  " + "GodRays Framebuffer 1 : " + base.getFramebufferLog());
            base.unbindFramebuffer();
        }

        #endregion 
    }
}
