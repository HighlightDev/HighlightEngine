﻿using OpenTK;
using MassiveGame.RenderCore.Lights;
using ShaderPattern;
using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;

namespace MassiveGame
{
    public class StaticEntityShader : Shader
    {
        #region Definitions

        private const string SHADER_NAME = "StaticEntity Shader";
        private static int MAX_LIGHTS_COUNT = DOUEngine.MAX_LIGHT_COUNT; //Максимальное количество источников света, доступных для обработки
        private int entityTexture,
            entityNormalMap,
            entitySpecularMap,
            glowingMap,
            normalMapEnDis,
            materialAmbient,
            materialDiffuse,
            materialSpecular,
            materialReflectivity,
            materialShineDamper,
            ModelMatrix,
            ViewMatrix,
            ProjectionMatrix,
            sunDirection,
            sunAmbientColour,
            sunDiffuseColour,
            sunSpecularColour,
            sunEnable,
            clipPlane,
            mistEnable,
            mistDensity,
            mistGradient,
            mistColour,
            directionalLightShadowMap,
            directionalLightShadowMatrix;
        private int[] lightPosition = new int[MAX_LIGHTS_COUNT],
            attenuation = new int[MAX_LIGHTS_COUNT],
            diffuseColour = new int[MAX_LIGHTS_COUNT],
            specularColour = new int[MAX_LIGHTS_COUNT],
            enableLight = new int[MAX_LIGHTS_COUNT];

        #endregion

        #region Getter

        protected override void getAllUniformLocations()
        {
            entityTexture = base.getUniformLocation("entitieTexture");
            entityNormalMap = base.getUniformLocation("normalMap");
            entitySpecularMap = base.getUniformLocation("specularMap");
            glowingMap = base.getUniformLocation("glowingMap");
            normalMapEnDis = base.getUniformLocation("normalMapEnableDisable");
            materialAmbient = base.getUniformLocation("materialAmbient");
            materialDiffuse = base.getUniformLocation("materialDiffuse");
            materialSpecular = base.getUniformLocation("materialSpecular");
            materialReflectivity = base.getUniformLocation("materialReflectivity");
            materialShineDamper = base.getUniformLocation("materialShineDamper");
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
            for (int i = 0; i < MAX_LIGHTS_COUNT; i++)
            {
                lightPosition[i] = base.getUniformLocation("lightPosition[" + i + "]");
                attenuation[i] = base.getUniformLocation("attenuation[" + i + "]");
                diffuseColour[i] = base.getUniformLocation("diffuseColour[" + i + "]");
                specularColour[i] = base.getUniformLocation("specularColour[" + i + "]");
                enableLight[i] = base.getUniformLocation("enableLight[" + i + "]");
            }
            clipPlane = base.getUniformLocation("clipPlane");
            directionalLightShadowMap = getUniformLocation("dirLightShadowMap");
            directionalLightShadowMatrix = getUniformLocation("dirLightShadowMatrix");
        }

        #endregion

        #region Setter
        public void SetDiffuseMap(int diffuseMapSampler)
        {
            base.loadInteger(entityTexture, diffuseMapSampler);
        }

        public void SetGlowingMap(int glowingMapSampler)
        {
            base.loadInteger(this.glowingMap, glowingMapSampler);
        }

        public void SetNormalMap(int normalMapSampler, bool bEnableNormalMap)
        {
            if (bEnableNormalMap)
                base.loadInteger(this.entityNormalMap, normalMapSampler);
            base.loadBool(this.normalMapEnDis, bEnableNormalMap);
        }

        public void SetSpecularMap(int specularMapSampler)
        {
            base.loadInteger(this.entitySpecularMap, specularMapSampler);
        }

