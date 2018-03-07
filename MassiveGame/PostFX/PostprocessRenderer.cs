﻿using GpuGraphics;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassiveGame
{
    #region PostprocessEnum

    public enum PostprocessType
    {
        MOTION_BLUR = 0,
        DOF_BLUR = 1,
        BLOOM = 2
    }

    #endregion

    public class PostprocessRenderer
    {
        #region Definitions
        private const int BLOOM_MAX_PASS_COUNT = 40;
        public static PostprocessType PostProcessType { private set; get; }

        private bool _blurWidthChanged;
        private float[] _blurWeights;

        private int _blurWidth;
        public int BlurWidth
        {
            set
            {
                _blurWidth = value < MIN_BLUR_WIDTH ? MIN_BLUR_WIDTH :
                    value > MAX_BLUR_WIDTH ? MAX_BLUR_WIDTH : value;
                this._blurWidthChanged = true;
            }
            get { return _blurWidth; }
        }

        private float _blurStartEdge;
        public float BlurStartEdge
        {
            set
            {
                _blurStartEdge = value < 0.0f ? 0.0f :
                    value > 1.0f ? 1.0f : value;
            }
            get { return _blurStartEdge; }
        }

        private float _blurEndEdge;
        public float BlurEndEdge
        {
            set
            {
                _blurEndEdge = value < 0.0f ? 0.0f :
                    value > 1.0f ? 1.0f : value;
            }
            get { return _blurEndEdge; }
        }

        private float _bloomThreshold;
        public float BloomThreshold
        {
            set {_bloomThreshold = value;}
            get { return _bloomThreshold; }
        }

        private int _bloomPass;
        public int BloomPass
        {
            set { _bloomPass = value < 1 ? 1 : value > BLOOM_MAX_PASS_COUNT ? BLOOM_MAX_PASS_COUNT : value; }
            get { return this._bloomPass; }
        }

        public const int MAX_BLUR_WIDTH = 10;
        private const int MIN_BLUR_WIDTH = 2;
        private Vector3 _luma;
        private float _stackLogAve;
        private bool _postConstructor;
        private PostprocessFBO _fbo;
        private PostprocessShader _shader;
        private VBOArrayF _attribs;
        private VAO _buffer;

        private uint _postprocessFilterResult;
        public uint PostprocessFilterResult
        { private set { _postprocessFilterResult = value; } get { return this._postprocessFilterResult; } }

        #endregion

        #region HDR_functions

        /*Mark of brightness RGB value*/
        private float computeLuminance(float r, float g, float b)
        {
            return Vector3.Dot(new Vector3(r, g, b), _luma);
        }

        private float getLogAve()
        {
            int size = _fbo.Texture.Rezolution[0].widthRezolution * _fbo.Texture.Rezolution[0].heightRezolution;
            float[] texData = new float[size * 3];
            GL.GetTexImage(TextureTarget.Texture2D, 0, PixelFormat.Rgb, PixelType.Float, texData);
            float sum = 0.0f;
            for (int i = 0; i < size; i++)
            {
                float lum = computeLuminance(texData[i * 3 + 0], texData[i * 3 + 1], texData[i * 3 + 2]);
                sum += Convert.ToSingle(Math.Log(lum + 0.0001f));
            }
            float logAve = Convert.ToSingle(Math.Exp(sum / size));
            if (Double.IsNaN(logAve)) return _stackLogAve;
            _stackLogAve = logAve;
            return logAve;
        }

        #endregion

        #region HDR_usage

        public void beginHDR()
        {
            postConstructor();
            this._fbo.renderToFBO(1, _fbo.Texture.Rezolution[0].widthRezolution, _fbo.Texture.Rezolution[0].heightRezolution);
        }

        public void endHDR(int screenWidth, int screenHeight)
        {
            this._fbo.unbindFramebuffer();
            GL.Disable(EnableCap.DepthTest);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Viewport(0, 0, screenWidth, screenHeight);

            this._shader.startProgram();
            this._fbo.Texture.bindTexture2D(TextureUnit.Texture0, this._fbo.Texture.TextureID[0]);
            this._shader.setUniforms(0, getLogAve());
            VAOManager.renderBuffers(this._buffer, PrimitiveType.Triangles);
            this._shader.stopProgram();
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

        #region PostProcess_usage

        public void beginPostProcessing()
        {
            postConstructor();
            this._fbo.renderToFBO(1, _fbo.Texture.Rezolution[0].widthRezolution, _fbo.Texture.Rezolution[0].heightRezolution);
        }

        public void endPostProcessing(int screenWidth, int screenHeight, bool blurEnable)
        {
            /*TO DO :
             * true - blur width has changed, and has to be recalculated,
             * flase - blur width hasn't changed, leave it. */
            if (this._blurWidthChanged)
            {
                this._blurWeights = this.normalizedWeights();
                this._blurWidthChanged = false;
            }

            switch (PostProcessType)
            {
                case PostprocessType.MOTION_BLUR:
                    {
                        if (blurEnable)
                        {
                            /*Vertical blur of image*/
                            this._fbo.renderToFBO(2, _fbo.Texture.Rezolution[1].widthRezolution, _fbo.Texture.Rezolution[1].heightRezolution);
                            GL.Disable(EnableCap.DepthTest);

                            this._shader.startProgram();
                            this._fbo.Texture.bindTexture2D(TextureUnit.Texture0, this._fbo.Texture.TextureID[0]);
                            this._shader.setBlur1Uniforms(0, _blurWeights, getPixOffset(), this._fbo.Texture.Rezolution[0].widthRezolution,
                                this._fbo.Texture.Rezolution[0].heightRezolution);
                            VAOManager.renderBuffers(this._buffer, PrimitiveType.Triangles);
                            this._shader.stopProgram();

                            /*Horizontal blur of image*/
                            this._fbo.unbindFramebuffer();
                            GL.Clear(ClearBufferMask.ColorBufferBit);
                            GL.Viewport(0, 0, screenWidth, screenHeight);

                            this._shader.startProgram();
                            this._fbo.Texture.bindTexture2D(TextureUnit.Texture0, this._fbo.Texture.TextureID[1]);
                            this._shader.setBlur2Uniforms(0, _blurWeights, getPixOffset(), this._fbo.Texture.Rezolution[1].widthRezolution,
                                this._fbo.Texture.Rezolution[1].heightRezolution);
                            VAOManager.renderBuffers(this._buffer, PrimitiveType.Triangles);
                            this._shader.stopProgram();
                        }
                        else
                        {
                            /*Just rendering the image*/
                            this._fbo.unbindFramebuffer();
                            GL.Disable(EnableCap.DepthTest);
                            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit); ;
                            GL.Viewport(0, 0, screenWidth, screenHeight);

                            this._shader.startProgram();
                            this._fbo.Texture.bindTexture2D(TextureUnit.Texture0, this._fbo.Texture.TextureID[0]);
                            this._shader.setDefaultUniforms(0);
                            VAOManager.renderBuffers(this._buffer, PrimitiveType.Triangles);
                            this._shader.stopProgram();
                        }
                        break;
                    }
                case PostprocessType.DOF_BLUR:
                    {
                        /*Vertical blur of image*/
                        this._fbo.renderToFBO(2, _fbo.Texture.Rezolution[1].widthRezolution, _fbo.Texture.Rezolution[1].heightRezolution);
                        GL.Disable(EnableCap.DepthTest);

                        this._shader.startProgram();
                        this._fbo.Texture.bindTexture2D(TextureUnit.Texture0, _fbo.Texture.TextureID[0]);
                        this._shader.setBlur1Uniforms(0, _blurWeights, getPixOffset(), this._fbo.Texture.Rezolution[0].widthRezolution,
                                this._fbo.Texture.Rezolution[0].heightRezolution);
                        VAOManager.renderBuffers(_buffer, PrimitiveType.Triangles);
                        this._shader.stopProgram();

                        /*Horizontal blur of image and 
                         comparing with depth texture*/
                        this._fbo.unbindFramebuffer();
                        GL.Clear(ClearBufferMask.ColorBufferBit);
                        GL.Viewport(0, 0, screenWidth, screenHeight);

                        this._shader.startProgram();
                        this._fbo.Texture.bindTexture2D(TextureUnit.Texture0, _fbo.Texture.TextureID[0]);
                        this._fbo.Texture.bindTexture2D(TextureUnit.Texture1, _fbo.Texture.TextureID[1]);
                        this._fbo.Texture.bindTexture2D(TextureUnit.Texture2, _fbo.Texture.TextureID[2]);
                        this._shader.setDoFUniforms(0, 1, 2, _blurWeights, getPixOffset(), this.BlurStartEdge, this.BlurEndEdge,
                            this._fbo.Texture.Rezolution[1].widthRezolution, this._fbo.Texture.Rezolution[1].heightRezolution);
                        VAOManager.renderBuffers(_buffer, PrimitiveType.Triangles);
                        this._shader.stopProgram();
                        break;
                    }
                case PostprocessType.BLOOM:
                    {
                        /*Get bright parts of the image*/
                        this._fbo.renderToFBO(2, _fbo.Texture.Rezolution[1].widthRezolution, _fbo.Texture.Rezolution[1].heightRezolution);
                        GL.Disable(EnableCap.DepthTest);
                        this._shader.startProgram();
                        this._fbo.Texture.bindTexture2D(TextureUnit.Texture0, _fbo.Texture.TextureID[0]);
                        this._shader.setBloom1Uniforms(0, BloomThreshold);
                        VAOManager.renderBuffers(_buffer, PrimitiveType.Triangles);
                        this._shader.stopProgram();

                        /*Blur multipass*/
                        for (int i = 1; i < BloomPass; i++)
                        {
                            /*Vertical blur of bright parts of image*/
                            this._fbo.renderToFBO(3, _fbo.Texture.Rezolution[2].widthRezolution, _fbo.Texture.Rezolution[2].heightRezolution);
                            this._shader.startProgram();
                            this._fbo.Texture.bindTexture2D(TextureUnit.Texture0, _fbo.Texture.TextureID[1]);
                            this._shader.setBlur1Uniforms(0, _blurWeights, getPixOffset(), this._fbo.Texture.Rezolution[1].widthRezolution,
                                this._fbo.Texture.Rezolution[1].heightRezolution);
                            VAOManager.renderBuffers(_buffer, PrimitiveType.Triangles);
                            this._shader.stopProgram();

                            /*Horizontal blur of image*/
                            this._fbo.renderToFBO(2, _fbo.Texture.Rezolution[1].widthRezolution, _fbo.Texture.Rezolution[1].heightRezolution);
                            this._shader.startProgram();
                            this._fbo.Texture.bindTexture2D(TextureUnit.Texture0, this._fbo.Texture.TextureID[2]);
                            this._shader.setBlur2Uniforms(0, _blurWeights, getPixOffset(), this._fbo.Texture.Rezolution[2].widthRezolution,
                                this._fbo.Texture.Rezolution[2].heightRezolution);
                            VAOManager.renderBuffers(this._buffer, PrimitiveType.Triangles);
                            this._shader.stopProgram();
                        }

                        /*Vertical blur of bright parts of image*/
                        this._fbo.renderToFBO(3, _fbo.Texture.Rezolution[2].widthRezolution, _fbo.Texture.Rezolution[2].heightRezolution);
                        this._shader.startProgram();
                        this._fbo.Texture.bindTexture2D(TextureUnit.Texture0, _fbo.Texture.TextureID[1]);
                        this._shader.setBlur1Uniforms(0, _blurWeights, getPixOffset(), this._fbo.Texture.Rezolution[1].widthRezolution,
                                this._fbo.Texture.Rezolution[1].heightRezolution);
                        VAOManager.renderBuffers(_buffer, PrimitiveType.Triangles);
                        this._shader.stopProgram();

                        /*Horizontal blur of brigth parts of image and 
                          adding them to default image*/
                        this._fbo.unbindFramebuffer();
                        GL.Clear(ClearBufferMask.ColorBufferBit);
                        GL.Viewport(0, 0, screenWidth, screenHeight);

                        this._shader.startProgram();
                        this._fbo.Texture.bindTexture2D(TextureUnit.Texture0, _fbo.Texture.TextureID[0]);
                        this._fbo.Texture.bindTexture2D(TextureUnit.Texture1, _fbo.Texture.TextureID[2]);
                        this._shader.setBloom2Uniforms(0, 1, _blurWeights, getPixOffset(), this._fbo.Texture.Rezolution[2].widthRezolution,
                                this._fbo.Texture.Rezolution[2].heightRezolution);
                        VAOManager.renderBuffers(_buffer, PrimitiveType.Triangles);
                        this._shader.stopProgram();

                        break;
                    }
                default:
                    {
                        /*Just rendering the image*/
                        this._fbo.unbindFramebuffer();
                        GL.Disable(EnableCap.DepthTest);
                        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit); ;
                        GL.Viewport(0, 0, screenWidth, screenHeight);

                        this._shader.startProgram();
                        this._fbo.Texture.bindTexture2D(TextureUnit.Texture0, this._fbo.Texture.TextureID[0]);
                        this._shader.setDefaultUniforms(0);
                        VAOManager.renderBuffers(this._buffer, PrimitiveType.Triangles);
                        this._shader.stopProgram();
                        break;
                    }
            }
            GL.Enable(EnableCap.DepthTest);
        }

        public void sendPostProcessingToGraphicsFilter(int screenWidth, int screenHeight, bool blurEnable)
        {
            /*TO DO :
            * true - blur width has changed, and has to be recalculated,
            * flase - blur width hasn't changed, leave it. */
            if (this._blurWidthChanged)
            {
                this._blurWeights = this.normalizedWeights();
                this._blurWidthChanged = false;
            }

            switch (PostProcessType)
            {
                case PostprocessType.MOTION_BLUR:
                    {
                        if (blurEnable)
                        {
                            /*Vertical blur of image*/
                            this._fbo.renderToFBO(2, _fbo.Texture.Rezolution[1].widthRezolution, _fbo.Texture.Rezolution[1].heightRezolution);
                            GL.Disable(EnableCap.DepthTest);

                            this._shader.startProgram();
                            this._fbo.Texture.bindTexture2D(TextureUnit.Texture0, this._fbo.Texture.TextureID[0]);
                            this._shader.setBlur1Uniforms(0, _blurWeights, getPixOffset(), this._fbo.Texture.Rezolution[0].widthRezolution,
                                this._fbo.Texture.Rezolution[0].heightRezolution);
                            VAOManager.renderBuffers(this._buffer, PrimitiveType.Triangles);
                            this._shader.stopProgram();

                            /*Horizontal blur of image*/
                            this._fbo.unbindFramebuffer();
                            this._fbo.renderToFBO(1, _fbo.Texture.Rezolution[0].widthRezolution, _fbo.Texture.Rezolution[0].heightRezolution);

                            this._shader.startProgram();
                            this._fbo.Texture.bindTexture2D(TextureUnit.Texture0, this._fbo.Texture.TextureID[1]);
                            this._shader.setBlur2Uniforms(0, _blurWeights, getPixOffset(), this._fbo.Texture.Rezolution[1].widthRezolution,
                                this._fbo.Texture.Rezolution[1].heightRezolution);
                            VAOManager.renderBuffers(this._buffer, PrimitiveType.Triangles);
                            this._shader.stopProgram();

                            //Send image result of postprocess filtering
                            this._postprocessFilterResult = this._fbo.Texture.TextureID[0];
                        }
                        else
                        {
                            //Send image result of postprocess filtering
                            this._postprocessFilterResult = this._fbo.Texture.TextureID[0];
                        }
                        break;
                    }
                case PostprocessType.DOF_BLUR:
                    {
                        /*Vertical blur of image*/
                        this._fbo.renderToFBO(2, _fbo.Texture.Rezolution[1].widthRezolution, _fbo.Texture.Rezolution[1].heightRezolution);
                        GL.Disable(EnableCap.DepthTest);

                        this._shader.startProgram();
                        this._fbo.Texture.bindTexture2D(TextureUnit.Texture0, _fbo.Texture.TextureID[0]);
                        this._shader.setBlur1Uniforms(0, _blurWeights, getPixOffset(), this._fbo.Texture.Rezolution[0].widthRezolution,
                                this._fbo.Texture.Rezolution[0].heightRezolution);
                        VAOManager.renderBuffers(_buffer, PrimitiveType.Triangles);
                        this._shader.stopProgram();

                        /*Horizontal blur of image and 
                         comparing with depth texture*/
                        this._fbo.renderToFBO(3, this._fbo.Texture.Rezolution[3].widthRezolution,
                            this._fbo.Texture.Rezolution[3].heightRezolution);

                        this._shader.startProgram();
                        this._fbo.Texture.bindTexture2D(TextureUnit.Texture0, _fbo.Texture.TextureID[0]);
                        this._fbo.Texture.bindTexture2D(TextureUnit.Texture1, _fbo.Texture.TextureID[1]);
                        this._fbo.Texture.bindTexture2D(TextureUnit.Texture2, _fbo.Texture.TextureID[2]);
                        this._shader.setDoFUniforms(0, 1, 2, _blurWeights, getPixOffset(), this.BlurStartEdge, this.BlurEndEdge,
                            this._fbo.Texture.Rezolution[1].widthRezolution, this._fbo.Texture.Rezolution[1].heightRezolution);
                        VAOManager.renderBuffers(_buffer, PrimitiveType.Triangles);
                        this._shader.stopProgram();

                        //Send image result of postprocess filtering
                        this._postprocessFilterResult = this._fbo.Texture.TextureID[3];
                        break;
                    }
                case PostprocessType.BLOOM:
                    {
                        /*Get bright parts of the image*/
                        this._fbo.renderToFBO(2, _fbo.Texture.Rezolution[1].widthRezolution, _fbo.Texture.Rezolution[1].heightRezolution);
                        GL.Disable(EnableCap.DepthTest);
                        this._shader.startProgram();
                        this._fbo.Texture.bindTexture2D(TextureUnit.Texture0, _fbo.Texture.TextureID[0]);
                        this._shader.setBloom1Uniforms(0, BloomThreshold);
                        VAOManager.renderBuffers(_buffer, PrimitiveType.Triangles);
                        this._shader.stopProgram();

                        /*Blur multipass*/
                        for (int i = 1; i < BloomPass; i++)
                        {
                            /*Vertical blur of bright parts of image*/
                            this._fbo.renderToFBO(3, _fbo.Texture.Rezolution[2].widthRezolution, _fbo.Texture.Rezolution[2].heightRezolution);
                            this._shader.startProgram();
                            this._fbo.Texture.bindTexture2D(TextureUnit.Texture0, _fbo.Texture.TextureID[1]);
                            this._shader.setBlur1Uniforms(0, _blurWeights, getPixOffset(), this._fbo.Texture.Rezolution[1].widthRezolution,
                                this._fbo.Texture.Rezolution[1].heightRezolution);
                            VAOManager.renderBuffers(_buffer, PrimitiveType.Triangles);
                            this._shader.stopProgram();

                            /*Horizontal blur of image*/
                            this._fbo.renderToFBO(2, _fbo.Texture.Rezolution[1].widthRezolution, _fbo.Texture.Rezolution[1].heightRezolution);
                            this._shader.startProgram();
                            this._fbo.Texture.bindTexture2D(TextureUnit.Texture0, this._fbo.Texture.TextureID[2]);
                            this._shader.setBlur2Uniforms(0, _blurWeights, getPixOffset(), this._fbo.Texture.Rezolution[2].widthRezolution,
                                this._fbo.Texture.Rezolution[2].heightRezolution);
                            VAOManager.renderBuffers(this._buffer, PrimitiveType.Triangles);
                            this._shader.stopProgram();
                        }

                        /*Vertical blur of bright parts of image*/
                        this._fbo.renderToFBO(3, _fbo.Texture.Rezolution[2].widthRezolution, _fbo.Texture.Rezolution[2].heightRezolution);
                        this._shader.startProgram();
                        this._fbo.Texture.bindTexture2D(TextureUnit.Texture0, _fbo.Texture.TextureID[1]);
                        this._shader.setBlur1Uniforms(0, _blurWeights, getPixOffset(), this._fbo.Texture.Rezolution[1].widthRezolution,
                                this._fbo.Texture.Rezolution[1].heightRezolution);
                        VAOManager.renderBuffers(_buffer, PrimitiveType.Triangles);
                        this._shader.stopProgram();

                        /*Horizontal blur of brigth parts of image and 
                          adding them to default image*/
                        this._fbo.renderToFBO(4, this._fbo.Texture.Rezolution[3].widthRezolution,
                           this._fbo.Texture.Rezolution[3].heightRezolution);

                        this._shader.startProgram();
                        this._fbo.Texture.bindTexture2D(TextureUnit.Texture0, _fbo.Texture.TextureID[0]);
                        this._fbo.Texture.bindTexture2D(TextureUnit.Texture1, _fbo.Texture.TextureID[2]);
                        this._shader.setBloom2Uniforms(0, 1, _blurWeights, getPixOffset(), this._fbo.Texture.Rezolution[2].widthRezolution,
                                this._fbo.Texture.Rezolution[2].heightRezolution);
                        VAOManager.renderBuffers(_buffer, PrimitiveType.Triangles);
                        this._shader.stopProgram();

                        //Send image result of postprocess filtering
                        this._postprocessFilterResult = this._fbo.Texture.TextureID[3];
                        break;
                    }
            }
            GL.Enable(EnableCap.DepthTest);
        }

        #endregion

        #region Cleaning

        public void cleanUp()
        {
            this._fbo.cleanUp();
            this._shader.cleanUp();
            VAOManager.cleanUp(this._buffer);
        }

        #endregion

        #region Constructor

        private void postConstructor()
        {
            if (this._postConstructor)
            {
                this._shader = new PostprocessShader(ProjectFolders.ShadersPath + "postprocessVS.glsl",
                    ProjectFolders.ShadersPath + "postprocessFS.glsl");
                VAOManager.genVAO(_buffer);
                VAOManager.setBufferData(BufferTarget.ArrayBuffer, _buffer);
                this._postConstructor = !this._postConstructor;
            }
        }

        public PostprocessRenderer(PostprocessType ppType)
        {
            /*Set postprocess type*/
            PostprocessRenderer.PostProcessType = ppType;
            /*Screen fill quad*/
            this._attribs = new VBOArrayF(new float[6, 3] { { -1.0f, -1.0f, 0.0f }, { 1.0f, -1.0f, 0.0f }, { 1.0f, 1.0f, 0.0f },
         { 1.0f, 1.0f, 0.0f }, { -1.0f, 1.0f, 0.0f } ,  { -1.0f, -1.0f, 0.0f} },
         new float[6, 2] { { 0, 1 }, { 1, 1 }, { 1, 0 }, { 1, 0 }, { 0, 0 }, { 0, 1 } }, null);
            _buffer = new VAO(_attribs);
            /*Enable post constructor*/
            this._postConstructor = true;
            this._fbo = new PostprocessFBO();
            this._luma = new Vector3(0.2126f, 0.7152f, 0.0722f);
            this.BlurWidth = 8;
            this.BlurStartEdge = 0.98f;
            this.BlurEndEdge = 0.95f;
            this.BloomThreshold = 0.5f;
            this.BloomPass = 1;
            this._blurWidthChanged = true;
        }

        #endregion
    }
}