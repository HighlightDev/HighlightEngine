using System;
using MassiveGame.API.ResourcePool.Pools;

namespace MassiveGame.API.ResourcePool.PoolHandling
{
    public class ObtainAnimationPool : IPoolObtainable<Pool>
    {
        public Pool GetPool() { return PoolCollector.GetInstance().s_AnimationPool; }
    }
}
