using System;
using OpenTK;
using MassiveGame.Core.RenderCore;

namespace MassiveGame.Core.GameCore.Entities.EnvironmentEntities
{
    public class EnvironmentEntitiesShader : ShaderBase
    {
        #region Definitions

        private const string SHADER_NAME = "Env. entity shader";

        private Int32 modelMatrix, viewMatrix, projectionMatrix, modelTexSampler, envMapSampler,
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
            Vector3 cameraPosition, Int32 modelTexSampler, Int32 envMapSampler)
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
            : base(SHADER_NAME, vsPath, fsPath)
        {
        }

        #endregion
    }
}
