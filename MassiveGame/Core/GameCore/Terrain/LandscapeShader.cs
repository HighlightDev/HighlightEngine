using OpenTK;
using System;
using System.Collections.Generic;
using MassiveGame.Core.RenderCore;
using MassiveGame.Core.RenderCore.Lights;
using MassiveGame.Core.GameCore.EntityComponents;
using ShaderPattern;

namespace MassiveGame.Core.GameCore.Terrain
{
    public class LandscapeShader : ShaderBase
    {
        #region Definitions

        /* Max available count of light sources */
        
        private const string SHADER_NAME = "Terrain Shader";
        private Uniform u_backTexture, u_rTexture, u_gTexture,
            u_bTexture, u_blendMap, u_materialAmbient, u_materialDiffuse,
            u_modelMatrix, u_viewMatrix, u_projectionMatrix, u_sunDirection,
            u_sunAmbientColour, u_sunDiffuseColour, u_sunEnable, u_clipPlane,
            u_mistEnable, u_mistDensity, u_mistGradient, u_mistColour;

        private Uniform[] u_lightPosition = new Uniform[EngineStatics.MAX_LIGHT_COUNT],
            u_attenuation = new Uniform[EngineStatics.MAX_LIGHT_COUNT],
            u_diffuseColour = new Uniform[EngineStatics.MAX_LIGHT_COUNT],
            u_enableLight = new Uniform[EngineStatics.MAX_LIGHT_COUNT];

        /*  Triggers to enable normal mapping for multitextured terrain */
        private Uniform u_enableNMr, u_enableNMg, u_enableNMb, u_enableNMblack;
        /*  Normal map sampler for each rgb component */
        private Uniform u_normalMapR, u_normalMapG, u_normalMapB, u_normalMapBlack;
        private Uniform u_directionalLightShadowMap, u_directionalLightShadowMatrix;
          
        #endregion

        #region Getter

        protected override void getAllUniformLocations()
        {
            try
            {
                u_backTexture = GetUniform("backgroundTexture");
                u_rTexture = GetUniform("rTexture");
                u_gTexture = GetUniform("gTexture");
                u_bTexture = GetUniform("bTexture");
                u_blendMap = GetUniform("blendMap");
                u_materialAmbient = GetUniform("materialAmbient");
                u_materialDiffuse = GetUniform("materialDiffuse");
                u_modelMatrix = GetUniform("ModelMatrix");
                u_viewMatrix = GetUniform("ViewMatrix");
                u_projectionMatrix = GetUniform("ProjectionMatrix");
                u_sunDirection = GetUniform("sunDirection");
                u_sunAmbientColour = GetUniform("sunAmbientColour");
                u_sunDiffuseColour = GetUniform("sunDiffuseColour");
                u_sunEnable = GetUniform("sunEnable");
                for (Int32 i = 0; i < EngineStatics.MAX_LIGHT_COUNT; i++)
                {
                    u_lightPosition[i] = GetUniform("lightPosition[" + i + "]");
                    u_attenuation[i] = GetUniform("attenuation[" + i + "]");
                    u_diffuseColour[i] = GetUniform("diffuseColour[" + i + "]");
                    u_enableLight[i] = GetUniform("enableLight[" + i + "]");
                }
                u_clipPlane = GetUniform("clipPlane");
                u_enableNMr = GetUniform("enableNMr");
                u_enableNMg = GetUniform("enableNMg");
                u_enableNMb = GetUniform("enableNMb");
                u_enableNMblack = GetUniform("enableNMblack");
                u_normalMapR = GetUniform("normalMapR");
                u_normalMapG = GetUniform("normalMapG");
                u_normalMapB = GetUniform("normalMapB");
                u_normalMapBlack = GetUniform("normalMapBlack");
                u_mistEnable = GetUniform("mistEnable");
                u_mistDensity = GetUniform("mistDensity");
                u_mistGradient = GetUniform("mistGradient");
                u_mistColour = GetUniform("mistColour");
                u_directionalLightShadowMap = GetUniform("dirLightShadowMap");
                u_directionalLightShadowMatrix = GetUniform("dirLightShadowMatrix");
            }
            catch (ArgumentNullException innerException)
            {
                Debug.Log.AddToFileStreamLog(innerException.Message);
                Debug.Log.AddToConsoleStreamLog(innerException.Message);
            }
        }

        #endregion

        #region Setter

        public void SetDirectionalLightShadowMap(Int32 shadowMapSampler)
        {
            u_directionalLightShadowMap.LoadUniform(shadowMapSampler);
        }

        public void SetDirectionalLightShadowMatrix(Matrix4 ShadowMatrix)
        {
            u_directionalLightShadowMatrix.LoadUniform(ref ShadowMatrix);
        }

        public void SetBlendMap(Int32 blendMapSampler)
        {
            u_blendMap.LoadUniform(blendMapSampler);
        }

