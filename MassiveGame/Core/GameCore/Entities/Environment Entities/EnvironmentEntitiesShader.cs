using System;
using OpenTK;
using MassiveGame.Core.RenderCore;
using ShaderPattern;

namespace MassiveGame.Core.GameCore.Entities.EnvironmentEntities
{
    public class EnvironmentEntitiesShader : ShaderBase
    {
        #region Definitions

        private const string SHADER_NAME = "Env. entity shader";

        Uniform u_modelMatrix, u_viewMatrix, u_projectionMatrix, u_modelTexSampler, u_envMapSampler, u_cameraPosition;

        #endregion

        #region Getter

        protected override void getAllUniformLocations()
        {
            u_modelMatrix = GetUniform("modelMatrix");
            u_viewMatrix = GetUniform("viewMatrix");
            u_projectionMatrix = GetUniform("projectionMatrix");
            u_modelTexSampler = GetUniform("modelTexture");
            u_envMapSampler = GetUniform("environmentMap");
            u_cameraPosition = GetUniform("cameraPosition");
        }

        #endregion

        #region Setter

        public void setUniformValues(ref Matrix4 modelMatrix, Matrix4 viewMatrix, ref Matrix4 projectionMatrix,
            Vector3 cameraPosition, Int32 modelTexSampler, Int32 envMapSampler)
        {
            u_modelMatrix.LoadUniform(ref modelMatrix);

            u_viewMatrix.LoadUniform(ref viewMatrix);
            u_projectionMatrix.LoadUniform(ref projectionMatrix);
            u_cameraPosition.LoadUniform(ref cameraPosition);
            u_modelTexSampler.LoadUniform(modelTexSampler);
            u_envMapSampler.LoadUniform(envMapSampler);
        }

        #endregion

        protected override void SetShaderMacros()
        {
        }

        #region Constructor

        public EnvironmentEntitiesShader() : base() { }

        public EnvironmentEntitiesShader(string vsPath, string fsPath)
            : base(SHADER_NAME, vsPath, fsPath)
        {
        }

        #endregion
    }
}
