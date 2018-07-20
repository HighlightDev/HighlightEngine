using System;
using System.Linq;
using VBO;

namespace MassiveGame.API.ResourcePool.Pools
{
    public class ModelPool : Pool
    {
        public ModelPool() { }

        public void AddModelToRoot(VertexArrayObject meshVAO, string key)
        {
            bool bModelIsInRoot = resourceMap.Any((keyValue) => { return (string)keyValue.Key == key; });
            if (bModelIsInRoot)
            {
                referenceMap[key]++;
            }
            else
            {
                resourceMap.Add(key, meshVAO);
                referenceMap.Add(key, 1);
            }
        }

        public Int32 GetModelReferenceCount(string key)
        {
            object value;
            if (resourceMap.TryGetValue(key, out value))
                return referenceMap[key];

            return 0;
        }
    }
}
