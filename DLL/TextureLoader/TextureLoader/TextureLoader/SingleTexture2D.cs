using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using SystemPixelFormat = System.Drawing.Imaging.PixelFormat;
using SystemBitmapData = System.Drawing.Imaging.BitmapData;

namespace TextureLoader
{
    public class SingleTexture2D
    {
        #region Constructors

        public SingleTexture2D(string texturePath, bool copyBitmap, bool generateMipMap = true, float anisotropyLevel = -1.0f)
        {
            _copyBitmap = copyBitmap;
            _textureID = CreateTexture(texturePath, generateMipMap, anisotropyLevel);
        }

        #endregion

        #region Definitions

        private float _maxAnisotropy;
        private int _textureID;
        private TextureRezolution _rezolution;
        private Bitmap _image;
        private bool _copyBitmap;

        public float MaxAnisotropy { get { return _maxAnisotropy; } }
        public int TextureID { get { return _textureID; } }
        public TextureRezolution Rezolution { private set { this._rezolution = value; } get { return this._rezolution; } }
        public Bitmap Image { get { return this._image; } }

        #endregion

        #region Externals

        private bool CheckForAnisotropicTextureFiltering()
        {
            var extensions = new HashSet<string>(GL.GetString(StringName.Extensions).Split(new char[] { ' ' }));
            if (extensions.Contains("GL_EXT_texture_filter_anisotropic"))
            {
                GL.GetFloat((GetPName)ExtTextureFilterAnisotropic.MaxTextureMaxAnisotropyExt, out _maxAnisotropy);
                extensions = null;
                return true;
            }
            extensions = null;
            return false;
        }

        #endregion

        #region UpdateImage

        public void updateTexture(Bitmap NewImage, TextureWrapMode texWrap = TextureWrapMode.Repeat,
            MipmapTextureFilter filterType = MipmapTextureFilter.LinearMipmapLinear)
        {
            // Update texture only if _copyBitmap enabled
            if (_copyBitmap == true)
            {
                this.cleanUp();
                CreateTexture(NewImage, texWrap, filterType);
            }
        }

        #endregion

        #region Source_parsing

        private int CreateTexture(Bitmap image, TextureWrapMode texWrap = TextureWrapMode.Repeat,
            MipmapTextureFilter filterType = MipmapTextureFilter.LinearMipmapLinear)
        {
            int glTextureID = 0;
            var success = false;

            // Save image to local variable
            _image = image;

            // если загрузка прошла успешно 
            // сохраняем размеры изображения 
            int width = image.Width;
            int height = image.Height;

            _rezolution = new TextureRezolution(width, height);

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
                    glTextureID = GenerateTexture(24, textureData.Scan0, width, height, false, -1f, texWrap, filterType);
                    // активируем флаг, сигнализирующий загрузку текстуры 
                    success = true;
                    break;
                case 32:
                    glTextureID = GenerateTexture(32, textureData.Scan0, width, height, false, -1f, texWrap, filterType);
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
            // Clean image if it's useless
            if (!_copyBitmap)
            {
                // очищаем память
                image.Dispose();
            }

            return glTextureID;
        }

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

            // Save bitmap 
            if (_copyBitmap == true)
            {
                // Save image to local variable
                _image = image;
            }

            // если загрузка прошла успешно 
            // сохраняем размеры изображения 
            int width = image.Width;
            int height = image.Height;

            _rezolution = new TextureRezolution(width, height);

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
            // Clean image if it's useless
            if (!_copyBitmap)
            {
                // очищаем память
                image.Dispose();
            }

            return glTextureID;
        }

        #endregion

        #region Gen of empty imagies

        private int genEmptyImage(int widthRes, int heightRes, int filtration,
            PixelInternalFormat colorComponentCount, PixelFormat pixelData, PixelType type,
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

            unbindTexture2D(TextureUnit.Texture0);

            return imageId;
        }

        public void genEmptyImg(int widthRes, int heightRes, int filtration,
            PixelInternalFormat colorComponentCount, PixelFormat pixelData, PixelType type,
            TextureWrapMode texWrap = TextureWrapMode.Repeat)
        {
            _textureID = genEmptyImage(widthRes, heightRes, filtration, colorComponentCount, pixelData, type, texWrap);
            _rezolution = new TextureRezolution(widthRes, heightRes);

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
                        (anisotropyLevel == -1.0f) ? _maxAnisotropy : (anisotropyLevel >= _maxAnisotropy) ? _maxAnisotropy : (anisotropyLevel < 0.0f) ? 0.0f : anisotropyLevel);
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
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, width, height, 0, PixelFormat.Bgr,
                        PixelType.UnsignedByte, pixels);
                    break;

                case 32:
                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Bgra,
                         PixelType.UnsignedByte, pixels);
                    break;
            }

            // возвращаем идентификатор текстурного объекта 
            return texObject;
        }

        #endregion

        #region Binding

        public void bindTexture2D(TextureUnit samplerID, int texID)
        {
            GL.ActiveTexture(samplerID);
            GL.BindTexture(TextureTarget.Texture2D, texID);
        }

        public void unbindTexture2D(TextureUnit samplerID)
        {
            GL.ActiveTexture(samplerID);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        #endregion

        #region Clean_up

        public void cleanUp()
        {
            GL.DeleteTexture(_textureID);
        }

        #endregion
    }
}
