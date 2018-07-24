using System;
using OpenTK;
using MassiveGame.Core.RenderCore;
using ShaderPattern;

namespace MassiveGame.Debug.UiPanel
{
    public class UiFrameShader : ShaderBase
    {
        #region Definitions

        private const string SHADER_NAME = "UiFrame Shader";
        private Uniform u_uiTexture, u_screenSpaceMatrix, u_bPerspectiveDepthTexture;

        #endregion

        #region Seter

        public void SetUiTextureSampler(Int32 uiTextureSampler)
        {
            u_uiTexture.LoadUniform(uiTextureSampler);
        }

        public void SetScreenSpaceMatrix(Matrix4 screenSpaceMatrix)
        {
            u_screenSpaceMatrix.LoadUniform(ref screenSpaceMatrix);
        }

        public void SetIsDepthTexture(bool bPerspectiveDepthTexture)
        {
            u_bPerspectiveDepthTexture.LoadUniform(bPerspectiveDepthTexture);
        }

        #endregion

        #region Geter

        protected override void getAllUniformLocations()
        {
            u_uiTexture = GetUniform("uiTexture");
            u_screenSpaceMatrix = GetUniform("screenSpaceMatrix");
            u_bPerspectiveDepthTexture = GetUniform("bPerspectiveDepthTexture");
        }

        #endregion

        protected override void SetShaderMacros()
        {
            SetDefine<float>(ShaderTypeFlag.FragmentShader, "zNearPlane", EngineStatics.NEAR_CLIPPING_PLANE);
            SetDefine<float>(ShaderTypeFlag.FragmentShader, "zFarPlane", EngineStatics.FAR_CLIPPING_PLANE);
        }

        #region Constructor

        public UiFrameShader() : base() { }

        public UiFrameShader(string vsPath, string fsPath)
            : base(SHADER_NAME, vsPath, fsPath)
        {
        }

        #endregion
    }
}
