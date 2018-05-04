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
    public class EnvironmentEntitiesShader : Shader
    {
        #region Definitions

        private const string SHADER_NAME = "Env. entity shader";
        private int modelMatrix, viewMatrix, projectionMatrix, modelTexSampler, envMapSampler,
            cameraPosition, iorValues;

        #endregion

        #region Getter

        protected override void getAllUniformLocations()
        {
            modelMatrix = base.getUniformLocation("modelMatrix");
            viewMatrix = base.getUniformLocation("viewMatrix");
            projectionMatrix = base.getUniformLocation("projectionMatrix");
            modelTexSampler = base.getUniformLocation("modelTexture");
            envMapSampler = base.getUniformLocation("environmentMap");
            cameraPosition = base.getUniformLocation("cameraPosition");
            iorValues = base.getUniformLocation("IOR");
        }

        #endregion

        #region Setter

        public void setUniformValues(ref Matrix4 modelMatrix, Matrix4 viewMatrix, ref Matrix4 projectionMatrix,
            Vector3 cameraPosition, int modelTexSampler, int envMapSampler)
        {
            base.loadMatrix(this.modelMatrix, false, modelMatrix);
            base.loadMatrix(this.viewMatrix, false, viewMatrix);
            base.loadMatrix(this.projectionMatrix, false, projectionMatrix);
            base.loadVector(this.cameraPosition, cameraPosition);
            base.loadInteger(this.modelTexSampler, modelTexSampler);
            base.loadInteger(this.envMapSampler, envMapSampler);
        }

        #endregion

        protected override void SetShaderMacros()
        {
        }

        #region Constructor

        public EnvironmentEntitiesShader(string vsPath, string fsPath)
            : base(vsPath, fsPath)
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
