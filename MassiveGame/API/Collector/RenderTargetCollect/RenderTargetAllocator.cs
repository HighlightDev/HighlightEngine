using FramebufferAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace MassiveGame.API.Collector.TextureBufferCollect
{
    public static class RenderTargetAllocator
    {
        public static Int32 AllocateRenderTarget(RenderTargetParams renderTargetParams)
        {
            Int32 renderTarget = GL.GenTexture();
            GL.BindTexture(renderTargetParams.TexTarget, renderTarget);
            GL.TexImage2D(renderTargetParams.TexTarget, renderTargetParams.TexMipLvl, renderTargetParams.TexPixelInternalFormat, renderTargetParams.TexBufferWidth,
                renderTargetParams.TexBufferHeight, 0, renderTargetParams.TexPixelFormat, renderTargetParams.TexPixelType, new IntPtr(0));

            float[] borderColor = new float[4] { 1.0f, 1.0f, 1.0f, 1.0f };
            GL.TexParameter(renderTargetParams.TexTarget, TextureParameterName.TextureMagFilter, (Int32)All.Nearest);
            GL.TexParameter(renderTargetParams.TexTarget, TextureParameterName.TextureMinFilter, (Int32)All.Nearest);
            GL.TexParameter(renderTargetParams.TexTarget, TextureParameterName.TextureWrapS, (Int32)All.ClampToBorder);
            GL.TexParameter(renderTargetParams.TexTarget, TextureParameterName.TextureWrapT, (Int32)All.ClampToBorder);
            GL.TexParameter(renderTargetParams.TexTarget, TextureParameterName.TextureBorderColor, borderColor);
            return renderTarget;
        }

        public static void ReleaseRenderTarget(Int32 RenderTarget)
        {
            GL.DeleteTexture(RenderTarget);
        }
    }
}
