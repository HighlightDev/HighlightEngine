using ShaderPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MassiveGame.API.Collector.ShaderCollect
{
    public class ShaderCollection
    {
        private Dictionary<string, Shader> shaderDictionary;
        private Dictionary<string, Int32> referenceCount;

        public ShaderCollection()
        {
            shaderDictionary = new Dictionary<string, Shader>();
            referenceCount = new Dictionary<string, Int32>();
        }

        public Shader RetrieveShader(string key, ConstructorInfo ctor)
        {
            return TryGetShader(key, ctor);
        }

        private Shader TryGetShader(string compositeKey, ConstructorInfo ctor)
        {
            Shader result = null;
            bool exist = shaderDictionary.TryGetValue(compositeKey, out result);
            if (!exist)
            {
                result = ShaderAllocator.LoadShaderFromFile(compositeKey, ctor);
                shaderDictionary.Add(compositeKey, result);
            }
            IncreaseRefCounter(compositeKey, exist);
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

        public void ReleaseShader(string key)
        {
            bool exist = false;
            exist = shaderDictionary.Any(value => value.Key == key);
            if (exist)
            {
                DecrementReference(key);
            }
        }

        public void ReleaseShader(Shader shader)
        {

            bool exist = false;
            string key = null;
            exist = shaderDictionary.Any(value =>
            {
                if (value.Value == shader)
                {
                    key = value.Key;
                    return true;
                }
                else
                {
                    return false;
                }
            });

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
                ShaderAllocator.ReleaseShader(shaderDictionary[key]);
                shaderDictionary.Remove(key);
                referenceCount.Remove(key);
            }
        }
    }
}
