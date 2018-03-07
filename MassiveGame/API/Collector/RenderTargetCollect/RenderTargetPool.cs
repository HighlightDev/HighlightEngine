﻿using FramebufferAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassiveGame.API.Collector.TextureBufferCollect
{
    public class RenderTargetPool
    {
        private RenderTargetCollection textureBufferCollection;

        public RenderTargetPool(Int32 MaxRenderTargets)
        {
            textureBufferCollection = new RenderTargetCollection(MaxRenderTargets);
        }

        public Int32 AllocateTextureBuffer(RenderTargetParams RenderTargetKey)
        {
            return textureBufferCollection.RetrieveRenderTarget(RenderTargetKey);
        }

        public void ReleaseRenderTarget(RenderTargetParams RenderTargetKey)
        {
            textureBufferCollection.ReleaseRenderTarget(RenderTargetKey);
        }

        public void ReleaseRenderTarget(Int32 RenderTarget)
        {
            textureBufferCollection.ReleaseRenderTarget(RenderTarget);
        }
    }
}