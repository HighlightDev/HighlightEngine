using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using OpenTK;
using System.Drawing;

using System.Drawing.Imaging;

using SystemPixelFormat = System.Drawing.Imaging.PixelFormat;
using SystemBitmapData = System.Drawing.Imaging.BitmapData;

namespace TextureLoader
{
    public class Texture2Dlite : ITexture
    {
        public int TextureID { private set; get; }
        public float MaxAnisotropy { private set; get; }
        public RectParams Rectangle { set; get; }

        public Texture2Dlite(string texturePath, bool generateMipMap, float anisotropyLevel = -1.0f)
        {
            TextureID = -1;
            TextureID = CreateTexture(texturePath, generateMipMap, anisotropyLevel);
        }

        public Texture2Dlite(int TextureID, RectParams rectParams)
        {
            this.TextureID = TextureID;
            this.Rectangle = rectParams;
        }

        public Texture2Dlite(Texture2D tex)
        {
            Rectangle = new RectParams(tex.Rezolution[0].widthRezolution, tex.Rezolution[0].heightRezolution);
            TextureID = (Int32)tex.TextureID[0];
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

        private int CreateTexture(string imagePath, bool generateMipMap, float anisotropyLevel,
            TextureWrapMode texWrap = TextureWrapMode.Repeat,
            MipmapTextureFilter filterType = MipmapTextureFilter.LinearMipmapLinear)
        {

            int glTextureID = 0;

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

            // если загрузка прошла успешно 
            // сохраняем размеры изображения 
            int width = image.Width;
            int height = image.Height;

            // определяем число бит на пиксель 
            int pixelFormat = 0;
            int bitsPerPixel = 0;
            SystemPixelFormat sPixelFormat = image.PixelFormat;

            switch (sPixelFormat)
            {
                case SystemPixelFormat.Format24bppRgb:
                    pixelFormat = (int)SystemPixelFormat.Format24bppRgb;
                    bitsPerPixel = 24;
                    break;
                case SystemPixelFormat.Format32bppRgb:
                    pixelFormat = (int)SystemPixelFormat.Format32bppRgb;
                    bitsPerPixel = 32;
                    break;
                case SystemPixelFormat.Format32bppArgb:
                    pixelFormat = (int)SystemPixelFormat.Format32bppArgb;
                    bitsPerPixel = 32;
                    break;
                case SystemPixelFormat.Format32bppPArgb:
                    pixelFormat = (int)SystemPixelFormat.Format32bppPArgb;
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

            switch (bitsPerPixel) // в зависимости от полученного результата 
            {
                // создаем текстуру, используя режим GL_RGB или GL_RGBA 
                case 24:
                    glTextureID = GenerateTexture(24, textureData.Scan0, width, height, generateMipMap, anisotropyLevel, texWrap, filterType);
                    // активируем флаг, сигнализирующий загрузку текстуры 
                    success = true;
                    break;
                case 32:
                    glTextureID = GenerateTexture(32, textureData.Scan0, width, height, generateMipMap, anisotropyLevel, texWrap, filterType);
                    // активируем флаг, сигнализирующий загрузку текстуры 
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
                // Bad image
                return -1;
            }

            image.UnlockBits(textureData);
                // очищаем память
            image.Dispose();
            return glTextureID;
        }

        #endregion

        #region Gen of empty imagies

        public static int genEmptyImage(int widthRes, int heightRes, int filtration,
            PixelInternalFormat colorComponentCount, OpenTK.Graphics.OpenGL.PixelFormat pixelData, PixelType type,
            TextureWrapMode texWrap)
        {
            int imageId;
            imageId = GL.GenTexture();

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, imageId);

            GL.TexImage2D(TextureTarget.Texture2D, 0, colorComponentCount, widthRes, heightRes, 0, pixelData,
                type, new IntPtr(0));

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, filtration);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, filtration);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)texWrap);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)texWrap);

            UnbindTexture2D(TextureUnit.Texture0);

            return imageId;
        }

        #endregion

        #region Gpu_texture_creation

        private int GenerateTexture(int format, IntPtr pixels, int width, int height,
            bool generateMipMap, float anisotropyLevel, TextureWrapMode texWrap, MipmapTextureFilter filterType)
        {
            // идентификатор текстурного объекта 
            int texObject;

            // генерируем текстурный объект 
            GL.GenTextures(1, out texObject);

            // устанавливаем режим упаковки пикселей 
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

            // создаем привязку к только что созданной текстуре 
            GL.BindTexture(TextureTarget.Texture2D, texObject);

            // устанавливаем режим фильтрации и повторения текстуры 
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)texWrap);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)texWrap);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            if (!generateMipMap)
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            else
            {
                if (CheckForAnisotropicTextureFiltering())
                {
                    GL.TexParameter(TextureTarget.Texture2D, (TextureParameterName)ExtTextureFilterAnisotropic.TextureMaxAnisotropyExt,
                        (anisotropyLevel == -1.0f) ? MaxAnisotropy : (anisotropyLevel >= MaxAnisotropy) ? MaxAnisotropy : (anisotropyLevel < 0.0f) ? 0.0f : anisotropyLevel);
                }
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)filterType);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.GenerateMipmap, 1); // 1 stands for TRUE statement
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureLodBias, -0.4f); // might need to use variable to change this value
            }
            GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (int)TextureEnvMode.Replace);

            // создаем RGB или RGBA текстуру 
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

        public uint GetTextureHandler()
        {
            return (UInt32)TextureID;
        }

        public TextureTarget GetTextureTarget()
        {
            return TextureTarget.Texture2D;
        }

        #region Clean_up

        public void CleanUp()
        {
            GL.DeleteTexture(TextureID);
        }

        #endregion

    }
}
