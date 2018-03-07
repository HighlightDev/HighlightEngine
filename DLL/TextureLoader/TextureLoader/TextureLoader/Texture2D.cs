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
    public class Texture2D
    {
        #region Constructors

        /// <summary>
        /// Загрузчик текстур с использованием технологий MipMapping и Anisotropic Filtering (если поддерживается аппаратно).
        /// </summary>
        /// <param name="imagePath">Путь(и) к текстуре(ам) для загрузки.</param>
        /// <param name="generateMipMap">Флаг использования технологии MipMapping. Если <b>true</b> - технология будет использована.</param>
        /// <param name="anisotropyLevel">Уровень анизотропной фильтрации (если поддерживается аппаратно). Если <b>-1.0f</b> - будет использован максимальный режим анизотропной фильтрации.</param>
        public Texture2D(string[] imagePath, bool generateMipMap = true, float anisotropyLevel = -1.0f)
        {
            TextureID = new List<uint>();
            Rezolution = new List<TextureRezolution>();

            uint[] tempID = GenImagies(imagePath, generateMipMap, anisotropyLevel);

            foreach (uint id in tempID)
            {
                TextureID.Add(id);
            }
        }

        /// <summary>
        /// Загрузчик текстур с использованием технологий MipMapping и Anisotropic Filtering (если поддерживается аппаратно).
        /// </summary>
        /// <param name="imagePath">Путь(и) к текстуре(ам) для загрузки.</param>
        /// <param name="texWrap">Режим повторения текстуры.</param>
        /// <param name="generateMipMap">Флаг использования технологии MipMapping. Если <b>true</b> - технология будет использована.</param>
        /// <param name="anisotropyLevel">Уровень анизотропной фильтрации (если поддерживается аппаратно). Если <b>-1.0f</b> - будет использован максимальный режим анизотропной фильтрации.</param>
        public Texture2D(string[] imagePath, TextureWrapMode texWrap, bool generateMipMap = true, float anisotropyLevel = -1.0f)
        {
            TextureID = new List<uint>();
            Rezolution = new List<TextureRezolution>();

            uint[] tempID = GenImagies(imagePath, generateMipMap, anisotropyLevel, texWrap);

            foreach (uint id in tempID)
            {
                TextureID.Add(id);
            }
        }

        /// <summary>
        /// Загрузчик текстур с использованием технологий MipMapping и Anisotropic Filtering (если поддерживается аппаратно).
        /// </summary>
        /// <param name="imagePath">Путь(и) к текстуре(ам) для загрузки.</param>
        /// <param name="texWrap">Режим повторения текстуры.</param>
        /// <param name="generateMipMap">Флаг использования технологии MipMapping. Если <b>true</b> - технология будет использована.</param>
        /// <param name="filterType">Режим фильтрации MipMap текстур.</param>
        /// <param name="anisotropyLevel">Уровень анизотропной фильтрации (если поддерживается аппаратно). Если <b>-1.0f</b> - будет использован максимальный режим анизотропной фильтрации.</param>
        public Texture2D(string[] imagePath, TextureWrapMode texWrap, bool generateMipMap = true, 
            MipmapTextureFilter filterType = MipmapTextureFilter.LinearMipmapLinear, float anisotropyLevel = -1.0f)
        {
            TextureID = new List<uint>();
            Rezolution = new List<TextureRezolution>();

            uint[] tempID = GenImagies(imagePath, generateMipMap, anisotropyLevel, texWrap, filterType);

            foreach (uint id in tempID)
            {
                TextureID.Add(id);
            }
        }

        public Texture2D()
        {
            TextureID = new List<uint>();
            Rezolution = new List<TextureRezolution>();
        }

        #endregion

        public List<uint> TextureID { private set; get; }
        public List<TextureRezolution> Rezolution { private set; get; }
        private float maxAnisotropy;
        public float MaxAnisotropy { get { return maxAnisotropy; } }

        private bool CheckForAnisotropicTextureFiltering()
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

        private uint[] GenImagies(string[] imagePath, bool generateMipMap, float anisotropyLevel,
            TextureWrapMode texWrap = TextureWrapMode.Repeat, 
            MipmapTextureFilter filterType = MipmapTextureFilter.LinearMipmapLinear)
        {

            uint[] glTextureID = new uint[imagePath.Length];
            int iterator = 0;

            foreach (string url in imagePath)
            {
                Bitmap image = null;
                var success = false;

                try
                {
                    image = new Bitmap(url);

                    #region Bitmap data
                    // если загрузка прошла успешно 
                    // сохраняем размеры изображения 
                    int width = image.Width;
                    int height = image.Height;

                    this.Rezolution.Add(new TextureRezolution(width, height));

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
                    #endregion

                    switch (bitsPerPixel) // в зависимости от полученного результата 
                    {
                        // создаем текстуру, используя режим GL_RGB или GL_RGBA 
                        case 24:
                            glTextureID[iterator] = MakeGlTexture(24, textureData.Scan0, width, height, generateMipMap, anisotropyLevel, texWrap, filterType);
                            // активируем флаг, сигнализирующий загрузку текстуры 
                            success = true;
                            break;
                        case 32:
                            glTextureID[iterator] = MakeGlTexture(32, textureData.Scan0, width, height, generateMipMap, anisotropyLevel, texWrap, filterType);
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
                    return null;
                }
            }

            return glTextureID;
        }

        #region Gen of empty imagies

        private static uint[] genEmptyImagies(int countImagies, int widthRes, int heightRes, int filtration,
            PixelInternalFormat colorComponentCount, PixelFormat pixelData, PixelType type, 
            TextureWrapMode texWrap)
        {
            uint[] imageId = new uint[countImagies];
            GL.GenTextures(countImagies, imageId);

            for (int i = 0; i < imageId.Length; i++)
            {
                GL.BindTexture(TextureTarget.Texture2D, imageId[i]);

                GL.TexImage2D(TextureTarget.Texture2D, 0, colorComponentCount, widthRes, heightRes, 0, pixelData,
                    type, new IntPtr(0));

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, filtration);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, filtration);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)texWrap);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)texWrap);
            }
            return imageId;
        }

        public void genEmptyImg(int countImagies, int widthRes, int heightRes, int filtration,
            PixelInternalFormat colorComponentCount, PixelFormat pixelData, PixelType type,
            TextureWrapMode texWrap = TextureWrapMode.Repeat)
        {
            TextureID.AddRange(genEmptyImagies(countImagies, widthRes, heightRes, filtration, colorComponentCount, pixelData, type, texWrap));
            for (int i = 0; i < countImagies; i++)
            {
                Rezolution.Add(new TextureRezolution(widthRes, heightRes));
            }
        }

        #endregion

        private uint MakeGlTexture(int format, IntPtr pixels, int width, int height, 
            bool generateMipMap, float anisotropyLevel, TextureWrapMode texWrap, MipmapTextureFilter filterType)
        {
            // идентификатор текстурного объекта 
            uint texObject;

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
                if (this.CheckForAnisotropicTextureFiltering())
                {
                    GL.TexParameter(TextureTarget.Texture2D, (TextureParameterName)ExtTextureFilterAnisotropic.TextureMaxAnisotropyExt,
                        (anisotropyLevel == -1.0f) ? maxAnisotropy : (anisotropyLevel >= maxAnisotropy) ? maxAnisotropy : (anisotropyLevel < 0.0f) ? 0.0f : anisotropyLevel);
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

        public void bindTexture2D(TextureUnit samplerID, uint texID)
        {
            GL.ActiveTexture(samplerID);
            GL.BindTexture(TextureTarget.Texture2D, texID);
        }

        public void cleanUp()
        {
            GL.DeleteTextures(TextureID.Count, TextureID.ToArray());
            TextureID.Clear();
            TextureID = null;
        }
    }

    public struct TextureRezolution
    {
        public int widthRezolution;
        public int heightRezolution;

        public TextureRezolution(int width, int height)
        {
            widthRezolution = width;
            heightRezolution = height;
        }
    }
}
