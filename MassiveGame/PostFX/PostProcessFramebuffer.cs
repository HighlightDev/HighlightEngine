using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextureLoader;

using OpenTK.Graphics.OpenGL;

namespace MassiveGame.PostFX
{
    public class BloomFramebuffer : FramebufferAPI.Framebuffer
    {
        public Texture2D Texture { get { return base.textures; } }

        protected override void setTextures()
        {
            base.textures = new Texture2D();
            base.textures.genEmptyImg(1, DOUEngine.ScreenRezolution.X, DOUEngine.ScreenRezolution.Y, (int)All.Nearest, PixelInternalFormat.Rgb,
                           PixelFormat.Rgb, PixelType.UnsignedByte); //for screen image and pass 2 bloom
            base.textures.genEmptyImg(2, DOUEngine.ScreenRezolution.X / 7, DOUEngine.ScreenRezolution.Y / 7, (int)All.Linear, PixelInternalFormat.Rgb,
                PixelFormat.Rgb, PixelType.UnsignedByte); //for blur pass 3 and 4 
                                                          /*If lens flare or god rays are enabled - gen additional texture for result*/
            if (LensFlareRenderer.LensFlareEnabled || GodRaysRenderer.GodRaysEnabled)
            {
                base.textures.genEmptyImg(1, DOUEngine.ScreenRezolution.X, DOUEngine.ScreenRezolution.Y, (int)All.Nearest, PixelInternalFormat.Rgb,
                 PixelFormat.Rgb, PixelType.UnsignedByte); //for lens flare result sending
            }
        }

        protected override void setFramebuffers()
        {
            if (LensFlareRenderer.LensFlareEnabled || GodRaysRenderer.GodRaysEnabled)
                base.genFramebuffers(4); 
            else
                base.genFramebuffers(3); 
            /*Blur framebuffer + blur texture*/
            base.bindFramebuffer(3);
            base.attachTextureToFramebuffer(FramebufferAttachment.ColorAttachment0, base.textures.TextureID[2]);
            /*Brightness framebuffer + brightness texture*/
            base.bindFramebuffer(2);
            base.attachTextureToFramebuffer(FramebufferAttachment.ColorAttachment0, base.textures.TextureID[1]);
            /*Scene framebuffer + frame texture*/
            base.bindFramebuffer(1);
            base.attachTextureToFramebuffer(FramebufferAttachment.ColorAttachment0, base.textures.TextureID[0]);
            if (LensFlareRenderer.LensFlareEnabled || GodRaysRenderer.GodRaysEnabled)
            {
                /* For Lens flare or god rays result*/
                base.bindFramebuffer(4);
                base.attachTextureToFramebuffer(FramebufferAttachment.ColorAttachment0, base.textures.TextureID[3]);
            }
        }

        protected override void setRenderbuffers()
        {
            /*Attaching depth to framebuffer*/
         
                base.bindFramebuffer(1);
                base.genRenderbuffers(1);
                base.bindRenderbuffer(1);
                base.renderbufferStorage(RenderbufferStorage.DepthComponent24, base.textures.Rezolution[0].widthRezolution,
                    base.textures.Rezolution[0].heightRezolution);
                base.attachRenderbufferToFramebuffer(FramebufferAttachment.DepthAttachment, 1);
        }

    }
}
