using MassiveGame.API.ResourcePool.Pools;

namespace MassiveGame.API.ResourcePool.PoolHandling
{
    public class ObtainRenderTargetPool : IPoolObtainable<Pool>
    {
        public Pool GetPool() { return PoolCollector.GetInstance().RenderTargetPool; }
    }
}
