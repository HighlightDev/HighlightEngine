using MassiveGame.Core.RenderCore;
using MassiveGame.Core.RenderCore.Lights;
using OpenTK;
using ShaderPattern;
using System;

namespace MassiveGame.Core.GameCore.Water
{
    public class WaterReflectionEntityShader : ShaderBase
    {
        private const string SHADER_NAME = "WaterReflecitonEntityShader";

        private Uniform u_texture,
            u_normalMap,
            u_materialAmbient,
            u_materialDiffuse,
            u_materialSpecular,
            u_materialReflectivity,
            u_materialShineDamper,
            u_worldMatrix,
            u_viewMatrix,
            u_projectionMatrix,
            u_sunEnable,
            u_sunDirection,
            u_sunAmbientColour,
            u_sunDiffuseColour,
            u_sunSpecularColour,
            u_clipPlane,
            u_specularMap,
            u_bSpecularMapEnable,
            u_lightWorldMatrix;

        public WaterReflectionEntityShader() : base() { }

        public WaterReflectionEntityShader(string VertexShaderFile, string FragmentShaderFile) 
            : base(SHADER_NAME, VertexShaderFile, FragmentShaderFile)
        {
        }

        public void SetTexture(Int32 textureSampler)
        {
            u_texture.LoadUniform(textureSampler);
        }

        public void SetNormalMap(Int32 normalMapSampler)
        {
            u_normalMap.LoadUniform(normalMapSampler);
        }

        public void SetSpecularMap(Int32 specularMapSampler)
        {
            u_specularMap.LoadUniform(specularMapSampler);
            u_bSpecularMapEnable.LoadUniform(true);
        }

        public void SetMaterial(Material material)
        {
            u_materialAmbient.LoadUniform(material.Ambient.Xyz);
            u_materialDiffuse.LoadUniform(material.Diffuse.Xyz);
            u_materialSpecular.LoadUniform(material.Specular.Xyz);
            u_materialReflectivity.LoadUniform(material.Reflectivity);
            u_materialShineDamper.LoadUniform(material.ShineDamper);
        }

        public void SetTransformationMatrices(ref Matrix4 worldMatrix, ref Matrix4 lightWorldMatrix, Matrix4 viewMatrix, ref Matrix4 projectionMatrix)
        {
            u_worldMatrix.LoadUniform(ref worldMatrix);
            u_viewMatrix.LoadUniform(ref viewMatrix);
            u_projectionMatrix.LoadUniform(ref projectionMatrix);
            u_lightWorldMatrix.LoadUniform(ref lightWorldMatrix);
        }

        public void SetDirectionalLight(DirectionalLight directionalLight)
        {
            bool bEnableSun = false;
            if (directionalLight != null)
            {
                u_sunDirection.LoadUniform(directionalLight.Direction);
                u_sunAmbientColour.LoadUniform(directionalLight.Ambient.Xyz);
                u_sunDiffuseColour.LoadUniform(directionalLight.Diffuse.Xyz);
                u_sunSpecularColour.LoadUniform(directionalLight.Specular.Xyz);
                bEnableSun = true;
            }

            u_sunEnable.LoadUniform(bEnableSun);
        }

        public void SetClipPlane(ref Vector4 clipPlane)
        {
            u_clipPlane.LoadUniform(ref clipPlane);
        }

        protected override void getAllUniformLocations()
        {
            try
            {
                u_texture = GetUniform("albedo");
                u_normalMap = GetUniform("normalMap");
                u_materialAmbient = GetUniform("matAmbient");
                u_materialDiffuse = GetUniform("matDiffuse");
                u_materialSpecular = GetUniform("matSpecular");
                u_materialReflectivity = GetUniform("matReflectivity");
                u_materialShineDamper = GetUniform("matShineDamper");
                u_worldMatrix = GetUniform("worldMatrix");
                u_viewMatrix = GetUniform("viewMatrix");
                u_projectionMatrix = GetUniform("projectionMatrix");
                u_sunEnable = GetUniform("bSunEnable");
                u_sunDirection = GetUniform("sunDirection");
                u_sunAmbientColour = GetUniform("sunAmbientColour");
                u_sunDiffuseColour = GetUniform("sunDiffuseColour");
                u_sunSpecularColour = GetUniform("sunSpecularColour");
                u_clipPlane = GetUniform("clipPlane");
                u_specularMap = GetUniform("specularMap");
                u_lightWorldMatrix = GetUniform("lightWorldMatrix");
                u_bSpecularMapEnable = GetUniform("bSpecularMapEnable");
            }
            catch (ArgumentNullException innerException)
            {
                Debug.Log.AddToFileStreamLog(innerException.Message);
                Debug.Log.AddToConsoleStreamLog(innerException.Message);
            }
        }

        protected override void SetShaderMacros() { }
    }
}
