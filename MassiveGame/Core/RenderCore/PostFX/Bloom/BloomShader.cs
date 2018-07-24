using System;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK;
using ShaderPattern;

namespace MassiveGame.Core.RenderCore.PostFX.Bloom
{
    public class BloomShader<SubsequenceType> : PostProcessShaderBase<SubsequenceType>
        where SubsequenceType : PostProcessSubsequenceType
    {
        #region Definations

        private const string SHADER_NAME = "Bloom shader";

        private const Int32 BLUR_WIDTH = PostProcessBase.MAX_BLUR_WIDTH;

        private Int32 s_VerticalBlur, s_HorizontalBlur, s_ExtractBrightParts, s_EndBloom;

        Uniform u_frameTexture, u_blurTexture, u_screenWidth, u_screenHeight, u_blurWidth, u_bloomThreshold;

        Uniform[] u_weights = new Uniform[BLUR_WIDTH], u_pixOffset = new Uniform[BLUR_WIDTH];

        #endregion

        #region Constructor

        public BloomShader() : base() { }

        public BloomShader(string vsPath, string fsPath)
            : base(SHADER_NAME, vsPath, fsPath)
        {
        }

        #endregion

        #region Getters uniform

        protected override void getAllUniformLocations()
        {
            base.getAllUniformLocations();
            u_frameTexture = GetUniform("frameTexture");
            u_blurTexture = GetUniform("blurTexture");
            u_screenWidth = GetUniform("screenWidth");
            u_screenHeight = GetUniform("screenHeight");
            u_blurWidth = GetUniform("blurWidth");
            u_bloomThreshold = GetUniform("bloomThreshold");
            for (Int32 i = 0; i < BLUR_WIDTH; i++)
            {
                this.u_weights[i] = GetUniform("Weight[" + i + "]");
                this.u_pixOffset[i] = GetUniform("PixOffset[" + i + "]");
            }
            s_VerticalBlur = base.getSubroutineIndex(ShaderType.FragmentShader, "verticalBlur");
            s_HorizontalBlur = base.getSubroutineIndex(ShaderType.FragmentShader, "horizontalBlur");
            s_ExtractBrightParts = base.getSubroutineIndex(ShaderType.FragmentShader, "extractBrightParts");
            s_EndBloom = base.getSubroutineIndex(ShaderType.FragmentShader, "endBloom");
        }

        #endregion

        #region Setters uniform

        public void setVerticalBlurUniforms(Int32 frameTexture, float[] weights, Int32[] pixOffset, Point screenRezolution)
        {
            u_frameTexture.LoadUniform(frameTexture);
            u_screenWidth.LoadUniform(screenRezolution.X);
            u_screenHeight.LoadUniform(screenRezolution.Y);
            for (Int32 i = 0; i < (weights.Length > BLUR_WIDTH ? BLUR_WIDTH : weights.Length); i++)
            {
                u_weights[i].LoadUniform(weights[i]);
                u_pixOffset[i].LoadUniform(pixOffset[i]);
            }
            u_blurWidth.LoadUniform(weights.Length);
            loadSubroutineIndex(ShaderType.FragmentShader, 1, this.s_VerticalBlur);
        }

        public void setHorizontalBlurUniforms(Int32 frameTexture, float[] weights, Int32[] pixOffset, Point screenRezolution)
        {
            u_frameTexture.LoadUniform(frameTexture);
            u_screenWidth.LoadUniform(screenRezolution.X);
            u_screenHeight.LoadUniform(screenRezolution.Y);
            for (Int32 i = 0; i < (weights.Length > BLUR_WIDTH ? BLUR_WIDTH : weights.Length); i++)
            {
                u_weights[i].LoadUniform(weights[i]);
                u_pixOffset[i].LoadUniform(pixOffset[i]);
            }
            u_blurWidth.LoadUniform(weights.Length);
            base.loadSubroutineIndex(ShaderType.FragmentShader, 1, s_HorizontalBlur);
        }
  
        public void setExtractingBrightPixelsUniforms(Int32 frameTexture, float bloomThreshold)
        {
            u_frameTexture.LoadUniform(frameTexture);
            u_bloomThreshold.LoadUniform(bloomThreshold);
            base.loadSubroutineIndex(ShaderType.FragmentShader, 1, s_ExtractBrightParts);
        }

        public void setEndBloomUniforms(Int32 bluredTexture)
        {
            u_blurTexture.LoadUniform(bluredTexture);
            base.loadSubroutineIndex(ShaderType.FragmentShader, 1, s_EndBloom);
        }

        #endregion

        protected override void SetShaderMacros()
        {
            base.SetShaderMacros();
            SetDefine<Vector3>(ShaderTypeFlag.FragmentShader, "lum", new Vector3(0.2126f, 0.7152f, 0.0722f));
            SetDefine<Int32>(ShaderTypeFlag.FragmentShader, "MAX_BLUR_WIDTH", BLUR_WIDTH);
        }
    }
}
