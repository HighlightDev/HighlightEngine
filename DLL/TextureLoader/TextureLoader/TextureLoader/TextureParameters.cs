using System;
using OpenTK.Graphics.OpenGL;

namespace TextureLoader
{
    public class TextureParameters
    {
        public TextureTarget TexTarget { set; get; }
        public TextureMagFilter MagFilter { set; get; }
        public TextureMinFilter MinFilter { set; get; }
        public Int32 TexMipLvl { set; get; }
        public PixelInternalFormat TexPixelInternalFormat { set; get; }
        public Int32 TexBufferWidth { set; get; }
        public Int32 TexBufferHeight { set; get; }
        public PixelFormat TexPixelFormat { set; get; }
        public PixelType TexPixelType { set; get; }

        public TextureParameters(TextureTarget texTarget, TextureMagFilter magFilter, TextureMinFilter minFilter,
            Int32 texMipLvl, PixelInternalFormat texPixelInternalFormat,
            Int32 texBufferWidth, Int32 texBufferHeight,
            PixelFormat texPixelFormat, PixelType texPixelType)
        {
            TexTarget = texTarget;
            TexMipLvl = texMipLvl;
            TexPixelInternalFormat = texPixelInternalFormat;
            TexBufferWidth = texBufferWidth;
            TexBufferHeight = texBufferHeight;
            TexPixelFormat = texPixelFormat;
            TexPixelType = texPixelType;
            MagFilter = magFilter;
            MinFilter = minFilter;
        }

        public TextureParameters()
        {
        }

        public static bool operator ==(TextureParameters left, TextureParameters right)
        {
            bool bEqual =
                left.TexTarget == right.TexTarget &&
                left.TexTarget == right.TexTarget &&
                left.TexMipLvl == right.TexMipLvl &&
                left.TexPixelInternalFormat == right.TexPixelInternalFormat &&
                left.TexBufferWidth == right.TexBufferWidth &&
                left.TexBufferHeight == right.TexBufferHeight &&
                left.TexPixelFormat == right.TexPixelFormat &&
                left.TexPixelType == right.TexPixelType &&
                left.MagFilter == right.MagFilter &&
                left.MinFilter == right.MinFilter;
            return bEqual;
        }

        public static bool operator !=(TextureParameters left, TextureParameters right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            return (TextureParameters)obj == this;
        }

        public override Int32 GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
