using System;
using ShaderPattern;
using OpenTK;
using MassiveGame.RenderCore.Lights;

namespace MassiveGame
{
    public class SunShader : Shader
    {
        #region Definitions 

        private const string SHADER_NAME = "Sun shader";
        private Int32 modelMatrix, viewMatrix, projectionMatrix, sunTexture1, sunTexture2,
            sunDirection, clipPlane;

        #endregion

        #region Getter

        protected override void getAllUniformLocations()
        {
            modelMatrix = base.getUniformLocation("modelMatrix");
            viewMatrix = base.getUniformLocation("viewMatrix");
            projectionMatrix = base.getUniformLocation("projectionMatrix");
            sunTexture1 = base.getUniformLocation("sunTexture1");
            sunTexture2 = base.getUniformLocation("sunTexture2");
            sunDirection = base.getUniformLocation("sunDirection");
            clipPlane = getUniformLocation("clipPlane");
        }

        #endregion

        #region Setter

        public void SetClipPlane(ref Vector4 clipPlane)
        {
            loadVector(this.clipPlane, clipPlane);
        }

        public void setUniformValues(ref Matrix4 modelMatrix, Matrix4 viewMatrix, ref Matrix4 projectionMatrix,
            DirectionalLight sun, Int32 sun1TexSampler, Int32 sun2TexSampler)
        {
            viewMatrix[3, 0] = 0.0f;
            viewMatrix[3, 1] = 0.0f;
            viewMatrix[3, 2] = 0.0f;
            loadMatrix(this.modelMatrix, false, modelMatrix);
            loadMatrix(this.viewMatrix, false, viewMatrix);
            loadMatrix(this.projectionMatrix, false, projectionMatrix);
            loadInteger(this.sunTexture1, sun1TexSampler);
            loadInteger(this.sunTexture2, sun2TexSampler);
            loadVector(sunDirection, sun.Direction);
        }

        #endregion

        protected override void SetShaderMacros()
        {
            SetDefine(ShaderTypeFlag.FragmentShader, "rCoef", "1.2000000");
            SetDefine(ShaderTypeFlag.FragmentShader, "gCoef", "0.7692307");
            SetDefine(ShaderTypeFlag.FragmentShader, "bCoef", "0.5555555");
        }

        #region Constructor

        public SunShader(string VSPath, string FSPath)
            : base(VSPath, FSPath)
        {
            if (base.ShaderLoaded)
            {
                base.showCompileLogInfo(SHADER_NAME);
                base.showLinkLogInfo(SHADER_NAME);
                Debug.Log.addToLog(getCompileLogInfo(SHADER_NAME));
                Debug.Log.addToLog(getLinkLogInfo(SHADER_NAME));
            }
            else Debug.Log.addToLog( DateTime.Now.ToString() + "  " + SHADER_NAME + " : shader file(s) not found!");
        }

        #endregion
    }
}
