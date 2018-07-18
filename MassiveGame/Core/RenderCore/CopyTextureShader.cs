using System;

namespace MassiveGame.Core.RenderCore
{
    public class CopyTextureShader : ShaderBase
    {
        const string SHADER_NAME = "CopyTexture Shader";

        private Int32 srcSampler;

        protected override void getAllUniformLocations()
        {
            srcSampler = getUniformLocation("SrcColor");
        }

        public void SetUniformValues(Int32 textureHandler)
        {
            loadInteger(srcSampler, textureHandler);
        }

        protected override void SetShaderMacros()
        {
        }

        public CopyTextureShader() : base() { }

        public CopyTextureShader(string VertexShaderFile, string FragmentShaderFile) 
            : base(SHADER_NAME, VertexShaderFile, FragmentShaderFile)
        {
        }
    }
}
