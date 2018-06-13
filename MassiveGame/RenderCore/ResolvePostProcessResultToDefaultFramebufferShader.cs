using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ShaderPattern;

namespace MassiveGame.RenderCore
{
    public class ResolvePostProcessResultToDefaultFramebufferShader : Shader
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

        public ResolvePostProcessResultToDefaultFramebufferShader(string VertexShaderFile, string FragmentShaderFile) : base(VertexShaderFile, FragmentShaderFile)
        {
            if (base.ShaderLoaded)
            {
                base.showCompileLogInfo(SHADER_NAME);
                base.showLinkLogInfo(SHADER_NAME);
                Debug.Log.addToLog(getCompileLogInfo(SHADER_NAME));
                Debug.Log.addToLog(getLinkLogInfo(SHADER_NAME));
            }
            else Debug.Log.addToLog(DateTime.Now.ToString() + "  " + SHADER_NAME + " : shader file(s) not found!");
        }

        protected override void SetShaderMacros()
        {
        }
    }
}
