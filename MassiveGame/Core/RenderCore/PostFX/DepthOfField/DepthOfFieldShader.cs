using System;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using ShaderPattern;

namespace MassiveGame.Core.RenderCore.PostFX.DepthOfField
{
    public class DepthOfFieldShader<SubsequenceType> : PostProcessShaderBase<SubsequenceType> where SubsequenceType : PostProcessSubsequenceType
    {
        const string SHADER_NAME = "DepthOfField Shader";

        private const Int32 BLUR_WIDTH = PostProcessBase.MAX_BLUR_WIDTH;
        Uniform u_blurTexture, u_depthTexture, u_frameTexture, u_screenWidth, u_screenHeight, u_blurWidth,
            u_blurStartEdge, u_blurEndEdge;
        Int32 subroutineVerticalBlur, subroutineHorizontalBlur, subroutineDepthOfField, subroutineDownsampling;

        Uniform[] u_weights = new Uniform[BLUR_WIDTH], u_pixOffset = new Uniform[BLUR_WIDTH];

        #region Getters uniform

        protected override void getAllUniformLocations()
        {
            base.getAllUniformLocations();

            if (PreviousPostProcessResult == PostProcessShaderBase<SubsequenceType>.PostProcessSubsequenceType_Inner.DiscardPreviousPostProcess)
                u_frameTexture = GetUniform("frameTexture");
            
            u_blurTexture = GetUniform("blurTexture");
            u_depthTexture = GetUniform("depthTexture");
            u_screenWidth = GetUniform("screenWidth");
            u_screenHeight = GetUniform("screenHeight");
            u_blurWidth = GetUniform("blurWidth");
            u_blurStartEdge = GetUniform("blurStartEdge");
            u_blurEndEdge = GetUniform("blurEndEdge");
            for (Int32 i = 0; i < BLUR_WIDTH; i++)
            {
                this.u_weights[i] = GetUniform("Weight[" + i + "]");
                this.u_pixOffset[i] = GetUniform("PixOffset[" + i + "]");
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
            u_blurTexture.LoadUniform(frameTexSampler);
            loadSubroutineIndex(ShaderType.FragmentShader, 1, subroutineDownsampling);
        }

        /*Blur first pass uniforms*/
        public void setVerticalBlurUniforms(Int32 blurTexSampler, float[] weights, Int32[] pixOffset, Point screenRezolution)
        {
            u_blurTexture.LoadUniform(blurTexSampler);
            u_screenWidth.LoadUniform(screenRezolution.X);
            u_screenHeight.LoadUniform(screenRezolution.Y);
            for (Int32 i = 0; i < (weights.Length > BLUR_WIDTH ? BLUR_WIDTH : weights.Length); i++)
            {
                u_weights[i].LoadUniform(weights[i]);
                u_pixOffset[i].LoadUniform(pixOffset[i]);
            }
            u_blurWidth.LoadUniform(weights.Length);
            loadSubroutineIndex(ShaderType.FragmentShader, 1, this.subroutineVerticalBlur);
        }

        /*Blur second pass uniforms*/
        public void setHorizontalBlurUniforms(Int32 blurTexSampler, float[] weights, Int32[] pixOffset, Point screenRezolution)
        {
            u_blurTexture.LoadUniform(blurTexSampler);
            u_screenWidth.LoadUniform(screenRezolution.X);
            u_screenHeight.LoadUniform(screenRezolution.Y);
            for (Int32 i = 0; i < (weights.Length > BLUR_WIDTH ? BLUR_WIDTH : weights.Length); i++)
            {
                u_weights[i].LoadUniform(weights[i]);
                u_pixOffset[i].LoadUniform(pixOffset[i]);
            }
            u_blurWidth.LoadUniform(weights.Length);
            loadSubroutineIndex(ShaderType.FragmentShader, 1, this.subroutineHorizontalBlur);
        }

        /*Depth of Field pass uniforms*/


        public void SetFrameTextureSampler(Int32 frameTexSampler)
        {
            if (PreviousPostProcessResult == PostProcessShaderBase<SubsequenceType>.PostProcessSubsequenceType_Inner.DiscardPreviousPostProcess)
                u_frameTexture.LoadUniform(frameTexSampler);
        }

        public void setDoFUniforms(Int32 blurTexSampler, Int32 depthTexSampler, float blurStartEdge, float blurEndEdge)
        {
            u_blurTexture.LoadUniform(blurTexSampler);
            u_depthTexture.LoadUniform(depthTexSampler);
            u_blurStartEdge.LoadUniform(blurStartEdge);
            u_blurEndEdge.LoadUniform(blurEndEdge);
            loadSubroutineIndex(ShaderType.FragmentShader, 1, this.subroutineDepthOfField);
        }

        #endregion

        protected override void SetShaderMacros()
        {
            base.SetShaderMacros();
            SetDefine<float>(ShaderTypeFlag.FragmentShader, "zNearPlane", EngineStatics.NEAR_CLIPPING_PLANE);
            SetDefine<float>(ShaderTypeFlag.FragmentShader, "zFarPlane", EngineStatics.FAR_CLIPPING_PLANE);
            SetDefine<Int32>(ShaderTypeFlag.FragmentShader, "MAX_BLUR_WIDTH", BLUR_WIDTH);
        }

        public DepthOfFieldShader() : base() { }

        public DepthOfFieldShader(string VertexShaderFile, string FragmentShaderFile) 
            : base(SHADER_NAME, VertexShaderFile, FragmentShaderFile)
        {
        }
    }
}
