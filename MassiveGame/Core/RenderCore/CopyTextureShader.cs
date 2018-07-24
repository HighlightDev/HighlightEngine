using ShaderPattern;
using System;

namespace MassiveGame.Core.RenderCore
{
    public class CopyTextureShader : ShaderBase
    {
        const string SHADER_NAME = "CopyTexture Shader";

        private Uniform srcSampler;

        public CopyTextureShader() : base() { }

        public CopyTextureShader(string VertexShaderFile, string FragmentShaderFile)
            : base(SHADER_NAME, VertexShaderFile, FragmentShaderFile)
        {
        }

        protected override void getAllUniformLocations()
        {
            srcSampler = GetUniform("SrcColor");
        }

        public void SetUniformValues(Int32 textureHandler)
        {
            srcSampler.LoadUniform(textureHandler);
        }

        protected override void SetShaderMacros()
        {
        }
     
    }
}
