using MassiveGame.Core.GameCore.EntityComponents;
using MassiveGame.Core.RenderCore;
using MassiveGame.Core.RenderCore.Lights;
using OpenTK;
using System;
using System.Collections.Generic;
using ShaderPattern;

namespace MassiveGame.Core.GameCore.Entities.StaticEntities
{
    public class StaticEntityShader : ShaderBase
    {
        #region Definitions

        private const string SHADER_NAME = "StaticEntity Shader";
        private Uniform u_entityTexture,
            u_entityNormalMap,
            u_entitySpecularMap,
            u_normalMapEnDis,
            u_materialAmbient,
            u_materialDiffuse,
            u_materialSpecular,
            u_materialReflectivity,
            u_materialShineDamper,
            u_modelMatrix,
            u_viewMatrix,
            u_projectionMatrix,
            u_sunEnable,
            u_sunDirection,
            u_sunAmbientColour,
            u_sunDiffuseColour,
            u_sunSpecularColour,
            u_clipPlane,
            u_mistEnable,
            u_mistDensity,
            u_mistGradient,
            u_mistColour,
            u_directionalLightShadowMap,
            u_directionalLightShadowMatrix;

        private Uniform[] u_lightPosition = new Uniform[EngineStatics.MAX_LIGHT_COUNT],
        u_attenuation = new Uniform[EngineStatics.MAX_LIGHT_COUNT],
        u_diffuseColour = new Uniform[EngineStatics.MAX_LIGHT_COUNT],
        u_specularColour = new Uniform[EngineStatics.MAX_LIGHT_COUNT],
        u_enableLight = new Uniform[EngineStatics.MAX_LIGHT_COUNT];

        #endregion

        #region Getter

        protected override void getAllUniformLocations()
        {
            try
            {
                u_entityTexture = GetUniform("entitieTexture");
                u_entityNormalMap = GetUniform("normalMap");
                u_normalMapEnDis = GetUniform("normalMapEnableDisable");
                u_materialAmbient = GetUniform("materialAmbient");
                u_materialDiffuse = GetUniform("materialDiffuse");
                u_materialSpecular = GetUniform("materialSpecular");
                u_materialReflectivity = GetUniform("materialReflectivity");
                u_materialShineDamper = GetUniform("materialShineDamper");
                u_modelMatrix = GetUniform("ModelMatrix");
                u_viewMatrix = GetUniform("ViewMatrix");
                u_projectionMatrix = GetUniform("ProjectionMatrix");
                u_sunDirection = GetUniform("sunDirection");
                u_sunAmbientColour = GetUniform("sunAmbientColour");
                u_sunDiffuseColour = GetUniform("sunDiffuseColour");
                u_sunSpecularColour = GetUniform("sunSpecularColour");
                u_sunEnable = GetUniform("sunEnable");
                u_mistEnable = GetUniform("mistEnable");
                u_mistDensity = GetUniform("mistDensity");
                u_mistGradient = GetUniform("mistGradient");
                u_mistColour = GetUniform("mistColour");
                for (Int32 i = 0; i < EngineStatics.MAX_LIGHT_COUNT; i++)
                {
                    u_lightPosition[i] = GetUniform("lightPosition[" + i + "]");
                    u_attenuation[i] = GetUniform("attenuation[" + i + "]");
                    u_diffuseColour[i] = GetUniform("diffuseColour[" + i + "]");
                    u_specularColour[i] = GetUniform("specularColour[" + i + "]");
                    u_enableLight[i] = GetUniform("enableLight[" + i + "]");
                }
                u_clipPlane = GetUniform("clipPlane");
                u_directionalLightShadowMap = GetUniform("dirLightShadowMap");
                u_directionalLightShadowMatrix = GetUniform("dirLightShadowMatrix");
                u_entitySpecularMap = GetUniform("specularMap");
            }
            catch (ArgumentNullException innerException)
            {
                Debug.Log.AddToFileStreamLog(innerException.Message);
                Debug.Log.AddToConsoleStreamLog(innerException.Message);
            }
        }

        #endregion

        #region Setter

        public void SetDiffuseMap(Int32 diffuseMapSampler)
        {
            u_entityTexture.LoadUniform(diffuseMapSampler);
        }

        public void SetNormalMap(Int32 normalMapSampler, bool bEnableNormalMap)
        {
            if (bEnableNormalMap)
                u_entityNormalMap.LoadUniform(normalMapSampler);
            u_normalMapEnDis.LoadUniform(bEnableNormalMap);
        }

        public void SetSpecularMap(Int32 specularMapSampler)
        {
            u_entitySpecularMap.LoadUniform(specularMapSampler);
        }

        public void SetMaterial(Material material)
        {
            u_materialAmbient.LoadUniform(material.Ambient.Xyz);
            u_materialDiffuse.LoadUniform(material.Diffuse.Xyz);
            u_materialSpecular.LoadUniform(material.Specular.Xyz);
            u_materialReflectivity.LoadUniform(material.Reflectivity);
            u_materialShineDamper.LoadUniform(material.ShineDamper);
        }

