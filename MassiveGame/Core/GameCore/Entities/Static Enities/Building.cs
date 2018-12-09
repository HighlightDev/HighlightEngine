using System;
using System.Collections.Generic;
using TextureLoader;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using MassiveGame.Core.GameCore.Water;
using MassiveGame.Core.RenderCore.Lights;
using MassiveGame.Settings;
using MassiveGame.API.ResourcePool.PoolHandling;
using MassiveGame.API.ResourcePool.Policies;
using MassiveGame.API.ResourcePool;
using System.Runtime.Serialization;
using MassiveGame.Core.RenderCore.Shadows;

namespace MassiveGame.Core.GameCore.Entities.StaticEntities
{
    [Serializable]
    public class Building : StaticEntity
    {
        private SpecialStaticEntityShader m_specialShader;

        private WaterReflectionEntityShader m_liteReflectionShader;

        private WaterRefractionEntityShader m_liteRefractionShader;

        #region Constructor

        private void postConstructor()
        {
            if (bPostConstructor)
                bPostConstructor = !bPostConstructor;
        }

        public Building(string modelPath, string texturePath, string normalMapPath, string specularMapPath
            , Vector3 translation = new Vector3(), Vector3 rotation = new Vector3(), Vector3 scale = new Vector3())
            : base(modelPath, texturePath, normalMapPath, specularMapPath, translation, rotation, scale)
        { }

        #endregion

        #region Serialization

        protected Building(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        #endregion

        private StaticEntityShader GetShader()
        {
            return m_shader as StaticEntityShader;
        }

        protected override void InitShader()
        {
            m_shader = PoolProxy.GetResource<GetShaderPool, ShaderAllocationPolicy<StaticEntityShader>, string, StaticEntityShader>(ProjectFolders.ShadersPath + "buildingVShader.glsl" + "," + ProjectFolders.ShadersPath + "buildingFShader.glsl");
            m_specialShader = PoolProxy.GetResource<GetShaderPool, ShaderAllocationPolicy<SpecialStaticEntityShader>, string, SpecialStaticEntityShader>(ProjectFolders.ShadersPath + "buildingSpecialVShader.glsl" + "," +
                ProjectFolders.ShadersPath + "buildingSpecialFShader.glsl" + "," + ProjectFolders.ShadersPath + "buildingSpecialGShader.glsl");

            m_liteReflectionShader = PoolProxy.GetResource<GetShaderPool, ShaderAllocationPolicy<WaterReflectionEntityShader>, string, WaterReflectionEntityShader>(ProjectFolders.ShadersPath + "waterReflectionEntityVS.glsl" + "," +
                    ProjectFolders.ShadersPath + "waterReflectionEntityFS.glsl");

            m_liteRefractionShader = PoolProxy.GetResource<GetShaderPool, ShaderAllocationPolicy<WaterRefractionEntityShader>, string, WaterRefractionEntityShader>(ProjectFolders.ShadersPath + "waterRefractionEntityVS.glsl" + "," +
                  ProjectFolders.ShadersPath + "waterRefractionEntityFS.glsl");
        }

        protected override void FreeShader()
        {
            PoolProxy.FreeResourceMemory<GetShaderPool, ShaderAllocationPolicy<StaticEntityShader>, string, StaticEntityShader>(GetShader());
            PoolProxy.FreeResourceMemory<GetShaderPool, ShaderAllocationPolicy<SpecialStaticEntityShader>, string, SpecialStaticEntityShader>(m_specialShader);
            PoolProxy.FreeResourceMemory<GetShaderPool, ShaderAllocationPolicy<WaterReflectionEntityShader>, string, WaterReflectionEntityShader>(m_liteReflectionShader);
            PoolProxy.FreeResourceMemory<GetShaderPool, ShaderAllocationPolicy<WaterRefractionEntityShader>, string, WaterRefractionEntityShader>(m_liteRefractionShader);
        }

        #region Overrides

        public sealed override bool IsInViewFrustum(ref Matrix4 projectionMatrix, Matrix4 viewMatrix)
        {
            if (bPostConstructor)
            {
                return IsVisibleByCamera = true;
            }
            return base.IsInViewFrustum(ref projectionMatrix, viewMatrix);
        }

        #endregion

        #region WaterReflection

        private Matrix4 GetMirrorMatrix(WaterPlane water)
        {
            Vector3 currentPosition = ComponentTranslation;
            float translationPositionY = (2 * water.GetTranslation().Y) - currentPosition.Y;
            Matrix4 mirrorMatrix = Matrix4.Identity;
            mirrorMatrix *= Matrix4.CreateRotationX(MathHelper.DegreesToRadians(ComponentRotation.X));
            mirrorMatrix *= Matrix4.CreateRotationY(MathHelper.DegreesToRadians(ComponentRotation.Y));
            mirrorMatrix *= Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(ComponentRotation.Z));
            mirrorMatrix *= Matrix4.CreateScale(ComponentScale.X, -ComponentScale.Y, ComponentScale.Z);
            mirrorMatrix *= Matrix4.CreateTranslation(currentPosition.X, translationPositionY, currentPosition.Z);
            return mirrorMatrix;
        }

