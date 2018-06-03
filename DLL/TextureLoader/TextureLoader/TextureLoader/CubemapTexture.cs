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
    public class CubemapTexture : ITexture
    {
        #region Constructors

        public CubemapTexture(string[] textureFiles)
        {
            textureObj = GenCubeMap(textureFiles);
        }

        #endregion

        private int textureObj;

        public int getInfo()
        {
            return textureObj;
        }

        private static bool checkException(int bitsPP, int bitsPattern)
        {
            try
            {
                if (bitsPattern != bitsPP)
                {
                    throw new Exception("disparity of bits per pixel in textures");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            return true;
        }

        private static int GenCubeMap(string[] textureFiles)
        {
            int bitsPattern = -1;
            int texID;

            GL.GenTextures(1, out texID);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.TextureCubeMap, texID);

            for (int i = 0; i < textureFiles.Length; i++)
            {
                Bitmap image = null;

                try
                {
                    image = new Bitmap(textureFiles[i]);

                    #region Bitmap data
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
                    #endregion

                    bitsPattern = bitsPattern == -1 ? bitsPerPixel : bitsPattern;
                    if (!checkException(bitsPerPixel, bitsPattern))   
                    {
                        /* Если одна из текстур не совпадает с другими по 
                        количеству бит на пиксель - произойдет исключение */
                        return -1;
                    }

                    switch (bitsPerPixel) // в зависимости от полученного результата 
                    {
                        // создаем текстуру, используя режим GL_RGB или GL_RGBA 
                        case 24:
                            GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, 
                                PixelInternalFormat.Rgb, width, height, 0,
                                PixelFormat.Rgb,PixelType.UnsignedByte, textureData.Scan0);
                            break;
                        case 32:
                            GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, 
                                PixelInternalFormat.Rgba, width, height, 0,
                                PixelFormat.Rgba, PixelType.UnsignedByte, textureData.Scan0);
                            break;
                    }

                    // очищаем память
                    image.UnlockBits(textureData);
                    image.Dispose();
                }
                catch
                {
                    throw new Exception("wrong image path or data : " + i.ToString());
                }
            }
            
            // Parameters
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);

            GL.BindTexture(TextureTarget.TextureCubeMap, 0);

            return texID;
        }
        
        public void BindTexture(TextureUnit samplerID)
        {
            GL.ActiveTexture(samplerID);
            GL.BindTexture(TextureTarget.TextureCubeMap, this.textureObj);
        }

        public void CleanUp()
        {
            GL.DeleteTextures(1, ref textureObj);
        }

        public void UnbindTexture(TextureUnit samplerID)
        {
            GL.ActiveTexture(samplerID);
            GL.BindTexture(TextureTarget.TextureCubeMap, 0);
        }

        public uint GetTextureHandler()
        {
            return (UInt32)textureObj;
        }

        public TextureTarget GetTextureTarget()
        {
            return TextureTarget.TextureCubeMap;
        }

        public Point GetTextureRezolution()
        {
            throw new NotImplementedException("Cubemap doesn't have one texture, it has six.");
        }
    }
}
