using System;
using OpenTK.Graphics.OpenGL;

namespace TextureLoader
{
    public class TextureParameters
    {
        public TextureTarget TexTarget { set; get; }
        public TextureMagFilter TexMagFilter { set; get; }
        public TextureMinFilter TexMinFilter { set; get; }
        public Int32 TexMipLvl { set; get; }
        public PixelInternalFormat TexPixelInternalFormat { set; get; }
        public Int32 TexBufferWidth { set; get; }
        public Int32 TexBufferHeight { set; get; }
        public PixelFormat TexPixelFormat { set; get; }
        public PixelType TexPixelType { set; get; }
        public TextureWrapMode TexWrapMode { set; get; }

        public TextureParameters(TextureTarget texTarget, TextureMagFilter magFilter, TextureMinFilter minFilter,
            Int32 texMipLvl, PixelInternalFormat texPixelInternalFormat,
            Int32 texBufferWidth, Int32 texBufferHeight,
            PixelFormat texPixelFormat, PixelType texPixelType, TextureWrapMode texWrapMode)
        {
            TexTarget = texTarget;
            TexMipLvl = texMipLvl;
            TexPixelInternalFormat = texPixelInternalFormat;
            TexBufferWidth = texBufferWidth;
            TexBufferHeight = texBufferHeight;
            TexPixelFormat = texPixelFormat;
            TexPixelType = texPixelType;
            TexMagFilter = magFilter;
            TexMinFilter = minFilter;
            TexWrapMode = texWrapMode;
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
                left.TexMagFilter == right.TexMagFilter &&
                left.TexMinFilter == right.TexMinFilter &&
                left.TexWrapMode == right.TexWrapMode;
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
