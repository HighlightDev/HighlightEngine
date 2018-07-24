using System;
using System.Collections.Generic;
using OpenTK;
using MassiveGame.Core.RenderCore;
using MassiveGame.Core.RenderCore.Lights;
using MassiveGame.Core.GameCore.EntityComponents;
using ShaderPattern;

namespace MassiveGame.Core.GameCore.Water
{
    public class WaterShader : ShaderBase
    {
        #region Definitions

        private const string SHADER_NAME = "Water Shader";
        private Uniform u_modelMatrix, u_viewMatrix, u_projectionMatrix,
            u_reflectionTexture, u_refractionTexture, u_dudvTexture,
            u_normalMap, u_depthTexture, u_cameraPosition,
            u_moveFactor, u_waveStrength, u_sunPos,
            u_sunSpecularColour, u_nearClipPlane, u_farClipPlane,
            u_transparencyDepth, u_mistEnable, u_mistDensity,
            u_mistGradient, u_mistColour, u_bEnableSun;

        private Uniform[]
           u_lightPosition = new Uniform[EngineStatics.MAX_LIGHT_COUNT],
           u_attenuation = new Uniform[EngineStatics.MAX_LIGHT_COUNT],
           u_specularColour = new Uniform[EngineStatics.MAX_LIGHT_COUNT],
           u_enableLight = new Uniform[EngineStatics.MAX_LIGHT_COUNT];

        #endregion

        #region Uniforms setter

        public void setTransformationMatrices(ref Matrix4 modelMatrix, Matrix4 viewMatrix, ref Matrix4 projectionMatrix)
        {
            u_modelMatrix.LoadUniform(ref modelMatrix);
            u_viewMatrix.LoadUniform(ref viewMatrix);
            u_projectionMatrix.LoadUniform(ref projectionMatrix);
        }

        public void setReflectionSampler(Int32 reflectionSampler)
        {
            u_reflectionTexture.LoadUniform(reflectionSampler);
        }

        public void setRefractionSampler(Int32 refractionSampler)
        {
            u_refractionTexture.LoadUniform(refractionSampler);
        }

        public void setDepthSampler(Int32 depthSampler)
        {
            u_depthTexture.LoadUniform(depthSampler);
        }

        public void setDuDvSampler(Int32 DuDvSampler)
        {
            u_dudvTexture.LoadUniform(DuDvSampler);
        }

        public void setNormalMapSampler(Int32 normalMapSampler)
        {
            u_normalMap.LoadUniform(normalMapSampler);
        }

        public void setCameraPosition(Vector3 cameraPosition)
        {
            u_cameraPosition.LoadUniform(ref cameraPosition);
        }

        public void setDistortionProperties(float moveFactor, float waveStrength)
        {
            u_moveFactor.LoadUniform(moveFactor);
            u_waveStrength.LoadUniform(waveStrength);
        }

        public void setClippingPlanes(ref float perspectiveClipPlaneNear, ref float perspectiveClipPlaneFar)
        {
            u_nearClipPlane.LoadUniform(perspectiveClipPlaneNear);
            u_farClipPlane.LoadUniform(perspectiveClipPlaneFar);
        }

        public void setTransparancyDepth(float transparencyDepth)
        {
            u_transparencyDepth.LoadUniform(transparencyDepth);
        }

        public void setDirectionalLight(DirectionalLight sun)
        {
            bool bEnableSun = false;
            if (sun != null)
            {
                u_sunPos.LoadUniform(sun.Direction);
                u_sunSpecularColour.LoadUniform(sun.Specular.Xyz);
                bEnableSun = true;
            }

            u_bEnableSun.LoadUniform(bEnableSun);
        }

        public void setMist(MistComponent mistComponent)
        {
            bool bEnableMist = false;
            if (mistComponent != null)
            {
                u_mistDensity.LoadUniform(mistComponent.MistDensity);
                u_mistGradient.LoadUniform(mistComponent.MistGradient);
                u_mistColour.LoadUniform(mistComponent.MistColour);
                bEnableMist = true;
            }

            u_mistEnable.LoadUniform(bEnableMist);
        }

        public void setPointLight(List<PointLight> lights)
        {
            if (lights != null)
            {
                for (Int32 i = 0; i < (lights.Count <= EngineStatics.MAX_LIGHT_COUNT ? lights.Count : EngineStatics.MAX_LIGHT_COUNT); i++)
                {
                    u_enableLight[i].LoadUniform(true);
                    u_lightPosition[i].LoadUniform(lights[i].Position.Xyz);
                    u_attenuation[i].LoadUniform(lights[i].Attenuation);
                    u_specularColour[i].LoadUniform(lights[i].Specular.Xyz);
                }
                for (Int32 i = lights.Count; i < EngineStatics.MAX_LIGHT_COUNT; i++)  
                {
                    u_enableLight[i].LoadUniform(false);
                }
            }
        }

        #endregion

        #region Uniforms getter

        protected override void getAllUniformLocations()
        {
            u_bEnableSun = GetUniform("bEnableSun");
            this.u_modelMatrix = GetUniform("modelMatrix");
            this.u_viewMatrix = GetUniform("viewMatrix");
            this.u_projectionMatrix = GetUniform("projectionMatrix");
            this.u_reflectionTexture = GetUniform("reflectionTexture");
            this.u_refractionTexture = GetUniform("refractionTexture");
            this.u_cameraPosition = GetUniform("cameraPosition");
            this.u_dudvTexture = GetUniform("dudvTexture");
            this.u_normalMap = GetUniform("normalMap");
            this.u_depthTexture = GetUniform("depthTexture");
            this.u_moveFactor = GetUniform("moveFactor");
            this.u_sunPos = GetUniform("sunPos");
            this.u_sunSpecularColour = GetUniform("sunSpecularColour");
            this.u_waveStrength = GetUniform("waveStrength");
            this.u_farClipPlane = GetUniform("farClipPlane");
            this.u_nearClipPlane = GetUniform("nearClipPlane");
            this.u_transparencyDepth = GetUniform("transparencyDepth");

            this.u_mistEnable = GetUniform("mistEnable");
            this.u_mistDensity = GetUniform("mistDensity");
            this.u_mistGradient = GetUniform("mistGradient");
            this.u_mistColour = GetUniform("mistColour");

            for (Int32 i = 0; i < EngineStatics.MAX_LIGHT_COUNT; i++)
            {
                u_lightPosition[i] = GetUniform("lightPosition[" + i + "]");
                u_attenuation[i] = GetUniform("attenuation[" + i + "]");
                u_specularColour[i] = GetUniform("specularColour[" + i + "]");
                u_enableLight[i] = GetUniform("enableLight[" + i + "]");
            }
        }

        #endregion

        protected override void SetShaderMacros()
        {
            SetDefine<Int32>(ShaderTypeFlag.VertexShader | ShaderTypeFlag.FragmentShader, "MAX_LIGHT_COUNT", EngineStatics.MAX_LIGHT_COUNT);
            SetDefine<float>(ShaderTypeFlag.VertexShader, "tiling", 3.0f);
            SetDefine<float>(ShaderTypeFlag.FragmentShader, "materialReflectivity", 1.1f);
            SetDefine<float>(ShaderTypeFlag.FragmentShader, "materialShineDamper", 100.0f);
        }

        #region Constructor

        public WaterShader() : base() { }

        public WaterShader(string vsPath, string fsPath)
            : base(SHADER_NAME, vsPath, fsPath)
        {
        }

        #endregion
    }
}
