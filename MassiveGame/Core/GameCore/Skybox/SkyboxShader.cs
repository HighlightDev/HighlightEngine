using MassiveGame.Core.RenderCore;
using MassiveGame.Core.RenderCore.Lights;
using OpenTK;
using System;

namespace MassiveGame.Core.GameCore.Skybox
{
    public class SkyboxShader : ShaderBase
    {
        #region Definitions

        private const string SHADER_NAME = "Skybox Shader";
        private Int32 modelMatrix, viewMatrix, projectionMatrix,
            skyboxDayTexture,
            skyboxNightTexture,
            sunPosition,
            sunEnable,
            mistEnable,
            mistColour,
            clipPlane;

        #endregion

        #region Getter

        protected override void getAllUniformLocations()
        {
            modelMatrix = base.getUniformLocation("modelMatrix");
            viewMatrix = base.getUniformLocation("ViewMatrix");
            projectionMatrix = base.getUniformLocation("ProjectionMatrix");
            skyboxDayTexture = base.getUniformLocation("skyboxSampler");
            skyboxNightTexture = base.getUniformLocation("skyboxSampler2");
            sunPosition = base.getUniformLocation("sunPosition");
            sunEnable = base.getUniformLocation("sunEnable");
            mistEnable = base.getUniformLocation("mistEnable");
            mistColour = base.getUniformLocation("mistColour");
            clipPlane = getUniformLocation("clipPlane");
        }

        #endregion

        #region Setter

        public void SetClipPlane(ref Vector4 clipPlane)
        {
            loadVector(this.clipPlane, clipPlane);
        }

        public void setAllUniforms(Matrix4 modelMatrix, Matrix4 viewMatrix,
            Matrix4 projectionMatrix, Int32 skyboxDayTexture, Int32 skyboxNightTexture, DirectionalLight sun, bool mistEnable, Vector3 mistColour)
        {
            viewMatrix[3, 0] = 0.0f;    //restrict x-translation
            viewMatrix[3, 1] = 0.0f;    //restrict y-translation
            viewMatrix[3, 2] = 0.0f;    //restrict z-translation
            base.loadMatrix(this.modelMatrix, false, modelMatrix);
            base.loadMatrix(this.viewMatrix, false, viewMatrix);
            base.loadMatrix(this.projectionMatrix, false, projectionMatrix);
            base.loadInteger(this.skyboxDayTexture, skyboxDayTexture);
            base.loadInteger(this.skyboxNightTexture, skyboxNightTexture);
            if (sun != null)
            {
                base.loadVector(this.sunPosition, sun.Direction);
                base.loadBool(this.sunEnable, true);
            }
            else { base.loadBool(this.sunEnable, false); }
            base.loadBool(this.mistEnable, mistEnable);
            base.loadVector(this.mistColour, mistColour);
        }

        #endregion

        protected override void SetShaderMacros()
        {
            SetDefine<Vector3>(ShaderTypeFlag.FragmentShader, "zenith", new Vector3(0, 1, 0));
            SetDefine<float>(ShaderTypeFlag.FragmentShader, "UPPER_LIMIT", 30.0f);
            SetDefine<float>(ShaderTypeFlag.FragmentShader, "LOWER_LIMIT", 0.0f);
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
