﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShaderPattern;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace MassiveGame.Debug.UiPanel
{
    public class UiFrameShader : Shader
    {
        #region Definitions

        private const string ShaderName = "UiFrame Shader";
        private int uiTexture, frameTexture;

        #endregion

        #region Seter

        public void SetUiTextureSampler(int uiTextureSampler)
        {
            base.loadInteger(uiTexture, uiTextureSampler);
        }

        public void SetFrameTextureSampler(int frameTextureSampler)
        {
            base.loadInteger(frameTexture, frameTextureSampler);
        }

        #endregion

        #region Geter

        protected override void getAllUniformLocations()
        {
            uiTexture = base.getUniformLocation("uiTexture");
            frameTexture = base.getUniformLocation("frameTexture");
        }

        #endregion

        #region Constructor

        public UiFrameShader(string vsPath, string fsPath)
            : base(vsPath, fsPath)
        {
            if (base.ShaderLoaded)
            {
                base.showCompileLogInfo(ShaderName);
                base.showLinkLogInfo(ShaderName);
                Debug.Log.addToLog(base.getCompileLogInfo(ShaderName));
                Debug.Log.addToLog(base.getLinkLogInfo(ShaderName));
            }
        }

        #endregion
    }
}