        public void SetMaterial(Material material)
        {
            base.loadVector(this.materialAmbient, material.Ambient.Xyz);
            base.loadVector(this.materialDiffuse, material.Diffuse.Xyz);
            base.loadVector(this.materialSpecular, material.Specular.Xyz);
            base.loadFloat(this.materialReflectivity, material.Reflectivity);
            base.loadFloat(this.materialShineDamper, material.ShineDamper);
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
                for (int i = 0; i < (lights.Count <= MAX_LIGHTS_COUNT ? lights.Count : MAX_LIGHTS_COUNT); i++) //Включенные источники света
                {
                    base.loadBool(this.enableLight[i], true);
                    base.loadVector(lightPosition[i], new Vector3(lights[i].Position.X, lights[i].Position.Y, lights[i].Position.Z));
                    base.loadVector(attenuation[i], new Vector3(lights[i].Attenuation.X, lights[i].Attenuation.Y, lights[i].Attenuation.Z));
                    base.loadVector(diffuseColour[i], new Vector3(lights[i].Diffuse.X, lights[i].Diffuse.Y, lights[i].Diffuse.Z));
                    base.loadVector(specularColour[i], new Vector3(lights[i].Specular.X, lights[i].Specular.Y, lights[i].Specular.Z));
                }
                for (int i = lights.Count; i < MAX_LIGHTS_COUNT; i++)      //Выключенные источники света
                {
                    base.loadBool(this.enableLight[i], false);
                }
            }
            else
            {
                for (int i = 0; i < MAX_LIGHTS_COUNT; i++)      //Выключенные источники света
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

        public new void PrecompileEdit()
        {
            base.SetDefine(ShaderType.FragmentShader, "MAX_LIGHT_COUNT", "5");
        }

        public void setUniformValuesWithNormalMap(int sampler, int normalMap, int specularMap, int glowingMap, Vector3 materialAmbient,
            Vector3 materialDiffuse, Vector3 materialSpecular, float reflectivity,
            float shineDamper, ref Matrix4 ModelMatrix, Matrix4 ViewMatrix,
            ref Matrix4 ProjectionMatrix, List<PointLight> lights, DirectionalLight Sun,
            ref Vector4 clipPlane, float mistDensity, float mistGradient, Vector3 MistColour, bool mistEnable)
        {
            base.loadInteger(entityTexture, sampler);
            base.loadInteger(this.entityNormalMap, normalMap);
            base.loadInteger(this.entitySpecularMap, specularMap);

            base.loadInteger(this.glowingMap, glowingMap);

            base.loadBool(this.normalMapEnDis, true);
            base.loadVector(this.materialAmbient, materialAmbient);
            base.loadVector(this.materialDiffuse, materialDiffuse);
            base.loadVector(this.materialSpecular, materialSpecular);
            base.loadFloat(this.materialReflectivity, reflectivity);
            base.loadFloat(this.materialShineDamper, shineDamper);
            base.loadMatrix(this.ModelMatrix, false, ModelMatrix);
            base.loadMatrix(this.ViewMatrix, false, ViewMatrix);
            base.loadMatrix(this.ProjectionMatrix, false, ProjectionMatrix);
            /*If sun is enabled*/
            if (Sun != null)
            {
                base.loadBool(this.sunEnable, true);
                base.loadVector(this.sunDirection, Sun.Direction);
                base.loadVector(this.sunAmbientColour, new Vector3(Sun.Ambient));
                base.loadVector(this.sunDiffuseColour, new Vector3(Sun.Diffuse));
                base.loadVector(this.sunSpecularColour, new Vector3(Sun.Specular));
            }
            else { base.loadBool(this.sunEnable, false); }

            /*If point lights are enabled*/
            if (lights != null)
            {
                for (int i = 0; i < (lights.Count <= MAX_LIGHTS_COUNT ? lights.Count : MAX_LIGHTS_COUNT); i++) //Включенные источники света
                {
                    base.loadBool(this.enableLight[i], true);
                    base.loadVector(lightPosition[i], new Vector3(lights[i].Position.X, lights[i].Position.Y, lights[i].Position.Z));
                    base.loadVector(attenuation[i], new Vector3(lights[i].Attenuation.X, lights[i].Attenuation.Y, lights[i].Attenuation.Z));
                    base.loadVector(diffuseColour[i], new Vector3(lights[i].Diffuse.X, lights[i].Diffuse.Y, lights[i].Diffuse.Z));
                    base.loadVector(specularColour[i], new Vector3(lights[i].Specular.X, lights[i].Specular.Y, lights[i].Specular.Z));
                }
                for (int i = lights.Count; i < MAX_LIGHTS_COUNT; i++)      //Выключенные источники света
                {
                    base.loadBool(this.enableLight[i], false);
                }
            }
            else
            {
                for (int i = 0; i < MAX_LIGHTS_COUNT; i++)      //Выключенные источники света
                {
                    base.loadBool(this.enableLight[i], false);
                }
            }

            base.loadVector(this.clipPlane, clipPlane);

            base.loadBool(this.mistEnable, mistEnable);
            base.loadFloat(this.mistDensity, mistDensity);
            base.loadFloat(this.mistGradient, mistGradient);
            base.loadVector(this.mistColour, MistColour);
        }

        public void setUniformValuesWithoutNormalMap(int sampler, int glowingMap, Vector3 materialAmbient,
           Vector3 materialDiffuse, Vector3 materialSpecular, float reflectivity,
           float shineDamper, ref Matrix4 ModelMatrix, Matrix4 ViewMatrix,
           ref Matrix4 ProjectionMatrix, List<PointLight> lights, DirectionalLight Sun,
            ref Vector4 clipPlane, float mistDensity, float mistGradient, Vector3 MistColour, bool mistEnable)
        {
            base.loadInteger(entityTexture, sampler);

            base.loadInteger(this.glowingMap, glowingMap);

            base.loadBool(this.normalMapEnDis, false);
            base.loadVector(this.materialAmbient, materialAmbient);
            base.loadVector(this.materialDiffuse, materialDiffuse);
            base.loadVector(this.materialSpecular, materialSpecular);
            base.loadFloat(this.materialReflectivity, reflectivity);
            base.loadFloat(this.materialShineDamper, shineDamper);
            base.loadMatrix(this.ModelMatrix, false, ModelMatrix);
            base.loadMatrix(this.ViewMatrix, false, ViewMatrix);
            base.loadMatrix(this.ProjectionMatrix, false, ProjectionMatrix);
            /*If sun is enabled*/
            if (Sun != null)
            {
                base.loadBool(this.sunEnable, true);
                base.loadVector(this.sunDirection, Sun.Direction);
                base.loadVector(this.sunAmbientColour, new Vector3(Sun.Ambient));
                base.loadVector(this.sunDiffuseColour, new Vector3(Sun.Diffuse));
                base.loadVector(this.sunSpecularColour, new Vector3(Sun.Specular));
            }
            else { base.loadBool(this.sunEnable, false); }

            /*If point lights are enabled*/
            if (lights != null)
            {
                for (int i = 0; i < (lights.Count <= MAX_LIGHTS_COUNT ? lights.Count : MAX_LIGHTS_COUNT); i++) //Включенные источники света
                {
                    base.loadBool(this.enableLight[i], true);
                    base.loadVector(lightPosition[i], new Vector3(lights[i].Position.X, lights[i].Position.Y, lights[i].Position.Z));
                    base.loadVector(attenuation[i], new Vector3(lights[i].Attenuation.X, lights[i].Attenuation.Y, lights[i].Attenuation.Z));
                    base.loadVector(diffuseColour[i], new Vector3(lights[i].Diffuse.X, lights[i].Diffuse.Y, lights[i].Diffuse.Z));
                    base.loadVector(specularColour[i], new Vector3(lights[i].Specular.X, lights[i].Specular.Y, lights[i].Specular.Z));
                }
                for (int i = lights.Count; i < MAX_LIGHTS_COUNT; i++)      //Выключенные источники света
                {
                    base.loadBool(this.enableLight[i], false);
                }
            }
            else
            {
                for (int i = 0; i < MAX_LIGHTS_COUNT; i++)      //Выключенные источники света
                {
                    base.loadBool(this.enableLight[i], false);
                }
            }

            base.loadVector(this.clipPlane, clipPlane);

            base.loadBool(this.mistEnable, mistEnable);
            base.loadFloat(this.mistDensity, mistDensity);
            base.loadFloat(this.mistGradient, mistGradient);
            base.loadVector(this.mistColour, MistColour);
        }

        #endregion

        #region Constructor

        public StaticEntityShader(string VertexShaderFile, string FragmentShaderFile, string GeometryShaderFile)
            : base(VertexShaderFile, FragmentShaderFile, GeometryShaderFile)
        {
            if (base.ShaderLoaded)
            {
                base.showCompileLogInfo(SHADER_NAME);
                base.showLinkLogInfo(SHADER_NAME);
                Debug.Log.addToLog(DateTime.Now.ToString() + "  " + base.getCompileLogInfo(SHADER_NAME));
                Debug.Log.addToLog(DateTime.Now.ToString() + "  " + base.getLinkLogInfo(SHADER_NAME));
            }
            else Debug.Log.addToLog(DateTime.Now.ToString() + "  " + SHADER_NAME + " : shader file(s) not found!");
        }

        public StaticEntityShader(string VertexShaderFile, string FragmentShaderFile)
            : base(VertexShaderFile, FragmentShaderFile)
        {
            if (base.ShaderLoaded)
            {
                base.showCompileLogInfo(SHADER_NAME);
                base.showLinkLogInfo(SHADER_NAME);
                Debug.Log.addToLog(DateTime.Now.ToString() + "  " + base.getCompileLogInfo(SHADER_NAME));
                Debug.Log.addToLog(DateTime.Now.ToString() + "  " + base.getLinkLogInfo(SHADER_NAME));
            }
            else Debug.Log.addToLog(DateTime.Now.ToString() + "  " + SHADER_NAME + " : shader file(s) not found!");
        }

        #endregion
    }

