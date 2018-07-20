using MassiveGame.API.ResourcePool.Pools;

namespace MassiveGame.API.ResourcePool.PoolHandling
{
    public class ObtainShaderPool : IPoolObtainable<Pool>
    {
        public Pool GetPool() { return PoolCollector.GetInstance().TexturePool; }
    }
}
