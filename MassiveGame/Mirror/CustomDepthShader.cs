using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ShaderPattern;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace MassiveGame
{
    public class CustomDepthShader : Shader
    {
        private const string SHADER_NAME = "CustomDepthShader";
        private int modelMatrix, viewMatrix, projectionMatrix;

        protected override void getAllUniformLocations()
        {
            modelMatrix = base.getUniformLocation("modelMatrix");
            viewMatrix = base.getUniformLocation("viewMatrix");
            projectionMatrix = base.getUniformLocation("projectionMatrix");
        }

        public void SetUniformValues(Matrix4 modelMatrix, Matrix4 viewMatrix, Matrix4 projectionMatrix)
        {
            base.loadMatrix(this.modelMatrix, false, modelMatrix);
            base.loadMatrix(this.viewMatrix, false, viewMatrix);
            base.loadMatrix(this.projectionMatrix, false, projectionMatrix);
        }

        public CustomDepthShader(string vsPath, string fsPath)
            : base(vsPath, fsPath)
        {
           if (base.ShaderLoaded)
           {
               base.showCompileLogInfo(SHADER_NAME);
               base.showLinkLogInfo(SHADER_NAME);
           }
        }
    }
}
