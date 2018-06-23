﻿using System;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK;

namespace MassiveGame.PostFX.LensFlare
{
    public class LensFlareShader<T> : PostProcessShaderBase<T> where T: PostProcessSubsequenceType
    {
        #region Difinitions

        private const Int32 BLUR_WIDTH = LensFlarePostProcess<T>.MAX_BLUR_WIDTH;
        private const string SHADER_NAME = "Lens flare shader";

        private Int32 frameTexture, threshold, lensColor, screenWidth, screenHeight, blurWidth, bluredTexture, GhostDispersal, HaloWidth, Distortion, Ghosts,
            lensThreshold, lensEffect, vertBlur, horizBlur, lensModifer, lensSimple;

        private Int32[] weights = new Int32[BLUR_WIDTH], pixOffset = new Int32[BLUR_WIDTH];

        #endregion

        #region Getters

        protected override void getAllUniformLocations()
        {
            base.getAllUniformLocations();

            frameTexture = base.getUniformLocation("frameTexture");
            threshold = base.getUniformLocation("threshold");
            lensColor = base.getUniformLocation("lensColor");
            screenWidth = base.getUniformLocation("screenWidth");
            screenHeight = base.getUniformLocation("screenHeight");
            for (Int32 i = 0; i < BLUR_WIDTH; i++)
            {
                this.weights[i] = base.getUniformLocation("Weight[" + i + "]");
                this.pixOffset[i] = base.getUniformLocation("PixOffset[" + i + "]");
            }
            blurWidth = base.getUniformLocation("blurWidth");
            bluredTexture = base.getUniformLocation("bluredTexture");
            GhostDispersal = base.getUniformLocation("GhostDispersal");
            HaloWidth = base.getUniformLocation("HaloWidth");
            Distortion = base.getUniformLocation("Distortion");
            Ghosts = base.getUniformLocation("Ghosts");
            lensThreshold = base.getSubroutineIndex(ShaderType.FragmentShader, "lensThreshold");
            lensEffect = base.getSubroutineIndex(ShaderType.FragmentShader, "lensEffect");
            vertBlur = base.getSubroutineIndex(ShaderType.FragmentShader, "vertBlur");
            horizBlur = base.getSubroutineIndex(ShaderType.FragmentShader, "horizBlur");
            lensModifer = base.getSubroutineIndex(ShaderType.FragmentShader, "lensModifer");
            lensSimple = base.getSubroutineIndex(ShaderType.FragmentShader, "lensSimple");
        }

        #endregion

        #region Setters

        public void setUniformValuesSimple(Int32 frameTexSampler)
        {
            base.loadInteger(this.frameTexture, frameTexSampler);
            base.loadSubroutineIndex(ShaderType.FragmentShader, 1, this.lensSimple);
        }

        public void setUniformValuesThreshold(Int32 frameTexSampler, float threshold)
        {
            base.loadInteger(this.frameTexture, frameTexSampler);
            base.loadFloat(this.threshold, threshold);
            base.loadSubroutineIndex(ShaderType.FragmentShader, 1, this.lensThreshold);
        }

        public void setUniformValuesLens(Int32 frameTexSampler, Int32 lensColorSampler, Int32 Ghosts,
            float HaloWidth, float Distortion, float GhostDispersal) 
        {
            base.loadInteger(this.frameTexture, frameTexSampler);
            base.loadInteger(this.lensColor, lensColorSampler);
            base.loadInteger(this.Ghosts, Ghosts);
            base.loadFloat(this.HaloWidth, HaloWidth);
            base.loadFloat(this.Distortion, Distortion);
            base.loadFloat(this.GhostDispersal, GhostDispersal);
            base.loadSubroutineIndex(ShaderType.FragmentShader, 1, this.lensEffect);
        }

        public void setUniformValuesVerticalBlur(Int32 frameTexSampler, float[] weights, Int32[] pixOffset,
            Int32 screenWidth, Int32 screenHeight)
        {
            base.loadInteger(this.frameTexture, frameTexSampler);
            base.loadInteger(this.screenWidth, screenWidth);
            base.loadInteger(this.screenHeight, screenHeight);
            for (Int32 i = 0; i < (weights.Length > BLUR_WIDTH ? BLUR_WIDTH : weights.Length); i++)
            {
                base.loadFloat(this.weights[i], weights[i]);
                base.loadInteger(this.pixOffset[i], pixOffset[i]);
            }
            base.loadInteger(this.blurWidth, weights.Length);
            base.loadSubroutineIndex(ShaderType.FragmentShader, 1, this.vertBlur);
        }

        public void setUniformValuesHorizontalBlur(Int32 frameTexSampler, float[] weights, Int32[] pixOffset,
           Point screenRezolution)
        {
            base.loadInteger(this.frameTexture, frameTexSampler);
            base.loadInteger(this.screenWidth, screenRezolution.X);
            base.loadInteger(this.screenHeight, screenRezolution.Y);
            for (Int32 i = 0; i < (weights.Length > BLUR_WIDTH ? BLUR_WIDTH : weights.Length); i++)
            {
                base.loadFloat(this.weights[i], weights[i]);
                base.loadInteger(this.pixOffset[i], pixOffset[i]);
            }
            base.loadInteger(this.blurWidth, weights.Length);
            base.loadSubroutineIndex(ShaderType.FragmentShader, 1, this.horizBlur);
        }

        public void setUniformValuesMod(Int32 bluredTexture)
        {
            base.loadInteger(this.bluredTexture, bluredTexture);
            base.loadSubroutineIndex(ShaderType.FragmentShader, 1, this.lensModifer);
        }

        protected override void SetShaderMacros()
        {
            base.SetShaderMacros();
            SetDefine<Vector3>(ShaderTypeFlag.FragmentShader, "lum", new Vector3(0.2126f, 0.7152f, 0.0722f));
            SetDefine<Int32>(ShaderTypeFlag.FragmentShader, "MAX_BLUR_WIDTH", BLUR_WIDTH);
        }

        #endregion

        #region Constructor

        public LensFlareShader(string VSPath, string FSPath)
            : base(SHADER_NAME, VSPath, FSPath)
        {
        }

        #endregion
    }
}