using System;
using System.Collections.Generic;
using System.Linq;
using VBO;

namespace MassiveGame.API.Collector.AnimationCollect
{
    public class AnimationCollection
    {
        private Dictionary<string, object> animationDictionary;
        private Dictionary<string, Int32> referenceCount;

        public AnimationCollection()
        {
            animationDictionary = new Dictionary<string, object>();
            referenceCount = new Dictionary<string, Int32>();
        }

        public void AddModelToRoot(VertexArrayObject modelBuffer, string key)
        {
            bool bModelIsInRoot = animationDictionary.Any((keyValue) => { return keyValue.Key == key; });
            if (bModelIsInRoot)
                referenceCount[key]++;
            else
            {
                animationDictionary.Add(key, modelBuffer);
                referenceCount.Add(key, 1);
            }
        }

        public Int32 GetAnimationReferenceCount(string key)
        {
            bool bModelIsInRoot = animationDictionary.Any((keyValue) => { return keyValue.Key == key; });
            Int32 references = 0;
            if (bModelIsInRoot)
                references = referenceCount[key];

            return references;
        }

        public object RetrieveAnimation(string key)
        {
            return TryGetAnimation(key);
        }

        public object TryGetAnimation(string key)
        {
            object result = null;
            bool exist = animationDictionary.TryGetValue(key, out result);
            if (!exist)
            {
                // load
                animationDictionary.Add(key, result);
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

        public void ReleaseAnimation(string key)
        {
            bool exist = false;
            exist = animationDictionary.Any( value => value.Key == key);
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
                // release
                animationDictionary.Remove(key);
                referenceCount.Remove(key);
            }
        }
    }
}
