using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FramebufferAPI;
using TextureLoader;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace MassiveGame
{
    public class PostprocessFBO : Framebuffer
    {
        #region Definitions
        
        public Texture2D Texture { get { return base.textures; } }

        #endregion

        #region Setters

        protected override void setTextures()
        {
            base.textures = new Texture2D();
            //base.textures.genEmptyImg(1, 1366, 768, (int)All.Nearest, PixelInternalFormat.Rgb32f, PixelFormat.Rgb); //for hdr

            /*  TO DO:
             *  DOF - generate 3 textures(default image, blured image, depth image)
             *  Motion_blur - generate 2 textures(default image, blured image)
             *  Bloom - generate 3 textures(default image, brightness image, blured image)*/
            switch (PostprocessRenderer.PostProcessType)
            {
                case PostprocessType.DOF_BLUR :
                    {
                        base.textures.genEmptyImg(2, 1366, 768, (int)All.Nearest, PixelInternalFormat.Rgb,
                            PixelFormat.Rgb, PixelType.UnsignedByte); //for screen image and blur
                        base.textures.genEmptyImg(1, 1366, 768, (int)All.Nearest, PixelInternalFormat.Depth24Stencil8, PixelFormat.DepthStencil,
                            PixelType.Float); //for depth image
                        /*If lens flare or god rays are enabled - gen additional texture for result*/
                        if (LensFlareRenderer.LensFlareEnabled || GodRaysRenderer.GodRaysEnabled)
                        {
                            base.textures.genEmptyImg(1, 1366, 768, (int)All.Nearest, PixelInternalFormat.Rgb,
                              PixelFormat.Rgb, PixelType.UnsignedByte); //for lens flare result sending
                        }
                        break;
                    }
                case PostprocessType.MOTION_BLUR :
                    {
                        base.textures.genEmptyImg(1, 1366, 768, (int)All.Nearest, PixelInternalFormat.Rgb,
                            PixelFormat.Rgb, PixelType.UnsignedByte); //for screen image
                        base.textures.genEmptyImg(1, 400, 200, (int)All.Nearest, PixelInternalFormat.Rgb,
                            PixelFormat.Rgb, PixelType.UnsignedByte); //for blur image
                        break;
                    }
                case PostprocessType.BLOOM :
                    {
                        base.textures.genEmptyImg(1, 1366, 768, (int)All.Nearest, PixelInternalFormat.Rgb,
                            PixelFormat.Rgb, PixelType.UnsignedByte); //for screen image and pass 2 bloom
                        base.textures.genEmptyImg(2, 1366 / 7, 768 / 7, (int)All.Linear, PixelInternalFormat.Rgb,
                            PixelFormat.Rgb, PixelType.UnsignedByte); //for blur pass 3 and 4 
                        /*If lens flare or god rays are enabled - gen additional texture for result*/
                        if (LensFlareRenderer.LensFlareEnabled || GodRaysRenderer.GodRaysEnabled)
                        {
                            base.textures.genEmptyImg(1, 1366, 768, (int)All.Nearest, PixelInternalFormat.Rgb,
                             PixelFormat.Rgb, PixelType.UnsignedByte); //for lens flare result sending
                        }
                        break;
                    }
            }
        }

        protected override void setFramebuffers()
        {
            switch (PostprocessRenderer.PostProcessType)
            {
                case PostprocessType.MOTION_BLUR:
                    {
                        base.genFramebuffers(2);
                        /*Blur framebuffer + blur texture*/
                        base.bindFramebuffer(2);
                        base.attachTextureToFramebuffer(FramebufferAttachment.ColorAttachment0, base.textures.TextureID[1]);
                        /*Scene framebuffer + frame texture*/
                        base.bindFramebuffer(1);
                        base.attachTextureToFramebuffer(FramebufferAttachment.ColorAttachment0, base.textures.TextureID[0]);
                        break;
                    }
                case PostprocessType.DOF_BLUR:
                    {
                        if (LensFlareRenderer.LensFlareEnabled || GodRaysRenderer.GodRaysEnabled) { base.genFramebuffers(3); }
                        else { base.genFramebuffers(2); }
                        /*Blur framebuffer + blur texture*/
                        base.bindFramebuffer(2);
                        base.attachTextureToFramebuffer(FramebufferAttachment.ColorAttachment0, base.textures.TextureID[1]);
                        /*Scene framebuffer + frame texture*/
                        base.bindFramebuffer(1);
                        base.attachTextureToFramebuffer(FramebufferAttachment.ColorAttachment0, base.textures.TextureID[0]);
                        /* + depth texture*/
                        base.attachTextureToFramebuffer(FramebufferAttachment.DepthAttachment, base.textures.TextureID[2]);
                        if (LensFlareRenderer.LensFlareEnabled || GodRaysRenderer.GodRaysEnabled)
                        {
                            /* For Lens flare or god rays result*/
                            base.bindFramebuffer(3);
                            base.attachTextureToFramebuffer(FramebufferAttachment.ColorAttachment0, base.textures.TextureID[3]);
                        }
                        break;
                    }
                case PostprocessType.BLOOM:
                    {
                        if (LensFlareRenderer.LensFlareEnabled || GodRaysRenderer.GodRaysEnabled) { base.genFramebuffers(4); }
                        else { base.genFramebuffers(3); }
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
                        break;
                    }
            }
        }

        protected override void setRenderbuffers()
        {
            /*Attaching depth to framebuffer*/
            if (PostprocessRenderer.PostProcessType == PostprocessType.MOTION_BLUR || PostprocessRenderer.PostProcessType == PostprocessType.BLOOM)
            {
                base.bindFramebuffer(1);
                base.genRenderbuffers(1);
                base.bindRenderbuffer(1);
                base.renderbufferStorage(RenderbufferStorage.Depth24Stencil8, base.textures.Rezolution[0].widthRezolution,
                    base.textures.Rezolution[0].heightRezolution);
                base.attachRenderbufferToFramebuffer(FramebufferAttachment.DepthStencilAttachment, 1);
            }
        }

        #endregion

        #region Constructor

        public PostprocessFBO()
        {
            base.bindFramebuffer(1);
            Debug.Log.addToLog( DateTime.Now.ToString() + "  " + "Postprocess Framebuffer 1 : " + base.getFramebufferLog());
            base.bindFramebuffer(2);
            Debug.Log.addToLog( DateTime.Now.ToString() + "  " + "Postprocess Framebuffer 2 : " + base.getFramebufferLog());
            if (PostprocessRenderer.PostProcessType == PostprocessType.BLOOM)
            {
                base.bindFramebuffer(3);
                Debug.Log.addToLog( DateTime.Now.ToString() + "  " + "Postprocess Framebuffer 3 : " + base.getFramebufferLog());
            }
            base.unbindFramebuffer();
        }

        #endregion
    }
}
