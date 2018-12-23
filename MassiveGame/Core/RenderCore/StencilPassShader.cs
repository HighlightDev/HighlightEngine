using MassiveGame.Core.DebugCore;
using OpenTK;
using ShaderPattern;
using System;

namespace MassiveGame.Core.RenderCore
{
    public class StencilPassShader : ShaderBase
    {
        private const string SHADER_NAME = "STENCIL PASS";
        private Uniform u_projectionMatrix, u_viewMatrix, u_worldMatrix;

        public StencilPassShader() : base() { }

        public StencilPassShader(string VertexShaderFile, string FragmentShaderFile) 
            : base(SHADER_NAME, VertexShaderFile, FragmentShaderFile)
        {
        }

        protected override void getAllUniformLocations()
        {
            try
            {
                u_projectionMatrix = GetUniform("projectionMatrix");
                u_viewMatrix = GetUniform("viewMatrix");
                u_worldMatrix = GetUniform("worldMatrix");
            }
            catch (ArgumentNullException innerException)
            {
               Log.AddToFileStreamLog(innerException.Message);
               Log.AddToConsoleStreamLog(innerException.Message);
            }
        }

        public void SetUniformVariables(ref Matrix4 projectionMatrix, Matrix4 viewMatrix, ref Matrix4 worldMatrix)
        {
            u_projectionMatrix.LoadUniform(ref projectionMatrix);
            u_viewMatrix.LoadUniform(ref viewMatrix);
            u_worldMatrix.LoadUniform(ref worldMatrix);
        }

        protected override void SetShaderMacros() { }
    }
}
