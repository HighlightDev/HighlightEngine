﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShaderPattern;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace MassiveGame
{
    class PostprocessShader : Shader
    {
        #region Definations

        private const string SHADER_NAME = "Postprocess Shader";
        private const int BLUR_WIDTH = PostprocessRenderer.MAX_BLUR_WIDTH;
        int frameTexture, blurTexture, depthTexture, AveLum, screenWidth, screenHeight, blurWidth,
            blurStartEdge, blurEndEdge, bloomThreshold;
        int[] weights = new int[BLUR_WIDTH], pixOffset = new int[BLUR_WIDTH];

        int subBlur1, subBlur2, subDefault, subDoF, subBloom1, subBloom2;

        #endregion

        #region Getters uniform

        protected override void getAllUniformLocations()
        {
            frameTexture = base.getUniformLocation("frameTexture");
            blurTexture = base.getUniformLocation("blurTexture");
            depthTexture = base.getUniformLocation("depthTexture");
            AveLum = base.getUniformLocation("AveLum");
            screenWidth = base.getUniformLocation("screenWidth");
            screenHeight = base.getUniformLocation("screenHeight");
            blurWidth = base.getUniformLocation("blurWidth");
            blurStartEdge = base.getUniformLocation("blurStartEdge");
            blurEndEdge = base.getUniformLocation("blurEndEdge");
            bloomThreshold = base.getUniformLocation("bloomThreshold");
            for (int i = 0; i < BLUR_WIDTH; i++)
            {
                this.weights[i] = base.getUniformLocation("Weight[" + i + "]");
                this.pixOffset[i] = base.getUniformLocation("PixOffset[" + i + "]");
            }
            subDefault = base.getSubroutineIndex(ShaderType.FragmentShader, "simple");
            subBlur1 = base.getSubroutineIndex(ShaderType.FragmentShader, "blur1");
            subBlur2 = base.getSubroutineIndex(ShaderType.FragmentShader, "blur2");
            subDoF = base.getSubroutineIndex(ShaderType.FragmentShader, "DoF");
            subBloom1 = base.getSubroutineIndex(ShaderType.FragmentShader, "bloom1");
            subBloom2 = base.getSubroutineIndex(ShaderType.FragmentShader, "bloom2");
        }

        #endregion

        #region Setters uniform

        /*HDR uniforms*/
        public void setUniforms(int frameTexture, float AveLum)
        {
            base.loadInteger(this.frameTexture, frameTexture);
            base.loadFloat(this.AveLum, AveLum);
        }

        /*Blur first pass uniforms*/
        public void setBlur1Uniforms(int frameTexture, float[] weights, int[] pixOffset, int screenWidth, int screenHeight)
        {
            base.loadInteger(this.frameTexture, frameTexture);
            base.loadInteger(this.screenWidth, screenWidth);
            base.loadInteger(this.screenHeight, screenHeight);
            for (int i = 0; i < (weights.Length > BLUR_WIDTH ? BLUR_WIDTH : weights.Length); i++)
            {
                base.loadFloat(this.weights[i], weights[i]);
                base.loadInteger(this.pixOffset[i], pixOffset[i]);
            }
            base.loadInteger(this.blurWidth, weights.Length);
            base.loadSubroutineIndex(ShaderType.FragmentShader, 1, this.subBlur1);
        }

        /*Blur second pass uniforms*/
        public void setBlur2Uniforms(int frameTexture, float[] weights, int[] pixOffset, int screenWidth, int screenHeight)
        {
            base.loadInteger(this.frameTexture, frameTexture);
            base.loadInteger(this.screenWidth, screenWidth);
            base.loadInteger(this.screenHeight, screenHeight);
            for (int i = 0; i < (weights.Length > BLUR_WIDTH ? BLUR_WIDTH : weights.Length); i++)
            {
                base.loadFloat(this.weights[i], weights[i]);
                base.loadInteger(this.pixOffset[i], pixOffset[i]);
            }
            base.loadInteger(this.blurWidth, weights.Length);
            base.loadSubroutineIndex(ShaderType.FragmentShader, 1, this.subBlur2);
        }

        /*Default pass uniforms*/
        public void setDefaultUniforms(int frameTexture)
        {
            base.loadInteger(this.frameTexture, frameTexture);
            base.loadSubroutineIndex(ShaderType.FragmentShader, 1, this.subDefault);
        }

        /*DoF blur second pass uniforms*/
        public void setDoFUniforms(int frameTexture, int blurTexture, int depthTexture,
            float[] weights, int[] pixOffset, float blurStartEdge, float blurEndEdge, int screenWidth, int screenHeight)
        {
            base.loadInteger(this.frameTexture, frameTexture);
            base.loadInteger(this.blurTexture, blurTexture);
            base.loadInteger(this.depthTexture, depthTexture);
            base.loadInteger(this.screenWidth, screenWidth);
            base.loadInteger(this.screenHeight, screenHeight);
            base.loadFloat(this.blurStartEdge, blurStartEdge);
            base.loadFloat(this.blurEndEdge, blurEndEdge);
            for (int i = 0; i < (weights.Length > BLUR_WIDTH ? BLUR_WIDTH : weights.Length); i++)
            {
                base.loadFloat(this.weights[i], weights[i]);
                base.loadInteger(this.pixOffset[i], pixOffset[i]);
            }
            base.loadInteger(this.blurWidth, weights.Length);
            base.loadSubroutineIndex(ShaderType.FragmentShader, 1, this.subDoF);
        }

        /*Bloom brightness pass*/
        public void setBloom1Uniforms(int frameTexture, float bloomThreshold)
        {
            base.loadInteger(this.frameTexture, frameTexture);
            base.loadFloat(this.bloomThreshold, bloomThreshold);
            base.loadSubroutineIndex(ShaderType.FragmentShader, 1, this.subBloom1);
        }

        /*Bloom blured + default image pass*/
        public void setBloom2Uniforms(int defaultTexture, int bluredTexture,
            float[] weights, int[] pixOffset, int screenWidth, int screenHeight)
        {
            base.loadInteger(this.frameTexture, defaultTexture);
            base.loadInteger(this.blurTexture, bluredTexture);
            for (int i = 0; i < (weights.Length > BLUR_WIDTH ? BLUR_WIDTH : weights.Length); i++)
            {
                base.loadFloat(this.weights[i], weights[i]);
                base.loadInteger(this.pixOffset[i], pixOffset[i]);
            }
            base.loadInteger(this.blurWidth, weights.Length);
            base.loadSubroutineIndex(ShaderType.FragmentShader, 1, this.subBloom2);
        }

        #endregion

        protected override void SetShaderMacros()
        {
            SetDefine(ShaderTypeFlag.FragmentShader, "White", "0.928");
            SetDefine(ShaderTypeFlag.FragmentShader, "Exposure", "0.35");
            SetDefine(ShaderTypeFlag.FragmentShader, "MAX_BLUR_WIDTH", "10");
            SetDefine(ShaderTypeFlag.FragmentShader, "lum", "vec3(0.2126, 0.7152, 0.0722)");
            SetDefine(ShaderTypeFlag.FragmentShader, "rgbTOxyz", "mat3(0.6326696, 0.2045558, 0.1269946,0.2284569, 0.7373523, 0.0341908,0.0000000, 0.0095142, 0.8156958)");
            SetDefine(ShaderTypeFlag.FragmentShader, "xyzTOrgb", "mat3(1.7552599, -0.4836786, -0.2530000,-0.5441336, 1.5068789, 0.0215528,0.0063467, -0.0175761, 1.2256959)");
            SetDefine(ShaderTypeFlag.FragmentShader, "White", "0.928");
            SetDefine(ShaderTypeFlag.FragmentShader, "White", "0.928");
        }

        #region Constructor

        public PostprocessShader(string vsPath, string fsPath)
            : base(vsPath, fsPath)
        {
            if (base.ShaderLoaded)
            {
                base.showCompileLogInfo(SHADER_NAME);
                base.showLinkLogInfo(SHADER_NAME);
                Debug.Log.addToLog(getCompileLogInfo(SHADER_NAME));
                Debug.Log.addToLog(getLinkLogInfo(SHADER_NAME));
            }
            else Debug.Log.addToLog( DateTime.Now.ToString() + "  " + SHADER_NAME + " : shader file(s) not found!");
        }

        #endregion
    }
}
