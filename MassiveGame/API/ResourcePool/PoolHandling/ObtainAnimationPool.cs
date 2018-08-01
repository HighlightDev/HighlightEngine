using System;
using MassiveGame.API.ResourcePool.Pools;

namespace MassiveGame.API.ResourcePool.PoolHandling
{
    public class ObtainAnimationPool : IPoolObtainable<Pool>
    {
        Pool IPoolObtainable<Pool>.GetPool()
        {
            return PoolCollector.GetInstance().AnimationPool;
        }
    }
}
