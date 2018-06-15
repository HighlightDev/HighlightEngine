using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextureLoader;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using FramebufferAPI;
using GpuGraphics;

namespace MassiveGame
{
    public class LensFlareFBO : Framebuffer
    {
        #region Definitions

        public ITexture verticalBlurTexture { private set; get; }
        public ITexture horizontalBlurTexture { private set; get; }
        public ITexture frameTextureHighRezolution { private set; get; }
        public ITexture frameTextureLowRezolution { private set; get; }

        public ITexture lensFlareResultTexture { private set; get; }

    #endregion

    #region Setters

    protected override void setTextures()
        {
            /* Img 1 - vertical blur stage image;
             * Img 2 - horizontal blur stage image;
             * Img 3 - default frametexture, with high rezolution; 
             * Img 4 - frametexture, with low rezolution, for bright parts of sun. */

            verticalBlurTexture = new Texture2Dlite(DOUEngine.ScreenRezolution.X / 5, DOUEngine.ScreenRezolution.Y / 5, PixelInternalFormat.Rgb,
                PixelFormat.Rgb, PixelType.UnsignedByte, (int)All.Linear);

            horizontalBlurTexture = new Texture2Dlite(DOUEngine.ScreenRezolution.X / 5, DOUEngine.ScreenRezolution.Y / 5, PixelInternalFormat.Rgb,
                PixelFormat.Rgb, PixelType.UnsignedByte, (int)All.Linear);

            frameTextureHighRezolution = new Texture2Dlite(DOUEngine.ScreenRezolution.X, DOUEngine.ScreenRezolution.Y, PixelInternalFormat.Rgb,
                PixelFormat.Rgb, PixelType.UnsignedByte);

            frameTextureLowRezolution = new Texture2Dlite(DOUEngine.ScreenRezolution.X / 5, DOUEngine.ScreenRezolution.Y / 5, PixelInternalFormat.Rgb,
                PixelFormat.Rgb, PixelType.UnsignedByte);

            lensFlareResultTexture = new Texture2Dlite(DOUEngine.ScreenRezolution.X, DOUEngine.ScreenRezolution.Y, PixelInternalFormat.Rgb,
                PixelFormat.Rgb, PixelType.UnsignedByte);
        }
        
        protected override void setFramebuffers()
        {
            base.genFramebuffers(5);

            base.bindFramebuffer(5);
            attachTextureToFramebuffer(FramebufferAttachment.ColorAttachment0, lensFlareResultTexture.GetTextureDescriptor());

            base.bindFramebuffer(1);
            base.attachTextureToFramebuffer(FramebufferAttachment.ColorAttachment0, verticalBlurTexture.GetTextureDescriptor());

            base.bindFramebuffer(2);
            base.attachTextureToFramebuffer(FramebufferAttachment.ColorAttachment0, horizontalBlurTexture.GetTextureDescriptor());

            base.bindFramebuffer(3);
            base.attachTextureToFramebuffer(FramebufferAttachment.ColorAttachment0, frameTextureHighRezolution.GetTextureDescriptor());
            base.bindFramebuffer(4);
            base.attachTextureToFramebuffer(FramebufferAttachment.ColorAttachment0, frameTextureLowRezolution.GetTextureDescriptor());
        }

        protected override void setRenderbuffers()
        {
            base.bindFramebuffer(3);
            base.genRenderbuffers(2);
            base.bindRenderbuffer(1);
            base.renderbufferStorage(RenderbufferStorage.DepthComponent24, frameTextureHighRezolution.GetTextureRezolution());
            base.attachRenderbufferToFramebuffer(FramebufferAttachment.DepthAttachment, 1);

            base.bindFramebuffer(4);
            base.bindRenderbuffer(2);
            base.renderbufferStorage(RenderbufferStorage.DepthComponent24, frameTextureLowRezolution.GetTextureRezolution());
            base.attachRenderbufferToFramebuffer(FramebufferAttachment.DepthAttachment, 2);
        }

        #endregion

        #region Constructor

        public LensFlareFBO()
            : base()
        {
            Debug.Log.addToLog(DateTime.Now.ToString() + "  " + "LensFlare Framebuffer 1 : " + base.getFramebufferLog());
            base.unbindFramebuffer();
        }

        #endregion

    }
}
