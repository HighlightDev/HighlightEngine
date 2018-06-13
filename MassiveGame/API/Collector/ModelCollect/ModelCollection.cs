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
        private Dictionary<string, Int32> referenceCount;

        public ModelCollection()
        {
            modelDictionary = new Dictionary<string, VAO>();
            referenceCount = new Dictionary<string, Int32>();
        }

        public void AddModelToRoot(VAO modelBuffer, string key)
        {
            bool bModelIsInRoot = modelDictionary.Any((keyValue) => { return keyValue.Key == key; });
            if (bModelIsInRoot)
                referenceCount[key]++;
            else
            {
                modelDictionary.Add(key, modelBuffer);
                referenceCount.Add(key, 1);
            }
        }

        public Int32 GetModelReferenceCount(string key)
        {
            bool bModelIsInRoot = modelDictionary.Any((keyValue) => { return keyValue.Key == key; });
            Int32 references = 0;
            if (bModelIsInRoot)
                references = referenceCount[key];

            return references;
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
