using OpenTK;
using ShaderPattern;
using System;

namespace MassiveGame.Core.RenderCore.Shadows
{
    public class BasicShadowShader : ShaderBase
    {
        private Uniform u_worldMatrix, u_shadowViewMatrix, u_shadowProjectionMatrix;
        private const string SHADER_NAME = "BasicShadowShader";

        public BasicShadowShader() : base() { }

        public BasicShadowShader(string VertexShaderFile, string FragmentShaderFile)
            : base(SHADER_NAME, VertexShaderFile, FragmentShaderFile)
        {
        }

        protected override void SetShaderMacros() { }

        protected override void getAllUniformLocations()
        {
            try
            {
                u_worldMatrix = GetUniform("worldMatrix");
                u_shadowViewMatrix = GetUniform("shadowViewMatrix");
                u_shadowProjectionMatrix = GetUniform("shadowProjectionMatrix");
            }
            catch (ArgumentNullException innerException)
            {
                Debug.Log.AddToFileStreamLog(innerException.Message);
                Debug.Log.AddToConsoleStreamLog(innerException.Message);
            }
        }

        public void SetUniformValues(Matrix4 worldMatrix, Matrix4 shadowViewMatrix, Matrix4 shadowProjectionMatrix)
        {
            u_worldMatrix.LoadUniform(ref worldMatrix);
            u_shadowViewMatrix.LoadUniform(ref shadowViewMatrix);
            u_shadowProjectionMatrix.LoadUniform(ref shadowProjectionMatrix);
        }
        
    }
}
