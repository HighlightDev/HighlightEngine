using MassiveGame.Core.RenderCore;
using OpenTK;
using ShaderPattern;
using System;

namespace MassiveGame.EngineEditor.Core.Entities
{
    public class PreviewEntityShader : ShaderBase
    {
        private Uniform u_worldMatrix, u_viewMatrix, u_projectionMatrix, u_diffuseTexture;
        public PreviewEntityShader() : base() { }

        public PreviewEntityShader(string vsPath, string fsPath) : base("PreviewEntityShader", vsPath, fsPath)
        {
        }

        protected override void getAllUniformLocations()
        {
            base.getAllUniformLocations();
            u_worldMatrix = GetUniform("worldMatrix");
            u_viewMatrix = GetUniform("viewMatrix");
            u_projectionMatrix = GetUniform("projectionMatrix");
            u_diffuseTexture = GetUniform("diffuseTexture");
        }

        public void SetTransformationMatrices(ref Matrix4 worldMatrix, ref Matrix4 viewMatrix, ref Matrix4 projectionMatrix)
        {
            u_worldMatrix.LoadUniform(ref worldMatrix);
            u_viewMatrix.LoadUniform(ref viewMatrix);
            u_projectionMatrix.LoadUniform(ref projectionMatrix);
        }

        public void SetDiffuseTexSampler(Int32 sampler)
        {
            u_diffuseTexture.LoadUniform(sampler);
        }

        protected override void SetShaderMacros() { }
    }
}
