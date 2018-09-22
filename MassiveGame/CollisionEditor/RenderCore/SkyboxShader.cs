using MassiveGame.Core.RenderCore;
using OpenTK;
using ShaderPattern;
using System;

namespace CollisionEditor.RenderCore
{
    public class SkyboxShader : ShaderBase
    {
        private Uniform u_modelMatrix, u_viewMatrix, u_projectionMatrix, u_cubemap;

        public void SetTransformationMatrices(Matrix4 modelMatrix, Matrix4 viewMatrix, Matrix4 projectionMatrix)
        {
            viewMatrix[3, 0] = 0.0f;
            viewMatrix[3, 1] = 0.0f;
            viewMatrix[3, 2] = 0.0f;

            u_modelMatrix.LoadUniform(ref modelMatrix);
            u_viewMatrix.LoadUniform(ref viewMatrix);
            u_projectionMatrix.LoadUniform(ref projectionMatrix);
        }

        public void SetCubemap(Int32 cubemapSampler)
        {
            u_cubemap.LoadUniform(cubemapSampler);
        }

        protected override void getAllUniformLocations()
        {
            base.getAllUniformLocations();
            u_modelMatrix = GetUniform("modelMatrix");
            u_viewMatrix = GetUniform("viewMatrix");
            u_projectionMatrix = GetUniform("projectionMatrix");
            u_cubemap = GetUniform("daySampler");
        }

        protected override void SetShaderMacros()
        {
            SetDefine<int>(ShaderTypeFlag.FragmentShader, "COMPLEX_SKYBOX", 0);
        }

        public SkyboxShader() : base() { }

        public SkyboxShader(string VertexShaderFile, string FragmentShaderFile) : base("SkyboxShader", VertexShaderFile, FragmentShaderFile)
        {
        }
    }
}
