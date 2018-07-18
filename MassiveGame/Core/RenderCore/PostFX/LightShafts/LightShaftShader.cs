using System;
using OpenTK;

namespace MassiveGame.Core.RenderCore.PostFX.LightShafts
{
    public class LightShaftShader<SubsequenceType> : PostProcessShaderBase<SubsequenceType>
        where SubsequenceType : PostProcessSubsequenceType
    {
        #region Definitions

        private const string SHADER_NAME = "GodRays Shader";
        private Int32 brightPartsTexture, exposure, decay, weight, radialPosition, density, numSamples;

        #endregion

        #region Getter

        protected override void getAllUniformLocations()
        {
            base.getAllUniformLocations();
            brightPartsTexture = base.getUniformLocation("bluredTexture");
            exposure = base.getUniformLocation("exposure");
            decay = base.getUniformLocation("decay");
            weight = base.getUniformLocation("weight");
            radialPosition = base.getUniformLocation("radialPosition");
            density = base.getUniformLocation("density");
            numSamples = base.getUniformLocation("numSamples");
        }

        #endregion

        #region Setter

        public void setUniformValuesRadialBlur(Int32 frameTexture, Int32 bluredTexture, float exposure,
            float decay, float weigth, float density, Int32 numSamples, Vector2 radialPosition)
        {
            base.loadInteger(this.brightPartsTexture, bluredTexture);
            base.loadFloat(this.exposure, exposure);
            base.loadFloat(this.decay, decay);
            base.loadFloat(this.weight, weight);
            base.loadFloat(this.density, density);
            base.loadInteger(this.numSamples, numSamples);
            base.loadVector(this.radialPosition, radialPosition);
        }

        public void SetBrightPartsTextureSampler(Int32 brightPartsTextureSampler)
        {
            base.loadInteger(this.brightPartsTexture, brightPartsTextureSampler);
        }

        public void SetRadialBlurExposure(float exposure)
        {
            base.loadFloat(this.exposure, exposure);
        }

        public void SetRadialBlurDecay(float decay)
        {
            base.loadFloat(this.decay, decay);
        }

        public void SetRadialBlurNumberOfSamples(Int32 numberOfSamples)
        {
            base.loadInteger(this.numSamples, numberOfSamples);
        }

        public void SetRadialBlurWeight(float weight)
        {
            base.loadFloat(this.weight, weight);
        }

        public void SetRadialBlurDensity(float density)
        {
            base.loadFloat(this.density, density);
        }

        public void SetRadialBlurCenterPositionInScreenSpace(Vector2 radialPosition)
        {
            base.loadVector(this.radialPosition, radialPosition);
        }

        #endregion

        protected override void SetShaderMacros()
        {
            base.SetShaderMacros();
        }

        #region Constructor

        public LightShaftShader() : base() { }

        public LightShaftShader(string VSPath, string FSPath)
            : base(SHADER_NAME, VSPath, FSPath)
        {
           
        }

        #endregion
    }
}
