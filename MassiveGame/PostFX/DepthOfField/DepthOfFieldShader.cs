using System;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace MassiveGame.PostFX.DepthOfField
{
    public class DepthOfFieldShader<T> : PostProcessShaderBase<T> where T : PostProcessSubsequenceType
    {
        const string SHADER_NAME = "DepthOfField Shader";

        private const Int32 BLUR_WIDTH = DepthOfFieldPostProcess<T>.MAX_BLUR_WIDTH;
        Int32 blurTexture, depthTexture, screenWidth, screenHeight, blurWidth,
            blurStartEdge, blurEndEdge, subroutineVerticalBlur, subroutineHorizontalBlur, subroutineDepthOfField;

        Int32[] weights = new Int32[BLUR_WIDTH], pixOffset = new Int32[BLUR_WIDTH];

        #region Getters uniform

        protected override void getAllUniformLocations()
        {
            base.getAllUniformLocations();
            blurTexture = base.getUniformLocation("blurTexture");
            depthTexture = base.getUniformLocation("depthTexture");
            screenWidth = base.getUniformLocation("screenWidth");
            screenHeight = base.getUniformLocation("screenHeight");
            blurWidth = base.getUniformLocation("blurWidth");
            blurStartEdge = base.getUniformLocation("blurStartEdge");
            blurEndEdge = base.getUniformLocation("blurEndEdge");
            for (Int32 i = 0; i < BLUR_WIDTH; i++)
            {
                this.weights[i] = base.getUniformLocation("Weight[" + i + "]");
                this.pixOffset[i] = base.getUniformLocation("PixOffset[" + i + "]");
            }
            subroutineVerticalBlur = base.getSubroutineIndex(ShaderType.FragmentShader, "verticalBlur");
            subroutineHorizontalBlur = base.getSubroutineIndex(ShaderType.FragmentShader, "horizontalBlur");
            subroutineDepthOfField = base.getSubroutineIndex(ShaderType.FragmentShader, "depthOfField");
        }

        #endregion

        #region Setters uniform

        /*Blur first pass uniforms*/
        public void setVerticalBlurUniforms(Int32 blurTexSampler, float[] weights, Int32[] pixOffset, Point screenRezolution)
        {
            loadInteger(this.blurTexture, blurTexSampler);
            loadInteger(this.screenWidth, screenRezolution.X);
            loadInteger(this.screenHeight, screenRezolution.Y);
            for (Int32 i = 0; i < (weights.Length > BLUR_WIDTH ? BLUR_WIDTH : weights.Length); i++)
            {
                base.loadFloat(this.weights[i], weights[i]);
                base.loadInteger(this.pixOffset[i], pixOffset[i]);
            }
            base.loadInteger(this.blurWidth, weights.Length);
            base.loadSubroutineIndex(ShaderType.FragmentShader, 1, this.subroutineVerticalBlur);
        }

        /*Blur second pass uniforms*/
        public void setHorizontalBlurUniforms(Int32 blurTexSampler, float[] weights, Int32[] pixOffset, Point screenRezolution)
        {
            loadInteger(this.blurTexture, blurTexSampler);
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

        /*Depth of Field pass uniforms*/
        public void setDoFUniforms(Int32 blurTexSampler, Int32 depthTexSampler, float blurStartEdge, float blurEndEdge)
        {
            base.loadInteger(this.blurTexture, blurTexture);
            base.loadInteger(this.depthTexture, depthTexture);
            base.loadFloat(this.blurStartEdge, blurStartEdge);
            base.loadFloat(this.blurEndEdge, blurEndEdge);
            for (Int32 i = 0; i < (weights.Length > BLUR_WIDTH ? BLUR_WIDTH : weights.Length); i++)
            {
                base.loadFloat(this.weights[i], weights[i]);
                base.loadInteger(this.pixOffset[i], pixOffset[i]);
            }
            base.loadInteger(this.blurWidth, weights.Length);
            base.loadSubroutineIndex(ShaderType.FragmentShader, 1, this.subroutineDepthOfField);
        }

        #endregion

        protected override void SetShaderMacros()
        {
            base.SetShaderMacros();
            SetDefine(ShaderTypeFlag.FragmentShader, "MAX_BLUR_WIDTH", BLUR_WIDTH.ToString());
        }

        public DepthOfFieldShader(string VertexShaderFile, string FragmentShaderFile) 
            : base(SHADER_NAME, VertexShaderFile, FragmentShaderFile)
        {
        }
    }
}
