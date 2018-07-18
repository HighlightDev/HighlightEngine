using OpenTK;
using System;

namespace MassiveGame.Core.RenderCore
{
    public class StencilPassShader : ShaderBase
    {
        private const string SHADER_NAME = "STENCIL PASS";
        private Int32 projectionMatrix, viewMatrix, worldMatrix;

        public StencilPassShader() : base() { }

        public StencilPassShader(string VertexShaderFile, string FragmentShaderFile) 
            : base(SHADER_NAME, VertexShaderFile, FragmentShaderFile)
        {
        }

        protected override void getAllUniformLocations()
        {
            projectionMatrix = base.getUniformLocation("projectionMatrix");
            viewMatrix = base.getUniformLocation("viewMatrix");
            worldMatrix = base.getUniformLocation("worldMatrix");
        }

        public void SetUniformVariables(ref Matrix4 projectionMatrix, Matrix4 viewMatrix, ref Matrix4 worldMatrix)
        {
            loadMatrix(this.projectionMatrix, false, projectionMatrix);
            loadMatrix(this.viewMatrix, false, viewMatrix);
            loadMatrix(this.worldMatrix, false, worldMatrix);
        }

        protected override void SetShaderMacros()
        {  
        }
    }
}
