using MassiveGame.Core.DebugCore;
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
            try
            {
                u_frameSampler = GetUniform("frameSampler");
                u_postProcessResultSampler = GetUniform("postProcessResultSampler");
            }
            catch (ArgumentNullException innerException)
            {
                Log.AddToFileStreamLog(innerException.Message);
                Log.AddToConsoleStreamLog(innerException.Message);
            }
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
