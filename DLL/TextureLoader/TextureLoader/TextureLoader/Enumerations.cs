using OpenTK.Graphics.OpenGL;


namespace TextureLoader
{
    public enum MipmapTextureFilter : int
    {
        LinearMipmapLinear = TextureMinFilter.LinearMipmapLinear,
        LinearMipmapNearest = TextureMinFilter.LinearMipmapNearest,
        NearestMipmapLinear = TextureMinFilter.NearestMipmapLinear,
        NearestMipmapNearest = TextureMinFilter.NearestMipmapNearest
    }
}
