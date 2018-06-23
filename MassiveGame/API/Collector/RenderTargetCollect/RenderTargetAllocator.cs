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
            result = new Texture2D(renderTargetParams);
            return result;
        }

        public static void ReleaseRenderTarget(ITexture RenderTarget)
        {
            RenderTarget.CleanUp();
        }
    }
}
