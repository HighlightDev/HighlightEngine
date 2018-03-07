using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextureLoader;
using FramebufferAPI;
using OpenTK.Graphics.OpenGL;

namespace MassiveGame
{
    public class WaterFBO : Framebuffer
    {
        #region Definitions

        public Texture2D Texture { private set { base.textures = value; } get { return base.textures; } }

        #endregion

        #region Setter

        protected override void setTextures()
        {
            /*Create 3 textures : 
             1 - for reflection 
             2 - for refraction
             3 - for depth*/
            base.textures = new Texture2D();
            base.textures.genEmptyImg(2, 1024, 512, (int)All.Nearest, PixelInternalFormat.Rgb, PixelFormat.Rgb, PixelType.UnsignedByte);
            base.textures.genEmptyImg(1, 1024, 512, (int)All.Nearest, PixelInternalFormat.Depth24Stencil8,
                PixelFormat.DepthComponent, PixelType.Float);
        }

        protected override void setFramebuffers()
        {
            /*Create 2 framebuffers :
             1 - for reflection
             2 - for refraction*/
            base.genFramebuffers(2);
            base.bindFramebuffer(1);
            base.attachTextureToFramebuffer(FramebufferAttachment.ColorAttachment0, base.textures.TextureID[0]);
            base.bindFramebuffer(2);
            base.attachTextureToFramebuffer(FramebufferAttachment.ColorAttachment0, base.textures.TextureID[1]);
            base.attachTextureToFramebuffer(FramebufferAttachment.DepthStencilAttachment, base.textures.TextureID[2]);
        }

        protected override void setRenderbuffers()
        {
            /*Attach 1 depthbuffer*/
            base.genRenderbuffers(1);
            base.bindFramebuffer(1);
            base.bindRenderbuffer(1);
            base.renderbufferStorage(RenderbufferStorage.Depth24Stencil8,
                base.textures.Rezolution[0].widthRezolution, base.textures.Rezolution[0].heightRezolution);
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
