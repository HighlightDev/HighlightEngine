using System;
using OpenTK;

namespace MassiveGame.Core.RenderCore.Light_visualization
{
    public class PointLightDebugShader : ShaderBase
    {
        #region Definitions 

        private const string SHADER_NAME = "Light Visualization";
        Int32 modelMatrix, viewMatrix, projectionMatrix, lampTexture;

        public PointLightDebugShader(string shaderName, string VertexShaderFile, string FragmentShaderFile, string GeometryShaderFile = "") : base(shaderName, VertexShaderFile, FragmentShaderFile, GeometryShaderFile)
        {
        }

        #endregion

        #region Getters uniform

        protected override void getAllUniformLocations()
        {
            this.modelMatrix = base.getUniformLocation("modelMatrix");
            this.viewMatrix = base.getUniformLocation("viewMatrix");
            this.projectionMatrix = base.getUniformLocation("projectionMatrix");
            this.lampTexture = base.getUniformLocation("lampTexture");
        }

        #endregion

        #region Setters uniform

        public void setUniformValues(Matrix4 modelMatrix, Matrix4 viewMatrix,
            Matrix4 projectionMatrix, Int32 lampTextureSamplerID)
        {
            base.loadMatrix(this.modelMatrix, false, modelMatrix);
            base.loadMatrix(this.viewMatrix, false, viewMatrix);
            base.loadMatrix(this.projectionMatrix, false, projectionMatrix);
            base.loadInteger(this.lampTexture, lampTextureSamplerID);
        }

        public void setLampTexture(Int32 lampTextureSamplerID)
        {
            base.loadInteger(this.lampTexture, lampTextureSamplerID);
        }

        #endregion

        protected override void SetShaderMacros()
        {
            SetDefine<float>(ShaderTypeFlag.GeometryShader, "SIZE", 1.4f);
        }

        #region Constructor

        public PointLightDebugShader(string vsPath, string fsPath, string gsPath)
            : base(SHADER_NAME, vsPath, fsPath, gsPath)
        {
        }

        #endregion
    }
}
