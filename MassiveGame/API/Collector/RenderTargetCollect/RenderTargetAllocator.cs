using FramebufferAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using TextureLoader;
using System.Drawing;

namespace MassiveGame.API.Collector.TextureBufferCollect
{
    public static class RenderTargetAllocator
    {
        public static ITexture AllocateRenderTarget(TextureParameters renderTargetParams)
        {
            ITexture result = null;
            Int32 renderTarget = GL.GenTexture();
            GL.BindTexture(renderTargetParams.TexTarget, renderTarget);
            GL.TexImage2D(renderTargetParams.TexTarget, renderTargetParams.TexMipLvl, renderTargetParams.TexPixelInternalFormat, renderTargetParams.TexBufferWidth,
                renderTargetParams.TexBufferHeight, 0, renderTargetParams.TexPixelFormat, renderTargetParams.TexPixelType, new IntPtr(0));

            GL.TexParameter(renderTargetParams.TexTarget, TextureParameterName.TextureMagFilter, (Int32)renderTargetParams.MagFilter);
            GL.TexParameter(renderTargetParams.TexTarget, TextureParameterName.TextureMinFilter, (Int32)renderTargetParams.MinFilter);
            GL.TexParameter(renderTargetParams.TexTarget, TextureParameterName.TextureWrapS, (Int32)All.Repeat);
            GL.TexParameter(renderTargetParams.TexTarget, TextureParameterName.TextureWrapT, (Int32)All.Repeat);

            result = new Texture2D(renderTarget, new Point(renderTargetParams.TexBufferWidth, renderTargetParams.TexBufferHeight));
            return result;
        }

        public static void ReleaseRenderTarget(ITexture RenderTarget)
        {
            RenderTarget.CleanUp();
        }
    }
}
