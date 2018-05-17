using OpenTK;
using ShaderPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTKWinForm.RenderCore
{
    public class BasicShader : Shader
    {
        private Int32 worldMatrix, viewMatrix, projectionMatrix,
            diffuseTexture, opacity;

        public void SetTransformatrionMatrices(Matrix4 worldMatrix, Matrix4 viewMatrix, Matrix4 projectionMatrix)
        {
            loadMatrix(this.worldMatrix, false, worldMatrix);
            loadMatrix(this.viewMatrix, false, viewMatrix);
            loadMatrix(this.projectionMatrix, false, projectionMatrix);
        }

        public void SetDiffuseTexture(Int32 diffuseTexture)
        {
            loadInteger(this.diffuseTexture, diffuseTexture);
        }

        public void SetOpacity(float opacity)
        {
            loadFloat(this.opacity, opacity);
        }

        protected override void getAllUniformLocations()
        {
            worldMatrix = getUniformLocation("worldMatrix");
            viewMatrix = getUniformLocation("viewMatrix");
            projectionMatrix = getUniformLocation("projectionMatrix");
            diffuseTexture = getUniformLocation("diffuseTexture");
            opacity = getUniformLocation("opacity");
        }

        protected override void SetShaderMacros()
        {
           
        }

        public BasicShader(string VertexShaderFile, string FragmentShaderFile) : base(VertexShaderFile, FragmentShaderFile)
        {
            if (ShaderLoaded)
            {
                showCompileLogInfo("BasicShader");
                showLinkLogInfo("BasicShader");
            }
        }
    }
}
