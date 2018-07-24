using ShaderPattern;
using System;

namespace MassiveGame.Core.RenderCore
{
    public class ResolvePostProcessResultToDefaultFramebufferShader : ShaderBase
    {
        private const string SHADER_NAME = "ResolvePostProcessResultToDefaultFramebuffer Shader";
        private Uniform u_frameSampler, u_postProcessResultSampler;

        protected override void getAllUniformLocations()
        {
            u_frameSampler = GetUniform("frameSampler");
            u_postProcessResultSampler = GetUniform("postProcessResultSampler");
        }

        public void setFrameSampler(Int32 frameSampler)
        {
            u_frameSampler.LoadUniform(frameSampler);
        }

        public void setPostProcessResultSampler(Int32 postProcessResultSampler)
        {
            u_postProcessResultSampler.LoadUniform(postProcessResultSampler);
        }

        public ResolvePostProcessResultToDefaultFramebufferShader() : base() { }

        public ResolvePostProcessResultToDefaultFramebufferShader(string VertexShaderFile, string FragmentShaderFile) 
            : base(SHADER_NAME, VertexShaderFile, FragmentShaderFile)
        {
        }

        protected override void SetShaderMacros() { }
    }
}