        public void RenderWaterReflection(WaterPlane water, DirectionalLight directionalLight, BaseCamera camera, ref Matrix4 ProjectionMatrix,
            Vector4 clipPlane = new Vector4())
        {
            if (bPostConstructor)
                return;

            Matrix4 modelMatrix, mirrorMatrix;
            mirrorMatrix = GetMirrorMatrix(water);
            modelMatrix = GetWorldMatrix();

            /*If clip plane is setted - enable clipping plane*/
            if (clipPlane.X == 0 && clipPlane.Y == 0 && clipPlane.Z == 0 && clipPlane.W == 0) { GL.Disable(EnableCap.ClipDistance0); }
            else { GL.Enable(EnableCap.ClipDistance0); }

            m_liteReflectionShader.startProgram();

            m_texture.BindTexture(TextureUnit.Texture0); // diffusemap texture

            bool bEnableNM = m_normalMap != null;
            bool bEnableSM = m_specularMap != null;
            if (bEnableNM)
                m_normalMap.BindTexture(TextureUnit.Texture1);  // normalmap
            if (bEnableSM)
                m_specularMap.BindTexture(TextureUnit.Texture2);  //Bind specular map

            m_liteReflectionShader.SetTexture(0);
            if (bEnableNM)
            {
                m_liteReflectionShader.SetNormalMap(1);
            }
            if (bEnableSM)
            {
                m_liteReflectionShader.SetSpecularMap(2);
            }
            m_liteReflectionShader.SetMaterial(m_material);
            m_liteReflectionShader.SetTransformationMatrices(ref mirrorMatrix, ref modelMatrix, camera.GetViewMatrix(), ref ProjectionMatrix);
            m_liteReflectionShader.SetDirectionalLight(directionalLight);
            m_liteReflectionShader.SetClipPlane(ref clipPlane);

            m_skin.Buffer.RenderVAO(PrimitiveType.Triangles);
            m_liteReflectionShader.stopProgram();
        }

