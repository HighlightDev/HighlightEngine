using System;
using System.Linq;
using TextureLoader;

namespace MassiveGame.API.ResourcePool.Policies
{
    public class TextureAllocationPolicy : AllocationPolicy<string, ITexture>
    {
        public override ITexture AllocateMemory(string arg)
        {
            var pathToTexture = arg.Split(',');
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

        public override void FreeMemory(ITexture arg)
        {
            arg.CleanUp();
        }

        private ITexture LoadTexture2dFromFile(string pathToFile)
        {
            return new Texture2D(pathToFile, EngineStatics.globalSettings.bSupported_MipMap, EngineStatics.globalSettings.AnisotropicFilterValue);
        }

        private ITexture LoadTextureCubeFromFile(string[] pathToFiles)
        {
            return new CubemapTexture(pathToFiles);
        }
    }
}
