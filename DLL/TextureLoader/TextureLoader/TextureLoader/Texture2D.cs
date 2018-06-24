using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

using SystemPixelFormat = System.Drawing.Imaging.PixelFormat;
using SystemBitmapData = System.Drawing.Imaging.BitmapData;

namespace TextureLoader
{
    public class Texture2D : ITexture
    {
        public Int32 TextureID { private set; get; }
        public TextureLoader.TextureParameters TextureParameters { set; get; }

        private float MaxAnisotropy { set; get; }

        public Texture2D(string texturePath, bool generateMipMap, float anisotropyLevel = -1.0f)
        {
            TextureID = -1;
            TextureParameters = new TextureParameters();
            TextureID = CreateTexture(texturePath, generateMipMap, anisotropyLevel);
        }

        public Texture2D(Int32 TextureID, Point rectParams)
        {
            this.TextureID = TextureID;
            TextureParameters = new TextureParameters();
            TextureParameters.TexBufferWidth = rectParams.X;
            TextureParameters.TexBufferHeight = rectParams.Y;
        }

        public Texture2D(TextureLoader.TextureParameters textureParameters)
        {
            TextureParameters = textureParameters;

            TextureID = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, TextureID);

            GL.TexImage2D(TextureTarget.Texture2D, 0, textureParameters.TexPixelInternalFormat, textureParameters.TexBufferWidth, textureParameters.TexBufferHeight, 0 , textureParameters.TexPixelFormat, textureParameters.TexPixelType, new IntPtr(0));

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (Int32)textureParameters.TexMagFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (Int32)textureParameters.TexMinFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (Int32)textureParameters.TexWrapMode);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (Int32)textureParameters.TexWrapMode);
        }

        #region Externals

        private bool CheckForAnisotropicTextureFiltering()
        {
            var extensions = new HashSet<string>(GL.GetString(StringName.Extensions).Split(new char[] { ' ' }));
            if (extensions.Contains("GL_EXT_texture_filter_anisotropic"))
            {
                float anisotropy;
                GL.GetFloat((GetPName)ExtTextureFilterAnisotropic.MaxTextureMaxAnisotropyExt, out anisotropy);
                MaxAnisotropy = anisotropy;
                extensions = null;
                return true;
            }
            extensions = null;
            return false;
        }

        #endregion

        #region Source_parsing

        private Int32 CreateTexture(string imagePath, bool generateMipMap, float anisotropyLevel,
            TextureWrapMode texWrap = TextureWrapMode.Repeat,
            MipmapTextureFilter filterType = MipmapTextureFilter.LinearMipmapLinear)
        {

            Int32 glTextureID = 0;

            Bitmap image = null;
            var success = false;

            try
            {
                image = new Bitmap(imagePath);
            }
            catch (ArgumentException)
            {
                // Error - file path is invalid
                return -1;
            }

            Int32 width = image.Width;
            Int32 height = image.Height;

            TextureParameters.TexBufferWidth = width;
            TextureParameters.TexBufferHeight = height;

            Int32 pixelFormat = 0;
            Int32 bitsPerPixel = 0;
            SystemPixelFormat sPixelFormat = image.PixelFormat;

            switch (sPixelFormat)
            {
                case SystemPixelFormat.Format24bppRgb:
                    pixelFormat = (Int32)SystemPixelFormat.Format24bppRgb;
                    bitsPerPixel = 24;
                    break;
                case SystemPixelFormat.Format32bppRgb:
                    pixelFormat = (Int32)SystemPixelFormat.Format32bppRgb;
                    bitsPerPixel = 32;
                    break;
                case SystemPixelFormat.Format32bppArgb:
                    pixelFormat = (Int32)SystemPixelFormat.Format32bppArgb;
                    bitsPerPixel = 32;
                    break;
                case SystemPixelFormat.Format32bppPArgb:
                    pixelFormat = (Int32)SystemPixelFormat.Format32bppPArgb;
                    bitsPerPixel = 32;
                    break;
                default:
                    pixelFormat = 0;
                    bitsPerPixel = 0;
                    break;
            }

            //get the data out of the bitmap
            SystemBitmapData textureData = image.LockBits(
                new Rectangle(0, 0, width, height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly,
                (SystemPixelFormat)pixelFormat
            );

            switch (bitsPerPixel) 
            {
                case 24:
                    glTextureID = GenerateTexture(24, textureData.Scan0, width, height, generateMipMap, anisotropyLevel, texWrap, filterType);
                    success = true;
                    break;
                case 32:
                    glTextureID = GenerateTexture(32, textureData.Scan0, width, height, generateMipMap, anisotropyLevel, texWrap, filterType);
                    success = true;
                    break;
                default:
                    success = false;
                    break;
            }

            try
            {
                if (success == false)
                {
                    throw new BadImageFormatException("wrong image path");
                }
            }
            catch (BadImageFormatException ex)
            {
                throw new EntryPointNotFoundException(ex.Message, ex);
            }

            image.UnlockBits(textureData);
            image.Dispose();
            return glTextureID;
        }

        #endregion

        #region Gpu_texture_creation

        private Int32 GenerateTexture(Int32 format, IntPtr pixels, Int32 width, Int32 height,
            bool generateMipMap, float anisotropyLevel, TextureWrapMode texWrap, MipmapTextureFilter filterType)
        {
            Int32 texObject;

            GL.GenTextures(1, out texObject);

            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

            GL.BindTexture(TextureTarget.Texture2D, texObject);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (Int32)texWrap);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (Int32)texWrap);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (Int32)TextureMagFilter.Linear);
            if (!generateMipMap)
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (Int32)TextureMinFilter.Linear);
            else
            {
                if (CheckForAnisotropicTextureFiltering())
                {
                    GL.TexParameter(TextureTarget.Texture2D, (TextureParameterName)ExtTextureFilterAnisotropic.TextureMaxAnisotropyExt,
                        (anisotropyLevel == -1.0f) ? MaxAnisotropy : (anisotropyLevel >= MaxAnisotropy) ? MaxAnisotropy : (anisotropyLevel < 0.0f) ? 0.0f : anisotropyLevel);
                }
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (Int32)filterType);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.GenerateMipmap, 1); // 1 stands for TRUE statement
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureLodBias, -0.4f); // might need to use variable to change this value
            }
            GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (Int32)TextureEnvMode.Replace);

            switch (format)
            {
                case 24:
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, width, height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgr,
                        PixelType.UnsignedByte, pixels);
                    break;

                case 32:
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                         PixelType.UnsignedByte, pixels);
                    break;
            }

            // возвращаем идентификатор текстурного объекта 
            return texObject;
        }

        #endregion

        #region Binding

        public void BindTexture(TextureUnit samplerID)
        {
            GL.ActiveTexture(samplerID);
            GL.BindTexture(TextureTarget.Texture2D, this.TextureID);
        }

        public static void UnbindTexture2D(TextureUnit samplerID)
        {
            GL.ActiveTexture(samplerID);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void UnbindTexture(TextureUnit samplerID)
        {
            GL.ActiveTexture(samplerID);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        #endregion

        public Point GetTextureRezolution()
        {
            return new Point(TextureParameters.TexBufferWidth, TextureParameters.TexBufferHeight);
        }

        public uint GetTextureDescriptor()
        {
            return (UInt32)TextureID;
        }

        public TextureParameters GetTextureParameters()
        {
            return TextureParameters;
        }

        #region Clean_up

        public void CleanUp()
        {
            GL.DeleteTexture(TextureID);
        }

        #endregion

    }
}
