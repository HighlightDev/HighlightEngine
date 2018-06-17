using FramebufferAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using MassiveGame.API.Collector;
using TextureLoader;
using System.Drawing;

namespace MassiveGame.RenderCore
{
    public class DefaultFrameBuffer
    {
        private Int32 FramebufferDescriptor;
        private ITexture ColorTexture;
        private ITexture DepthStencilTexture;

        private RenderTargetParams ColorTextureParams;
        private RenderTargetParams DepthStencilTextureParams;
        
        public void Bind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FramebufferDescriptor);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
            GL.Viewport(0, 0, ColorTextureParams.TexBufferWidth, ColorTextureParams.TexBufferHeight);
        }

        public void Unbind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public RenderTargetParams GetParams()
        {
            return ColorTextureParams;
        }

        public DefaultFrameBuffer(Point virtualScreenRezolution)
        {
            InitFramebuffer(virtualScreenRezolution.X, virtualScreenRezolution.Y);
        }

        public DefaultFrameBuffer(Int32 WidthRezolution, Int32 HeightRezolution)
        {
            InitFramebuffer(WidthRezolution, HeightRezolution);
        }

        private void InitFramebuffer(Int32 WidthRezolution, Int32 HeightRezolution)
        {
            ColorTextureParams = new RenderTargetParams(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, WidthRezolution, HeightRezolution, PixelFormat.Rgb, PixelType.UnsignedByte);
            DepthStencilTextureParams = new RenderTargetParams(TextureTarget.Texture2D, 0, PixelInternalFormat.Depth24Stencil8, WidthRezolution, HeightRezolution, PixelFormat.DepthComponent, PixelType.Float);
            ColorTexture = ResourcePool.GetRenderTarget(ColorTextureParams);
            DepthStencilTexture = ResourcePool.GetRenderTarget(DepthStencilTextureParams);

            FramebufferDescriptor = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FramebufferDescriptor);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, ColorTexture.GetTextureDescriptor(), 0);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, TextureTarget.Texture2D, DepthStencilTexture.GetTextureDescriptor(), 0);
            GL.DrawBuffer(DrawBufferMode.ColorAttachment0);
            GL.ReadBuffer(ReadBufferMode.None);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public ITexture GetColorTexture()
        {
            return ColorTexture;
        }

        public ITexture GetDepthStencilTexture()
        {
            return DepthStencilTexture;
        }

        public void CleanUp()
        {
            ResourcePool.ReleaseRenderTarget(ColorTexture);
            ResourcePool.ReleaseRenderTarget(DepthStencilTexture);
            GL.DeleteFramebuffer(FramebufferDescriptor);
        }
    }
}
