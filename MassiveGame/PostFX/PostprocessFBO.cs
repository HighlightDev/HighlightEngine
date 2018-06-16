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

        public ITexture FrameTexture;

        public ITexture Dof_BluredTexture;
        public ITexture Dof_DepthTexture;
        public ITexture Dof_LensFlareTexture;

        public ITexture Bloom_VerticalBlurTexture;
        public ITexture Bloom_HorizontalBlurTexture;
        public ITexture Bloom_LensFlareTexture;

        #endregion

        #region Setters

        protected override void setTextures()
        {

            //base.textures.genEmptyImg(1, DOUEngine.ScreenRezolution.X, DOUEngine.ScreenRezolution.Y, (Int32)All.Nearest, PixelInternalFormat.Rgb32f, PixelFormat.Rgb); //for hdr

            /*  TO DO:
             *  DOF - generate 3 textures(default image, blured image, depth image)
             *  Bloom - generate 3 textures(default image, brightness image, blured image)*/

            FrameTexture = new Texture2Dlite(DOUEngine.domainFramebufferRezolution.X, DOUEngine.domainFramebufferRezolution.Y, PixelInternalFormat.Rgb, PixelFormat.Rgb, PixelType.UnsignedByte);

            switch (PostprocessRenderer.PostProcessType)
            {
                case PostprocessType.DOF_BLUR :
                    {
                        Dof_BluredTexture = new Texture2Dlite(DOUEngine.domainFramebufferRezolution.X, DOUEngine.domainFramebufferRezolution.Y, PixelInternalFormat.Rgb,
                            PixelFormat.Rgb, PixelType.UnsignedByte);
                        Dof_DepthTexture = new Texture2Dlite(DOUEngine.domainFramebufferRezolution.X, DOUEngine.domainFramebufferRezolution.Y, PixelInternalFormat.Depth24Stencil8, PixelFormat.DepthStencil,
                            PixelType.Float);

                        /*If lens flare or god rays are enabled - gen additional texture for result*/
                        if (true/*LensFlareRenderer.LensFlareEnabled || GodRaysRenderer.GodRaysEnabled*/)
                        {
                            Dof_LensFlareTexture = new Texture2Dlite(DOUEngine.domainFramebufferRezolution.X, DOUEngine.domainFramebufferRezolution.Y, PixelInternalFormat.Rgb,
                              PixelFormat.Rgb, PixelType.UnsignedByte);
                        }
                        break;
                    }
                case PostprocessType.BLOOM :
                    {
                        Bloom_HorizontalBlurTexture = new Texture2Dlite(DOUEngine.domainFramebufferRezolution.X / 7, DOUEngine.domainFramebufferRezolution.Y / 7, PixelInternalFormat.Rgb,
                            PixelFormat.Rgb, PixelType.UnsignedByte, (Int32)All.Linear);

                        Bloom_VerticalBlurTexture = new Texture2Dlite(DOUEngine.domainFramebufferRezolution.X / 7, DOUEngine.domainFramebufferRezolution.Y / 7, PixelInternalFormat.Rgb,
                           PixelFormat.Rgb, PixelType.UnsignedByte, (Int32)All.Linear);
                      
                        /*If lens flare or god rays are enabled - gen additional texture for result*/
                        if (true/*LensFlareRenderer.LensFlareEnabled || GodRaysRenderer.GodRaysEnabled*/)
                        {
                            Bloom_LensFlareTexture = new Texture2Dlite(DOUEngine.domainFramebufferRezolution.X, DOUEngine.domainFramebufferRezolution.Y, PixelInternalFormat.Rgb,
                             PixelFormat.Rgb, PixelType.UnsignedByte);
                        }
                        break;
                    }
            }
        }

        protected override void setFramebuffers()
        {
            switch (PostprocessRenderer.PostProcessType)
            {
                case PostprocessType.DOF_BLUR:
                    {
                        if (/*LensFlareRenderer.LensFlareEnabled || GodRaysRenderer.GodRaysEnabled*/true)
                            base.genFramebuffers(3); 
                        else
                            base.genFramebuffers(2); 

                        /*Blur framebuffer + blur texture*/
                        base.bindFramebuffer(2);
                        base.attachTextureToFramebuffer(FramebufferAttachment.ColorAttachment0, Dof_BluredTexture.GetTextureDescriptor());
                        /*Scene framebuffer + frame texture*/
                        base.bindFramebuffer(1);
                        base.attachTextureToFramebuffer(FramebufferAttachment.ColorAttachment0, FrameTexture.GetTextureDescriptor());
                        /* + depth texture*/
                        base.attachTextureToFramebuffer(FramebufferAttachment.DepthAttachment, Dof_DepthTexture.GetTextureDescriptor());
                        if (true/*LensFlareRenderer.LensFlareEnabled || GodRaysRenderer.GodRaysEnabled*/)
                        {
                            /* For Lens flare or god rays result*/
                            base.bindFramebuffer(3);
                            base.attachTextureToFramebuffer(FramebufferAttachment.ColorAttachment0, Dof_LensFlareTexture.GetTextureDescriptor());
                        }
                        break;
                    }
                case PostprocessType.BLOOM:
                    {
                        if (true/*LensFlareRenderer.LensFlareEnabled || GodRaysRenderer.GodRaysEnabled*/)
                        {
                            base.genFramebuffers(4);
                        }
                        else
                        {
                            base.genFramebuffers(3);
                        }
                        /*Blur framebuffer + blur texture*/
                        base.bindFramebuffer(3);
                        base.attachTextureToFramebuffer(FramebufferAttachment.ColorAttachment0, Bloom_HorizontalBlurTexture.GetTextureDescriptor());
                        /*Brightness framebuffer + brightness texture*/
                        base.bindFramebuffer(2);
                        base.attachTextureToFramebuffer(FramebufferAttachment.ColorAttachment0, Bloom_VerticalBlurTexture.GetTextureDescriptor());
                        /*Scene framebuffer + frame texture*/
                        base.bindFramebuffer(1);
                        base.attachTextureToFramebuffer(FramebufferAttachment.ColorAttachment0, FrameTexture.GetTextureDescriptor());
                        if (true/*LensFlareRenderer.LensFlareEnabled || GodRaysRenderer.GodRaysEnabled*/)
                        {
                            /* For Lens flare or god rays result*/
                            base.bindFramebuffer(4);
                            base.attachTextureToFramebuffer(FramebufferAttachment.ColorAttachment0, Bloom_LensFlareTexture.GetTextureDescriptor());
                        }
                        break;
                    }
            }
        }

        protected override void setRenderbuffers()
        {
            /*Attaching depth to framebuffer*/
            if (PostprocessRenderer.PostProcessType == PostprocessType.BLOOM)
            {
                base.bindFramebuffer(1);
                base.genRenderbuffers(1);
                base.bindRenderbuffer(1);
                base.renderbufferStorage(RenderbufferStorage.Depth24Stencil8, FrameTexture.GetTextureRezolution().X,
                    FrameTexture.GetTextureRezolution().Y);
                base.attachRenderbufferToFramebuffer(FramebufferAttachment.DepthStencilAttachment, 1);
            }
        }

        #endregion

        public override void cleanUp()
        {
            FrameTexture.CleanUp();
            if (PostprocessRenderer.PostProcessType == PostprocessType.DOF_BLUR)
            {
                Dof_BluredTexture.CleanUp();
                Dof_DepthTexture.CleanUp();
                if (Dof_LensFlareTexture != null)
                    Dof_LensFlareTexture.CleanUp();
            }

            else if (PostprocessRenderer.PostProcessType == PostprocessType.BLOOM)
            {
                Bloom_VerticalBlurTexture.CleanUp();
                Bloom_HorizontalBlurTexture.CleanUp();
                if (Bloom_LensFlareTexture != null)
                    Bloom_LensFlareTexture.CleanUp();
            }
            base.cleanUp();
        }

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
