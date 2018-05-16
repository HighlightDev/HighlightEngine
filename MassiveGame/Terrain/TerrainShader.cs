using OpenTK;
using MassiveGame.RenderCore.Lights;
using ShaderPattern;
using System;
using System.Collections.Generic;

namespace MassiveGame
{
    public class TerrainShader : Shader
    {
        #region Definitions

        /* Max available count of light sources */
        private static int MAX_LIGHTS_COUNT = DOUEngine.MAX_LIGHT_COUNT;
        
        private const string SHADER_NAME = "Terrain Shader";
        private int backTexture, rTexture, gTexture,
            bTexture, blendMap, materialAmbient, materialDiffuse,
            ModelMatrix, ViewMatrix, ProjectionMatrix, sunDirection,
            sunAmbientColour, sunDiffuseColour, sunEnable, clipPlane,
            mistEnable, mistDensity, mistGradient, mistColour;
        private int[] lightPosition = new int[MAX_LIGHTS_COUNT],
            attenuation = new int[MAX_LIGHTS_COUNT],
            diffuseColour = new int[MAX_LIGHTS_COUNT],
            enableLight = new int[MAX_LIGHTS_COUNT];

        /*  Triggers to enable normal mapping for multitextured terrain */
        private int enableNMr, enableNMg, enableNMb, enableNMblack;
        /*  Normal map sampler for each rgb component */
        private int normalMapR, normalMapG, normalMapB, normalMapBlack;
        private int directionalLightShadowMap, directionalLightShadowMatrix;
          
        #endregion

        #region Getter

        protected override void getAllUniformLocations()
        {
            backTexture = base.getUniformLocation("backgroundTexture");
            rTexture = base.getUniformLocation("rTexture");
            gTexture = base.getUniformLocation("gTexture");
            bTexture = base.getUniformLocation("bTexture");
            blendMap = base.getUniformLocation("blendMap");
            materialAmbient = base.getUniformLocation("materialAmbient");
            materialDiffuse = base.getUniformLocation("materialDiffuse");
            ModelMatrix = base.getUniformLocation("ModelMatrix");
            ViewMatrix = base.getUniformLocation("ViewMatrix");
            ProjectionMatrix = base.getUniformLocation("ProjectionMatrix");
            sunDirection = base.getUniformLocation("sunDirection");
            sunAmbientColour = base.getUniformLocation("sunAmbientColour");
            sunDiffuseColour = base.getUniformLocation("sunDiffuseColour");
            sunEnable = base.getUniformLocation("sunEnable");
            for (int i = 0; i < MAX_LIGHTS_COUNT; i++)
            {
                lightPosition[i] = base.getUniformLocation("lightPosition[" + i + "]");
                attenuation[i] = base.getUniformLocation("attenuation[" + i + "]");
                diffuseColour[i] = base.getUniformLocation("diffuseColour[" + i + "]");
                enableLight[i] = base.getUniformLocation("enableLight[" + i + "]");
            }
            clipPlane = base.getUniformLocation("clipPlane");
            enableNMr = base.getUniformLocation("enableNMr");
            enableNMg = base.getUniformLocation("enableNMg");
            enableNMb = base.getUniformLocation("enableNMb");
            enableNMblack = base.getUniformLocation("enableNMblack");
            normalMapR = base.getUniformLocation("normalMapR");
            normalMapG = base.getUniformLocation("normalMapG");
            normalMapB = base.getUniformLocation("normalMapB");
            normalMapBlack = base.getUniformLocation("normalMapBlack");
            mistEnable = base.getUniformLocation("mistEnable");
            mistDensity = base.getUniformLocation("mistDensity");
            mistGradient = base.getUniformLocation("mistGradient");
            mistColour = base.getUniformLocation("mistColour");
            directionalLightShadowMap = base.getUniformLocation("dirLightShadowMap");
            directionalLightShadowMatrix = base.getUniformLocation("dirLightShadowMatrix");
        }

        #endregion

        #region Setter

        public void SetDirectionalLightShadowMap(Int32 shadowMapSampler)
        {
            base.loadInteger(this.directionalLightShadowMap, shadowMapSampler);
        }

        public void SetDirectionalLightShadowMatrix(Matrix4 ShadowMatrix)
        {
            base.loadMatrix(directionalLightShadowMatrix, false, ShadowMatrix);
        }

        public void SetBlendMap(Int32 blendMapSampler)
        {
            loadInteger(blendMap, blendMapSampler);
        }

