using OpenTK.Graphics.OpenGL;
using System;

namespace TextureLoader
{
    public enum MipmapTextureFilter : Int32
    {
        LinearMipmapLinear = TextureMinFilter.LinearMipmapLinear,
        LinearMipmapNearest = TextureMinFilter.LinearMipmapNearest,
        NearestMipmapLinear = TextureMinFilter.NearestMipmapLinear,
        NearestMipmapNearest = TextureMinFilter.NearestMipmapNearest
    }
}
