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

        public Texture2D Texture { private set { base.textures = value; } get { return base.textures; } }

        #endregion

        #region Setter

        protected override void setTextures()
        {
            base.textures = new Texture2D();
            textures.genEmptyImg(1, DOUEngine.ScreenRezolution.X, DOUEngine.ScreenRezolution.Y, (int)All.Nearest,PixelInternalFormat.Rgb, PixelFormat.Rgb, PixelType.UnsignedByte);
            textures.genEmptyImg(1, DOUEngine.ScreenRezolution.X / 10, DOUEngine.ScreenRezolution.Y / 10, (int)All.Linear, PixelInternalFormat.Rgb, PixelFormat.Rgb, PixelType.UnsignedByte);

            /*IF pp is disabled and lens flare is enabled*/
            if (LensFlareRenderer.LensFlareEnabled)
            {
                textures.genEmptyImg(1, DOUEngine.ScreenRezolution.X, DOUEngine.ScreenRezolution.Y, (int)All.Linear, PixelInternalFormat.Rgb, PixelFormat.Rgb, PixelType.UnsignedByte);
            }
        }

        protected override void setFramebuffers()
        {
            base.genFramebuffers(3);
           
            base.bindFramebuffer(2);
            base.attachTextureToFramebuffer(FramebufferAttachment.ColorAttachment0, base.textures.TextureID[1]);

            base.bindFramebuffer(1);
            base.attachTextureToFramebuffer(FramebufferAttachment.ColorAttachment0, base.textures.TextureID[0]);

             /*IF pp is disabled and lens flare is enabled*/
            if (LensFlareRenderer.LensFlareEnabled)
            {
                base.bindFramebuffer(3);
                base.attachTextureToFramebuffer(FramebufferAttachment.ColorAttachment0, base.textures.TextureID[2]);
            }
        }

        protected override void setRenderbuffers()
        {
            base.genRenderbuffers(2);

            base.bindFramebuffer(1);
            base.bindRenderbuffer(1);
            base.renderbufferStorage(RenderbufferStorage.DepthComponent24, base.textures.Rezolution[0].widthRezolution,
                base.textures.Rezolution[0].heightRezolution);
            base.attachRenderbufferToFramebuffer(FramebufferAttachment.DepthAttachment, 1);

            base.bindFramebuffer(2);
            base.bindRenderbuffer(2);
            base.renderbufferStorage(RenderbufferStorage.DepthComponent24, base.textures.Rezolution[1].widthRezolution,
                base.textures.Rezolution[1].heightRezolution);
            base.attachRenderbufferToFramebuffer(FramebufferAttachment.DepthAttachment, 2);
        }

        #endregion

        #region Constructor

        public GodRaysFBO() : base()
        {
            Debug.Log.addToLog(DateTime.Now.ToString() + "  " + "GodRays Framebuffer 1 : " + base.getFramebufferLog());
            base.unbindFramebuffer();
        }

        #endregion 
    }
}
