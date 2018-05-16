using OpenTK;
using MassiveGame.RenderCore.Lights;
using ShaderPattern;
using System;

namespace MassiveGame
{
    public class MirrorShader : Shader
    {
        #region Definitions

        private const string SHADER_NAME = "Mirror Shader";
        private static int MAX_LIGHTS_COUNT = DOUEngine.MAX_LIGHT_COUNT; //Максимальное количество источников света, доступных для обработки
        int modelMatrix, viewMatrix, projectionMatrix, normalMatrix,
            frameTexture, sunSpecular, sunPosition, sunEnable;
        int[] pointLSpecular = new int[MAX_LIGHTS_COUNT],
            pointLEnable = new int[MAX_LIGHTS_COUNT],
            pointLAttenuation = new int[MAX_LIGHTS_COUNT],
            pointLPosition = new int[MAX_LIGHTS_COUNT];

        #endregion

        #region Getter

        protected override void getAllUniformLocations()
        {
            this.modelMatrix = base.getUniformLocation("modelMatrix");
            this.viewMatrix = base.getUniformLocation("viewMatrix");
            this.projectionMatrix = base.getUniformLocation("projectionMatrix");
            this.normalMatrix = base.getUniformLocation("normalMatrix");
            this.frameTexture = base.getUniformLocation("frameTexture");
            this.sunSpecular = base.getUniformLocation("sunSpecular");
            this.sunPosition = base.getUniformLocation("sunPosition");
            this.sunEnable = base.getUniformLocation("sunEnable");
            for (int i = 0; i < MAX_LIGHTS_COUNT; i++)
            {
                this.pointLSpecular[i] = base.getUniformLocation("pointSpecular[" + i + "]");
                this.pointLPosition[i] = base.getUniformLocation("pointPosition[" + i + "]");
                this.pointLAttenuation[i] = base.getUniformLocation("pointAttenuation[" + i + "]");
                this.pointLEnable[i] = base.getUniformLocation("pointEnable[" + i + "]");
            }
        }


        #endregion

        #region Setter

        public void setUniforms(Matrix4 modelMatrix, Matrix4 viewMatrix, Matrix4 projectionMatrix, Matrix3 normalMatrix,
            int frameTexture, PointLight[] lights, DirectionalLight sun)
        {
            base.loadMatrix(this.modelMatrix, false, modelMatrix);
            base.loadMatrix(this.viewMatrix, false, viewMatrix);
            base.loadMatrix(this.projectionMatrix, false, projectionMatrix);
            base.loadNormalMatrix(this.normalMatrix, false, normalMatrix);
            base.loadInteger(this.frameTexture, frameTexture);
            /*True - load specular values 
             False - don't load values*/
            if (sun != null) 
            {
                base.loadVector(this.sunSpecular, new Vector3(sun.Specular));
                base.loadVector(this.sunPosition, new Vector3(sun.Direction));
                base.loadBool(this.sunEnable, true);
            }
            else { base.loadBool(this.sunEnable, false); }

            /*Load point lights values , if  they are enabled*/
            for (int i = 0; i < (lights.Length <= MAX_LIGHTS_COUNT ? lights.Length : MAX_LIGHTS_COUNT); i++)
            {
                base.loadVector(this.pointLSpecular[i], new Vector3(lights[i].Specular));
                base.loadVector(this.pointLPosition[i], new Vector3(lights[i].Position));
                base.loadVector(this.pointLAttenuation[i], lights[i].Attenuation);
                base.loadBool(this.pointLEnable[i], true);
            } 
            /*Don't load values for disabled lights*/
            for (int i = lights.Length; i < MAX_LIGHTS_COUNT; i++)
            {
                base.loadBool(this.pointLEnable[i], false);
            }
             
        }

        #endregion

        protected override void SetShaderMacros()
        { 
            SetDefine(ShaderTypeFlag.VertexShader | ShaderTypeFlag.FragmentShader, "MAX_LIGHT_COUNT", DOUEngine.MAX_LIGHT_COUNT.ToString());
            SetDefine(ShaderTypeFlag.FragmentShader, "shininess", "150");
        }

        #region Constructor

        public MirrorShader(string vsPath, string fsPath)
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
