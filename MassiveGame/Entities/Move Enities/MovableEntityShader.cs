﻿using OpenTK;
using ShaderPattern;
using System;
using System.Collections.Generic;
using MassiveGame.RenderCore.Lights;
using MassiveGame.RenderCore.Shadows;
using MassiveGame.RenderCore;

namespace MassiveGame
{
    public class MovableEntityShader : ShaderBase
    {
        #region Definitions 

        private const string SHADER_NAME = "MotionEntity Shader";
        private static Int32 MAX_LIGHTS_COUNT = EngineStatics.MAX_LIGHT_COUNT; //Максимальное количество источников света, доступных для обработки
        private Int32 entityTexture,
            entityNormalMap,
            normalMapEnDis,
            materialAmbient,
            materialDiffuse,
            materialSpecular,
            materialReflectivity,
            meterialShineDamper,
            ModelMatrix,
            ViewMatrix,
            ProjectionMatrix,
            sunEnable,
            sunDirection,
            sunAmbientColour,
            sunDiffuseColour,
            sunSpecularColour,
            clipPlane,
            mistEnable,
            mistDensity,
            mistGradient,
            mistColour,
            directionalLightShadowMap,
            directionalLightShadowMatrix;
        private Int32[] lightPosition = new Int32[MAX_LIGHTS_COUNT],
            attenuation = new Int32[MAX_LIGHTS_COUNT],
            diffuseColour = new Int32[MAX_LIGHTS_COUNT],
            specularColour = new Int32[MAX_LIGHTS_COUNT],
            enableLight = new Int32[MAX_LIGHTS_COUNT];

        #endregion

        #region Getter

        protected override void getAllUniformLocations()
        {
            entityTexture = base.getUniformLocation("entitieTexture");
            entityNormalMap = base.getUniformLocation("normalMap");
            normalMapEnDis = base.getUniformLocation("normalMapEnableDisable");
            materialAmbient = base.getUniformLocation("materialAmbient");
            materialDiffuse = base.getUniformLocation("materialDiffuse");
            materialSpecular = base.getUniformLocation("materialSpecular");
            materialReflectivity = base.getUniformLocation("materialReflectivity");
            meterialShineDamper = base.getUniformLocation("meterialShineDamper");
            ModelMatrix = base.getUniformLocation("ModelMatrix");
            ViewMatrix = base.getUniformLocation("ViewMatrix");
            ProjectionMatrix = base.getUniformLocation("ProjectionMatrix");
            sunDirection = base.getUniformLocation("sunDirection");
            sunAmbientColour = base.getUniformLocation("sunAmbientColour");
            sunDiffuseColour = base.getUniformLocation("sunDiffuseColour");
            sunSpecularColour = base.getUniformLocation("sunSpecularColour");
            sunEnable = base.getUniformLocation("sunEnable");
            mistEnable = base.getUniformLocation("mistEnable");
            mistDensity = base.getUniformLocation("mistDensity");
            mistGradient = base.getUniformLocation("mistGradient");
            mistColour = base.getUniformLocation("mistColour");
            for (Int32 i = 0; i < MAX_LIGHTS_COUNT; i++)
            {
                lightPosition[i] = base.getUniformLocation("lightPosition[" + i + "]");
                attenuation[i] = base.getUniformLocation("attenuation[" + i + "]");
                diffuseColour[i] = base.getUniformLocation("diffuseColour[" + i + "]");
                specularColour[i] = base.getUniformLocation("specularColour[" + i + "]");
                enableLight[i] = base.getUniformLocation("enableLight[" + i + "]");
            }
            clipPlane = base.getUniformLocation("clipPlane");
            directionalLightShadowMap = base.getUniformLocation("dirLightShadowMap");
            directionalLightShadowMatrix = base.getUniformLocation("dirLightShadowMatrix");
        }

        #endregion

        #region Setter

        public void SetDiffuseMap(Int32 diffuseMapSampler)
        {
            base.loadInteger(entityTexture, diffuseMapSampler);
        }

        public void SetNormalMap(Int32 normalMapSampler, bool bEnableNormalMap)
        {
            if (bEnableNormalMap)
                base.loadInteger(this.entityNormalMap, normalMapSampler);
            base.loadBool(this.normalMapEnDis, bEnableNormalMap);
        }

