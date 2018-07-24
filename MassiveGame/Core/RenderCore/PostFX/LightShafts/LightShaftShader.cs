using System;
using OpenTK;
using ShaderPattern;

namespace MassiveGame.Core.RenderCore.PostFX.LightShafts
{
    public class LightShaftShader<SubsequenceType> : PostProcessShaderBase<SubsequenceType>
        where SubsequenceType : PostProcessSubsequenceType
    {
        #region Definitions

        private const string SHADER_NAME = "GodRays Shader";
        private Uniform u_brightPartsTexture, u_exposure, u_decay, u_weight, u_radialPosition, u_density, u_numSamples;

        #endregion

        #region Constructor

        public LightShaftShader() : base() { }

        public LightShaftShader(string VSPath, string FSPath)
            : base(SHADER_NAME, VSPath, FSPath)
        {

        }

        #endregion

        #region Getter

        protected override void getAllUniformLocations()
        {
            base.getAllUniformLocations();
            u_brightPartsTexture = GetUniform("bluredTexture");
            u_exposure = GetUniform("exposure");
            u_decay = GetUniform("decay");
            u_weight = GetUniform("weight");
            u_radialPosition = GetUniform("radialPosition");
            u_density = GetUniform("density");
            u_numSamples = GetUniform("numSamples");
        }

        #endregion

        #region Setter

        public void setUniformValuesRadialBlur(Int32 frameTexture, Int32 bluredTexture, float exposure,
            float decay, float weight, float density, Int32 numSamples, Vector2 radialPosition)
        {
            u_brightPartsTexture.LoadUniform(bluredTexture);
            u_exposure.LoadUniform(exposure);
            u_decay.LoadUniform(decay);
            u_weight.LoadUniform(weight);
            u_density.LoadUniform(density);
            u_numSamples.LoadUniform(numSamples);
            u_radialPosition.LoadUniform(radialPosition);
        }

        public void SetBrightPartsTextureSampler(Int32 brightPartsTextureSampler)
        {
            u_brightPartsTexture.LoadUniform(brightPartsTextureSampler);
        }

        public void SetRadialBlurExposure(float exposure)
        {
            u_exposure.LoadUniform(exposure);
        }

        public void SetRadialBlurDecay(float decay)
        {
            u_decay.LoadUniform(decay);
        }

        public void SetRadialBlurNumberOfSamples(Int32 numberOfSamples)
        {
            u_numSamples.LoadUniform(numberOfSamples);
        }

        public void SetRadialBlurWeight(float weight)
        {
            u_weight.LoadUniform(weight);
        }

        public void SetRadialBlurDensity(float density)
        {
            u_density.LoadUniform(density);
        }

        public void SetRadialBlurCenterPositionInScreenSpace(Vector2 radialPosition)
        {
            u_radialPosition.LoadUniform(radialPosition);
        }

        #endregion

        protected override void SetShaderMacros()
        {
            base.SetShaderMacros();
        }

    }
}
