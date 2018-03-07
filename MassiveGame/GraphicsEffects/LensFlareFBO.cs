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

        public Texture2D Texture { private set { base.textures = value; } get { return base.textures; } }

        #endregion

        #region Setters

        protected override void setTextures()
        {
            /* Img 1 - vertical blur stage image;
             * Img 2 - horizontal blur stage image;
             * Img 3 - default frametexture, with high rezolution; 
             * Img 4 - frametexture, with low rezolution, for bright parts of sun. */
            base.textures = new Texture2D();
            base.textures.genEmptyImg(2, 1366 / 5, 768 / 5, (int)All.Linear, PixelInternalFormat.Rgb,
                PixelFormat.Rgb, PixelType.UnsignedByte);
            base.textures.genEmptyImg(1, 1366, 768, (int)All.Nearest, PixelInternalFormat.Rgb,
                PixelFormat.Rgb, PixelType.UnsignedByte);
            base.textures.genEmptyImg(1, 1366 / 5, 768 / 5, (int)All.Nearest, PixelInternalFormat.Rgb,
                PixelFormat.Rgb, PixelType.UnsignedByte);
        }
        
        protected override void setFramebuffers()
        {
            base.genFramebuffers(4);
            base.bindFramebuffer(1);
            base.attachTextureToFramebuffer(FramebufferAttachment.ColorAttachment0, base.textures.TextureID[0]);

            base.bindFramebuffer(2);
            base.attachTextureToFramebuffer(FramebufferAttachment.ColorAttachment0, base.textures.TextureID[1]);

            base.bindFramebuffer(3);
            base.attachTextureToFramebuffer(FramebufferAttachment.ColorAttachment0, base.textures.TextureID[2]);
            base.bindFramebuffer(4);
            base.attachTextureToFramebuffer(FramebufferAttachment.ColorAttachment0, base.textures.TextureID[3]);
        }

        protected override void setRenderbuffers()
        {
            base.bindFramebuffer(3);
            base.genRenderbuffers(2);
            base.bindRenderbuffer(1);
            base.renderbufferStorage(RenderbufferStorage.DepthComponent24, base.textures.Rezolution[2].widthRezolution,
                base.textures.Rezolution[2].heightRezolution);
            base.attachRenderbufferToFramebuffer(FramebufferAttachment.DepthAttachment, 1);

            base.bindFramebuffer(4);
            base.bindRenderbuffer(2);
            base.renderbufferStorage(RenderbufferStorage.DepthComponent24, base.textures.Rezolution[3].widthRezolution,
                base.textures.Rezolution[3].heightRezolution);
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
