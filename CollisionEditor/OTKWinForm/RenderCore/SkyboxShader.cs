using OpenTK;
using ShaderPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTKWinForm.RenderCore
{
    public class SkyboxShader : Shader
    {
        private Int32 viewMatrix, projectionMatrix, cubemap;

        public void SetTransformationMatrices(Matrix4 viewMatrix, Matrix4 projectionMatrix)
        {
            viewMatrix[3, 0] = 0.0f; 
            viewMatrix[3, 1] = 0.0f; 
            viewMatrix[3, 2] = 0.0f; 

            loadMatrix(this.viewMatrix, false, viewMatrix);
            loadMatrix(this.projectionMatrix, false, projectionMatrix);
        }

        public void SetCubemap(Int32 cubemapSampler)
        {
            loadInteger(this.cubemap, cubemapSampler);
        }

        protected override void getAllUniformLocations()
        {
            viewMatrix = getUniformLocation("viewMatrix");
            projectionMatrix = getUniformLocation("projectionMatrix");
            cubemap = getUniformLocation("cubemap");
        }

        protected override void SetShaderMacros()
        {
            
        }

        public SkyboxShader(string VertexShaderFile, string FragmentShaderFile) : base(VertexShaderFile, FragmentShaderFile)
        {
            if (ShaderLoaded)
            {
                showCompileLogInfo("SkyboxShader");
                showLinkLogInfo("SkyboxShader");
            }
        }
    }
}
