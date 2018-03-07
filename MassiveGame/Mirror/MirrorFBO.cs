using FramebufferAPI;
using GpuGraphics;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using ShaderPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextureLoader;

namespace MassiveGame
{
    public class MirrorFBO : Framebuffer
    {
        #region Definitions

        public Texture2D Texture { get { return base.textures; } }
        private const int IMG_REZOLUTION = 256;

        #endregion

        #region Setter

        protected override void setTextures()
        {
            base.textures = new Texture2D();
            base.textures.genEmptyImg(1, IMG_REZOLUTION, IMG_REZOLUTION, (int)All.Nearest, PixelInternalFormat.Rgb,
                PixelFormat.Rgb, PixelType.UnsignedByte);
        }

        protected override void setFramebuffers()
        {
            base.genFramebuffers(1);
            base.bindFramebuffer(1);
            base.attachTextureToFramebuffer(FramebufferAttachment.ColorAttachment0, base.textures.TextureID[0]);
            GL.DrawBuffer(DrawBufferMode.ColorAttachment0);
        }

        protected override void setRenderbuffers()
        {
            base.genRenderbuffers(1);
            base.bindRenderbuffer(1);
            base.renderbufferStorage(RenderbufferStorage.DepthComponent24,
                base.textures.Rezolution[0].widthRezolution, base.textures.Rezolution[0].heightRezolution);
            base.attachRenderbufferToFramebuffer(FramebufferAttachment.DepthAttachment);
        }

        #endregion

        #region Constructor

        public MirrorFBO()
            : base()
        {
            base.bindFramebuffer(1);
            Debug.Log.addToLog( DateTime.Now.ToString() + "  " + "Mirror Framebuffer 1 : " + base.getFramebufferLog());
            base.unbindFramebuffer();
        }

        #endregion
    }
}
