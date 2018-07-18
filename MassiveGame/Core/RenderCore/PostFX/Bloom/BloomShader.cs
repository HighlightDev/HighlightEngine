using System;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenTK;

namespace MassiveGame.Core.RenderCore.PostFX.Bloom
{
    public class BloomShader<SubsequenceType> : PostProcessShaderBase<SubsequenceType>
        where SubsequenceType : PostProcessSubsequenceType
    {
        #region Definations

        private const string SHADER_NAME = "Bloom shader";

        private const Int32 BLUR_WIDTH = BloomPostProcess<SubsequenceType>.MAX_BLUR_WIDTH;
        Int32 frameTexture, blurTexture, screenWidth, screenHeight, blurWidth,
            bloomThreshold, subroutineVerticalBlur, subroutineHorizontalBlur, subroutineExtractBrightParts, subroutineEndBloom;

        Int32[] weights = new Int32[BLUR_WIDTH], pixOffset = new Int32[BLUR_WIDTH];

        #endregion

        #region Getters uniform

        protected override void getAllUniformLocations()
        {
            base.getAllUniformLocations();
            frameTexture = base.getUniformLocation("frameTexture");
            blurTexture = base.getUniformLocation("blurTexture");
            screenWidth = base.getUniformLocation("screenWidth");
            screenHeight = base.getUniformLocation("screenHeight");
            blurWidth = base.getUniformLocation("blurWidth");
            bloomThreshold = base.getUniformLocation("bloomThreshold");
            for (Int32 i = 0; i < BLUR_WIDTH; i++)
            {
                this.weights[i] = base.getUniformLocation("Weight[" + i + "]");
                this.pixOffset[i] = base.getUniformLocation("PixOffset[" + i + "]");
            }
            subroutineVerticalBlur = base.getSubroutineIndex(ShaderType.FragmentShader, "verticalBlur");
            subroutineHorizontalBlur = base.getSubroutineIndex(ShaderType.FragmentShader, "horizontalBlur");
            subroutineExtractBrightParts = base.getSubroutineIndex(ShaderType.FragmentShader, "extractBrightParts");
            subroutineEndBloom = base.getSubroutineIndex(ShaderType.FragmentShader, "endBloom");
        }

        #endregion

        #region Setters uniform

        public void setVerticalBlurUniforms(Int32 frameTexture, float[] weights, Int32[] pixOffset, Point screenRezolution)
        {
            base.loadInteger(this.frameTexture, frameTexture);
            base.loadInteger(this.screenWidth, screenRezolution.X);
            base.loadInteger(this.screenHeight, screenRezolution.Y);
            for (Int32 i = 0; i < (weights.Length > BLUR_WIDTH ? BLUR_WIDTH : weights.Length); i++)
            {
                base.loadFloat(this.weights[i], weights[i]);
                base.loadInteger(this.pixOffset[i], pixOffset[i]);
            }
            base.loadInteger(this.blurWidth, weights.Length);
            base.loadSubroutineIndex(ShaderType.FragmentShader, 1, this.subroutineVerticalBlur);
        }

        public void setHorizontalBlurUniforms(Int32 frameTexture, float[] weights, Int32[] pixOffset, Point screenRezolution)
        {
            base.loadInteger(this.frameTexture, frameTexture);
            base.loadInteger(this.screenWidth, screenRezolution.X);
            base.loadInteger(this.screenHeight, screenRezolution.Y);
            for (Int32 i = 0; i < (weights.Length > BLUR_WIDTH ? BLUR_WIDTH : weights.Length); i++)
            {
                base.loadFloat(this.weights[i], weights[i]);
                base.loadInteger(this.pixOffset[i], pixOffset[i]);
            }
            base.loadInteger(this.blurWidth, weights.Length);
            base.loadSubroutineIndex(ShaderType.FragmentShader, 1, this.subroutineHorizontalBlur);
        }
  
        public void setExtractingBrightPixelsUniforms(Int32 frameTexture, float bloomThreshold)
        {
            base.loadInteger(this.frameTexture, frameTexture);
            base.loadFloat(this.bloomThreshold, bloomThreshold);
            base.loadSubroutineIndex(ShaderType.FragmentShader, 1, this.subroutineExtractBrightParts);
        }

        public void setEndBloomUniforms(Int32 bluredTexture)
        {
            base.loadInteger(this.blurTexture, bluredTexture);
            base.loadSubroutineIndex(ShaderType.FragmentShader, 1, this.subroutineEndBloom);
        }

        #endregion

        protected override void SetShaderMacros()
        {
            base.SetShaderMacros();
            SetDefine<Vector3>(ShaderTypeFlag.FragmentShader, "lum", new Vector3(0.2126f, 0.7152f, 0.0722f));
            SetDefine<Int32>(ShaderTypeFlag.FragmentShader, "MAX_BLUR_WIDTH", BLUR_WIDTH);
        }

        #region Constructor
        
        public BloomShader() : base() { }

        public BloomShader(string vsPath, string fsPath)
            : base(SHADER_NAME, vsPath, fsPath)
        {
        }

        #endregion
    }
}
