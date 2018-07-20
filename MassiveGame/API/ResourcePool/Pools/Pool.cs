using MassiveGame.API.ResourcePool.Policies;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MassiveGame.API.ResourcePool.Pools
{
    public abstract class Pool
    {
        protected Dictionary<object, object> resourceMap;
        protected Dictionary<object, Int32> referenceMap;

        public Pool()
        {
            resourceMap = new Dictionary<object, object>();
            referenceMap = new Dictionary<object, int>();
        }

        public Int32 GetResourceCount()
        {
            return resourceMap.Count;
        }

        public Int32 GetReferenceCount(object arg)
        {
            Int32 count;
            referenceMap.TryGetValue(arg, out count);
            return count;
        }

        private bool AlreadyIsInPool<ArgType, ReturnType>(ArgType key, ref ReturnType resource)
        {
            object result;
            bool exist = resourceMap.TryGetValue(key, out result);
            resource = (ReturnType)result;
            return exist;
        }

        private void IncreaseRefCounter<ArgType>(ArgType key, bool exist)
        {
            if (exist)
            {
                referenceMap[key]++;
            }
            else
            {
                referenceMap.Add(key, 1);
            }
        }

        private void FreeResource<Policy, ArgType, ReturnType>(ArgType key)
            where Policy : AllocationPolicy<ArgType, ReturnType>, new()
        {
            referenceMap[key]--;
            if (referenceMap[key] == 0)
            {
                new Policy().FreeMemory((ReturnType)resourceMap[key]);
                resourceMap.Remove(key);
                referenceMap.Remove(key);
            }
        }

        private bool TryToFreeMemory<Policy, ArgType, ReturnType>(ArgType key)
            where Policy : AllocationPolicy<ArgType, ReturnType>, new()
        {
            bool bMemoryFreed = false;
            object resource;
            bool bExist = resourceMap.TryGetValue(key, out resource);

            if (bExist)
            {
                bMemoryFreed = true;
                FreeResource<Policy, ArgType, ReturnType>((ArgType)key);
            }

            return bMemoryFreed;
        }

        private bool TryToFreeMemory<Policy, ArgType, ReturnType>(ReturnType value)
            where Policy : AllocationPolicy<ArgType, ReturnType>, new()
        {
            object key = null;
            bool bMemoryFreed = false;
            bool bExist = resourceMap.Any(item =>
            {
                if (item.Value == (object)value)
                {
                    key = item.Key;
                    return true;
                }
                else
                {
                    return false;
                }
            });

            if (bExist)
            {
                bMemoryFreed = true;
                FreeResource<Policy, ArgType, ReturnType>((ArgType)key);
            }

            return bMemoryFreed;
        }

        public ReturnType GetResourceFromPool<Policy, ArgType, ReturnType>(ArgType arg)
          where Policy : AllocationPolicy<ArgType, ReturnType>, new()
        {
            ReturnType resource = default(ReturnType);
            bool bHasInPool = AlreadyIsInPool(arg, ref resource);
            if (!bHasInPool)
            {
                resource = new Policy().AllocateMemory(arg);
                resourceMap.Add(arg, resource);
            }
            IncreaseRefCounter(arg, bHasInPool);
            return resource;
        }

        public void CleanUpByKey<Policy, ArgType, ReturnType>(ArgType arg)
            where Policy : AllocationPolicy<ArgType, ReturnType>, new()
        {
            TryToFreeMemory<Policy, ArgType, ReturnType>(arg);
        }

        public void CleanUpByValue<Policy, ArgType, ReturnType>(ReturnType arg)
            where Policy : AllocationPolicy<ArgType, ReturnType>, new()
        {
            TryToFreeMemory<Policy, ArgType, ReturnType>(arg);
        }
    }
}
