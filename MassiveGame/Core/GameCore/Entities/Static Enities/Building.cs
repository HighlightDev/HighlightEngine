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

namespace MassiveGame.Core.GameCore.Entities.StaticEntities
{
    public sealed class Building : StaticEntity
    {
        private SpecialStaticEntityShader m_specialShader;

        private Material m_material;

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
        {
            m_material = new Material(Vector3.One, Vector3.One, Vector3.One, Vector3.Zero, 10.0f, 10.0f);
        }

        #endregion

        private StaticEntityShader GetShader()
        {
            return m_shader as StaticEntityShader;
        }

        protected override void InitShader()
        {
            m_shader = PoolProxy.GetResource<ObtainShaderPool, ShaderAllocationPolicy<StaticEntityShader>, string, StaticEntityShader>(ProjectFolders.ShadersPath + "buildingVShader.glsl" + "," + ProjectFolders.ShadersPath + "buildingFShader.glsl");
            m_specialShader = PoolProxy.GetResource<ObtainShaderPool, ShaderAllocationPolicy<SpecialStaticEntityShader>, string, SpecialStaticEntityShader>(ProjectFolders.ShadersPath + "buildingSpecialVShader.glsl" + "," +
                ProjectFolders.ShadersPath + "buildingSpecialFShader.glsl" + "," + ProjectFolders.ShadersPath + "buildingSpecialGShader.glsl");

            m_liteReflectionShader = PoolProxy.GetResource<ObtainShaderPool, ShaderAllocationPolicy<WaterReflectionEntityShader>, string, WaterReflectionEntityShader>(ProjectFolders.ShadersPath + "waterReflectionEntityVS.glsl" + "," +
                    ProjectFolders.ShadersPath + "waterReflectionEntityFS.glsl");

            m_liteRefractionShader = PoolProxy.GetResource<ObtainShaderPool, ShaderAllocationPolicy<WaterRefractionEntityShader>, string, WaterRefractionEntityShader>(ProjectFolders.ShadersPath + "waterRefractionEntityVS.glsl" + "," +
                  ProjectFolders.ShadersPath + "waterRefractionEntityFS.glsl");
        }

        protected override void FreeShader()
        {
            PoolProxy.FreeResourceMemory<ObtainShaderPool, ShaderAllocationPolicy<StaticEntityShader>, string, StaticEntityShader>(GetShader());
            PoolProxy.FreeResourceMemory<ObtainShaderPool, ShaderAllocationPolicy<SpecialStaticEntityShader>, string, SpecialStaticEntityShader>(m_specialShader);
            PoolProxy.FreeResourceMemory<ObtainShaderPool, ShaderAllocationPolicy<WaterReflectionEntityShader>, string, WaterReflectionEntityShader>(m_liteReflectionShader);
            PoolProxy.FreeResourceMemory<ObtainShaderPool, ShaderAllocationPolicy<WaterRefractionEntityShader>, string, WaterRefractionEntityShader>(m_liteRefractionShader);
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

        public Matrix4 GetMirrorMatrix(WaterPlane water)
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

        public void RenderWaterReflection(WaterPlane water, DirectionalLight Sun, BaseCamera camera, ref Matrix4 ProjectionMatrix,
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

            m_liteReflectionShader.startProgram();      //Бинд шейдера

            m_texture.BindTexture(TextureUnit.Texture0); // diffusemap texture
            if (m_normalMap != null)
                m_normalMap.BindTexture(TextureUnit.Texture1);  // normalmap
            if (m_specularMap != null)
                m_specularMap.BindTexture(TextureUnit.Texture2);  //Bind specular map

            m_liteReflectionShader.SetTexture(0);
            m_liteReflectionShader.SetNormalMap(1);
            m_liteReflectionShader.SetSpecularMap(2);
            m_liteReflectionShader.SetMaterial(m_material);
            m_liteReflectionShader.SetTransformationMatrices(ref mirrorMatrix, ref modelMatrix, camera.GetViewMatrix(), ref ProjectionMatrix);
            m_liteReflectionShader.SetDirectionalLight(Sun);
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
            if (m_normalMap != null)
                m_normalMap.BindTexture(TextureUnit.Texture1);  // normalmap
            if (m_specularMap != null)
                m_specularMap.BindTexture(TextureUnit.Texture2);  //Bind specular map

            m_liteRefractionShader.SetAlbedoTexture(0);
            m_liteRefractionShader.SetNormalMap(1);
            m_liteRefractionShader.SetSpecularMap(2);
            m_liteRefractionShader.SetMaterial(m_material);
            m_liteRefractionShader.SetTransformationMatrices(ref modelMatrix, camera.GetViewMatrix(), ref ProjectionMatrix);
            m_liteRefractionShader.SetDirectionalLight(Sun);
            m_liteRefractionShader.SetClipPlane(ref clipPlane);

            m_skin.Buffer.RenderVAO(PrimitiveType.Triangles);
            m_liteRefractionShader.stopProgram();
        }

        #endregion

        #region Renderer

        public override void renderObject(PrimitiveType mode, bool enableNormalMapping, DirectionalLight Sun,
           List<PointLight> lights, BaseCamera camera, ref Matrix4 ProjectionMatrix, Vector4 clipPlane = new Vector4())
        {
            postConstructor();

            Matrix4 modelMatrix;
            modelMatrix = GetWorldMatrix();

            /*If clip plane is set - enable clipping plane*/
            if (clipPlane.X == 0 && clipPlane.Y == 0 && clipPlane.Z == 0 && clipPlane.W == 0) { GL.Disable(EnableCap.ClipDistance0); }
            else { GL.Enable(EnableCap.ClipDistance0); }

            GetShader().startProgram();
            m_texture.BindTexture(TextureUnit.Texture0);  //Bind texture

            if (enableNormalMapping && m_normalMap != null)
                m_normalMap.BindTexture(TextureUnit.Texture1);  //Bind normal map

            if (m_specularMap != null)
                m_specularMap.BindTexture(TextureUnit.Texture2);  //Bind specular map

            GetShader().SetDiffuseMap(0);
            GetShader().SetNormalMap(1, enableNormalMapping);
            GetShader().SetSpecularMap(2);
            GetShader().SetMaterial(m_material);
            GetShader().SetTransformationMatrices(ref modelMatrix, camera.GetViewMatrix(), ref ProjectionMatrix);
            GetShader().SetPointLights(GetRelevantPointLights(lights));
            GetShader().SetDirectionalLight(Sun);
            GetShader().SetClippingPlane(ref clipPlane);
            GetShader().SetMist(m_mist);

            if (Sun != null)
            {
                ITexture shadowMap = Sun.GetShadowHolder().GetShadowMapTexture();
                shadowMap.BindTexture(TextureUnit.Texture4); // shadowmap
                GetShader().SetDirectionalLightShadowMatrix(Sun.GetShadowHolder().GetShadowMatrix(ref modelMatrix, ref ProjectionMatrix));
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
