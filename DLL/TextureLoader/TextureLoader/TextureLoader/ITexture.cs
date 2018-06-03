using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextureLoader
{
    public interface ITexture
    {
        void BindTexture(TextureUnit samplerID);
        void UnbindTexture(TextureUnit samplerID);
        void CleanUp();
        UInt32 GetTextureHandler();
        TextureTarget GetTextureTarget();
        Point GetTextureRezolution();
    }
}
