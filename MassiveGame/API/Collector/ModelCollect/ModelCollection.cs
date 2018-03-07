using GpuGraphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassiveGame.API.Collector.ModelCollect
{
    public class ModelCollection
    {
        private Dictionary<string, VAO> modelDictionary;
        private Dictionary<string, int> referenceCount;

        public ModelCollection()
        {
            modelDictionary = new Dictionary<string, VAO>();
            referenceCount = new Dictionary<string, int>();
        }

        public VAO RetrieveModel(string key)
        {
            return TryGetModel(key);
        }

        private VAO TryGetModel(string key)
        {
            VAO result = null;
            bool exist = modelDictionary.TryGetValue(key, out result);
            if (!exist)
            {
                result = VaoAllocator.LoadModelFromFile(key);
                modelDictionary.Add(key, result);
            }
            IncreaseRefCounter(key, exist);
            return result;
        }

        private void IncreaseRefCounter(string key, bool exist)
        {
            if (exist)
            {
                referenceCount[key]++;
            }
            else
            {
                referenceCount.Add(key, 1);
            }
        }

        public void ReleaseModel(string key)
        {
            bool exist = false;
            exist = modelDictionary.Any( value => value.Key == key);
            if (exist)
            {
                DecrementReference(key);
            }
        }

        private void DecrementReference(string key)
        {
            referenceCount[key]--;
            if (referenceCount[key] == 0)
            {
                VaoAllocator.ReleaseVAO(modelDictionary[key]);
                modelDictionary.Remove(key);
                referenceCount.Remove(key);
            }
        }
    }
}
