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
        #region Definitions 

        private StaticEntityShader m_shader;
        private SpecialStaticEntityShader m_specialShader;
        private ITexture m_glowingMap;
        private Material m_material;

        private WaterReflectionEntityShader liteReflectionShader;
        private WaterRefractionEntityShader liteRefractionShader;

        #endregion

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

            liteReflectionShader.startProgram();      //Бинд шейдера

            m_texture.BindTexture(TextureUnit.Texture0); // diffusemap texture
            if (m_normalMap != null)
                m_normalMap.BindTexture(TextureUnit.Texture1);  // normalmap
            if (m_specularMap != null)
                m_specularMap.BindTexture(TextureUnit.Texture2);  //Bind specular map

            liteReflectionShader.SetTexture(0);
            liteReflectionShader.SetNormalMap(1);
            liteReflectionShader.SetSpecularMap(2);
            liteReflectionShader.SetMaterial(m_material);
            liteReflectionShader.SetTransformationMatrices(ref mirrorMatrix, ref modelMatrix, camera.GetViewMatrix(), ref ProjectionMatrix);
            liteReflectionShader.SetDirectionalLight(Sun);
            liteReflectionShader.SetClipPlane(ref clipPlane);

            m_skin.Buffer.RenderVAO(PrimitiveType.Triangles);
            liteReflectionShader.stopProgram();
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

            liteRefractionShader.startProgram();      //Бинд шейдера

            m_texture.BindTexture(TextureUnit.Texture0); // diffusemap texture
            if (m_normalMap != null)
                m_normalMap.BindTexture(TextureUnit.Texture1);  // normalmap
            if (m_specularMap != null)
                m_specularMap.BindTexture(TextureUnit.Texture2);  //Bind specular map

            liteRefractionShader.SetTexture(0);
            liteRefractionShader.SetNormalMap(1);
            liteRefractionShader.SetSpecularMap(2);
            liteRefractionShader.SetMaterial(m_material);
            liteRefractionShader.SetTransformationMatrices(ref modelMatrix, camera.GetViewMatrix(), ref ProjectionMatrix);
            liteRefractionShader.SetDirectionalLight(Sun);
            liteRefractionShader.SetClipPlane(ref clipPlane);

            m_skin.Buffer.RenderVAO(PrimitiveType.Triangles);
            liteRefractionShader.stopProgram();
        }

        #endregion

        #region Renderer

        private void SafeBindGlowingMap(TextureUnit activeTexture)
        {
            if (!Object.Equals(m_glowingMap, null))
                m_glowingMap.BindTexture(activeTexture); // Bind glowing map
        }

        public override void renderObject(PrimitiveType mode, bool enableNormalMapping, DirectionalLight Sun,
           List<PointLight> lights, BaseCamera camera, ref Matrix4 ProjectionMatrix, Vector4 clipPlane = new Vector4())
        {
            postConstructor();

            Matrix4 modelMatrix;
            modelMatrix = GetWorldMatrix();

            /*If clip plane is setted - enable clipping plane*/
            if (clipPlane.X == 0 && clipPlane.Y == 0 && clipPlane.Z == 0 && clipPlane.W == 0) { GL.Disable(EnableCap.ClipDistance0); }
            else { GL.Enable(EnableCap.ClipDistance0); }

            m_shader.startProgram();
            m_texture.BindTexture(TextureUnit.Texture0);  //Bind texture

            SafeBindGlowingMap(TextureUnit.Texture3);

            if (enableNormalMapping && m_normalMap != null)
                m_normalMap.BindTexture(TextureUnit.Texture1);  //Bind normal map

            if (m_specularMap != null)
                m_specularMap.BindTexture(TextureUnit.Texture2);  //Bind specular map

            m_shader.SetDiffuseMap(0);
            m_shader.SetNormalMap(1, enableNormalMapping);
            m_shader.SetSpecularMap(2);
            m_shader.SetGlowingMap(3);
            m_shader.SetMaterial(m_material);
            m_shader.SetTransformationMatrices(ref modelMatrix, camera.GetViewMatrix(), ref ProjectionMatrix);
            m_shader.SetPointLights(GetRelevantPointLights(lights));
            m_shader.SetDirectionalLight(Sun);
            m_shader.SetClippingPlane(ref clipPlane);
            m_shader.SetMist(m_mist);

            if (Sun != null)
            {
                ITexture shadowMap = Sun.GetShadow().GetShadowMapTexture();
                shadowMap.BindTexture(TextureUnit.Texture4); // shadowmap
                m_shader.SetDirectionalLightShadowMatrix(Sun.GetShadow().GetShadowMatrix(ref modelMatrix, ref ProjectionMatrix));
            }
            m_shader.SetDirectionalLightShadowMap(4);

            m_skin.Buffer.RenderVAO(mode);
            m_shader.stopProgram();

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

        #region Seter

        public void setGlowingMap(string glowingMapPath)
        {
            m_glowingMap = PoolProxy.GetResource<ObtainTexturePool, TextureAllocationPolicy, string, ITexture>(glowingMapPath);
        }

        #endregion

        #region Constructor

        private void postConstructor()
        {
            if (bPostConstructor)
            {
               
                bPostConstructor = !bPostConstructor;
            }
        }

        public Building(string modelPath, string texturePath, string normalMapPath, string specularMapPath
            , Vector3 translation = new Vector3(), Vector3 rotation = new Vector3(), Vector3 scale = new Vector3())
            : base( modelPath,  texturePath,  normalMapPath,  specularMapPath, translation, rotation, scale)
        {
            m_shader = PoolProxy.GetResource<ObtainShaderPool, ShaderAllocationPolicy<StaticEntityShader>, string, StaticEntityShader>(ProjectFolders.ShadersPath + "buildingVShader.glsl" + "," + ProjectFolders.ShadersPath + "buildingFShader.glsl");
            m_specialShader = PoolProxy.GetResource<ObtainShaderPool, ShaderAllocationPolicy< SpecialStaticEntityShader >, string, SpecialStaticEntityShader>(ProjectFolders.ShadersPath + "buildingSpecialVShader.glsl" + "," +
                ProjectFolders.ShadersPath + "buildingSpecialFShader.glsl" + "," + ProjectFolders.ShadersPath + "buildingSpecialGShader.glsl");

            liteReflectionShader = PoolProxy.GetResource<ObtainShaderPool, ShaderAllocationPolicy<WaterReflectionEntityShader>, string, WaterReflectionEntityShader>(ProjectFolders.ShadersPath + "waterReflectionEntityVS.glsl" + "," +
                    ProjectFolders.ShadersPath + "waterReflectionEntityFS.glsl");

            liteRefractionShader = PoolProxy.GetResource<ObtainShaderPool, ShaderAllocationPolicy<WaterRefractionEntityShader>, string, WaterRefractionEntityShader>(ProjectFolders.ShadersPath + "waterRefractionEntityVS.glsl" + "," +
                  ProjectFolders.ShadersPath + "waterRefractionEntityFS.glsl");

            m_material = new Material(new Vector3(1, 1, 1), new Vector3(1, 1, 1), new Vector3(1, 1, 1), new Vector3(0, 0, 0),
                10.0f, 10.0f);
        }

        #endregion

        #region Cleaning

        public override void CleanUp()
        {
            base.CleanUp();
            PoolProxy.FreeResourceMemoryByValue<ObtainShaderPool, ShaderAllocationPolicy<StaticEntityShader>, string, StaticEntityShader>(m_shader);
            PoolProxy.FreeResourceMemoryByValue<ObtainShaderPool, ShaderAllocationPolicy<SpecialStaticEntityShader>, string, SpecialStaticEntityShader>(m_specialShader);
            PoolProxy.FreeResourceMemoryByValue<ObtainShaderPool, ShaderAllocationPolicy<WaterReflectionEntityShader>, string, WaterReflectionEntityShader>(liteReflectionShader);
            PoolProxy.FreeResourceMemoryByValue<ObtainShaderPool, ShaderAllocationPolicy<WaterRefractionEntityShader>, string, WaterRefractionEntityShader>(liteRefractionShader);
            PoolProxy.FreeResourceMemoryByValue<ObtainTexturePool, TextureAllocationPolicy, string, ITexture>(m_glowingMap);
        }

        #endregion
    }
}