        public void SetDirectionalLight(DirectionalLight directionalLight)
        {
            if (directionalLight != null)
            {
                u_sunEnable.LoadUniform(true);
                u_sunDirection.LoadUniform(directionalLight.Direction);
                u_sunAmbientColour.LoadUniform(directionalLight.Ambient.Xyz);
                u_sunDiffuseColour.LoadUniform(directionalLight.Diffuse.Xyz);
                u_sunSpecularColour.LoadUniform(directionalLight.Specular.Xyz);
            }
            else
            {
                u_sunEnable.LoadUniform(false);
            }
        }

        public void SetTransformationMatrices(ref Matrix4 WorldMatrix, Matrix4 ViewMatrix, ref Matrix4 ProjectionMatrix)
        {
            u_modelMatrix.LoadUniform(ref WorldMatrix);
            u_viewMatrix.LoadUniform(ref ViewMatrix);
            u_projectionMatrix.LoadUniform(ref ProjectionMatrix);
        }

        public void SetPointLights(List<PointLight> lights)
        {
            /*If point lights are enabled*/
            if (lights != null)
            {
                for (Int32 i = 0; i < (lights.Count <= EngineStatics.MAX_LIGHT_COUNT ? lights.Count : EngineStatics.MAX_LIGHT_COUNT); i++)
                {
                    u_enableLight[i].LoadUniform(true);
                    u_lightPosition[i].LoadUniform(lights[i].Position.Xyz);
                    u_attenuation[i].LoadUniform(lights[i].Position.Xyz);
                    u_diffuseColour[i].LoadUniform(lights[i].Position.Xyz);
                    u_specularColour[i].LoadUniform(lights[i].Position.Xyz);
                }
                for (Int32 i = lights.Count; i < EngineStatics.MAX_LIGHT_COUNT; i++)
                {
                    u_enableLight[i].LoadUniform(false);
                }
            }
            else
            {
                for (Int32 i = 0; i < EngineStatics.MAX_LIGHT_COUNT; i++)
                {
                    u_enableLight[i].LoadUniform(false);
                }
            }
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

        public void SetClippingPlane(ref Vector4 clipPlane)
        {
            u_clipPlane.LoadUniform(ref clipPlane);
        }

        public void SetDirectionalLightShadowMap(Int32 shadowMapSampler)
        {
            u_directionalLightShadowMap.LoadUniform(shadowMapSampler);
        }

        public void SetDirectionalLightShadowMatrix(Matrix4 ShadowMatrix)
        {
            u_directionalLightShadowMatrix.LoadUniform(ref ShadowMatrix);
        }

        #endregion

        protected override void SetShaderMacros()
        {
            SetDefine<Int32>(ShaderTypeFlag.VertexShader | ShaderTypeFlag.FragmentShader, "MAX_LIGHT_COUNT", EngineStatics.MAX_LIGHT_COUNT);
            SetDefine<float>(ShaderTypeFlag.FragmentShader, "MAX_MIST_VISIBLE_AREA", 1.0f);
            SetDefine<float>(ShaderTypeFlag.FragmentShader, "SHADOWMAP_BIAS", 0.005f);
            SetDefine<Int32>(ShaderTypeFlag.FragmentShader, "PCF_SAMPLES", 2);
        }

        #region Constructor

        public StaticEntityShader() : base() { }

        public StaticEntityShader(string VertexShaderFile, string FragmentShaderFile)
            : base(SHADER_NAME, VertexShaderFile, FragmentShaderFile)
        {
        }

        #endregion
    }

    public class SpecialStaticEntityShader : ShaderBase
    {
        #region Definitions

        private const string SHADER_NAME = "SpecialStaticEntity Shader";

        private Uniform u_modelMatrix,
            u_viewMatrix,
            u_projectionMatrix;

        #endregion

        #region Getter

        protected override void getAllUniformLocations()
        {
            try
            {
                u_modelMatrix = GetUniform("ModelMatrix");
                u_viewMatrix = GetUniform("ViewMatrix");
                u_projectionMatrix = GetUniform("ProjectionMatrix");
            }
            catch (ArgumentNullException innerException)
            {
                Debug.Log.AddToFileStreamLog(innerException.Message);
                Debug.Log.AddToConsoleStreamLog(innerException.Message);
            }
        }

        #endregion

        #region Setter

        public void setUniformValues(ref Matrix4 ModelMatrix, Matrix4 ViewMatrix,
           ref Matrix4 ProjectionMatrix)
        {

            u_modelMatrix.LoadUniform(ref ModelMatrix);
            u_viewMatrix.LoadUniform(ref ViewMatrix);
            u_projectionMatrix.LoadUniform(ref ProjectionMatrix);
        }

        #endregion

        protected override void SetShaderMacros() { }

        #region Constructor 

        public SpecialStaticEntityShader() : base() { }

        public SpecialStaticEntityShader(string VertexShaderFile, string FragmentShaderFile, string GeometryShaderFile)
            : base(SHADER_NAME, VertexShaderFile, FragmentShaderFile, GeometryShaderFile)
        {
        }
        #endregion
    }
}


