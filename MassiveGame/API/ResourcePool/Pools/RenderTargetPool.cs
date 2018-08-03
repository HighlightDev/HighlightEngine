using System;
using System.Linq;
using TextureLoader;

namespace MassiveGame.API.ResourcePool.Pools
{
    public class RenderTargetPool : Pool
    {
        public RenderTargetPool() { }

        public ITexture GetRenderTargetAt(Int32 index)
        {
            ITexture result = null;
            if (index < resourceMap.Count)
                result = (ITexture)resourceMap.ToList()[index].Value;

            return result;
        }

        protected override bool IsValidResourceType(object arg)
        {
            return arg is ITexture;
        }
    }

}
