using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace FramebufferAPI
{
    public class RenderTargetParams
    {
        public TextureTarget TexTarget { set; get; }
        public Int32 TexMipLvl { set; get; }
        public PixelInternalFormat TexPixelInternalFormat { set; get; }
        public Int32 TexBufferWidth { set; get; }
        public Int32 TexBufferHeight { set; get; }
        public PixelFormat TexPixelFormat { set; get; }
        public PixelType TexPixelType { set; get; }

        public RenderTargetParams(TextureTarget texTarget, Int32 texMipLvl, PixelInternalFormat texPixelInternalFormat, Int32 texBufferWidth, Int32 texBufferHeight, PixelFormat texPixelFormat,
            PixelType texPixelType)
        {
            TexTarget = texTarget;
            TexMipLvl = texMipLvl;
            TexPixelInternalFormat = texPixelInternalFormat;
            TexBufferWidth = texBufferWidth;
            TexBufferHeight = texBufferHeight;
            TexPixelFormat = texPixelFormat;
            TexPixelType = texPixelType;
        }

        public static bool operator ==(RenderTargetParams left, RenderTargetParams right)
        {
            bool bEqual =
                left.TexTarget == right.TexTarget &&
                left.TexTarget == right.TexTarget &&
                left.TexMipLvl == right.TexMipLvl &&
                left.TexPixelInternalFormat == right.TexPixelInternalFormat &&
                left.TexBufferWidth == right.TexBufferWidth &&
                left.TexBufferHeight == right.TexBufferHeight &&
                left.TexPixelFormat == right.TexPixelFormat &&
                left.TexPixelType == right.TexPixelType;
            return bEqual;
        }

        public static bool operator !=(RenderTargetParams left, RenderTargetParams right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            return (RenderTargetParams)obj == this;
        }

        public override Int32 GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
