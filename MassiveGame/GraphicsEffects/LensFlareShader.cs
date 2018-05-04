using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShaderPattern;
using GpuGraphics;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace MassiveGame
{
    public class LensFlareShader : Shader
    {
        #region Difinitions

        private const int BLUR_WIDTH = LensFlareRenderer.MAX_BLUR_WIDTH;
        private const string SHADER_NAME = "Lens flare shader";
        private int frameTexture, threshold, lensColor,
             screenWidth, screenHeight, blurWidth, bluredTexture, GhostDispersal, HaloWidth, Distortion, Ghosts;
        private int[] weights = new int[BLUR_WIDTH], pixOffset = new int[BLUR_WIDTH];

        private int lensThreshold, lensEffect, vertBlur, horizBlur, lensModifer, lensSimple;
        #endregion

        #region Getters

        protected override void getAllUniformLocations()
        {
            frameTexture = base.getUniformLocation("frameTexture");
            threshold = base.getUniformLocation("threshold");
            lensColor = base.getUniformLocation("lensColor");
            screenWidth = base.getUniformLocation("screenWidth");
            screenHeight = base.getUniformLocation("screenHeight");
            for (int i = 0; i < BLUR_WIDTH; i++)
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

        public void setUniformValuesSimple(int frameTexSampler)
        {
            base.loadInteger(this.frameTexture, frameTexSampler);
            base.loadSubroutineIndex(ShaderType.FragmentShader, 1, this.lensSimple);
        }

        public void setUniformValuesThreshold(int frameTexSampler, float threshold)
        {
            base.loadInteger(this.frameTexture, frameTexSampler);
            base.loadFloat(this.threshold, threshold);
            base.loadSubroutineIndex(ShaderType.FragmentShader, 1, this.lensThreshold);
        }

        public void setUniformValuesLens(int frameTexSampler, int lensColorSampler, int Ghosts,
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

        public void setUniformValuesVerticalBlur(int frameTexSampler, float[] weights, int[] pixOffset,
            int screenWidth, int screenHeight)
        {
            base.loadInteger(this.frameTexture, frameTexSampler);
            base.loadInteger(this.screenWidth, screenWidth);
            base.loadInteger(this.screenHeight, screenHeight);
            for (int i = 0; i < (weights.Length > BLUR_WIDTH ? BLUR_WIDTH : weights.Length); i++)
            {
                base.loadFloat(this.weights[i], weights[i]);
                base.loadInteger(this.pixOffset[i], pixOffset[i]);
            }
            base.loadInteger(this.blurWidth, weights.Length);
            base.loadSubroutineIndex(ShaderType.FragmentShader, 1, this.vertBlur);
        }

        public void setUniformValuesHorizontalBlur(int frameTexSampler, float[] weights, int[] pixOffset,
           int screenWidth, int screenHeight)
        {
            base.loadInteger(this.frameTexture, frameTexSampler);
            base.loadInteger(this.screenWidth, screenWidth);
            base.loadInteger(this.screenHeight, screenHeight);
            for (int i = 0; i < (weights.Length > BLUR_WIDTH ? BLUR_WIDTH : weights.Length); i++)
            {
                base.loadFloat(this.weights[i], weights[i]);
                base.loadInteger(this.pixOffset[i], pixOffset[i]);
            }
            base.loadInteger(this.blurWidth, weights.Length);
            base.loadSubroutineIndex(ShaderType.FragmentShader, 1, this.horizBlur);
        }

        public void setUniformValuesMod(int frameTexSampler, int bluredTexture)
        {
            base.loadInteger(this.frameTexture, frameTexSampler);
            base.loadInteger(this.bluredTexture, bluredTexture);
            base.loadSubroutineIndex(ShaderType.FragmentShader, 1, this.lensModifer);
        }

        protected override void SetShaderMacros()
        {
            SetDefine(ShaderTypeFlag.FragmentShader, "lum", "vec3(0.2126, 0.7152, 0.0722)");
            SetDefine(ShaderTypeFlag.FragmentShader, "MAX_BLUR_WIDTH", "30");
        }

        #endregion

        #region Constructor

        public LensFlareShader(string VSPath, string FSPath)
            : base(VSPath, FSPath)
        {
            if (base.ShaderLoaded)
            {
                base.showCompileLogInfo(SHADER_NAME);
                base.showLinkLogInfo(SHADER_NAME);
                Debug.Log.addToLog(DateTime.Now.ToString() + "  " + base.getCompileLogInfo(SHADER_NAME));
                Debug.Log.addToLog(DateTime.Now.ToString() + "  " + base.getLinkLogInfo(SHADER_NAME));
            }
            else Debug.Log.addToLog(DateTime.Now.ToString() + "  " + SHADER_NAME + " : shader file(s) not found!");
        }

        #endregion
    }
}
