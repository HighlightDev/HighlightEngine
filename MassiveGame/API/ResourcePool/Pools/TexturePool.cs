using TextureLoader;

namespace MassiveGame.API.ResourcePool.Pools
{
    public class TexturePool : Pool
    {
        public TexturePool() { }

        protected override bool IsValidResourceType(object arg)
        {
            return arg is ITexture;
        }
    }
}