        public void SetMaterial(Material material)
        {
            base.loadVector(this.materialAmbient, material.Ambient.Xyz);
            base.loadVector(this.materialDiffuse, material.Diffuse.Xyz);
            base.loadVector(this.materialSpecular, material.Specular.Xyz);
            base.loadFloat(this.materialReflectivity, material.Reflectivity);
            base.loadFloat(this.meterialShineDamper, material.ShineDamper);
        }

        public void SetDirectionalLight(DirectionalLight directionalLight)
        {
            if (directionalLight != null)
            {
                base.loadBool(this.sunEnable, true);
                base.loadVector(this.sunDirection, directionalLight.Direction);
                base.loadVector(this.sunAmbientColour, new Vector3(directionalLight.Ambient));
                base.loadVector(this.sunDiffuseColour, new Vector3(directionalLight.Diffuse));
                base.loadVector(this.sunSpecularColour, new Vector3(directionalLight.Specular));
            }
            else { base.loadBool(this.sunEnable, false); }
        }

        public void SetTransformationMatrices(ref Matrix4 WorldMatrix, Matrix4 ViewMatrix, ref Matrix4 ProjectionMatrix)
        {
            base.loadMatrix(this.ModelMatrix, false, WorldMatrix);
            base.loadMatrix(this.ViewMatrix, false, ViewMatrix);
            base.loadMatrix(this.ProjectionMatrix, false, ProjectionMatrix);
        }

        public void SetPointLights(List<PointLight> lights)
        {
            /*If point lights are enabled*/
            if (lights != null)
            {
                for (Int32 i = 0; i < (lights.Count <= MAX_LIGHTS_COUNT ? lights.Count : MAX_LIGHTS_COUNT); i++) //Включенные источники света
                {
                    base.loadBool(this.enableLight[i], true);
                    base.loadVector(lightPosition[i], new Vector3(lights[i].Position.X, lights[i].Position.Y, lights[i].Position.Z));
                    base.loadVector(attenuation[i], new Vector3(lights[i].Attenuation.X, lights[i].Attenuation.Y, lights[i].Attenuation.Z));
                    base.loadVector(diffuseColour[i], new Vector3(lights[i].Diffuse.X, lights[i].Diffuse.Y, lights[i].Diffuse.Z));
                    base.loadVector(specularColour[i], new Vector3(lights[i].Specular.X, lights[i].Specular.Y, lights[i].Specular.Z));
                }
                for (Int32 i = lights.Count; i < MAX_LIGHTS_COUNT; i++)      //Выключенные источники света
                {
                    base.loadBool(this.enableLight[i], false);
                }
            }
            else
            {
                for (Int32 i = 0; i < MAX_LIGHTS_COUNT; i++)      //Выключенные источники света
                {
                    base.loadBool(this.enableLight[i], false);
                }
            }
        }

        public void SetMist(MistComponent mist)
        {
            if (mist != null)
            {
                base.loadBool(this.mistEnable, true);
                base.loadFloat(this.mistGradient, mist.MistGradient);
                base.loadFloat(this.mistDensity, mist.MistDensity);
                base.loadVector(this.mistColour, mist.MistColour);
            }
            else
            {
                base.loadBool(this.mistEnable, false);
            }
        }

        public void SetClippingPlane(ref Vector4 clipPlane)
        {
            base.loadVector(this.clipPlane, clipPlane);
        }

        public void SetDirectionalLightShadowMap(Int32 shadowMapSampler)
        {
            base.loadInteger(this.directionalLightShadowMap, shadowMapSampler);
        }

        public void SetDirectionalLightShadowMatrix(Matrix4 ShadowMatrix)
        {
            base.loadMatrix(directionalLightShadowMatrix, false, ShadowMatrix);
        }

        #endregion

        protected override void SetShaderMacros()
        {
            SetDefine<Int32>(ShaderTypeFlag.VertexShader | ShaderTypeFlag.FragmentShader, "MAX_LIGHT_COUNT", EngineStatics.MAX_LIGHT_COUNT);
            SetDefine<float>(ShaderTypeFlag.FragmentShader, "SHADOWMAP_BIAS", 0.005f);
            SetDefine<Int32>(ShaderTypeFlag.FragmentShader, "PCF_SAMPLES", 2);
        }

        #region Constructor

        public MovableEntityShader(string VertexShaderFile, string FragmentShaderFile)
            : base(SHADER_NAME, VertexShaderFile, FragmentShaderFile)
        {
        }

        #endregion
    }
}
