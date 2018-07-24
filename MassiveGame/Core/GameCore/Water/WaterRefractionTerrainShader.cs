using MassiveGame.Core.RenderCore;
using MassiveGame.Core.RenderCore.Lights;
using OpenTK;
using ShaderPattern;
using System;

namespace MassiveGame.Core.GameCore.Water
{
    public class WaterRefractionTerrainShader : ShaderBase
    {
        private const string SHADER_NAME = "WaterRefractionTerrainShader";

        private Uniform u_backTexture, u_rTexture, u_gTexture,
              u_bTexture, u_blendMap, u_materialAmbient, u_materialDiffuse,
              u_modelMatrix, u_viewMatrix, u_projectionMatrix, u_sunDirection,
              u_sunAmbientColour, u_sunDiffuseColour, u_sunEnable, u_clipPlane;

        public WaterRefractionTerrainShader() : base() { }

        public WaterRefractionTerrainShader(string VertexShaderFile, string FragmentShaderFile)
            : base(SHADER_NAME, VertexShaderFile, FragmentShaderFile)
        {
        }

        protected override void getAllUniformLocations()
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
            u_clipPlane = GetUniform("clipPlane");
        }

        public void SetBlendMap(Int32 blendMapSampler)
        {
            u_blendMap.LoadUniform(blendMapSampler);
        }

        public void SetTextureR(Int32 textureSamplerR)
        {
            u_rTexture.LoadUniform(textureSamplerR);
        }

        public void SetTextureG(Int32 textureSamplerG)
        {
            u_gTexture.LoadUniform(textureSamplerG);
        }

        public void SetTextureB(Int32 textureSamplerB)
        {
            u_bTexture.LoadUniform(textureSamplerB);
        }

        public void SetTextureBlack(Int32 textureSamplerBlack)
        {
            u_backTexture.LoadUniform(textureSamplerBlack);
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
            else
            {
                u_sunEnable.LoadUniform(false);
            }
        }

        public void SetClippingPlane(ref Vector4 clippingPlane)
        {
            u_clipPlane.LoadUniform(ref clippingPlane);
        }

        protected override void SetShaderMacros() { }
    }
}
