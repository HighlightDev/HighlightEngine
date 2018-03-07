using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GpuGraphics;
using ShaderPattern;
using FramebufferAPI;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using TextureLoader;
using MassiveGame.API.Collector;

namespace MassiveGame
{
    public class LensFlareRenderer
    {
        #region Definitions 

        private const int BLUR_MAX_PASS_COUNT = 20;
        public const int MAX_BLUR_WIDTH = 30;
        private const int MIN_BLUR_WIDTH = 2;
        public static bool LensFlareEnabled = false;

        private VBOArrayF _attribs;
        private VAO _buffer;
        private bool _postConstructor;
        private LensFlareFBO _fbo;
        private LensFlareShader _lensShader;
        private Texture1D _lensColor;
        private ITexture _dirtTexture;
        private ITexture _starTexture;
        private int _blurWidth;
        public int BlurWidth
        {
            set
            {
                _blurWidth = value < MIN_BLUR_WIDTH ? MIN_BLUR_WIDTH :
                    value > MAX_BLUR_WIDTH ? MAX_BLUR_WIDTH : value;
            }
            get { return _blurWidth; }
        }

        private float _ghostDispersal;
        public float GhostDispersal
        {
            set { _ghostDispersal = value < 0f ? 0.0f : value; }
            get { return this._ghostDispersal; }
        }

        private int _ghosts;
        public int Ghosts
        {
            set { _ghosts = value < 0 ? 0 : value; }
            get { return this._ghosts; }
        }

        private float _haloWidth;
        public float HaloWidth
        {
            set { _haloWidth = value < 0f ? 0.0f : value; }
            get { return this._haloWidth; }
        }

        private float _distortion;
        public float Distortion
        {
            set { _distortion = value < 0f ? 0.0f : value; }
            get { return this._distortion; }
        }

        private float _threshold;
        public float Threshold
        {
            set { _threshold = value < 0f ? 0.0f : value; }
            get { return this._threshold; }
        }

        private int _blurPassCount;
        public int BlurPassCount
        {
            set { _blurPassCount = value < 1 ? 1 : value > BLUR_MAX_PASS_COUNT ? BLUR_MAX_PASS_COUNT : value; }
            get { return _blurPassCount; }
        }

        #endregion

        #region Renderer

        public void beginLensFlareDefaultScene()
        {
            postConstructor();
            _fbo.renderToFBO(3, this._fbo.Texture.Rezolution[2].widthRezolution,
                this._fbo.Texture.Rezolution[2].heightRezolution);     
        }

        public void beginLensFlareSpecialScene()
        {
            postConstructor();
            _fbo.renderToFBO(4, this._fbo.Texture.Rezolution[3].widthRezolution,
                this._fbo.Texture.Rezolution[3].heightRezolution); 
        }

