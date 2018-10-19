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

        protected abstract bool IsValidResourceType(object arg);

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

        private bool CheckIsInPoolByKey<ArgType, ReturnType>(ArgType key, ref ReturnType resource)
        {
            object result;
            bool exist = resourceMap.TryGetValue(key, out result);
            resource = (ReturnType)result;
            return exist;
        }

        private bool CheckIsInPoolByValue<ArgType, ReturnType>(ReturnType value, ref ArgType keyOut)
        {
            object key = null;
            bool bExist = resourceMap.Any(item =>
            {
                if (item.Value == (object)value)
                {
                    key = (ArgType)item.Key;
                    return true;
                }
                else
                {
                    return false;
                }
            });
            keyOut = (ArgType)key;
            return bExist;
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

        public ArgType GetResourceKeyFromPool<ArgType, ReturnType>(ReturnType arg) 
        {
            ArgType key = default(ArgType);
            bool bHasInPool = CheckIsInPoolByValue(arg, ref key);
            if (!bHasInPool)
                throw new ArgumentException("Wrong argument, it doesn't exist in pool.");

            return key;
        }

        public bool TryToFreeMemory<Policy, ArgType, ReturnType>(ArgType key)
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

        public bool TryToFreeMemory<Policy, ArgType, ReturnType>(ReturnType value)
            where Policy : AllocationPolicy<ArgType, ReturnType>, new()
        {
            ArgType key = default(ArgType);
            bool bMemoryFreed = false;
            bool bExist = CheckIsInPoolByValue(value, ref key);

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
            bool bHasInPool = CheckIsInPoolByKey(arg, ref resource);
            if (!bHasInPool)
            {
                resource = new Policy().AllocateMemory(arg);

                if (resource != null)
                {
                    if (!IsValidResourceType(resource))
                    {
                        TryToFreeMemory<Policy, ArgType, ReturnType>(resource);
                        throw new Exception("WRONG ALLOCATED TYPE FOR CURRENT POOL");
                    }
                    resourceMap.Add(arg, resource);
                }
            }

            if (resource != null)
                IncreaseRefCounter(arg, bHasInPool);

            return resource;
        }
    }
}
