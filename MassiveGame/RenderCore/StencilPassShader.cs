using OpenTK;
using ShaderPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassiveGame.RenderCore
{
    public class StencilPassShader : Shader
    {
        private const string SHADER_NAME = "STENCIL PASS";
        private Int32 projectionMatrix, viewMatrix, worldMatrix;

        public StencilPassShader(string VertexShaderFile, string FragmentShaderFile) : base(VertexShaderFile, FragmentShaderFile)
        {
            if (ShaderLoaded)
            {
                showCompileLogInfo(SHADER_NAME);
                showLinkLogInfo(SHADER_NAME);
                Debug.Log.addToLog(getCompileLogInfo(SHADER_NAME));
                Debug.Log.addToLog(getLinkLogInfo(SHADER_NAME));
            }
        }

        protected override void getAllUniformLocations()
        {
            projectionMatrix = base.getUniformLocation("projectionMatrix");
            viewMatrix = base.getUniformLocation("viewMatrix");
            worldMatrix = base.getUniformLocation("worldMatrix");
        }

        public void SetUniformVariables(ref Matrix4 projectionMatrix, Matrix4 viewMatrix, ref Matrix4 worldMatrix)
        {
            loadMatrix(this.projectionMatrix, false, projectionMatrix);
            loadMatrix(this.viewMatrix, false, viewMatrix);
            loadMatrix(this.worldMatrix, false, worldMatrix);
        }

        protected override void SetShaderMacros()
        {  
        }
    }
}
