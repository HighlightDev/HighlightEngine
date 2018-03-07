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

namespace MassiveGame.RenderCore
{
    public static class RenderAPI
    {
        private static ResolveTextureShader resolveShader;
        static RenderAPI()
        {
            resolveShader = (ResolveTextureShader)ResourcePool.GetShaderProgram(ProjectFolders.ShadersPath + "resolveTextureVS.glsl", ProjectFolders.ShadersPath + "resolveTextureFS.glsl", "",
                    typeof(ResolveTextureShader));
        }

        public static ITexture ResolveTexture(ITexture src, RectParams rectParams)
        {
            ITexture dst = null;
            var emptyTexture = Texture2Dlite.genEmptyImage(rectParams.Width, rectParams.Height, (Int32)All.Nearest, PixelInternalFormat.Rgb, OpenTK.Graphics.OpenGL.PixelFormat.Rgb, PixelType.UnsignedByte, TextureWrapMode.Repeat);
            var renderTarget = GL.GenFramebuffer();

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, renderTarget);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, src.GetTextureTarget(), emptyTexture, 0);
            GL.DrawBuffer(DrawBufferMode.ColorAttachment0);

            GL.Disable(EnableCap.DepthTest);
            GL.Viewport(0, 0, rectParams.Width, rectParams.Height);
            // start copy texture to render target
            resolveShader.startProgram();
            src.BindTexture(TextureUnit.Texture0);
            resolveShader.SetUniformValues(0);
            var quadBuffer = ScreenQuad.GetScreenQuadBuffer();
            VAOManager.renderBuffers(quadBuffer, PrimitiveType.Triangles);
            resolveShader.stopProgram();
            GL.Enable(EnableCap.DepthTest);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.DeleteFramebuffer(renderTarget);

            dst = new Texture2Dlite(emptyTexture, rectParams);
            return dst;
        }
    }
}
