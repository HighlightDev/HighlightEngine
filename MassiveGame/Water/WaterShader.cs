using System;
using System.Collections.Generic;
using ShaderPattern;
using OpenTK;
using MassiveGame.RenderCore.Lights;
using MassiveGame.RenderCore;

namespace MassiveGame
{
    public class WaterShader : ShaderBase
    {
        #region Definitions

        private const string SHADER_NAME = "Water Shader";
        private static Int32 MAX_LIGHTS_COUNT = DOUEngine.MAX_LIGHT_COUNT;
        Int32 modelMatrix, viewMatrix, projectionMatrix, reflectionTexture, refractionTexture,
            dudvTexture, normalMap, depthTexture, cameraPosition, moveFactor, waveStrength, sunPos, sunSpecularColour,
            nearClipPlane, farClipPlane, transparencyDepth, mistEnable, mistDensity, mistGradient, mistColour;

        private Int32[] lightPosition = new Int32[MAX_LIGHTS_COUNT],
           attenuation = new Int32[MAX_LIGHTS_COUNT],
           specularColour = new Int32[MAX_LIGHTS_COUNT],
           enableLight = new Int32[MAX_LIGHTS_COUNT];

        #endregion

        #region Uniforms setter

        public void setTransformationMatrices(ref Matrix4 modelMatrix, Matrix4 viewMatrix, ref Matrix4 projectionMatrix)
        {
            base.loadMatrix(this.modelMatrix, false, modelMatrix);
            base.loadMatrix(this.viewMatrix, false, viewMatrix);
            base.loadMatrix(this.projectionMatrix, false, projectionMatrix);
        }

        public void setReflectionSampler(Int32 reflectionSampler)
        {
            base.loadInteger(this.reflectionTexture, reflectionSampler);
        }

        public void setRefractionSampler(Int32 refractionSampler)
        {
            base.loadInteger(this.refractionTexture, refractionSampler);
        }

        public void setDepthSampler(Int32 depthSampler)
        {
            base.loadInteger(this.depthTexture, depthSampler);
        }

        public void setDuDvSampler(Int32 DuDvSampler)
        {
            base.loadInteger(this.dudvTexture, DuDvSampler);
        }

        public void setNormalMapSampler(Int32 normalMapSampler)
        {
            base.loadInteger(this.normalMap, normalMapSampler);
        }

        public void setCameraPosition(Vector3 cameraPosition)
        {
            base.loadVector(this.cameraPosition, cameraPosition);
        }

        public void setDistortionProperties(float moveFactor, float waveStrength)
        {
            base.loadFloat(this.moveFactor, moveFactor);
            base.loadFloat(this.waveStrength, waveStrength);
        }

        public void setClippingPlanes(ref float nearClipPlane, ref float farClipPlane)
        {
            base.loadFloat(this.nearClipPlane, nearClipPlane);
            base.loadFloat(this.farClipPlane, farClipPlane);
        }

        public void setTransparancyDepth(float transparencyDepth)
        {
            base.loadFloat(this.transparencyDepth, transparencyDepth);
        }

        public void setDirectionalLight(DirectionalLight sun)
        {
            if (sun != null)
            {
                base.loadVector(this.sunPos, sun.Direction);
                base.loadVector(this.sunSpecularColour, sun.Specular.Xyz);
            }
        }

        public void setMist(MistComponent mistComponent)
        {
            if (mistComponent != null)
            {
                base.loadBool(this.mistEnable, true);
                base.loadFloat(this.mistDensity, mistComponent.MistDensity);
                base.loadFloat(this.mistGradient, mistComponent.MistGradient);
                base.loadVector(this.mistColour, mistComponent.MistColour);
            }
            else
                base.loadBool(this.mistEnable, false);
        }

        public void setPointLight(List<PointLight> lights)
        {
            if (lights != null)
            {
                for (Int32 i = 0; i < (lights.Count <= MAX_LIGHTS_COUNT ? lights.Count : MAX_LIGHTS_COUNT); i++)
                {
                    base.loadBool(this.enableLight[i], true);
                    base.loadVector(lightPosition[i], new Vector3(lights[i].Position.X, lights[i].Position.Y, lights[i].Position.Z));
                    base.loadVector(attenuation[i], new Vector3(lights[i].Attenuation.X, lights[i].Attenuation.Y, lights[i].Attenuation.Z));
                    base.loadVector(specularColour[i], new Vector3(lights[i].Specular.X, lights[i].Specular.Y, lights[i].Specular.Z));
                }
                for (Int32 i = lights.Count; i < MAX_LIGHTS_COUNT; i++)  
                {
                    base.loadBool(this.enableLight[i], false);
                }
            }
        }

        #endregion

        #region Uniforms getter

        protected override void getAllUniformLocations()
        {
            this.modelMatrix = base.getUniformLocation("modelMatrix");
            this.viewMatrix = base.getUniformLocation("viewMatrix");
            this.projectionMatrix = base.getUniformLocation("projectionMatrix");
            this.reflectionTexture = base.getUniformLocation("reflectionTexture");
            this.refractionTexture = base.getUniformLocation("refractionTexture");
            this.cameraPosition = base.getUniformLocation("cameraPosition");
            this.dudvTexture = base.getUniformLocation("dudvTexture");
            this.normalMap = base.getUniformLocation("normalMap");
            this.depthTexture = base.getUniformLocation("depthTexture");
            this.moveFactor = base.getUniformLocation("moveFactor");
            this.sunPos = base.getUniformLocation("sunPos");
            this.sunSpecularColour = base.getUniformLocation("sunSpecularColour");
            this.waveStrength = base.getUniformLocation("waveStrength");
            this.farClipPlane = base.getUniformLocation("farClipPlane");
            this.nearClipPlane = base.getUniformLocation("nearClipPlane");
            this.transparencyDepth = base.getUniformLocation("transparencyDepth");

            this.mistEnable = base.getUniformLocation("mistEnable");
            this.mistDensity = base.getUniformLocation("mistDensity");
            this.mistGradient = base.getUniformLocation("mistGradient");
            this.mistColour = base.getUniformLocation("mistColour");

            for (Int32 i = 0; i < MAX_LIGHTS_COUNT; i++)
            {
                lightPosition[i] = base.getUniformLocation("lightPosition[" + i + "]");
                attenuation[i] = base.getUniformLocation("attenuation[" + i + "]");
                specularColour[i] = base.getUniformLocation("specularColour[" + i + "]");
                enableLight[i] = base.getUniformLocation("enableLight[" + i + "]");
            }
        }

        #endregion

        protected override void SetShaderMacros()
        {
            SetDefine<Int32>(ShaderTypeFlag.VertexShader | ShaderTypeFlag.FragmentShader, "MAX_LIGHT_COUNT", DOUEngine.MAX_LIGHT_COUNT);
            SetDefine<float>(ShaderTypeFlag.VertexShader, "tiling", 3.0f);
            SetDefine<float>(ShaderTypeFlag.FragmentShader, "materialReflectivity", 1.1f);
            SetDefine<float>(ShaderTypeFlag.FragmentShader, "materialShineDamper", 100.0f);
        }

        #region Constructor

        public WaterShader(string vsPath, string fsPath)
            : base(SHADER_NAME, vsPath, fsPath)
        {
        }

        #endregion
    }
}
