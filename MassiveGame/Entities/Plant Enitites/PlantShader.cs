using OpenTK;
using ShaderPattern;
using System;
using MassiveGame.RenderCore.Lights;
using MassiveGame.RenderCore;

namespace MassiveGame
{
    public sealed class PlantShader : ShaderBase
    {
        #region Definitions 

        private const string SHADER_NAME = "Plant Shader";
        private Int32 plantTexture,
            materialAmbient,
            materialDiffuse,
            ModelMatrix,
            ViewMatrix,
            ProjectionMatrix,
            sunDirection,
            sunAmbientColour,
            sunDiffuseColour,
            sunEnable,
            windDirection,
            windPower,
            windLoop,
            clipPlane,
            mistEnable,
            mistDensity,
            mistGradient,
            mistColour,
            time;

        #endregion

        protected override void SetShaderMacros()
        {
            SetDefine(ShaderTypeFlag.FragmentShader, "MAX_MIST_VISIBLE_AREA", "1.0");
        }

        #region Getter

        protected override void getAllUniformLocations()
        {
            plantTexture = base.getUniformLocation("backgroundTexture");
            materialAmbient = base.getUniformLocation("materialAmbient");
            materialDiffuse = base.getUniformLocation("materialDiffuse");
            ModelMatrix = base.getUniformLocation("ModelMatrix");
            ViewMatrix = base.getUniformLocation("ViewMatrix");
            ProjectionMatrix = base.getUniformLocation("ProjectionMatrix");
            sunDirection = base.getUniformLocation("sunDirection");
            sunAmbientColour = base.getUniformLocation("sunAmbientColour");
            sunDiffuseColour = base.getUniformLocation("sunDiffuseColour");
            sunEnable = base.getUniformLocation("sunEnable");
            windDirection = base.getUniformLocation("windDirection");
            windPower = base.getUniformLocation("windPower");
            windLoop = base.getUniformLocation("windLoop");
            clipPlane = base.getUniformLocation("clipPlane");
            mistEnable = base.getUniformLocation("mistEnable");
            mistDensity = base.getUniformLocation("mistDensity");
            mistGradient = base.getUniformLocation("mistGradient");
            mistColour = base.getUniformLocation("mistColour");
            time = base.getUniformLocation("time");
        }

         #endregion

         #region Setter

         public void setTextureSampler(Int32 sampler)
         {
             base.loadInteger(this.plantTexture, sampler);
         }

         public void setMaterial(Material material)
         {
             base.loadVector(this.materialAmbient, material.Ambient.Xyz);
             base.loadVector(this.materialDiffuse, material.Diffuse.Xyz);
         }

        public void setViewMatrix(Matrix4 viewMatrix)
         {
             base.loadMatrix(this.ViewMatrix, false, viewMatrix);
         }

        public void setProjectionMatrix(ref Matrix4 projectionMatrix)
        {
            base.loadMatrix(this.ProjectionMatrix, false, projectionMatrix);
        }

        public void setSun(DirectionalLight sun)
        {
            /*If sun is enabled*/
            if (sun != null)
            {
                base.loadBool(this.sunEnable, true);
                base.loadVector(this.sunDirection, sun.Direction);
                base.loadVector(this.sunAmbientColour, sun.Ambient.Xyz);
                base.loadVector(this.sunDiffuseColour, sun.Diffuse.Xyz);
            }
            else { base.loadBool(this.sunEnable, false); }
        }

        public void setWind(WindComponent wind)
        {
            base.loadVector(this.windDirection, wind.WindDirection);
            base.loadFloat(this.windPower, wind.WindPower);
        }

        public void setTime(float time)
        {
            base.loadFloat(this.time, time);
        }

        public void setClipPlane(ref Vector4 clipPlane)
        {
            base.loadVector(this.clipPlane, clipPlane);
        }

        public void setMist(MistComponent mist)
        {
            if (mist != null)
            {
                base.loadBool(this.mistEnable, true);
                base.loadFloat(this.mistDensity, mist.MistDensity);
                base.loadFloat(this.mistGradient, mist.MistGradient);
                base.loadVector(this.mistColour, mist.MistColour);
            }
            else
            {
                base.loadBool(this.mistEnable, false);
            }
        }

         #endregion

         #region Constructor

         public PlantShader(string VertexShaderFile, string FragmentShaderFile)
             : base(SHADER_NAME, VertexShaderFile, FragmentShaderFile)
         {
         }

         #endregion
    }
}
