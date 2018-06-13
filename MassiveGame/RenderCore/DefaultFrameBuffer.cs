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
        private Int32 FramebufferHandler;
        private Int32 RenderTargetHandler;
        private Int32 RenderBufferHandler;
        private RenderTargetParams FrameBufferSettings; 

        public void Bind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FramebufferHandler);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Viewport(0, 0, FrameBufferSettings.TexBufferWidth, FrameBufferSettings.TexBufferHeight);
        }

        public void Unbind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public RenderTargetParams GetParams()
        {
            return FrameBufferSettings;
        }

        public Int32 GetRenderTargetHandler()
        {
            return RenderTargetHandler;
        }

        public DefaultFrameBuffer(Int32 WidthRezolution, Int32 HeightRezolution)
        {
            FrameBufferSettings = new RenderTargetParams(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, WidthRezolution, HeightRezolution, PixelFormat.Rgb, PixelType.UnsignedByte);
            RenderTargetHandler = ResourcePool.GetRenderTarget(FrameBufferSettings);
            FramebufferHandler = GL.GenFramebuffer();
            RenderBufferHandler = GL.GenRenderbuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FramebufferHandler);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, RenderTargetHandler, 0);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, RenderBufferHandler);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, WidthRezolution, HeightRezolution);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, RenderBufferHandler);
            GL.DrawBuffer(DrawBufferMode.ColorAttachment0);
            GL.ReadBuffer(ReadBufferMode.None);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public ITexture GetTextureHandler()
        {
            ITexture result = null;
            if (RenderTargetHandler != -1)
                result = new Texture2Dlite(RenderTargetHandler, new Point(FrameBufferSettings.TexBufferWidth, FrameBufferSettings.TexBufferHeight));
            return result;
        }

        public void CleanUp()
        {
            ResourcePool.ReleaseRenderTarget(RenderTargetHandler);
            GL.DeleteFramebuffer(FramebufferHandler);
        }
    }
}
