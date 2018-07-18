﻿using TextureLoader;
using OpenTK.Graphics.OpenGL;
using MassiveGame.API.Collector;
using GpuGraphics;
using System.Drawing;
using MassiveGame.Settings;

namespace MassiveGame.Core.RenderCore
{
    public static class TextureResolver
    {
        private static CopyTextureShader copyShader;
        private static ResolvePostProcessResultToDefaultFramebufferShader resolvePostProcessShader;

        static TextureResolver()
        {
            copyShader = ResourcePool.GetShaderProgram<CopyTextureShader>(ProjectFolders.ShadersPath + "copyTextureVS.glsl", ProjectFolders.ShadersPath + "copyTextureFS.glsl", "");

            resolvePostProcessShader = ResourcePool.GetShaderProgram<ResolvePostProcessResultToDefaultFramebufferShader>(ProjectFolders.ShadersPath + "resolvePostProcessResultToDefaultFramebufferVS.glsl",
                ProjectFolders.ShadersPath + "resolvePostProcessResultToDefaultFramebufferFS.glsl", "");
        }

        public static void ResolvePostProcessResultToDefaultFramebuffer(ITexture frameTexture, ITexture postProcessResultTexture, Point actualScreenRezolution)
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit | ClearBufferMask.StencilBufferBit);
            GL.Viewport(0, 0, actualScreenRezolution.X, actualScreenRezolution.Y);

            //GL.Disable(EnableCap.DepthTest);
            resolvePostProcessShader.startProgram();

            frameTexture.BindTexture(TextureUnit.Texture0);
            postProcessResultTexture.BindTexture(TextureUnit.Texture1);
            resolvePostProcessShader.setPostProcessResultSampler(1);
            var quadBuffer = ScreenQuad.GetScreenQuadBuffer();
            quadBuffer.RenderVAO(PrimitiveType.Triangles);
            resolvePostProcessShader.stopProgram();
            GL.Enable(EnableCap.DepthTest);
        }

        public static ITexture CopyTexture(ITexture src)
        {
            ITexture dst = ResourcePool.GetRenderTarget(src.GetTextureParameters());

            var renderTarget = GL.GenFramebuffer();

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, renderTarget);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, src.GetTextureParameters().TexTarget, dst.GetTextureDescriptor(), 0);
            GL.DrawBuffer(DrawBufferMode.ColorAttachment0);

            //GL.Disable(EnableCap.DepthTest);
            GL.Viewport(0, 0, src.GetTextureRezolution().X, src.GetTextureRezolution().Y);
            // start copy texture to render target
            copyShader.startProgram();
            src.BindTexture(TextureUnit.Texture0);
            copyShader.SetUniformValues(0);
            var quadBuffer = ScreenQuad.GetScreenQuadBuffer();
            quadBuffer.RenderVAO(PrimitiveType.Triangles);
            copyShader.stopProgram();
            GL.Enable(EnableCap.DepthTest);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.DeleteFramebuffer(renderTarget);

            return dst;
        }
    }
}