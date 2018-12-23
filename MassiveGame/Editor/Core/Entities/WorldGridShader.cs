using MassiveGame.Core.RenderCore;
using ShaderPattern;
using OpenTK;
using System;

namespace MassiveGame.EngineEditor.Core.Entities
{
    public class WorldGridShader : ShaderBase
    {
        private const float fadeOutRadius = 10;

        private Uniform u_viewMatrix, u_projectionMatrix, u_gridTexSampler;

        public WorldGridShader() : base() { }

        public WorldGridShader(string vsShader, string fsShader) : base("WorldGridShader", vsShader, fsShader, "") { }

        protected override void SetShaderMacros()
        {
            SetDefine<float>(ShaderTypeFlag.FragmentShader, "alphaRadiusSqr", fadeOutRadius * fadeOutRadius);
        }

        protected override void getAllUniformLocations()
        {
            base.getAllUniformLocations();
            u_viewMatrix = GetUniform("viewMatrix");
            u_projectionMatrix = GetUniform("projectionMatrix");
            u_gridTexSampler = GetUniform("gridSampler");

        }

        public void SetGridTexSampler(Int32 sampler)
        {
            u_gridTexSampler.LoadUniform(sampler);
        }

        public void SetTransformationMatrices(ref Matrix4 viewMatrix, ref Matrix4 projectionMatrix)
        {
            u_viewMatrix.LoadUniform(ref viewMatrix);
            u_projectionMatrix.LoadUniform(ref projectionMatrix);
        }
    }
}
