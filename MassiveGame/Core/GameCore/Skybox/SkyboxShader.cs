using MassiveGame.Core.GameCore.EntityComponents;
using MassiveGame.Core.RenderCore;
using MassiveGame.Core.RenderCore.Lights;
using OpenTK;
using ShaderPattern;
using System;

namespace MassiveGame.Core.GameCore.Skybox
{
    public class SkyboxShader : ShaderBase
    {
        #region Definitions

        private const string SHADER_NAME = "Skybox Shader";
        private Uniform u_modelMatrix, u_viewMatrix, u_projectionMatrix,
            u_skyboxDayTexture, u_skyboxNightTexture, u_dayCycleValue, u_mistEnable, u_mistColour,
            u_clipPlane;

        #endregion

        #region Getter

        protected override void getAllUniformLocations()
        {
            base.getAllUniformLocations();
            try
            {
                u_modelMatrix = GetUniform("modelMatrix");
                u_viewMatrix = GetUniform("viewMatrix");
                u_projectionMatrix = GetUniform("projectionMatrix");
                u_skyboxDayTexture = GetUniform("daySampler");
                u_skyboxNightTexture = GetUniform("nightSampler");
                u_dayCycleValue = GetUniform("dayCycleValue");
                u_mistEnable = GetUniform("mistEnable");
                u_mistColour = GetUniform("mistColour");
                u_clipPlane = GetUniform("clipPlane");
            }
            catch (ArgumentNullException innerException)
            {
                Debug.Log.AddToFileStreamLog(innerException.Message);
                Debug.Log.AddToConsoleStreamLog(innerException.Message);
            }
        }

        #endregion

        #region Setter

        public void SetClipPlane(ref Vector4 clipPlane)
        {
            u_clipPlane.LoadUniform(ref clipPlane);
        }

        public void SetTransformationMatrices(ref Matrix4 projectionMatrix, Matrix4 viewMatrix, ref Matrix4 modelMatrix)
        {
            viewMatrix[3, 0] = 0.0f;    //restrict x-translation
            viewMatrix[3, 1] = 0.0f;    //restrict y-translation
            viewMatrix[3, 2] = 0.0f;    //restrict z-translation
            u_modelMatrix.LoadUniform(ref modelMatrix);
            u_viewMatrix.LoadUniform(ref viewMatrix);
            u_projectionMatrix.LoadUniform(ref projectionMatrix);
        }

        public void SetDayCubeTexture(Int32 skyboxDayTexture)
        {
            u_skyboxDayTexture.LoadUniform(skyboxDayTexture);
        }

        public void SetNightCubeTexture(Int32 skyboxNightTexture)
        {
            u_skyboxNightTexture.LoadUniform(skyboxNightTexture);
        }

        public void SetDayCycleValue(float value)
        {
            u_dayCycleValue.LoadUniform(value);
        }

        public void SetMist(MistComponent mist)
        {
            bool bMistEnable = false;
            if (mist != null)
            {
                u_mistColour.LoadUniform(mist.MistColour);
                bMistEnable = true;
            }

            u_mistEnable.LoadUniform(bMistEnable);
        }

        #endregion

        protected override void SetShaderMacros()
        {
            SetDefine<float>(ShaderTypeFlag.FragmentShader, "UPPER_LIMIT", 30.0f);
            SetDefine<float>(ShaderTypeFlag.FragmentShader, "LOWER_LIMIT", 0.0f);
            SetDefine<int>(ShaderTypeFlag.FragmentShader, "COMPLEX_SKYBOX", 1);
        }

        #region Constructor

        public SkyboxShader() : base() { }

        public SkyboxShader(string VertexShaderFile, string FragmentShaderFile)
            : base(SHADER_NAME, VertexShaderFile, FragmentShaderFile)
        {
        }

        #endregion
    }
}
