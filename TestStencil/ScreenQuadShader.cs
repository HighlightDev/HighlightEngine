using ShaderPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestStencil
{
    public class ScreenQuadShader : Shader
    {
        private Int32 screenTexture;

        protected override void getAllUniformLocations()
        {
            screenTexture = getUniformLocation("screenTexture");
        }

        public void SetTexture(Int32 screenTexture)
        {
            loadInteger(this.screenTexture, screenTexture);
        }

        public ScreenQuadShader(string VertexShaderFile, string FragmentShaderFile) : base(VertexShaderFile, FragmentShaderFile)
        {
            if (ShaderLoaded)
            {
                showCompileLogInfo("ScreenQuadShader");
                showLinkLogInfo("ScreenQuadShader");
            }
        }
    }
}
