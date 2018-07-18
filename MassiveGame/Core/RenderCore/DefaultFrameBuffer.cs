﻿using System;
using OpenTK.Graphics.OpenGL;
using MassiveGame.API.Collector;
using TextureLoader;
using System.Drawing;

namespace MassiveGame.Core.RenderCore
{
    public class DefaultFrameBuffer
    {
        private Int32 FramebufferDescriptor;
        private ITexture ColorTexture;
        private ITexture DepthStencilTexture;

        public void Bind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FramebufferDescriptor);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
            GL.Viewport(0, 0, ColorTexture.GetTextureRezolution().X, ColorTexture.GetTextureRezolution().Y);
        }

        public void Unbind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public TextureParameters GetParams()
        {
            return ColorTexture.GetTextureParameters();
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
            var ColorTextureParams = new TextureParameters(TextureTarget.Texture2D, TextureMagFilter.Nearest, TextureMinFilter.Nearest, 0, PixelInternalFormat.Rgb, WidthRezolution, HeightRezolution, PixelFormat.Rgb, PixelType.UnsignedByte, TextureWrapMode.Repeat);
            var DepthStencilTextureParams = new TextureParameters(TextureTarget.Texture2D, TextureMagFilter.Nearest, TextureMinFilter.Nearest, 0, PixelInternalFormat.Depth24Stencil8, WidthRezolution, HeightRezolution, PixelFormat.DepthComponent, PixelType.Float, TextureWrapMode.Repeat);
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