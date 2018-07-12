using System;
using System.Linq;
using TextureLoader;

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
            return new Texture2D(pathToFile, EngineStatics.globalSettings.bSupported_MipMap, EngineStatics.globalSettings.AnisotropicFilterValue);
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
