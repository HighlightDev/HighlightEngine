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
        private Uniform u_uiTexture, u_screenSpaceMatrix, u_bPerspectiveDepthTexture, u_bSeparated;

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
   
        public void SetIsSeparatedScreen(bool bSeparatedScreen)
        {
            u_bSeparated.LoadUniform(bSeparatedScreen);
        }

        #endregion

        #region Geter

        protected override void getAllUniformLocations()
        {
            try
            {
                u_uiTexture = GetUniform("uiTexture");
                u_screenSpaceMatrix = GetUniform("screenSpaceMatrix");
                u_bPerspectiveDepthTexture = GetUniform("bPerspectiveDepthTexture");
                u_bSeparated = GetUniform("bSeparated");
            }
            catch (ArgumentNullException innerException)
            {
                Debug.Log.AddToFileStreamLog(innerException.Message);
                Debug.Log.AddToConsoleStreamLog(innerException.Message);
            }
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
