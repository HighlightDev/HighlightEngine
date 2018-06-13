using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextureLoader;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using ShaderPattern;
using MassiveGame.API.Collector;
using GpuGraphics;
using System.Drawing;

namespace MassiveGame.RenderCore
{
    public static class TextureResolver
    {
        private static CopyTextureShader copyShader;
        private static ResolvePostProcessResultToDefaultFramebufferShader resolvePostProcessShader;

        static TextureResolver()
        {
            copyShader = (CopyTextureShader)ResourcePool.GetShaderProgram(ProjectFolders.ShadersPath + "copyTextureVS.glsl", ProjectFolders.ShadersPath + "copyTextureFS.glsl", "",
                    typeof(CopyTextureShader));

            resolvePostProcessShader = (ResolvePostProcessResultToDefaultFramebufferShader)ResourcePool.GetShaderProgram(ProjectFolders.ShadersPath + "resolvePostProcessResultToDefaultFramebufferVS.glsl",
                ProjectFolders.ShadersPath + "resolvePostProcessResultToDefaultFramebufferFS.glsl", "", typeof(ResolvePostProcessResultToDefaultFramebufferShader));
        }

        public static void ResolvePostProcessResultToDefaultFramebuffer(ITexture frameTexture, ITexture postProcessResultTexture, Point actualScreenRezolution)
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit | ClearBufferMask.StencilBufferBit);
            GL.Viewport(0, 0, actualScreenRezolution.X, actualScreenRezolution.Y);

            GL.Disable(EnableCap.DepthTest);
            resolvePostProcessShader.startProgram();

            frameTexture.BindTexture(TextureUnit.Texture0);
            postProcessResultTexture.BindTexture(TextureUnit.Texture1);
            resolvePostProcessShader.setFrameSampler(0);
            resolvePostProcessShader.setPostProcessResultSampler(1);
            var quadBuffer = ScreenQuad.GetScreenQuadBuffer();
            VAOManager.renderBuffers(quadBuffer, PrimitiveType.Triangles);
            resolvePostProcessShader.stopProgram();
            GL.Enable(EnableCap.DepthTest);
        }

        public static ITexture CopyTexture(ITexture src)
        {
            ITexture dst = null;
            var emptyTexture = Texture2Dlite.genEmptyImage(src.GetTextureRezolution().X, src.GetTextureRezolution().Y,
                (Int32)All.Nearest, PixelInternalFormat.Rgb, OpenTK.Graphics.OpenGL.PixelFormat.Rgb, PixelType.UnsignedByte, TextureWrapMode.Repeat);

            var renderTarget = GL.GenFramebuffer();

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, renderTarget);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, src.GetTextureTarget(), emptyTexture, 0);
            GL.DrawBuffer(DrawBufferMode.ColorAttachment0);

            GL.Disable(EnableCap.DepthTest);
            GL.Viewport(0, 0, src.GetTextureRezolution().X, src.GetTextureRezolution().Y);
            // start copy texture to render target
            copyShader.startProgram();
            src.BindTexture(TextureUnit.Texture0);
            copyShader.SetUniformValues(0);
            var quadBuffer = ScreenQuad.GetScreenQuadBuffer();
            VAOManager.renderBuffers(quadBuffer, PrimitiveType.Triangles);
            copyShader.stopProgram();
            GL.Enable(EnableCap.DepthTest);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.DeleteFramebuffer(renderTarget);

            dst = new Texture2Dlite(emptyTexture, src.GetTextureRezolution());
            return dst;
        }
    }
}
