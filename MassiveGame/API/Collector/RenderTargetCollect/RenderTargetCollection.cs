using FramebufferAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TextureLoader;

namespace MassiveGame.API.Collector.TextureBufferCollect
{
    public class RenderTargetCollection
    {
        private readonly Int32 MAX_RENDER_TARGETS;
        private Int32 AllocatedRenderTargets;

        private Dictionary<RenderTargetParams, ITexture> renderTargetDictionary;
        private Dictionary<RenderTargetParams, Int32> referenceCount;

        public RenderTargetCollection(Int32 MaxRenderTargets)
        {
            MAX_RENDER_TARGETS = MaxRenderTargets;
            AllocatedRenderTargets = 0;

            renderTargetDictionary = new Dictionary<RenderTargetParams, ITexture>();
            referenceCount = new Dictionary<RenderTargetParams, Int32>();
        }

        public ITexture RetrieveRenderTarget(RenderTargetParams RenderTargetKey)
        {
            return TryGetRenderTarget(RenderTargetKey);
        }

        private ITexture TryGetRenderTarget(RenderTargetParams RenderTargetKey)
        {
            ITexture result = null;
            bool bAccessForbidden = false;
            bool bExist = renderTargetDictionary.TryGetValue(RenderTargetKey, out result);
            // Check if current render target is busy, if yes - we can't use it, else - we can reuse this render target

            // todo : still don't have ideas how to resolve problem with multiple render targets stored in dictionary
            // I can't use render target which is currently been used by some module, so I have to create another render target with 
            // same settings, but I can't store new render target in dictionary, because it has the same key - render target settings 
            if (bExist && referenceCount[RenderTargetKey] != 0)
            {
                bExist = false;
            }

            IncreaseRefCounter(RenderTargetKey, bExist, bAccessForbidden);
            if (!bExist)
            {
                result = RenderTargetAllocator.AllocateRenderTarget(RenderTargetKey);
                renderTargetDictionary.Add(RenderTargetKey, result);
            }
          
            return result;
        }

        private void IncreaseRefCounter(RenderTargetParams RenderTargetKey, bool exist, bool bAccessForbidden)
        {
            if (exist)
            {
                referenceCount[RenderTargetKey]++;
            }
            else
            {
                AllocatedRenderTargets++;
                referenceCount.Add(RenderTargetKey, 1);
            }
        }

        public void ReleaseRenderTarget(RenderTargetParams RenderTargetKey)
        {
            bool bExist = false;
            bExist = renderTargetDictionary.Any(value => value.Key == RenderTargetKey);
            if (bExist)
            {
                DecrementReference(RenderTargetKey);
            }
        }

        public void ReleaseRenderTarget(ITexture renderTargetHandle)
        {
            bool bExist = false;
            RenderTargetParams key = null;
            bExist = renderTargetDictionary.Any(new Func<KeyValuePair<RenderTargetParams, ITexture>, bool>(value =>
            {
                if (value.Value == renderTargetHandle)
                {
                    key = value.Key;
                    return true;
                }
                else
                {
                    return false;
                }
            }));

            if (bExist)
            {
                DecrementReference(key);
            }
        }

        private void DecrementReference(RenderTargetParams RenderTargetKey)
        {
            referenceCount[RenderTargetKey]--;
            if (referenceCount[RenderTargetKey] == 0/* && AllocatedRenderTargets > MAX_RENDER_TARGETS*/)
            {
                AllocatedRenderTargets--;
                RenderTargetAllocator.ReleaseRenderTarget(renderTargetDictionary[RenderTargetKey]);
                renderTargetDictionary.Remove(RenderTargetKey);
                referenceCount.Remove(RenderTargetKey);
            }
        }
    }
}
