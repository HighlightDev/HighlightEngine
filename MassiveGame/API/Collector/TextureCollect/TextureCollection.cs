using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using global::TextureLoader;

namespace MassiveGame.API.Collector.TextureCollect
{
    public class TextureCollection
    {
        private Dictionary<string, ITexture> textureDictionary;
        private Dictionary<string, int> referenceCount;

        public TextureCollection()
        {
            textureDictionary = new Dictionary<string, ITexture>();
            referenceCount = new Dictionary<string, int>();
        }

        public ITexture RetrieveTexture(string key)
        {
            return TryGetTexture(key);
        }

        private ITexture TryGetTexture(string compositeKey)
        {
            ITexture result = null;
            bool exist = textureDictionary.TryGetValue(compositeKey, out result);
            if (!exist)
            {
                result = TextureAllocator.LoadTextureFromFile(compositeKey);
                textureDictionary.Add(compositeKey, result);
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

        public void ReleaseTexture(string key)
        {
            bool exist = false;
            exist = textureDictionary.Any( value => value.Key == key);
            if (exist)
            {
                DecrementReference(key);
            }
        }

        public void ReleaseTexture(ITexture texture)
        {
            bool exist = false;
            string key = null;
            exist = textureDictionary.Any(value =>
            {
                if (value.Value == texture)
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
                TextureAllocator.ReleaseTexture(textureDictionary[key]);
                textureDictionary.Remove(key);
                referenceCount.Remove(key);
            }
        }
    }
}