    public class SpecialStaticEntityShader : Shader
    {
        #region Definitions

        private const string SHADER_NAME = "SpecialStaticEntity Shader";

        private int ModelMatrix,
            ViewMatrix,
            ProjectionMatrix;
            
        #endregion

        #region Getter

        protected override void getAllUniformLocations()
        {
            ModelMatrix = base.getUniformLocation("ModelMatrix");
            ViewMatrix = base.getUniformLocation("ViewMatrix");
            ProjectionMatrix = base.getUniformLocation("ProjectionMatrix");
        }

        #endregion

        #region Setter
        
        public void setUniformValues(ref Matrix4 ModelMatrix, Matrix4 ViewMatrix,
           ref Matrix4 ProjectionMatrix)
        {
           
            base.loadMatrix(this.ModelMatrix, false, ModelMatrix);
            base.loadMatrix(this.ViewMatrix, false, ViewMatrix);
            base.loadMatrix(this.ProjectionMatrix, false, ProjectionMatrix);
        }

        #endregion

        #region Constructor

        public SpecialStaticEntityShader(string VertexShaderFile, string FragmentShaderFile, string GeometryShaderFile)
            : base(VertexShaderFile, FragmentShaderFile, GeometryShaderFile)
        {
            if (base.ShaderLoaded)
            {
                base.showCompileLogInfo(SHADER_NAME);
                base.showLinkLogInfo(SHADER_NAME);
                Debug.Log.addToLog(DateTime.Now.ToString() + "  " + base.getCompileLogInfo(SHADER_NAME));
                Debug.Log.addToLog(DateTime.Now.ToString() + "  " + base.getLinkLogInfo(SHADER_NAME));
            }
            else Debug.Log.addToLog(DateTime.Now.ToString() + "  " + SHADER_NAME + " : shader file(s) not found!");
        }
        #endregion
    }
}