        public void SetTextureR(Int32 textureSamplerR, Int32 normalMapSamplerR, bool bEnableNM)
        {
            u_rTexture.LoadUniform(textureSamplerR);
            if (bEnableNM)
                u_normalMapR.LoadUniform(normalMapSamplerR);
            u_enableNMr.LoadUniform(bEnableNM);
        }

        public void SetTextureG(Int32 textureSamplerG, Int32 normalMapSamplerG, bool bEnableNM)
        {
            u_gTexture.LoadUniform(textureSamplerG);
            if (bEnableNM)
                u_normalMapG.LoadUniform(normalMapSamplerG);
            u_enableNMg.LoadUniform(bEnableNM);
        }

        public void SetTextureB(Int32 textureSamplerB, Int32 normalMapSamplerB, bool bEnableNM)
        {
            u_bTexture.LoadUniform(textureSamplerB);
            if (bEnableNM)
                u_normalMapB.LoadUniform(normalMapSamplerB);
            u_enableNMb.LoadUniform(bEnableNM);
        }

        public void SetTextureBlack(Int32 textureSamplerBlack, Int32 normalMapSamplerBlack, bool bEnableNM)
        {
            u_backTexture.LoadUniform(textureSamplerBlack);
            if (bEnableNM)
                u_normalMapBlack.LoadUniform(normalMapSamplerBlack);
            u_enableNMblack.LoadUniform(bEnableNM);
        }

        public void SetTransformationMatrices(ref Matrix4 ModelMatrix, Matrix4 ViewMatrix, ref Matrix4 ProjectionMatrix)
        {
            u_modelMatrix.LoadUniform(ref ModelMatrix);
            u_viewMatrix.LoadUniform(ref ViewMatrix);
            u_projectionMatrix.LoadUniform(ref ProjectionMatrix);
        }

        public void SetMaterial(Material material)
        {
            u_materialAmbient.LoadUniform(material.Ambient.Xyz);
            u_materialDiffuse.LoadUniform(material.Diffuse.Xyz);
        }

        public void SetDirectionalLight(DirectionalLight Sun)
        {
            /*If sun is enabled*/
            if (Sun != null)
            {
                u_sunEnable.LoadUniform(true);
                u_sunDirection.LoadUniform(Sun.Direction);
                u_sunAmbientColour.LoadUniform(Sun.Ambient.Xyz);
                u_sunDiffuseColour.LoadUniform(Sun.Diffuse.Xyz);
            }
            else { u_sunEnable.LoadUniform(false); }
        }

        public void SetPointLights(List<PointLight> lights)
        {
            /*If point lights are enabled*/
            if (lights != null)
            {
                for (Int32 i = 0; i < (lights.Count <=  EngineStatics.MAX_LIGHT_COUNT ? lights.Count :  EngineStatics.MAX_LIGHT_COUNT); i++)
                {
                    u_enableLight[i].LoadUniform(true);
                    u_lightPosition[i].LoadUniform(lights[i].Position.Xyz);
                    u_attenuation[i].LoadUniform(lights[i].Attenuation);
                    u_diffuseColour[i].LoadUniform(lights[i].Diffuse.Xyz);
                }
                for (Int32 i = lights.Count; i <  EngineStatics.MAX_LIGHT_COUNT; i++)
                {
                    u_enableLight[i].LoadUniform(false);
                }
            }
            else
            {
                for (Int32 i = 0; i <  EngineStatics.MAX_LIGHT_COUNT; i++) 
                {
                    u_enableLight[i].LoadUniform(false);
                }
            }
        }

        public void SetClippingPlane(ref Vector4 clippingPlane)
        {
            u_clipPlane.LoadUniform(ref clippingPlane);
        }

        public void SetMist(MistComponent mist)
        {
            if (mist != null)
            {
                u_mistEnable.LoadUniform(true);
                u_mistGradient.LoadUniform(mist.MistGradient);
                u_mistDensity.LoadUniform(mist.MistDensity);
                u_mistColour.LoadUniform(mist.MistColour);
            }
            else
            {
                u_mistEnable.LoadUniform(false);
            }
        }

        #endregion

        protected override void SetShaderMacros()
        {
            SetDefine<Int32>(ShaderTypeFlag.VertexShader | ShaderTypeFlag.FragmentShader, "LIGHT_COUNT", EngineStatics.MAX_LIGHT_COUNT);
            SetDefine<float>(ShaderTypeFlag.FragmentShader, "MAX_MIST_VISIBLE_AREA", 1.0f);
            SetDefine<float>(ShaderTypeFlag.FragmentShader, "SHADOWMAP_BIAS", 0.005f);
            SetDefine<Int32>(ShaderTypeFlag.FragmentShader, "PCF_SAMPLES", 2);
        }

        #region Constructor

        public LandscapeShader() : base() { }

        public LandscapeShader(string VertexShaderFile, string FragmentShaderFile)
            : base(SHADER_NAME, VertexShaderFile, FragmentShaderFile)
        {
        }

        #endregion

    }
}

