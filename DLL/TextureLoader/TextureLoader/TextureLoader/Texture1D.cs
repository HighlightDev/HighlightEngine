using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using SystemPixelFormat = System.Drawing.Imaging.PixelFormat;
using SystemBitmapData = System.Drawing.Imaging.BitmapData;


namespace TextureLoader
{
    public class Texture1D
    {
        #region Constructors

        /// <summary>
        /// Загрузчик текстур с использованием технологий MipMapping и Anisotropic Filtering (если поддерживается аппаратно).
        /// </summary>
        /// <param name="imagePath">Путь(и) к текстуре(ам) для загрузки.</param>
        /// <param name="generateMipMap">Флаг использования технологии MipMapping. Если <b>true</b> - технология будет использована.</param>
        /// <param name="anisotropyLevel">Уровень анизотропной фильтрации (если поддерживается аппаратно). Если <b>-1.0f</b> - будет использован максимальный режим анизотропной фильтрации.</param>
        public Texture1D(string[] imagePath, bool generateMipMap = true, float anisotropyLevel = -1.0f)
        {
            TextureID = new List<uint>();

            uint[] tempID = Texture1D.GenImagies(imagePath, generateMipMap, anisotropyLevel);
            foreach (uint id in tempID)
            {
                TextureID.Add(id);
            }
        }

        /// <summary>
        /// Загрузчик текстур с использованием технологий MipMapping и Anisotropic Filtering (если поддерживается аппаратно).
        /// </summary>
        /// <param name="imagePath">Путь(и) к текстуре(ам) для загрузки.</param>
        /// <param name="generateMipMap">Флаг использования технологии MipMapping. Если <b>true</b> - технология будет использована.</param>
        /// <param name="filterType">Режим фильтрации MipMap текстур.</param>
        /// <param name="anisotropyLevel">Уровень анизотропной фильтрации (если поддерживается аппаратно). Если <b>-1.0f</b> - будет использован максимальный режим анизотропной фильтрации.</param>
        public Texture1D(string[] imagePath, bool generateMipMap = true, 
            MipmapTextureFilter filterType = MipmapTextureFilter.LinearMipmapLinear, float anisotropyLevel = -1.0f)
        {
            TextureID = new List<uint>();

            uint[] tempID = Texture1D.GenImagies(imagePath, generateMipMap, anisotropyLevel, filterType);
            foreach (uint id in tempID)
            {
                TextureID.Add(id);
            }
        }

        #endregion


        public List<uint> TextureID { private set; get; }
        private static bool success;
        private static float maxAnisotropy;
        public static float MaxAnisotropy { get { return maxAnisotropy; } }
    
        private static bool CheckForAnisotropicTextureFiltering()
        {
            var extensions = new HashSet<string>(GL.GetString(StringName.Extensions).Split(new char[] { ' ' }));
            if (extensions.Contains("GL_EXT_texture_filter_anisotropic"))
            {
                GL.GetFloat((GetPName)ExtTextureFilterAnisotropic.MaxTextureMaxAnisotropyExt, out maxAnisotropy);
                extensions = null;
                return true;
            }
            extensions = null;
            return false;
        }

        private static uint[] GenImagies(string[] imagePath, bool generateMipMap, float anisotropyLevel,
            MipmapTextureFilter filterType = MipmapTextureFilter.LinearMipmapLinear)
        {
            uint[] glTextureID = new uint[imagePath.Length];
            int iterator = 0;

            foreach (string url in imagePath)
            {
                Bitmap image = null;
                success = false;

                try
                {
                    image = new Bitmap(url);

                    #region Bitmap data
                    // если загрузка прошла успешно 
                    // сохраняем размеры изображения 
                    int width = image.Width;

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
                        new Rectangle(0, 0, width, 1),
                        System.Drawing.Imaging.ImageLockMode.ReadOnly,
                        (SystemPixelFormat)pixelFormat
                    );
                    #endregion

                    switch (bitsPerPixel) // в зависимости от полученного результата 
                    {
                        // создаем текстуру, используя режим GL_RGB или GL_RGBA 
                        case 24:
                            glTextureID[iterator] = MakeGlTexture(24, textureData.Scan0, width, generateMipMap, anisotropyLevel, filterType);
                            // активируем флаг, сигнализирующий загрузку текстуры 
                            success = true;
                            break;
                        case 32:
                            glTextureID[iterator] = MakeGlTexture(32, textureData.Scan0, width, generateMipMap, anisotropyLevel, filterType);
                            // активируем флаг, сигнализирующий загрузку текстуры 
                            success = true;
                            break;
                        default:
                            success = false;
                            break;
                    }
                    
                    if (success == false)
                    {
                        throw new Exception("wrong image path : " + iterator.ToString());
                    }

                    // очищаем память 
                    image.UnlockBits(textureData);
                    image.Dispose();

                    iterator++;
                }
                catch
                {
                    throw new Exception("wrong image path or data : " + iterator.ToString());
                }
            }

            return glTextureID;
        }

        private static uint MakeGlTexture(int Format, IntPtr pixels, int width, bool generateMipMap, float anisotropyLevel, MipmapTextureFilter filterType)
        {
            // идентификатор текстурного объекта 
            uint texObject;

            // генерируем текстурный объект 
            GL.GenTextures(1, out texObject);

            // устанавливаем режим упаковки пикселей 
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

            // создаем привязку к только что созданной текстуре 
            GL.BindTexture(TextureTarget.Texture1D, texObject);

            // устанавливаем режим фильтрации и повторения текстуры 
            GL.TexParameter(TextureTarget.Texture1D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture1D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture1D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            if (!generateMipMap)
                GL.TexParameter(TextureTarget.Texture1D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            else
            {
                if (CheckForAnisotropicTextureFiltering())
                {
                    GL.TexParameter(TextureTarget.Texture1D, (TextureParameterName)ExtTextureFilterAnisotropic.TextureMaxAnisotropyExt,
                        (anisotropyLevel == -1.0f) ? maxAnisotropy : (anisotropyLevel >= maxAnisotropy) ? maxAnisotropy : (anisotropyLevel < 0.0f) ? 0.0f : anisotropyLevel);
                }
                GL.TexParameter(TextureTarget.Texture1D, TextureParameterName.TextureMinFilter, (int)filterType);
                GL.TexParameter(TextureTarget.Texture1D, TextureParameterName.GenerateMipmap, 1);
                GL.TexParameter(TextureTarget.Texture1D, TextureParameterName.TextureLodBias, -0.4f); // might need to use variable to change this value
            }
            GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (int)TextureEnvMode.Replace);

            // создаем RGB или RGBA текстуру 
            switch (Format)
            {
                case 24:
                    GL.TexImage1D(TextureTarget.Texture1D, 0, PixelInternalFormat.Rgb, width, 0, PixelFormat.Rgb,
                        PixelType.UnsignedByte, pixels);
                    break;

                case 32:
                    GL.TexImage1D(TextureTarget.Texture1D, 0, PixelInternalFormat.Rgba, width, 0, PixelFormat.Rgba,
                         PixelType.UnsignedByte, pixels);
                    break;
            }

            // возвращаем идентификатор текстурного объекта 
            return texObject;
        }

        public void bindTexture1D(TextureUnit samplerID, uint texID)
        {
            GL.ActiveTexture(samplerID);
            GL.BindTexture(TextureTarget.Texture1D, texID);
        }

        public void cleanUp()
        {
            GL.DeleteTextures(TextureID.Count, TextureID.ToArray());
            this.TextureID.Clear();
            this.TextureID = null;
        }
    }

}
