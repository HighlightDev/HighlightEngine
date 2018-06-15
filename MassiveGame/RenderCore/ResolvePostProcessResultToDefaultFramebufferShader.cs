using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ShaderPattern;

namespace MassiveGame.RenderCore
{
    public class ResolvePostProcessResultToDefaultFramebufferShader : ShaderBase
    {
        private const string SHADER_NAME = "ResolvePostProcessResultToDefaultFramebuffer Shader";
        private Int32 frameSampler, postProcessResultSampler;

        protected override void getAllUniformLocations()
        {
            frameSampler = getUniformLocation("frameSampler");
            postProcessResultSampler = getUniformLocation("postProcessResultSampler");
        }

        public void setFrameSampler(Int32 frameSampler)
        {
            loadInteger(this.frameSampler, frameSampler);
        }

        public void setPostProcessResultSampler(Int32 postProcessResultSampler)
        {
            loadInteger(this.postProcessResultSampler, postProcessResultSampler);
        }

        public ResolvePostProcessResultToDefaultFramebufferShader(string VertexShaderFile, string FragmentShaderFile) 
            : base(SHADER_NAME, VertexShaderFile, FragmentShaderFile)
        {
        }

        protected override void SetShaderMacros()
        {
        }
    }
}
