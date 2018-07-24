using System;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK;
using ShaderPattern;

namespace MassiveGame.Core.RenderCore.PostFX.LensFlare
{
    public class LensFlareShader<SubsequenceType> : PostProcessShaderBase<SubsequenceType> where SubsequenceType: PostProcessSubsequenceType
    {
        #region Difinitions

        private const Int32 BLUR_WIDTH = LensFlarePostProcess<SubsequenceType>.MAX_BLUR_WIDTH;
        private const string SHADER_NAME = "Lens flare shader";

        private Uniform u_frameTexture, u_threshold, u_screenWidth, u_screenHeight, u_blurWidth, u_bluredTexture, u_GhostDispersal, u_HaloWidth, u_Distortion, u_Ghosts;
        private Int32 s_lensThreshold, s_lensEffect, s_vertBlur, s_horizBlur, s_lensModifer, s_lensSimple;

        private Uniform[] u_weights = new Uniform[BLUR_WIDTH], u_pixOffset = new Uniform[BLUR_WIDTH];

        #endregion

        #region Constructor

        public LensFlareShader() : base() { }

        public LensFlareShader(string VSPath, string FSPath)
            : base(SHADER_NAME, VSPath, FSPath)
        {
        }

        #endregion

        #region Getters

        protected override void getAllUniformLocations()
        {
            base.getAllUniformLocations();

            u_frameTexture = GetUniform("frameTexture");
            u_threshold = GetUniform("threshold");
            u_screenWidth = GetUniform("screenWidth");
            u_screenHeight = GetUniform("screenHeight");
            for (Int32 i = 0; i < BLUR_WIDTH; i++)
            {
                this.u_weights[i] = GetUniform("Weight[" + i + "]");
                this.u_pixOffset[i] = GetUniform("PixOffset[" + i + "]");
            }
            u_blurWidth = GetUniform("blurWidth");
            u_bluredTexture = GetUniform("bluredTexture");
            u_GhostDispersal = GetUniform("GhostDispersal");
            u_HaloWidth = GetUniform("HaloWidth");
            u_Distortion = GetUniform("Distortion");
            u_Ghosts = GetUniform("Ghosts");
            s_lensThreshold = base.getSubroutineIndex(ShaderType.FragmentShader, "lensThreshold");
            s_lensEffect = base.getSubroutineIndex(ShaderType.FragmentShader, "lensEffect");
            s_vertBlur = base.getSubroutineIndex(ShaderType.FragmentShader, "vertBlur");
            s_horizBlur = base.getSubroutineIndex(ShaderType.FragmentShader, "horizBlur");
            s_lensModifer = base.getSubroutineIndex(ShaderType.FragmentShader, "lensModifer");
            s_lensSimple = base.getSubroutineIndex(ShaderType.FragmentShader, "lensSimple");
        }

        #endregion

        #region Setters

        public void setUniformValuesSimple(Int32 frameTexSampler)
        {
            u_frameTexture.LoadUniform(frameTexSampler);
            base.loadSubroutineIndex(ShaderType.FragmentShader, 1, this.s_lensSimple);
        }

        public void setUniformValuesThreshold(Int32 frameTexSampler, float threshold)
        {
            u_frameTexture.LoadUniform(frameTexSampler);
            u_threshold.LoadUniform(threshold);
            base.loadSubroutineIndex(ShaderType.FragmentShader, 1, this.s_lensThreshold);
        }

        public void setUniformValuesLens(Int32 frameTexSampler, Int32 Ghosts,
            float HaloWidth, float Distortion, float GhostDispersal) 
        {
            u_frameTexture.LoadUniform(frameTexSampler);
            u_Ghosts.LoadUniform(Ghosts);
            u_HaloWidth.LoadUniform(HaloWidth);
            u_Distortion.LoadUniform(Distortion);
            u_GhostDispersal.LoadUniform(GhostDispersal);
            base.loadSubroutineIndex(ShaderType.FragmentShader, 1, this.s_lensEffect);
        }

        public void setUniformValuesVerticalBlur(Int32 frameTexSampler, float[] weights, Int32[] pixOffset,
            Int32 screenWidth, Int32 screenHeight)
        {
            u_frameTexture.LoadUniform(frameTexSampler);
            u_screenWidth.LoadUniform(screenWidth);
            u_screenHeight.LoadUniform(screenHeight);
            for (Int32 i = 0; i < (weights.Length > BLUR_WIDTH ? BLUR_WIDTH : weights.Length); i++)
            {
                u_weights[i].LoadUniform(weights[i]);
                u_pixOffset[i].LoadUniform(pixOffset[i]);
            }
            u_blurWidth.LoadUniform(weights.Length);
            base.loadSubroutineIndex(ShaderType.FragmentShader, 1, this.s_vertBlur);
        }

        public void setUniformValuesHorizontalBlur(Int32 frameTexSampler, float[] weights, Int32[] pixOffset,
           Point screenRezolution)
        {
            u_frameTexture.LoadUniform(frameTexSampler);
            u_screenWidth.LoadUniform(screenRezolution.X);
            u_screenHeight.LoadUniform(screenRezolution.Y);
            for (Int32 i = 0; i < (weights.Length > BLUR_WIDTH ? BLUR_WIDTH : weights.Length); i++)
            {
                u_weights[i].LoadUniform(weights[i]);
                u_pixOffset[i].LoadUniform(pixOffset[i]);
            }
            u_blurWidth.LoadUniform(weights.Length);
            base.loadSubroutineIndex(ShaderType.FragmentShader, 1, this.s_horizBlur);
        }

        public void setUniformValuesMod(Int32 bluredTexture)
        {
            u_bluredTexture.LoadUniform(bluredTexture);
            base.loadSubroutineIndex(ShaderType.FragmentShader, 1, this.s_lensModifer);
        }

        protected override void SetShaderMacros()
        {
            base.SetShaderMacros();
            SetDefine<Vector3>(ShaderTypeFlag.FragmentShader, "lum", new Vector3(0.2126f, 0.7152f, 0.0722f));
            SetDefine<Int32>(ShaderTypeFlag.FragmentShader, "MAX_BLUR_WIDTH", BLUR_WIDTH);
        }

        #endregion
    }
}
