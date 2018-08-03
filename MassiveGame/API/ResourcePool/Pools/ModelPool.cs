using MassiveGame.API.Mesh;
using System;
using System.Linq;

namespace MassiveGame.API.ResourcePool.Pools
{
    public class ModelPool : Pool
    {
        public ModelPool() { }

        public void AddModelToRoot(Skin skin, string key)
        {
            bool bModelIsInRoot = resourceMap.Any((keyValue) => { return (string)keyValue.Key == key; });
            if (bModelIsInRoot)
            {
                referenceMap[key]++;
            }
            else
            {
                resourceMap.Add(key, skin);
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

        protected override bool IsValidResourceType(object arg)
        {
            return arg is Skin;
        }
    }
}
