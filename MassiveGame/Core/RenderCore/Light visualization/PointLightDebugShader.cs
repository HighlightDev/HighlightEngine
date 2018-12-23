using System;
using MassiveGame.Core.DebugCore;
using OpenTK;
using ShaderPattern;

namespace MassiveGame.Core.RenderCore.Light_visualization
{
    public class PointLightDebugShader : ShaderBase
    {
        #region Definitions 

        private const string SHADER_NAME = "Light Visualization";
        private Uniform u_modelMatrix, u_viewMatrix, u_projectionMatrix, u_lampTexture;

        #endregion

        #region Constructor

        public PointLightDebugShader() : base() { }

        public PointLightDebugShader(string vsPath, string fsPath, string gsPath)
            : base(SHADER_NAME, vsPath, fsPath, gsPath)
        {
        }

        #endregion

        #region Getters uniform

        protected override void getAllUniformLocations()
        {
            try
            {
                u_modelMatrix = GetUniform("modelMatrix");
                u_viewMatrix = GetUniform("viewMatrix");
                u_projectionMatrix = GetUniform("projectionMatrix");
                u_lampTexture = GetUniform("lampTexture");
            }
            catch (ArgumentNullException innerException)
            {
                Log.AddToFileStreamLog(innerException.Message);
                Log.AddToConsoleStreamLog(innerException.Message);
            }
        }

        #endregion

        #region Setters uniform

        public void setUniformValues(Matrix4 modelMatrix, Matrix4 viewMatrix,
            Matrix4 projectionMatrix, Int32 lampTextureSamplerID)
        {
            u_modelMatrix.LoadUniform(ref modelMatrix);
            u_viewMatrix.LoadUniform(ref viewMatrix);
            u_projectionMatrix.LoadUniform(ref projectionMatrix);
            u_lampTexture.LoadUniform(lampTextureSamplerID);
        }

        public void setLampTexture(Int32 lampTextureSamplerID)
        {
            u_lampTexture.LoadUniform(lampTextureSamplerID);
        }

        #endregion

        protected override void SetShaderMacros()
        {
            SetDefine<float>(ShaderTypeFlag.GeometryShader, "SIZE", 1.4f);
        }

    }
}
