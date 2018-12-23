using System;
using OpenTK;
using MassiveGame.Core.RenderCore;
using MassiveGame.Core.RenderCore.Lights;
using ShaderPattern;
using MassiveGame.Core.DebugCore;

namespace MassiveGame.Core.GameCore.Sun
{
    public class SunShader : ShaderBase
    {
        #region Definitions 

        private const string SHADER_NAME = "Sun shader";
        private Uniform u_modelMatrix, u_viewMatrix, u_projectionMatrix, u_sunTexture1, u_sunTexture2,
            u_sunDirection, u_clipPlane;

        #endregion

        #region Constructor

        public SunShader() : base() { }

        public SunShader(string VSPath, string FSPath)
            : base(SHADER_NAME, VSPath, FSPath) { }

        #endregion

        #region Getter

        protected override void getAllUniformLocations()
        {
            try
            {
                u_modelMatrix = GetUniform("modelMatrix");
                u_viewMatrix = GetUniform("viewMatrix");
                u_projectionMatrix = GetUniform("projectionMatrix");
                u_sunTexture1 = GetUniform("sunTexture1");
                u_sunTexture2 = GetUniform("sunTexture2");
                u_sunDirection = GetUniform("sunDirection");
                u_clipPlane = GetUniform("clipPlane");
            }
            catch (ArgumentNullException innerException)
            {
                Log.AddToFileStreamLog(innerException.Message);
                Log.AddToConsoleStreamLog(innerException.Message);
            }
        }

        #endregion

        #region Setter

        public void SetClipPlane(ref Vector4 clipPlane)
        {
            u_clipPlane.LoadUniform(ref clipPlane);
        }

        public void setUniformValues(ref Matrix4 modelMatrix, Matrix4 viewMatrix, ref Matrix4 projectionMatrix,
            DirectionalLight sun, Int32 sun1TexSampler, Int32 sun2TexSampler)
        {
            viewMatrix[3, 0] = 0.0f;
            viewMatrix[3, 1] = 0.0f;
            viewMatrix[3, 2] = 0.0f;
            u_modelMatrix.LoadUniform(ref modelMatrix);
            u_viewMatrix.LoadUniform(ref viewMatrix);
            u_projectionMatrix.LoadUniform(ref projectionMatrix);
            u_sunTexture1.LoadUniform(sun1TexSampler);
            u_sunTexture2.LoadUniform(sun2TexSampler);
            u_sunDirection.LoadUniform(sun.Direction);
        }

        #endregion

        protected override void SetShaderMacros()
        {
            SetDefine<float>(ShaderTypeFlag.FragmentShader, "rCoef", 1.2f);
            SetDefine<float>(ShaderTypeFlag.FragmentShader, "gCoef", 0.7692307f);
            SetDefine<float>(ShaderTypeFlag.FragmentShader, "bCoef", 0.5555555f);
        }

    }
}
