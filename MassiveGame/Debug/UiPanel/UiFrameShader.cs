﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShaderPattern;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using MassiveGame.RenderCore;

namespace MassiveGame.Debug.UiPanel
{
    public class UiFrameShader : ShaderBase
    {
        #region Definitions

        private const string SHADER_NAME = "UiFrame Shader";
        private Int32 uiTexture, screenSpaceMatrix;

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

        #endregion

        #region Geter

        protected override void getAllUniformLocations()
        {
            uiTexture = base.getUniformLocation("uiTexture");
            screenSpaceMatrix = getUniformLocation("screenSpaceMatrix");
        }

        #endregion

        protected override void SetShaderMacros()
        {
        }

        #region Constructor

        public UiFrameShader(string vsPath, string fsPath)
            : base(SHADER_NAME, vsPath, fsPath)
        {
        }

        #endregion
    }
}
