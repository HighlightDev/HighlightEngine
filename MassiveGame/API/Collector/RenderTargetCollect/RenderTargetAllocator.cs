using TextureLoader;

namespace MassiveGame.API.Collector.RenderTargetCollect
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
