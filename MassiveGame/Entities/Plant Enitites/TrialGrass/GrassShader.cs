using System;

using OpenTK;
using ShaderPattern;
using MassiveGame.RenderCore.Lights;

namespace MassiveGame
{
    public class GrassShader : Shader
    {
        private int modelMatrix, viewMatrix, projectionMatrix, angle, color, grassTexture, sunDirection,
            sunDiffuseColour, sunAmbientColour;
        private readonly string SHADER_NAME = "Grass Shader";

        protected override void getAllUniformLocations()
        {
            this.modelMatrix = base.getUniformLocation("modelMatrix");
            this.viewMatrix = base.getUniformLocation("viewMatrix");
            this.projectionMatrix = base.getUniformLocation("projectionMatrix");
            this.angle = base.getUniformLocation("angle");
            this.color = base.getUniformLocation("color");
            this.grassTexture = base.getUniformLocation("grassTexture");
            this.sunDirection = base.getUniformLocation("sunDirection");
            this.sunDiffuseColour = base.getUniformLocation("sunDiffuseColour");
            this.sunAmbientColour = base.getUniformLocation("sunAmbientColour");
        }

        public void setUniformValues(ref Matrix4 modelMatrix, Matrix4 viewMatrix, ref Matrix4 projectionMatrix, DirectionalLight sun, ref Vector3 color,
            ref float angle, int grassTextureSampler)
        {
            base.loadMatrix(this.modelMatrix, false, modelMatrix);
            base.loadMatrix(this.viewMatrix, false, viewMatrix);
            base.loadMatrix(this.projectionMatrix, false, projectionMatrix);
            base.loadVector(this.color, color);
            base.loadFloat(this.angle, angle);
            base.loadInteger(this.grassTexture, grassTextureSampler);
            try
            {
                base.loadVector(this.sunDirection, sun.Direction);
                base.loadVector(this.sunDiffuseColour, sun.Diffuse.Xyz);
                base.loadVector(this.sunAmbientColour, sun.Ambient.Xyz);
            }
            catch (NullReferenceException)
            {
                // error
                return;
            }
        }

        protected override void SetShaderMacros()
        {
        }

        public GrassShader(string vsPath, string fsPath, string gsPath)
            : base(vsPath, fsPath, gsPath)
        {
            if (base.ShaderLoaded)
            {
                base.showCompileLogInfo(SHADER_NAME);
                base.showLinkLogInfo(SHADER_NAME);
            }
        }
    }
}
