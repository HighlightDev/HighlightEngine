using MassiveGame.API.ResourcePool.Pools;

namespace MassiveGame.API.ResourcePool.PoolHandling
{
    class ObtainTexturePool : IPoolObtainable<Pool>
    {
        public Pool GetPool()
        {
            return PoolCollector.GetInstance().TexturePool;
        }
    }
}
