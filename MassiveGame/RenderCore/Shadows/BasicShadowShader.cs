using OpenTK;
using ShaderPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassiveGame.RenderCore.Shadows
{
    public class BasicShadowShader : ShaderBase
    {
        private Int32 worldMatrix, shadowViewMatrix, shadowProjectionMatrix;
        private const string SHADER_NAME = "BasicShadowShader";

        protected override void SetShaderMacros()
        {
        }

        protected override void getAllUniformLocations()
        {
            worldMatrix = getUniformLocation("worldMatrix");
            shadowViewMatrix = getUniformLocation("shadowViewMatrix");
            shadowProjectionMatrix = getUniformLocation("shadowProjectionMatrix");
        }

        public void SetUniformValues(Matrix4 worldMatrix, Matrix4 shadowViewMatrix, Matrix4 shadowProjectionMatrix)
        {
            loadMatrix(this.worldMatrix, false, worldMatrix);
            loadMatrix(this.shadowViewMatrix, false, shadowViewMatrix);
            loadMatrix(this.shadowProjectionMatrix, false, shadowProjectionMatrix);
        }

        public BasicShadowShader(string VertexShaderFile, string FragmentShaderFile) 
            : base(SHADER_NAME, VertexShaderFile, FragmentShaderFile)
        {
        }
    }
}