        public void endLensFlareWithoutPostprocess(Camera camera, int width, int height)
        {
            /*Extracting brigth parts*/
            _fbo.renderToFBO(2, this._fbo.Texture.Rezolution[1].widthRezolution,
                this._fbo.Texture.Rezolution[1].heightRezolution);

            _lensShader.startProgram();
            _fbo.Texture.bindTexture2D(TextureUnit.Texture0, _fbo.Texture.TextureID[3]);
            _lensShader.setUniformValuesThreshold(0, this._threshold);
            VAOManager.renderBuffers(_buffer, PrimitiveType.Triangles);
            _lensShader.stopProgram();

            /*Lens effect*/
            _fbo.renderToFBO(1, this._fbo.Texture.Rezolution[0].widthRezolution,
                  this._fbo.Texture.Rezolution[0].heightRezolution);

            _lensShader.startProgram();
            _fbo.Texture.bindTexture2D(TextureUnit.Texture0, _fbo.Texture.TextureID[1]);
            _lensColor.bindTexture1D(TextureUnit.Texture1, _lensColor.TextureID[0]);
            _lensShader.setUniformValuesLens(0, 1, this.Ghosts, this.HaloWidth, this.Distortion, this.GhostDispersal);
            VAOManager.renderBuffers(_buffer, PrimitiveType.Triangles);
            _lensShader.stopProgram();

            /*Extra passes for blur*/
            for (int i = 0; i < BlurPassCount; i++)
            {
                /*Vertical Blur effect*/
                _fbo.renderToFBO(2, this._fbo.Texture.Rezolution[1].widthRezolution,
                    this._fbo.Texture.Rezolution[1].heightRezolution);

                _lensShader.startProgram();
                _fbo.Texture.bindTexture2D(TextureUnit.Texture0, _fbo.Texture.TextureID[0]);
                _lensShader.setUniformValuesVerticalBlur(0, normalizedWeights(), getPixOffset(), width, height);
                VAOManager.renderBuffers(_buffer, PrimitiveType.Triangles);
                _lensShader.stopProgram();

                /*Horizontal Blur effect*/
                _fbo.renderToFBO(1, this._fbo.Texture.Rezolution[0].widthRezolution,
                        this._fbo.Texture.Rezolution[0].heightRezolution);

                _lensShader.startProgram();
                _fbo.Texture.bindTexture2D(TextureUnit.Texture0, _fbo.Texture.TextureID[1]);
                _lensShader.setUniformValuesHorizontalBlur(0, normalizedWeights(), getPixOffset(), width, height);
                VAOManager.renderBuffers(_buffer, PrimitiveType.Triangles);
                _lensShader.stopProgram();
            }

            /*Add some enhancements to result image*/
            _fbo.unbindFramebuffer();
            GL.Viewport(0, 0, width, height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _lensShader.startProgram();
            _fbo.Texture.bindTexture2D(TextureUnit.Texture0, _fbo.Texture.TextureID[2]);
            _fbo.Texture.bindTexture2D(TextureUnit.Texture1, _fbo.Texture.TextureID[0]);
            _dirtTexture.BindTexture(TextureUnit.Texture2);
            _starTexture.BindTexture(TextureUnit.Texture3);
            _lensShader.setUniformValuesMod(0, 1, 2, 3, getStarburstMatrix(getCamrot(camera)));
            VAOManager.renderBuffers(_buffer, PrimitiveType.Triangles);
            _lensShader.stopProgram(); 
        }

        public void endLensFlareWithPostprocess(Camera camera, int width, int height, uint postprocessFilterResult)
        {
            /*Extracting brigth parts*/
            _fbo.renderToFBO(2, this._fbo.Texture.Rezolution[1].widthRezolution,
                this._fbo.Texture.Rezolution[1].heightRezolution);

            _lensShader.startProgram();
            _fbo.Texture.bindTexture2D(TextureUnit.Texture0, _fbo.Texture.TextureID[3]);
            _lensShader.setUniformValuesThreshold(0, this._threshold);
            VAOManager.renderBuffers(_buffer, PrimitiveType.Triangles);
            _lensShader.stopProgram();

            /*Lens effect*/
            _fbo.renderToFBO(1, this._fbo.Texture.Rezolution[0].widthRezolution,
                  this._fbo.Texture.Rezolution[0].heightRezolution);

            _lensShader.startProgram();
            _fbo.Texture.bindTexture2D(TextureUnit.Texture0, _fbo.Texture.TextureID[1]);
            _lensColor.bindTexture1D(TextureUnit.Texture1, _lensColor.TextureID[0]);
            _lensShader.setUniformValuesLens(0, 1, this.Ghosts, this.HaloWidth, this.Distortion, this.GhostDispersal);
            VAOManager.renderBuffers(_buffer, PrimitiveType.Triangles);
            _lensShader.stopProgram();

            /*Extra passes for blur*/
            for (int i = 1; i < 8; i++)
            {
                /*Vertical Blur effect*/
                _fbo.renderToFBO(2, this._fbo.Texture.Rezolution[1].widthRezolution,
                    this._fbo.Texture.Rezolution[1].heightRezolution);

                _lensShader.startProgram();
                _fbo.Texture.bindTexture2D(TextureUnit.Texture0, _fbo.Texture.TextureID[0]);
                _lensShader.setUniformValuesVerticalBlur(0, normalizedWeights(), getPixOffset(), width, height);
                VAOManager.renderBuffers(_buffer, PrimitiveType.Triangles);
                _lensShader.stopProgram();

                /*Horizontal Blur effect*/
                _fbo.renderToFBO(1, this._fbo.Texture.Rezolution[0].widthRezolution,
                        this._fbo.Texture.Rezolution[0].heightRezolution);

                _lensShader.startProgram();
                _fbo.Texture.bindTexture2D(TextureUnit.Texture0, _fbo.Texture.TextureID[1]);
                _lensShader.setUniformValuesHorizontalBlur(0, normalizedWeights(), getPixOffset(), width, height);
                VAOManager.renderBuffers(_buffer, PrimitiveType.Triangles);
                _lensShader.stopProgram();
            }

            /*Add some enhancements to result image*/
            _fbo.unbindFramebuffer();
            GL.Viewport(0, 0, width, height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _lensShader.startProgram();
            _fbo.Texture.bindTexture2D(TextureUnit.Texture0, postprocessFilterResult);
            _fbo.Texture.bindTexture2D(TextureUnit.Texture1, _fbo.Texture.TextureID[0]);
            _dirtTexture.BindTexture(TextureUnit.Texture2);
            _starTexture.BindTexture(TextureUnit.Texture3);
            _lensShader.setUniformValuesMod(0, 1, 2, 3, getStarburstMatrix(getCamrot(camera)));
            VAOManager.renderBuffers(_buffer, PrimitiveType.Triangles);
            _lensShader.stopProgram();
        }

        #endregion

        #region Blur_functions

        private float gaussFunction(float x, float sigma)   //Рассчет функции Гаусса
        {
            float pi = Convert.ToSingle(Math.PI);
            float e = Convert.ToSingle(Math.E);
            float power = -((x * x) / (2 * (sigma * sigma)));
            float oneDimentionGauss = (1.0f / (Convert.ToSingle(Math.Sqrt(2 * pi * (sigma * sigma)))))
                * Convert.ToSingle(Math.Pow(e, power));
            return oneDimentionGauss;
        }

        private float[] normalizedWeights() //Подсчитываем нормализованные веса 
        {
            float[] weights = new float[BlurWidth];
            float sum, sigma2 = 4.0f;
            // Compute and sum the weights
            weights[0] = gaussFunction(0, sigma2); // The 1-D Gaussian function
            sum = weights[0];
            for (int i = 1; i < weights.Length; i++)
            {
                weights[i] = gaussFunction(i, sigma2);
                sum += 2 * weights[i];
            }
            // Normalize the weights and set the uniform
            for (int i = 0; i < weights.Length; i++)
            {
                weights[i] = weights[i] / sum;
            }
            return weights;
        }

        private int[] getPixOffset()
        {
            /*True - blur width lower then min value
             False - 
             *  True - blur width greater then max value
                False - blur width lower then max value*/
            int[] pixOffset = new int[BlurWidth];
            for (int i = 0; i < pixOffset.Length; i++)
            {
                pixOffset[i] = i;
            }
            return pixOffset;
        }

        #endregion

        #region Starburst_matrix_calculations

        private float getCamrot(Camera camera)
        {
            Vector3 camx = new Vector3(camera.getViewMatrix().Column0); // camera x (left) vector
            Vector3 camz = new Vector3(camera.getViewMatrix().Column1); // camera z (forward) vector
            float camrot = Vector3.Dot(camx, new Vector3(0, 0, 1)) + Vector3.Dot(camz, new Vector3(0, 1, 0));
            return camrot;
        }

        private Matrix3 getStarburstMatrix(float camrot)
        {
            Matrix3 scaleBias1 = new Matrix3(
             2.0f, 0.0f, -1.0f,
             0.0f, 2.0f, -1.0f,
             0.0f, 0.0f, 1.0f);

            float cosCam = Convert.ToSingle(Math.Cos(camrot));
            float sinCam = Convert.ToSingle(Math.Sin(camrot));

            Matrix3 rotation = new Matrix3(
            cosCam, -sinCam, 0.0f,
            sinCam, cosCam, 0.0f,
            0.0f, 0.0f, 1.0f);

            Matrix3 scaleBias2 = new Matrix3(
               0.5f, 0.0f, 0.5f,
               0.0f, 0.5f, 0.5f,
               0.0f, 0.0f, 1.0f);

            Matrix3 uLensStarMatrix = scaleBias2 * rotation * scaleBias1;
            return uLensStarMatrix;
        }

        #endregion

        #region Constructor

        private void postConstructor()
        {
            if (_postConstructor)
            {
                VAOManager.genVAO(_buffer);
                VAOManager.setBufferData(BufferTarget.ArrayBuffer, _buffer);
                _fbo = new LensFlareFBO();
                _lensShader = new LensFlareShader(ProjectFolders.ShadersPath + "lensFlareVS.glsl",
                    ProjectFolders.ShadersPath + "lensFlareFS.glsl");
                _postConstructor = !_postConstructor;
            }
        }

        public LensFlareRenderer()
        {
            this._attribs = new VBOArrayF(new float[6, 3] { { -1.0f, -1.0f, 0.0f }, { 1.0f, -1.0f, 0.0f }, { 1.0f, 1.0f, 0.0f },
            { 1.0f, 1.0f, 0.0f }, { -1.0f, 1.0f, 0.0f } ,  { -1.0f, -1.0f, 0.0f} },
            new float[6, 2] { { 0, 1 }, { 1, 1 }, { 1, 0 }, { 1, 0 }, { 0, 0 }, { 0, 1 } }, null);
            _buffer = new VAO(_attribs);
            _postConstructor = true;

            this.BlurWidth = 6;
            this._ghostDispersal = 0.37f;
            this._ghosts = 8;
            this._haloWidth = 0.55f;
            this._distortion = 3.50f;
            this._threshold = 0.5f;
            this._blurPassCount = 8;
            LensFlareRenderer.LensFlareEnabled = true;
            this._lensColor = new Texture1D(new string[] { ProjectFolders.LensFlareTexturePath + "lenscolor.png" }, false, 0);
            this._dirtTexture = ResourcePool.GetTexture(ProjectFolders.LensFlareTexturePath + "lensdirt.png"); 
            this._starTexture = ResourcePool.GetTexture(ProjectFolders.LensFlareTexturePath + "lenscolor.png");
        }

        #endregion

        #region Cleaning

        public void cleanUp()
        {
            VAOManager.cleanUp(_buffer);
            _lensColor.cleanUp();
            _dirtTexture.CleanUp();
            _starTexture.CleanUp();
            _lensShader.cleanUp();
            _fbo.cleanUp();
        }

        #endregion

    }
}
