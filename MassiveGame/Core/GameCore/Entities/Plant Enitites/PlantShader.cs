using MassiveGame.Core.GameCore.EntityComponents;
using MassiveGame.Core.RenderCore;
using MassiveGame.Core.RenderCore.Lights;
using OpenTK;
using ShaderPattern;
using System;

namespace MassiveGame.Core.GameCore.Entities.StaticEntities
{
    public sealed class PlantShader : ShaderBase
    {
        #region Definitions 

        private const string SHADER_NAME = "Plant Shader";
        private Uniform u_plantTexture,
            u_materialAmbient,
            u_materialDiffuse,
            u_modelMatrix,
            u_viewMatrix,
            u_projectionMatrix,
            u_sunDirection,
            u_sunAmbientColour,
            u_sunDiffuseColour,
            u_sunEnable,
            u_windDirection,
            u_windPower,
            u_windLoop,
            u_clipPlane,
            u_mistEnable,
            u_mistDensity,
            u_mistGradient,
            u_mistColour,
            u_time;

        #endregion

        protected override void SetShaderMacros()
        {
            SetDefine<float>(ShaderTypeFlag.FragmentShader, "MAX_MIST_VISIBLE_AREA", 1.0f);
        }

         #region Getter

        protected override void getAllUniformLocations()
        {
            try
            {
                u_plantTexture = GetUniform("backgroundTexture");
                u_materialAmbient = GetUniform("materialAmbient");
                u_materialDiffuse = GetUniform("materialDiffuse");
                u_modelMatrix = GetUniform("ModelMatrix");
                u_viewMatrix = GetUniform("ViewMatrix");
                u_projectionMatrix = GetUniform("ProjectionMatrix");
                u_sunDirection = GetUniform("sunDirection");
                u_sunAmbientColour = GetUniform("sunAmbientColour");
                u_sunDiffuseColour = GetUniform("sunDiffuseColour");
                u_sunEnable = GetUniform("sunEnable");
                u_windDirection = GetUniform("windDirection");
                u_windPower = GetUniform("windPower");
                u_windLoop = GetUniform("windLoop");
                u_clipPlane = GetUniform("clipPlane");
                u_mistEnable = GetUniform("mistEnable");
                u_mistDensity = GetUniform("mistDensity");
                u_mistGradient = GetUniform("mistGradient");
                u_mistColour = GetUniform("mistColour");
                u_time = GetUniform("time");
            }
            catch (ArgumentNullException innerException)
            {
                Debug.Log.AddToFileStreamLog(innerException.Message);
                Debug.Log.AddToConsoleStreamLog(innerException.Message);
            }
        }

         #endregion

         #region Setter

         public void setTextureSampler(Int32 sampler)
         {
            u_plantTexture.LoadUniform(sampler);
         }

         public void setMaterial(Material material)
         {
            u_materialAmbient.LoadUniform(material.Ambient.Xyz);
            u_materialDiffuse.LoadUniform(material.Diffuse.Xyz);
         }

        public void setViewMatrix(Matrix4 viewMatrix)
         {
             u_viewMatrix.LoadUniform(ref viewMatrix);
         }

        public void setProjectionMatrix(ref Matrix4 projectionMatrix)
        {
            u_projectionMatrix.LoadUniform(ref projectionMatrix);
        }

        public void setSun(DirectionalLight sun)
        {
            /*If sun is enabled*/
            if (sun != null)
            {
                u_sunEnable.LoadUniform(true);
                u_sunDirection.LoadUniform(sun.Direction);
                u_sunAmbientColour.LoadUniform(sun.Ambient.Xyz);
                u_sunDiffuseColour.LoadUniform(sun.Diffuse.Xyz);
            }
            else
            {
                u_sunEnable.LoadUniform(false);
            }
        }

        public void setWind(WindComponent wind)
        {
            u_windDirection.LoadUniform(wind.WindDirection);
            u_windPower.LoadUniform(wind.WindPower);
        }

        public void setTime(float time)
        {
            u_time.LoadUniform(time);
        }

        public void setClipPlane(ref Vector4 clipPlane)
        {
            u_clipPlane.LoadUniform(ref clipPlane);
        }

        public void setMist(MistComponent mist)
        {
            if (mist != null)
            {
                u_mistEnable.LoadUniform(true);
                u_mistDensity.LoadUniform(mist.MistDensity);
                u_mistGradient.LoadUniform(mist.MistGradient);
                u_mistColour.LoadUniform(mist.MistColour);
            }
            else
            {
                u_mistEnable.LoadUniform(false);
            }
        }

        #endregion

        #region Constructor

        public PlantShader() : base() { }

         public PlantShader(string VertexShaderFile, string FragmentShaderFile)
             : base(SHADER_NAME, VertexShaderFile, FragmentShaderFile)
         {
         }

         #endregion
    }
}
