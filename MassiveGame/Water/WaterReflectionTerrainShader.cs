using MassiveGame.RenderCore.Lights;
using OpenTK;
using ShaderPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassiveGame.Water
{
    public class WaterReflectionTerrainShader : Shader
    {
        private const string SHADER_NAME = "WaterReflectionTerrainShader";

        private int backTexture, rTexture, gTexture,
              bTexture, blendMap, materialAmbient, materialDiffuse,
              MirrorMatrix, ModelMatrix, ViewMatrix, ProjectionMatrix, sunDirection,
              sunAmbientColour, sunDiffuseColour, sunEnable, clipPlane;
              
        public WaterReflectionTerrainShader(string VertexShaderFile, string FragmentShaderFile) : base(VertexShaderFile, FragmentShaderFile)
        {
            if (ShaderLoaded)
            {
                showCompileLogInfo(SHADER_NAME);
                showLinkLogInfo(SHADER_NAME);
                Debug.Log.addToLog(DateTime.Now.ToString() + "  " + base.getCompileLogInfo(SHADER_NAME));
                Debug.Log.addToLog(DateTime.Now.ToString() + "  " + base.getLinkLogInfo(SHADER_NAME));
            }
        }

        protected override void getAllUniformLocations()
        {
            backTexture = base.getUniformLocation("backgroundTexture");
            rTexture = base.getUniformLocation("rTexture");
            gTexture = base.getUniformLocation("gTexture");
            bTexture = base.getUniformLocation("bTexture");
            blendMap = base.getUniformLocation("blendMap");
            materialAmbient = base.getUniformLocation("materialAmbient");
            materialDiffuse = base.getUniformLocation("materialDiffuse");
            ModelMatrix = base.getUniformLocation("ModelMatrix");
            MirrorMatrix = base.getUniformLocation("MirrorMatrix");
            ViewMatrix = base.getUniformLocation("ViewMatrix");
            ProjectionMatrix = base.getUniformLocation("ProjectionMatrix");
            sunDirection = base.getUniformLocation("sunDirection");
            sunAmbientColour = base.getUniformLocation("sunAmbientColour");
            sunDiffuseColour = base.getUniformLocation("sunDiffuseColour");
            sunEnable = base.getUniformLocation("sunEnable");
            clipPlane = base.getUniformLocation("clipPlane");
        }

        public void SetBlendMap(Int32 blendMapSampler)
        {
            loadInteger(blendMap, blendMapSampler);
        }

        public void SetTextureR(Int32 textureSamplerR)
        {
            loadInteger(rTexture, textureSamplerR);
        }

        public void SetTextureG(Int32 textureSamplerG)
        {
            loadInteger(gTexture, textureSamplerG);
        }

        public void SetTextureB(Int32 textureSamplerB)
        {
            loadInteger(bTexture, textureSamplerB);
        }

        public void SetTextureBlack(Int32 textureSamplerBlack)
        {
            loadInteger(backTexture, textureSamplerBlack);
        }

        public void SetTransformationMatrices(ref Matrix4 MirrorMatrix, ref Matrix4 ModelMatrix, Matrix4 ViewMatrix, ref Matrix4 ProjectionMatrix)
        {
            base.loadMatrix(this.ModelMatrix, false, ModelMatrix);
            base.loadMatrix(this.ViewMatrix, false, ViewMatrix);
            base.loadMatrix(this.ProjectionMatrix, false, ProjectionMatrix);
            base.loadMatrix(this.MirrorMatrix, false, MirrorMatrix);
        }

        public void SetMaterial(Material material)
        {
            base.loadVector(this.materialAmbient, material.Ambient.Xyz);
            base.loadVector(this.materialDiffuse, material.Diffuse.Xyz);
        }

        public void SetDirectionalLight(DirectionalLight Sun)
        {
            /*If sun is enabled*/
            if (Sun != null)
            {
                base.loadBool(this.sunEnable, true);
                base.loadVector(this.sunDirection, Sun.Direction);
                base.loadVector(this.sunAmbientColour, new Vector3(Sun.Ambient));
                base.loadVector(this.sunDiffuseColour, new Vector3(Sun.Diffuse));
            }
            else { base.loadBool(this.sunEnable, false); }
        }

        public void SetClippingPlane(ref Vector4 clippingPlane)
        {
            base.loadVector(this.clipPlane, clippingPlane);
        }
    }
}
