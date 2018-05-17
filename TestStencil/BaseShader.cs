using OpenTK;
using ShaderPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestStencil
{
    public class BaseShader : Shader
    {
        private Int32 worldMatrix, viewMatrix, projectionMatrix, color;

        public BaseShader(string VertexShaderFile, string FragmentShaderFile) : base(VertexShaderFile, FragmentShaderFile)
        {
            if (ShaderLoaded)
            {
                showCompileLogInfo("Base Shader");
                showLinkLogInfo("Base Shader");
            }
        }

        public void SetTransformationMatrices(ref Matrix4 worldMatrix, ref Matrix4 viewMatrix, ref Matrix4 projectionMatrix)
        {
            loadMatrix(this.worldMatrix, false, worldMatrix);
            loadMatrix(this.viewMatrix, false, viewMatrix);
            loadMatrix(this.projectionMatrix, false, projectionMatrix);
        }

        public void SetColor(Vector3 color)
        {
            loadVector(this.color, color);
        }

        protected override void getAllUniformLocations()
        {
            worldMatrix = getUniformLocation("worldMatrix");
            viewMatrix = getUniformLocation("viewMatrix");
            projectionMatrix = getUniformLocation("projectionMatrix");
            color = getUniformLocation("color");
        }

        protected override void SetShaderMacros()
        {
            
        }
    }
}
