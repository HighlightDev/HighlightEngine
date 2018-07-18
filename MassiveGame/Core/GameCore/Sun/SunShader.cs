using System;
using OpenTK;
using MassiveGame.Core.RenderCore;
using MassiveGame.Core.RenderCore.Lights;

namespace MassiveGame.Core.GameCore.Sun
{
    public class SunShader : ShaderBase
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
            SetDefine<float>(ShaderTypeFlag.FragmentShader, "rCoef", 1.2f);
            SetDefine<float>(ShaderTypeFlag.FragmentShader, "gCoef", 0.7692307f);
            SetDefine<float>(ShaderTypeFlag.FragmentShader, "bCoef", 0.5555555f);
        }

        #region Constructor

        public SunShader() : base() { }

        public SunShader(string VSPath, string FSPath)
            : base(SHADER_NAME, VSPath, FSPath)
        {
        }

        #endregion
    }
}
