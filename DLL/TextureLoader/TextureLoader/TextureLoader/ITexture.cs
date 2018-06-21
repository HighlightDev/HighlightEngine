using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;

namespace TextureLoader
{
    public interface ITexture
    {
        void BindTexture(TextureUnit samplerID);
        void UnbindTexture(TextureUnit samplerID);
        void CleanUp();
        UInt32 GetTextureDescriptor();
        Point GetTextureRezolution();
        TextureParameters GetTextureParameters();
    }
}
