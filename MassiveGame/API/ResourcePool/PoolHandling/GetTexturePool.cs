using MassiveGame.API.ResourcePool.Pools;

namespace MassiveGame.API.ResourcePool.PoolHandling
{
    public class GetTexturePool : IPoolObtainable<Pool>
    {
        public Pool GetPool() { return PoolCollector.GetInstance().s_TexturePool; }
    }
}
