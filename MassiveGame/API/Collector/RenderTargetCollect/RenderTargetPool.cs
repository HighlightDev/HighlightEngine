using FramebufferAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextureLoader;

namespace MassiveGame.API.Collector.TextureBufferCollect
{
    public class RenderTargetPool
    {
        private RenderTargetCollection renderTargetBufferCollection;

        public RenderTargetPool(Int32 MaxRenderTargets)
        {
            renderTargetBufferCollection = new RenderTargetCollection(MaxRenderTargets);
        }

        public Int32 GetRenderTargetCount()
        {
            return renderTargetBufferCollection.GetRenderTargetCount();
        }

        public ITexture GetRenderTargetAt(Int32 index)
        {
            return renderTargetBufferCollection.GetRenderTargetAt(index);
        }

        public ITexture AllocateTextureBuffer(TextureParameters RenderTargetKey)
        {
            return renderTargetBufferCollection.RetrieveRenderTarget(RenderTargetKey);
        }

        public void ReleaseRenderTarget(TextureParameters RenderTargetKey)
        {
            renderTargetBufferCollection.ReleaseRenderTarget(RenderTargetKey);
        }

        public void ReleaseRenderTarget(ITexture RenderTarget)
        {
            renderTargetBufferCollection.ReleaseRenderTarget(RenderTarget);
        }
    }
}
