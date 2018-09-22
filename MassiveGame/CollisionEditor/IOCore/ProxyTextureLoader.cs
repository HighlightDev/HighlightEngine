using TextureLoader;

namespace CollisionEditor.IOCore
{
    public static class ProxyTextureLoader
    {
        public static ITexture LoadSingleTexture(string texturePath)
        {
            return new Texture2D(texturePath, false);
        }

        internal static ITexture LoadCubemap(string[] textures)
        {
            return new CubemapTexture(textures);
        }
    }
}
