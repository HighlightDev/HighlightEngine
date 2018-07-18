﻿using System;
using OpenTK;
using MassiveGame.Core.RenderCore;

namespace MassiveGame.Debug.UiPanel
{
    public class UiFrameShader : ShaderBase
    {
        #region Definitions

        private const string SHADER_NAME = "UiFrame Shader";
        private Int32 uiTexture, screenSpaceMatrix, bPerspectiveDepthTexture;

        #endregion

        #region Seter

        public void SetUiTextureSampler(Int32 uiTextureSampler)
        {
            base.loadInteger(uiTexture, uiTextureSampler);
        }

        public void SetScreenSpaceMatrix(Matrix4 screenSpaceMatrix)
        {
            loadMatrix(this.screenSpaceMatrix, false, screenSpaceMatrix);
        }

        public void SetIsDepthTexture(bool bPerspectiveDepthTexture)
        {
            loadBool(this.bPerspectiveDepthTexture, bPerspectiveDepthTexture);
        }

        #endregion

        #region Geter

        protected override void getAllUniformLocations()
        {
            uiTexture = base.getUniformLocation("uiTexture");
            screenSpaceMatrix = getUniformLocation("screenSpaceMatrix");
            bPerspectiveDepthTexture = getUniformLocation("bPerspectiveDepthTexture");
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