        public void RenderWaterRefraction(DirectionalLight Sun, BaseCamera camera, ref Matrix4 ProjectionMatrix,
            Vector4 clipPlane = new Vector4())
        {
            if (bPostConstructor)
                return;

            Matrix4 modelMatrix;
            modelMatrix = GetWorldMatrix();

            /*If clip plane is setted - enable clipping plane*/
            if (clipPlane.X == 0 && clipPlane.Y == 0 && clipPlane.Z == 0 && clipPlane.W == 0) { GL.Disable(EnableCap.ClipDistance0); }
            else { GL.Enable(EnableCap.ClipDistance0); }

            m_liteRefractionShader.startProgram();      //Бинд шейдера

            m_texture.BindTexture(TextureUnit.Texture0); // diffusemap texture
            bool bEnableNM = m_normalMap != null;
            bool bEnableSM = m_specularMap != null;
            if (bEnableNM)
                m_normalMap.BindTexture(TextureUnit.Texture1);  // normalmap
            if (bEnableSM)
                m_specularMap.BindTexture(TextureUnit.Texture2);  //Bind specular map

            m_liteReflectionShader.SetTexture(0);
            if (bEnableNM)
            {
                m_liteRefractionShader.SetNormalMap(1);
            }
            if (bEnableSM)
            {
                m_liteRefractionShader.SetSpecularMap(2);
            }
            m_liteRefractionShader.SetAlbedoTexture(0);
            m_liteRefractionShader.SetMaterial(m_material);
            m_liteRefractionShader.SetTransformationMatrices(ref modelMatrix, camera.GetViewMatrix(), ref ProjectionMatrix);
            m_liteRefractionShader.SetDirectionalLight(Sun);
            m_liteRefractionShader.SetClipPlane(ref clipPlane);

            m_skin.Buffer.RenderVAO(PrimitiveType.Triangles);
            m_liteRefractionShader.stopProgram();
        }

        #endregion

        #region Renderer

        public override void renderObject(PrimitiveType mode, DirectionalLight directionalLight, List<PointLight> lights,
            BaseCamera camera, ref Matrix4 ProjectionMatrix, Vector4 clipPlane = new Vector4())
        {
            postConstructor();

            Matrix4 modelMatrix;
            modelMatrix = GetWorldMatrix();

            /*If clip plane is set - enable clipping plane*/
            if (clipPlane.X == 0 && clipPlane.Y == 0 && clipPlane.Z == 0 && clipPlane.W == 0) { GL.Disable(EnableCap.ClipDistance0); }
            else { GL.Enable(EnableCap.ClipDistance0); }

            GetShader().startProgram();
            m_texture.BindTexture(TextureUnit.Texture0);  //Bind texture

            bool bEnableNormalMap = m_normalMap != null;
            bool bEnableSpecularMap = m_specularMap != null;
            if (bEnableNormalMap)
                m_normalMap.BindTexture(TextureUnit.Texture1);  //Bind normal map

            if (m_specularMap != null)
                m_specularMap.BindTexture(TextureUnit.Texture2);  //Bind specular map

            GetShader().SetDiffuseMap(0);
            if (bEnableNormalMap)
            {
                GetShader().SetNormalMap(1);
            }
            if (bEnableSpecularMap)
            {
                GetShader().SetSpecularMap(2);
            }
            GetShader().SetMaterial(m_material);
            GetShader().SetTransformationMatrices(ref modelMatrix, camera.GetViewMatrix(), ref ProjectionMatrix);
            GetShader().SetPointLights(GetRelevantPointLights(lights));
            GetShader().SetDirectionalLight(directionalLight);
            GetShader().SetClippingPlane(ref clipPlane);
            GetShader().SetMist(m_mist);

            if (directionalLight != null && directionalLight.GetHasShadow())
            {
                DirectionalLightWithShadow lightWithShadow = directionalLight as DirectionalLightWithShadow;
                ITexture shadowMap = lightWithShadow.GetShadowMapTexture();
                shadowMap.BindTexture(TextureUnit.Texture4); // shadowmap
                GetShader().SetDirectionalLightShadowMatrix(lightWithShadow.GetShadowMatrix(ref modelMatrix, ref ProjectionMatrix));
            }
            GetShader().SetDirectionalLightShadowMap(4);

            m_skin.Buffer.RenderVAO(mode);
            GetShader().stopProgram();

            /*Show normal for every vertex*/
            if (mode == PrimitiveType.Lines)
            {
                m_specialShader.startProgram();
                m_specialShader.setUniformValues(ref modelMatrix, camera.GetViewMatrix(),
                    ref ProjectionMatrix);
                m_skin.Buffer.RenderVAO(PrimitiveType.Triangles);
                m_specialShader.stopProgram();
            }
        }

        #endregion

        #region Cleaning

        public override void CleanUp()
        {
            base.CleanUp();
        }

        #endregion
    }
}
