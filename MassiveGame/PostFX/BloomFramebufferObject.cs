using FramebufferAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextureLoader;
using OpenTK.Graphics.OpenGL;

namespace MassiveGame.PostFX
{
    public class BloomFramebufferObject : Framebuffer
    {

        #region Definitions

        public ITexture verticalBlurTexture;
        public ITexture horizontalBlurTexture;
        public ITexture bloomResultTexture;

        #endregion

        #region Setters

        protected override void setTextures()
        {
            verticalBlurTexture = new Texture2D(DOUEngine.domainFramebufferRezolution.X / 10, DOUEngine.domainFramebufferRezolution.Y / 10, PixelInternalFormat.Rgb,
                PixelFormat.Rgb, PixelType.UnsignedByte, (Int32)All.Linear);

            horizontalBlurTexture = new Texture2D(DOUEngine.domainFramebufferRezolution.X / 10, DOUEngine.domainFramebufferRezolution.Y / 10, PixelInternalFormat.Rgb,
               PixelFormat.Rgb, PixelType.UnsignedByte, (Int32)All.Linear);

            bloomResultTexture = new Texture2D(DOUEngine.domainFramebufferRezolution.X, DOUEngine.domainFramebufferRezolution.Y, PixelInternalFormat.Rgb,
                 PixelFormat.Rgb, PixelType.UnsignedByte);
        }

        protected override void setFramebuffers()
        {
            base.genFramebuffers(3);

            base.bindFramebuffer(1);
            base.attachTextureToFramebuffer(FramebufferAttachment.ColorAttachment0, verticalBlurTexture.GetTextureDescriptor());
            base.bindFramebuffer(2);
            base.attachTextureToFramebuffer(FramebufferAttachment.ColorAttachment0, horizontalBlurTexture.GetTextureDescriptor());
            base.bindFramebuffer(3);
            base.attachTextureToFramebuffer(FramebufferAttachment.ColorAttachment0, bloomResultTexture.GetTextureDescriptor());
            base.unbindFramebuffer();
        }

        protected override void setRenderbuffers()
        {
        }

        #endregion

        public override void cleanUp()
        {
            verticalBlurTexture.CleanUp();
            horizontalBlurTexture.CleanUp();
            bloomResultTexture.CleanUp();
            base.cleanUp();
        }

        #region Constructor

        public BloomFramebufferObject()
        {
            base.bindFramebuffer(1);
            Debug.Log.addToLog(DateTime.Now.ToString() + "  " + "vertical blur framebuffer : " + base.getFramebufferLog());
            base.bindFramebuffer(2);
            Debug.Log.addToLog(DateTime.Now.ToString() + "  " + "horizontal blur framebuffer: " + base.getFramebufferLog());
            base.bindFramebuffer(3);
            Debug.Log.addToLog(DateTime.Now.ToString() + "  " + "bloom result framebuffer: " + base.getFramebufferLog());
            base.unbindFramebuffer();
        }

        #endregion
    }
}
