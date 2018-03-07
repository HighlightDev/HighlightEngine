using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using ShaderPattern;

namespace MassiveGame
{
    public class GodRaysShader : Shader
    {
        #region Definitions

        private const string SHADER_NAME = "GodRays Shader";
        private int frameTexture, bluredTexture, exposure, decay, weight,
            radialPosition, density, numSamples;

        #endregion

        #region Getter

        protected override void getAllUniformLocations()
        {
            frameTexture = base.getUniformLocation("frameTexture");
            bluredTexture = base.getUniformLocation("bluredTexture");
            exposure = base.getUniformLocation("exposure");
            decay = base.getUniformLocation("decay");
            weight = base.getUniformLocation("weight");
            radialPosition = base.getUniformLocation("radialPosition");
            density = base.getUniformLocation("density");
            numSamples = base.getUniformLocation("numSamples");
        }

        #endregion

        #region Setter

        public void setUniformValuesRadialBlur(int frameTexture, int bluredTexture, float exposure,
            float decay, float weigth, float density, int numSamples, Vector2 radialPosition)
        {
            base.loadInteger(this.frameTexture, frameTexture);
            base.loadInteger(this.bluredTexture, bluredTexture);
            base.loadFloat(this.exposure, exposure);
            base.loadFloat(this.decay, decay);
            base.loadFloat(this.weight, weight);
            base.loadFloat(this.density, density);
            base.loadInteger(this.numSamples, numSamples);
            base.loadVector(this.radialPosition, radialPosition);
        }

        #endregion

        #region Constructor

        public GodRaysShader(string VSPath, string FSPath)
            : base(VSPath, FSPath)
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