        public void SetTextureR(Int32 textureSamplerR, Int32 normalMapSamplerR, bool bEnableNM)
        {
            loadInteger(rTexture, textureSamplerR);
            if (bEnableNM)
                loadInteger(normalMapR, normalMapSamplerR);
            loadBool(enableNMr, bEnableNM);
        }

        public void SetTextureG(Int32 textureSamplerG, Int32 normalMapSamplerG, bool bEnableNM)
        {
            loadInteger(gTexture, textureSamplerG);
            if (bEnableNM)
                loadInteger(normalMapG, normalMapSamplerG);
            loadBool(enableNMg, bEnableNM);
        }

        public void SetTextureB(Int32 textureSamplerB, Int32 normalMapSamplerB, bool bEnableNM)
        {
            loadInteger(bTexture, textureSamplerB);
            if (bEnableNM)
                loadInteger(normalMapB, normalMapSamplerB);
            loadBool(enableNMb, bEnableNM);
        }

        public void SetTextureBlack(Int32 textureSamplerBlack, Int32 normalMapSamplerBlack, bool bEnableNM)
        {
            loadInteger(backTexture, textureSamplerBlack);
            if (bEnableNM)
                loadInteger(normalMapBlack, normalMapSamplerBlack);
            loadBool(enableNMblack, bEnableNM);
        }

        public void SetTransformationMatrices(ref Matrix4 ModelMatrix, Matrix4 ViewMatrix, ref Matrix4 ProjectionMatrix)
        {
            base.loadMatrix(this.ModelMatrix, false, ModelMatrix);
            base.loadMatrix(this.ViewMatrix, false, ViewMatrix);
            base.loadMatrix(this.ProjectionMatrix, false, ProjectionMatrix);
        }

        public void SetMaterial(Material material)
        {
            base.loadVector(this.materialAmbient, material.Ambient.Xyz);
            base.loadVector(this.materialDiffuse, material.Diffuse.Xyz);
        }

        public void SetDirectionalLight(DirectionalLight Sun)
        {
            /*If sun is enabled*/
            if (Sun != null)
            {
                base.loadBool(this.sunEnable, true);
                base.loadVector(this.sunDirection, Sun.Direction);
                base.loadVector(this.sunAmbientColour, new Vector3(Sun.Ambient));
                base.loadVector(this.sunDiffuseColour, new Vector3(Sun.Diffuse));
            }
            else { base.loadBool(this.sunEnable, false); }
        }

        public void SetPointLights(List<PointLight> lights)
        {
            /*If point lights are enabled*/
            if (lights != null)
            {
                for (int i = 0; i < (lights.Count <= MAX_LIGHTS_COUNT ? lights.Count : MAX_LIGHTS_COUNT); i++) //Включенные источники света
                {
                    base.loadBool(this.enableLight[i], true);
                    base.loadVector(lightPosition[i], lights[i].Position.Xyz);
                    base.loadVector(attenuation[i], lights[i].Attenuation);
                    base.loadVector(diffuseColour[i], lights[i].Diffuse.Xyz);
                }
                for (int i = lights.Count; i < MAX_LIGHTS_COUNT; i++)      //Выключенные источники света
                {
                    base.loadBool(this.enableLight[i], false);
                }
            }
            else
            {
                for (int i = 0; i < MAX_LIGHTS_COUNT; i++)      //Выключенные источники света
                {
                    base.loadBool(this.enableLight[i], false);
                }
            }
        }

        public void SetClippingPlane(ref Vector4 clippingPlane)
        {
            base.loadVector(this.clipPlane, clippingPlane);
        }

        public void SetMist(MistComponent mist)
        {
            if (mist != null)
            {
                base.loadBool(this.mistEnable, true);
                base.loadFloat(this.mistGradient, mist.MistGradient);
                base.loadFloat(this.mistDensity, mist.MistDensity);
                base.loadVector(this.mistColour, mist.MistColour);
            }
            else
            {
                base.loadBool(this.mistEnable, false);
            }
        }

        #endregion


        protected override void SetShaderMacros()
        {
            SetDefine(ShaderTypeFlag.VertexShader | ShaderTypeFlag.FragmentShader, "LIGHT_COUNT", DOUEngine.MAX_LIGHT_COUNT.ToString());
            SetDefine(ShaderTypeFlag.FragmentShader, "MAX_MIST_VISIBLE_AREA", "0.95");
        }

        #region Constructor

        public TerrainShader(string VertexShaderFile, string FragmentShaderFile)
            : base(VertexShaderFile, FragmentShaderFile)
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

