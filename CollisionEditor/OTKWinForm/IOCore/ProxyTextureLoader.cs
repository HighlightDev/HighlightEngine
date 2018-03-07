using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextureLoader;

namespace OTKWinForm.IOCore
{
    public static class ProxyTextureLoader
    {
        public static ITexture LoadSingleTexture(string texturePath)
        {
            return new Texture2Dlite(texturePath, false);
        }

        internal static ITexture LoadCubemap(string[] textures)
        {
            return new CubemapTexture(textures);
        }
    }
}
