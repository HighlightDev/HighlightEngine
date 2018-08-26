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
        private Uniform u_viewMatrix, u_projectionMatrix, u_cubemap;

        public void SetTransformationMatrices(Matrix4 viewMatrix, Matrix4 projectionMatrix)
        {
            viewMatrix[3, 0] = 0.0f; 
            viewMatrix[3, 1] = 0.0f; 
            viewMatrix[3, 2] = 0.0f;

            u_viewMatrix.LoadUniform(ref viewMatrix);
            u_projectionMatrix.LoadUniform(ref projectionMatrix);
        }

        public void SetCubemap(Int32 cubemapSampler)
        {
            u_cubemap.LoadUniform(cubemapSampler);
        }

        protected override void getAllUniformLocations()
        {
            u_viewMatrix = GetUniform("viewMatrix");
            u_projectionMatrix = GetUniform("projectionMatrix");
            u_cubemap = GetUniform("cubemap");
        }

        protected override void SetShaderMacros()
        {
            
        }

        public SkyboxShader(string VertexShaderFile, string FragmentShaderFile) : base("SkyboxShader", VertexShaderFile, FragmentShaderFile)
        {
            if (m_shaderLoaded)
            {
                showCompileLogInfo("SkyboxShader");
                showLinkLogInfo("SkyboxShader");
            }
        }
    }
}
