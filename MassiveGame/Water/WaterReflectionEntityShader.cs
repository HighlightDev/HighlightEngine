using MassiveGame.RenderCore.Lights;
using OpenTK;
using ShaderPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassiveGame.RenderCore
{
    public class WaterReflectionEntityShader : Shader
    {
        private const string SHADER_NAME = "WaterReflecitonEntityShader";

        private Int32 Texture,
            NormalMap,
            materialAmbient,
            materialDiffuse,
            materialSpecular,
            materialReflectivity,
            materialShineDamper,
            worldMatrix,
            viewMatrix,
            projectionMatrix,
            sunEnable,
            sunDirection,
            sunAmbientColour,
            sunDiffuseColour,
            sunSpecularColour,
            clipPlane,
            specularMap,
            bSpecularMapEnable,
            lightWorldMatrix;

        public WaterReflectionEntityShader(string VertexShaderFile, string FragmentShaderFile) : base(VertexShaderFile, FragmentShaderFile)
        {
            if (ShaderLoaded)
            {
                showCompileLogInfo(SHADER_NAME);
                showLinkLogInfo(SHADER_NAME);
                Debug.Log.addToLog(DateTime.Now.ToString() + "  " + base.getCompileLogInfo(SHADER_NAME));
                Debug.Log.addToLog(DateTime.Now.ToString() + "  " + base.getLinkLogInfo(SHADER_NAME));
            }
        }

        public void SetTexture(Int32 textureSampler)
        {
            loadInteger(Texture, textureSampler);
        }

        public void SetNormalMap(Int32 normalMapSampler)
        {
            loadInteger(NormalMap, normalMapSampler);
        }

        public void SetSpecularMap(Int32 specularMapSampler)
        {
            loadInteger(this.specularMap, specularMap);
            loadBool(this.bSpecularMapEnable, true);
        }

        public void SetMaterial(Material material)
        {
            loadVector(materialAmbient, material.Ambient.Xyz);
            loadVector(materialDiffuse, material.Diffuse.Xyz);
            loadVector(materialSpecular, material.Specular.Xyz);
            loadFloat(materialReflectivity, material.Reflectivity);
            loadFloat(materialShineDamper, material.ShineDamper);
        }

        public void SetTransformationMatrices(ref Matrix4 worldMatrix, ref Matrix4 lightWorldMatrix, Matrix4 viewMatrix, ref Matrix4 projectionMatrix)
        {
            loadMatrix(this.worldMatrix, false, worldMatrix);
            loadMatrix(this.viewMatrix, false, viewMatrix);
            loadMatrix(this.projectionMatrix, false, projectionMatrix);
            loadMatrix(this.lightWorldMatrix, false, lightWorldMatrix);
        }

        public void SetDirectionalLight(DirectionalLight directionalLight)
        {
            if (directionalLight != null)
            {
                base.loadBool(this.sunEnable, true);
                base.loadVector(this.sunDirection, directionalLight.Direction);
                base.loadVector(this.sunAmbientColour, directionalLight.Ambient.Xyz);
                base.loadVector(this.sunDiffuseColour, directionalLight.Diffuse.Xyz);
                base.loadVector(this.sunSpecularColour, directionalLight.Specular.Xyz);
            }
            else { base.loadBool(this.sunEnable, false); }
        }

        public void SetClipPlane(ref Vector4 clipPlane)
        {
            loadVector(this.clipPlane, clipPlane);
        }

        protected override void getAllUniformLocations()
        {
            Texture = getUniformLocation("texture");
            NormalMap = getUniformLocation("normalMap");
            materialAmbient = getUniformLocation("matAmbient");
            materialDiffuse = getUniformLocation("matDiffuse");
            materialSpecular = getUniformLocation("matSpecular");
            materialReflectivity = getUniformLocation("matReflectivity");
            materialShineDamper = getUniformLocation("matShineDamper");
            worldMatrix = getUniformLocation("worldMatrix");
            viewMatrix = getUniformLocation("viewMatrix");
            projectionMatrix = getUniformLocation("projectionMatrix");
            sunEnable = getUniformLocation("bSunEnable");
            sunDirection = getUniformLocation("sunDirection");
            sunAmbientColour = getUniformLocation("sunAmbientColour");
            sunDiffuseColour = getUniformLocation("sunDiffuseColour");
            sunSpecularColour = getUniformLocation("sunSpecularColour");
            clipPlane = getUniformLocation("clipPlane");
            specularMap = getUniformLocation("specularMap");
            lightWorldMatrix = getUniformLocation("lightWorldMatrix");
            bSpecularMapEnable = getUniformLocation("bSpecularMapEnable");
        }

        protected override void SetShaderMacros()
        {
        }
    }
}
