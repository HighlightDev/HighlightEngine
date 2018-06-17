using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using global::TextureLoader;

namespace MassiveGame.API.Collector.TextureCollect
{
    public class TextureAllocator
    {
        public static ITexture LoadTextureFromFile(string compositeKey)
        {
            var pathToTexture = compositeKey.Split(',');
            ITexture resultTexture = null;
            switch (pathToTexture.Length)
            {
                case 1:
                    resultTexture = LoadTexture2dFromFile(pathToTexture.First());
                    break;
                case 6:
                    resultTexture = LoadTextureCubeFromFile(pathToTexture);
                    break;
                default: throw new ArgumentException("Undefined count of files.");
            }
            return resultTexture;
        }

        private static ITexture LoadTexture2dFromFile(string pathToFile)
        {
            return new Texture2D(pathToFile, false);
        }

        private static ITexture LoadTextureCubeFromFile(string[] pathToFiles)
        {
            return new CubemapTexture(pathToFiles);
        }

        public static void ReleaseTexture(ITexture texture)
        {
            texture.CleanUp();
        }
    }
}
