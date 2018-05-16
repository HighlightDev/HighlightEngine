using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using PhysicsBox;
using ShaderPattern;

namespace MassiveGame.Light_visualization
{
    public class LampShader : Shader
    {
        #region Definitions 

        private const string SHADER_NAME = "Light Visualization";
        int modelMatrix, viewMatrix, projectionMatrix, lampTexture;

        #endregion

        #region Getters uniform

        protected override void getAllUniformLocations()
        {
            this.modelMatrix = base.getUniformLocation("modelMatrix");
            this.viewMatrix = base.getUniformLocation("viewMatrix");
            this.projectionMatrix = base.getUniformLocation("projectionMatrix");
            this.lampTexture = base.getUniformLocation("lampTexture");
        }

        #endregion

        #region Setters uniform

        public void setUniformValues(Matrix4 modelMatrix, Matrix4 viewMatrix,
            Matrix4 projectionMatrix, int lampTextureSamplerID)
        {
            base.loadMatrix(this.modelMatrix, false, modelMatrix);
            base.loadMatrix(this.viewMatrix, false, viewMatrix);
            base.loadMatrix(this.projectionMatrix, false, projectionMatrix);
            base.loadInteger(this.lampTexture, lampTextureSamplerID);
        }

        public void setLampTexture(int lampTextureSamplerID)
        {
            base.loadInteger(this.lampTexture, lampTextureSamplerID);
        }

        #endregion

        protected override void SetShaderMacros()
        {
            SetDefine(ShaderTypeFlag.GeometryShader, "SIZE", "1.4");
        }

        #region Constructor

        public LampShader(string vsPath, string fsPath, string gsPath)
            : base(vsPath, fsPath, gsPath)
        {
            if (base.ShaderLoaded)
            {
                base.showCompileLogInfo(SHADER_NAME);
                base.showLinkLogInfo(SHADER_NAME);
                Debug.Log.addToLog(getCompileLogInfo(SHADER_NAME));
                Debug.Log.addToLog(getLinkLogInfo(SHADER_NAME));
            }
            else { Debug.Log.addToLog( DateTime.Now.ToString() + "  " + SHADER_NAME + "shader file(s) not found!"); }
        }

        #endregion
    }
}
