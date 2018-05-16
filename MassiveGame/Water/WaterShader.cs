using System;
using System.Collections.Generic;
using ShaderPattern;
using OpenTK;
using MassiveGame.RenderCore.Lights;

namespace MassiveGame
{
    public class WaterShader : Shader
    {
        #region Definitions

        private const string SHADER_NAME = "Water Shader";
        private static int MAX_LIGHTS_COUNT = DOUEngine.MAX_LIGHT_COUNT; //Максимальное количество источников света, доступных для обработки
        int modelMatrix, viewMatrix, projectionMatrix, reflectionTexture, refractionTexture,
            dudvTexture, normalMap, depthTexture, cameraPosition, moveFactor, waveStrength, sunPos, sunSpecularColour,
            nearClipPlane, farClipPlane, transparencyDepth, mistEnable, mistDensity, mistGradient, mistColour;

        private int[] lightPosition = new int[MAX_LIGHTS_COUNT],
           attenuation = new int[MAX_LIGHTS_COUNT],
           specularColour = new int[MAX_LIGHTS_COUNT],
           enableLight = new int[MAX_LIGHTS_COUNT];

        #endregion

        #region Uniforms setter

        public void setUniformValues(ref Matrix4 modelMatrix, Matrix4 viewMatrix,
            ref Matrix4 projectionMatrix, int reflectionSampler, int refractionSampler, int dudvSampler, int normalMap, int depthTexture,
            Vector3 cameraPosition, float moveFactor, float waveStrength, DirectionalLight sun, List<PointLight> lights,
            ref float nearClipPlane, ref float farClipPlane, float transparencyDepth, bool mistEnable, float mistDensity, float mistGradient,
            Vector3 mistColour)
        {
            base.loadMatrix(this.modelMatrix, false, modelMatrix);
            base.loadMatrix(this.viewMatrix, false, viewMatrix);
            base.loadMatrix(this.projectionMatrix, false, projectionMatrix);
            base.loadInteger(this.reflectionTexture, reflectionSampler);
            base.loadInteger(this.refractionTexture, refractionSampler);
            base.loadInteger(this.dudvTexture, dudvSampler);
            base.loadInteger(this.normalMap, normalMap);
            base.loadInteger(this.depthTexture, depthTexture);
            base.loadVector(this.cameraPosition, cameraPosition);
            base.loadFloat(this.moveFactor, moveFactor);
            base.loadFloat(this.waveStrength, waveStrength);
            base.loadFloat(this.nearClipPlane, nearClipPlane);
            base.loadFloat(this.farClipPlane, farClipPlane);
            base.loadFloat(this.transparencyDepth, transparencyDepth);

            if (sun != null)
            {
                base.loadVector(this.sunPos, sun.Direction);
                base.loadVector(this.sunSpecularColour, new Vector3(sun.Specular));
            }

            if (lights != null)
            {
                for (int i = 0; i < (lights.Count <= MAX_LIGHTS_COUNT ? lights.Count : MAX_LIGHTS_COUNT); i++) //Включенные источники света
                {
                    base.loadBool(this.enableLight[i], true);
                    base.loadVector(lightPosition[i], new Vector3(lights[i].Position.X, lights[i].Position.Y, lights[i].Position.Z));
                    base.loadVector(attenuation[i], new Vector3(lights[i].Attenuation.X, lights[i].Attenuation.Y, lights[i].Attenuation.Z));
                    base.loadVector(specularColour[i], new Vector3(lights[i].Specular.X, lights[i].Specular.Y, lights[i].Specular.Z));
                }
                for (int i = lights.Count; i < MAX_LIGHTS_COUNT; i++)      //Выключенные источники света
                {
                    base.loadBool(this.enableLight[i], false);
                }
            }

            base.loadBool(this.mistEnable, mistEnable);
            base.loadFloat(this.mistDensity, mistDensity);
            base.loadFloat(this.mistGradient, mistGradient);
            base.loadVector(this.mistColour, mistColour);
        }

        #endregion

        #region Uniforms getter

        protected override void getAllUniformLocations()
        {
            this.modelMatrix = base.getUniformLocation("modelMatrix");
            this.viewMatrix = base.getUniformLocation("viewMatrix");
            this.projectionMatrix = base.getUniformLocation("projectionMatrix");
            this.reflectionTexture = base.getUniformLocation("reflectionTexture");
            this.refractionTexture = base.getUniformLocation("refractionTexture");
            this.cameraPosition = base.getUniformLocation("cameraPosition");
            this.dudvTexture = base.getUniformLocation("dudvTexture");
            this.normalMap = base.getUniformLocation("normalMap");
            this.depthTexture = base.getUniformLocation("depthTexture");
            this.moveFactor = base.getUniformLocation("moveFactor");
            this.sunPos = base.getUniformLocation("sunPos");
            this.sunSpecularColour = base.getUniformLocation("sunSpecularColour");
            this.waveStrength = base.getUniformLocation("waveStrength");
            this.farClipPlane = base.getUniformLocation("farClipPlane");
            this.nearClipPlane = base.getUniformLocation("nearClipPlane");
            this.transparencyDepth = base.getUniformLocation("transparencyDepth");

            this.mistEnable = base.getUniformLocation("mistEnable");
            this.mistDensity = base.getUniformLocation("mistDensity");
            this.mistGradient = base.getUniformLocation("mistGradient");
            this.mistColour = base.getUniformLocation("mistColour");

            for (int i = 0; i < MAX_LIGHTS_COUNT; i++)
            {
                lightPosition[i] = base.getUniformLocation("lightPosition[" + i + "]");
                attenuation[i] = base.getUniformLocation("attenuation[" + i + "]");
                specularColour[i] = base.getUniformLocation("specularColour[" + i + "]");
                enableLight[i] = base.getUniformLocation("enableLight[" + i + "]");
            }
        }

        #endregion

        protected override void SetShaderMacros()
        {
            SetDefine(ShaderTypeFlag.VertexShader | ShaderTypeFlag.FragmentShader, "MAX_LIGHT_COUNT", DOUEngine.MAX_LIGHT_COUNT.ToString());
            SetDefine(ShaderTypeFlag.VertexShader, "tiling", "3.0");
            SetDefine(ShaderTypeFlag.FragmentShader, "materialReflectivity", "1.1f");
            SetDefine(ShaderTypeFlag.FragmentShader, "materialShineDamper", "100.0f");
        }

        #region Constructor

        public WaterShader(string vsPath, string fsPath)
            : base(vsPath, fsPath)
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
