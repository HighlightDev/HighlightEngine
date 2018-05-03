using ShaderPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextureLoader;

namespace MassiveGame.RenderCore
{
    public class ResolveTextureShader : Shader
    {
        const string SHADER_NAME = "ResolveTextureShader";

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

        public ResolveTextureShader(string VertexShaderFile, string FragmentShaderFile) : base(VertexShaderFile, FragmentShaderFile)
        {
            if (base.ShaderLoaded)
            {
                base.showCompileLogInfo(SHADER_NAME);
                base.showLinkLogInfo(SHADER_NAME);
                Debug.Log.addToLog(DateTime.Now.ToString() + "  " + base.getCompileLogInfo(SHADER_NAME));
                Debug.Log.addToLog(DateTime.Now.ToString() + "  " + base.getLinkLogInfo(SHADER_NAME));
            }
            else Debug.Log.addToLog(DateTime.Now.ToString() + "  " + SHADER_NAME + " : shader file(s) not found!");
        }
    }
}
