using System;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace MassiveGame.PostFX.DepthOfField
{
    public class DepthOfFieldShader<T> : PostProcessShaderBase<T> where T : PostProcessSubsequenceType
    {
        const string SHADER_NAME = "DepthOfField Shader";

        private const Int32 BLUR_WIDTH = DepthOfFieldPostProcess<T>.MAX_BLUR_WIDTH;
        Int32 blurTexture, depthTexture, frameTexture, screenWidth, screenHeight, blurWidth,
            blurStartEdge, blurEndEdge, subroutineVerticalBlur, subroutineHorizontalBlur, subroutineDepthOfField, subroutineDownsampling;

        Int32[] weights = new Int32[BLUR_WIDTH], pixOffset = new Int32[BLUR_WIDTH];

        #region Getters uniform

        protected override void getAllUniformLocations()
        {
            base.getAllUniformLocations();

            if (PreviousPostProcessResult == PostProcessShaderBase<T>.PostProcessSubsequenceType_Inner.DiscardPreviousPostProcess)
                frameTexture = getUniformLocation("frameTexture");
            
            blurTexture = getUniformLocation("blurTexture");
            depthTexture = getUniformLocation("depthTexture");
            screenWidth = getUniformLocation("screenWidth");
            screenHeight = getUniformLocation("screenHeight");
            blurWidth = getUniformLocation("blurWidth");
            blurStartEdge = getUniformLocation("blurStartEdge");
            blurEndEdge = getUniformLocation("blurEndEdge");
            for (Int32 i = 0; i < BLUR_WIDTH; i++)
            {
                this.weights[i] = getUniformLocation("Weight[" + i + "]");
                this.pixOffset[i] = getUniformLocation("PixOffset[" + i + "]");
            }
            subroutineVerticalBlur = getSubroutineIndex(ShaderType.FragmentShader, "verticalBlur");
            subroutineHorizontalBlur = getSubroutineIndex(ShaderType.FragmentShader, "horizontalBlur");
            subroutineDepthOfField = getSubroutineIndex(ShaderType.FragmentShader, "depthOfField");
            subroutineDownsampling = getSubroutineIndex(ShaderType.FragmentShader, "downsampling");
        }

        #endregion

        #region Setters uniform

        public void setDownsamplerUniforms(Int32 frameTexSampler)
        {
            loadInteger(blurTexture, frameTexSampler);
            loadSubroutineIndex(ShaderType.FragmentShader, 1, subroutineDownsampling);
        }

        /*Blur first pass uniforms*/
        public void setVerticalBlurUniforms(Int32 blurTexSampler, float[] weights, Int32[] pixOffset, Point screenRezolution)
        {
            loadInteger(this.blurTexture, blurTexSampler);
            loadInteger(this.screenWidth, screenRezolution.X);
            loadInteger(this.screenHeight, screenRezolution.Y);
            for (Int32 i = 0; i < (weights.Length > BLUR_WIDTH ? BLUR_WIDTH : weights.Length); i++)
            {
                loadFloat(this.weights[i], weights[i]);
                loadInteger(this.pixOffset[i], pixOffset[i]);
            }
            loadInteger(this.blurWidth, weights.Length);
            loadSubroutineIndex(ShaderType.FragmentShader, 1, this.subroutineVerticalBlur);
        }

        /*Blur second pass uniforms*/
        public void setHorizontalBlurUniforms(Int32 blurTexSampler, float[] weights, Int32[] pixOffset, Point screenRezolution)
        {
            loadInteger(this.blurTexture, blurTexSampler);
            loadInteger(this.screenWidth, screenRezolution.X);
            loadInteger(this.screenHeight, screenRezolution.Y);
            for (Int32 i = 0; i < (weights.Length > BLUR_WIDTH ? BLUR_WIDTH : weights.Length); i++)
            {
                loadFloat(this.weights[i], weights[i]);
                loadInteger(this.pixOffset[i], pixOffset[i]);
            }
            loadInteger(this.blurWidth, weights.Length);
            loadSubroutineIndex(ShaderType.FragmentShader, 1, this.subroutineHorizontalBlur);
        }

        /*Depth of Field pass uniforms*/


        public void SetFrameTextureSampler(Int32 frameTexSampler)
        {
            if (PreviousPostProcessResult == PostProcessShaderBase<T>.PostProcessSubsequenceType_Inner.DiscardPreviousPostProcess)
                loadInteger(this.frameTexture, frameTexSampler);
        }

        public void setDoFUniforms(Int32 blurTexSampler, Int32 depthTexSampler, float blurStartEdge, float blurEndEdge)
        {
            loadInteger(this.blurTexture, blurTexSampler);
            loadInteger(this.depthTexture, depthTexSampler);
            loadFloat(this.blurStartEdge, blurStartEdge);
            loadFloat(this.blurEndEdge, blurEndEdge);
            for (Int32 i = 0; i < (weights.Length > BLUR_WIDTH ? BLUR_WIDTH : weights.Length); i++)
            {
                loadFloat(this.weights[i], weights[i]);
                loadInteger(this.pixOffset[i], pixOffset[i]);
            }
            loadInteger(this.blurWidth, weights.Length);
            loadSubroutineIndex(ShaderType.FragmentShader, 1, this.subroutineDepthOfField);
        }

        #endregion

        protected override void SetShaderMacros()
        {
            base.SetShaderMacros();
            SetDefine<float>(ShaderTypeFlag.FragmentShader, "zNearPlane", DOUEngine.NEAR_CLIPPING_PLANE);
            SetDefine<float>(ShaderTypeFlag.FragmentShader, "zFarPlane", DOUEngine.FAR_CLIPPING_PLANE);
            SetDefine<Int32>(ShaderTypeFlag.FragmentShader, "MAX_BLUR_WIDTH", BLUR_WIDTH);
        }

        public DepthOfFieldShader(string VertexShaderFile, string FragmentShaderFile) 
            : base(SHADER_NAME, VertexShaderFile, FragmentShaderFile)
        {
        }
    }
}
