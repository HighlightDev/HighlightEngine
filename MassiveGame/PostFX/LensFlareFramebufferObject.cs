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

            verticalBlurTexture = new Texture2Dlite(DOUEngine.domainFramebufferRezolution.X / 10, DOUEngine.domainFramebufferRezolution.Y / 10, PixelInternalFormat.Rgb,
                PixelFormat.Rgb, PixelType.UnsignedByte, (int)All.Linear);

            horizontalBlurTexture = new Texture2Dlite(DOUEngine.domainFramebufferRezolution.X / 10, DOUEngine.domainFramebufferRezolution.Y / 10, PixelInternalFormat.Rgb,
                PixelFormat.Rgb, PixelType.UnsignedByte, (int)All.Linear);

            frameTextureLowRezolution = new Texture2Dlite(DOUEngine.domainFramebufferRezolution.X / 10, DOUEngine.domainFramebufferRezolution.Y / 10, PixelInternalFormat.Rgb,
                PixelFormat.Rgb, PixelType.UnsignedByte, (int)All.Linear);

            lensFlareResultTexture = new Texture2Dlite(DOUEngine.domainFramebufferRezolution.X, DOUEngine.domainFramebufferRezolution.Y, PixelInternalFormat.Rgb,
                PixelFormat.Rgb, PixelType.UnsignedByte);
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
            Debug.Log.addToLog(DateTime.Now.ToString() + "  " + "LensFlare Framebuffer 1 : " + base.getFramebufferLog());
            base.unbindFramebuffer();
        }

        #endregion

    }
}
