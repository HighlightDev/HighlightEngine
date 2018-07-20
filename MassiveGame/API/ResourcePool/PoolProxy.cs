using MassiveGame.API.ResourcePool.Policies;
using MassiveGame.API.ResourcePool.PoolHandling;
using MassiveGame.API.ResourcePool.Pools;
using System;

namespace MassiveGame.API.ResourcePool
{
    public static class PoolProxy
    {
        public static Int32 GetResourceCountInPool<PoolType>()
              where PoolType : IPoolObtainable<Pool>, new()
        {
            PoolType pool = new PoolType();
            return pool.GetPool().GetResourceCount();
        }

        public static Int32 GetResourceReferencesInPool<PoolType>(object arg)
            where PoolType : IPoolObtainable<Pool>, new()
        {
            PoolType pool = new PoolType();
            return pool.GetPool().GetReferenceCount(arg);
        }

        public static ReturnType GetResource<PoolType, Policy, ArgType, ReturnType>(ArgType arg)
              where PoolType : IPoolObtainable<Pool>, new()
              where Policy : AllocationPolicy<ArgType, ReturnType>, new()

        {
            PoolType pool = new PoolType();
            return pool.GetPool().GetResourceFromPool<Policy, ArgType, ReturnType>(arg);
        }

        public static void FreeResourceMemoryByKey<PoolType, Policy, ArgType, ReturnType>(ArgType arg)
             where PoolType : IPoolObtainable<Pool>, new()
              where Policy : AllocationPolicy<ArgType, ReturnType>, new()
        {
            PoolType pool = new PoolType();
            pool.GetPool().CleanUpByKey<Policy, ArgType, ReturnType>(arg);
        }

        public static void FreeResourceMemoryByValue<PoolType, Policy, ArgType, ReturnType>(ReturnType arg)
             where PoolType : IPoolObtainable<Pool>, new()
              where Policy : AllocationPolicy<ArgType, ReturnType>, new()
        {
            PoolType pool = new PoolType();
            pool.GetPool().CleanUpByValue<Policy, ArgType, ReturnType>(arg);
        }
    }
}
