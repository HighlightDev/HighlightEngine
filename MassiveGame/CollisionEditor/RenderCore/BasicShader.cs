using OpenTK;
using ShaderPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollisionEditor.RenderCore
{
    public class BasicShader : Shader
    {
        private Uniform u_worldMatrix, u_viewMatrix, u_projectionMatrix,
            u_diffuseTexture, u_opacity;

        public void SetTransformatrionMatrices(Matrix4 worldMatrix, Matrix4 viewMatrix, Matrix4 projectionMatrix)
        {
            u_worldMatrix.LoadUniform(ref worldMatrix);
            u_viewMatrix.LoadUniform(ref viewMatrix);
            u_projectionMatrix.LoadUniform(ref projectionMatrix);
        }

        public void SetDiffuseTexture(Int32 diffuseTexture)
        {
            u_diffuseTexture.LoadUniform(diffuseTexture);
        }

        public void SetOpacity(float opacity)
        {
            u_opacity.LoadUniform(opacity);
        }

        protected override void getAllUniformLocations()
        {
            u_worldMatrix = GetUniform("worldMatrix");
            u_viewMatrix = GetUniform("viewMatrix");
            u_projectionMatrix = GetUniform("projectionMatrix");
            u_diffuseTexture = GetUniform("diffuseTexture");
            u_opacity = GetUniform("opacity");
        }

        protected override void SetShaderMacros()
        {
           
        }

        public BasicShader(string VertexShaderFile, string FragmentShaderFile) : base("Basic Shader", VertexShaderFile, FragmentShaderFile)
        {
            if (m_shaderLoaded)
            {
                showCompileLogInfo("BasicShader");
                showLinkLogInfo("BasicShader");
            }
        }
    }
}
