using TextureLoader;

namespace MassiveGame.API.ResourcePool.Policies
{
    public class RenderTargetAllocationPolicy : AllocationPolicy<TextureParameters, ITexture>
    {
        public override ITexture AllocateMemory(TextureParameters arg)
        {
            ITexture result = null;
            result = new Texture2D(arg);
            return result;
        }

        public override void FreeMemory(ITexture arg)
        {
            arg.CleanUp();
        }
    }
}
