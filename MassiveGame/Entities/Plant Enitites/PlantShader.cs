using OpenTK;
using ShaderPattern;
using System;
using MassiveGame.RenderCore.Lights;

namespace MassiveGame
{
    public sealed class PlantShader : Shader
    {
        #region Definitions 

        private const string SHADER_NAME = "Plant Shader";
         private int plantTexture,
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

         //public void setUniformValues(int sampler, Vector3 materialAmbient,
         //   Vector3 materialDiffuse, Matrix4 ModelMatrix, Matrix4 ViewMatrix, Matrix4 ProjectionMatrix, DirectionalLight Sun,
         //   WindComponent wind, float windLoop, Vector4 clipPlane, bool mistEnable, float mistDensity, float mistGradient, Vector3 mistColour)
         //{
         //    base.loadInteger(plantTexture, sampler);
         //    base.loadVector(this.materialAmbient, materialAmbient);
         //    base.loadVector(this.materialDiffuse, materialDiffuse);
         //    base.loadMatrix(this.ModelMatrix, false, ModelMatrix);
         //    base.loadMatrix(this.ViewMatrix, false, ViewMatrix);
         //    base.loadMatrix(this.ProjectionMatrix, false, ProjectionMatrix);
         //    /*If sun is enabled*/
         //    if (Sun != null)
         //    {
         //        base.loadBool(this.sunEnable, true);
         //        base.loadVector(this.sunDirection, Sun.Direction);
         //        base.loadVector(this.sunAmbientColour, new Vector3(Sun.Ambient));
         //        base.loadVector(this.sunDiffuseColour, new Vector3(Sun.Diffuse));
         //    }
         //    else { base.loadBool(this.sunEnable, false); }

         //    base.loadVector(this.windDirection, wind.WindDirection);
         //    base.loadFloat(this.windPower, wind.WindPower);
         //    base.loadFloat(this.windLoop, windLoop);
         //    base.loadVector(this.clipPlane, clipPlane);

         //    base.loadBool(this.mistEnable, mistEnable);
         //    base.loadFloat(this.mistDensity, mistDensity);
         //    base.loadFloat(this.mistGradient, mistGradient);
         //    base.loadVector(this.mistColour, mistColour);
         //}

         public void setTextureSampler(int sampler)
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
             : base(VertexShaderFile, FragmentShaderFile)
         {
             if (base.ShaderLoaded)
             {
                 base.showCompileLogInfo(SHADER_NAME);
                 base.showLinkLogInfo(SHADER_NAME);
                 Debug.Log.addToLog( DateTime.Now.ToString() + "  " + base.getCompileLogInfo(SHADER_NAME));
                 Debug.Log.addToLog( DateTime.Now.ToString() + "  " + base.getLinkLogInfo(SHADER_NAME));
             }
             else Debug.Log.addToLog( DateTime.Now.ToString() + "  " + SHADER_NAME + " : shader file(s) not found!");
         }

         #endregion
    }
}
