using System;
using System.Collections.Generic;
using GpuGraphics;
using TextureLoader;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using MassiveGame.API.Collector;
using MassiveGame.RenderCore.Lights;
using MassiveGame.RenderCore;

namespace MassiveGame
{
    public sealed class Building : StaticEntity
    {
        #region Definitions 

        private StaticEntityShader _shader;
        private SpecialStaticEntityShader _specialShader;
        private ITexture _glowingMap;
        private Material _material;

        private WaterReflectionEntityShader liteReflectionShader;
        private WaterRefractionEntityShader liteRefractionShader;

        #endregion

        #region Overrides

        public sealed override bool IsInViewFrustum(ref Matrix4 projectionMatrix, Matrix4 viewMatrix)
        {
            if (_postConstructor)
            {
                return IsInCameraView = true;
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

        public void RenderWaterReflection(WaterPlane water, DirectionalLight Sun, LiteCamera camera, ref Matrix4 ProjectionMatrix,
            Vector4 clipPlane = new Vector4())
        {
            if (_postConstructor)
                return;

            Matrix4 modelMatrix, mirrorMatrix;
            mirrorMatrix = GetMirrorMatrix(water);
            modelMatrix = GetWorldMatrix();

            /*If clip plane is setted - enable clipping plane*/
            if (clipPlane.X == 0 && clipPlane.Y == 0 && clipPlane.Z == 0 && clipPlane.W == 0) { GL.Disable(EnableCap.ClipDistance0); }
            else { GL.Enable(EnableCap.ClipDistance0); }

            liteReflectionShader.startProgram();      //Бинд шейдера

            _texture.BindTexture(TextureUnit.Texture0); // diffusemap texture
            if (_normalMap != null)
                _normalMap.BindTexture(TextureUnit.Texture1);  // normalmap
            if (_specularMap != null)
                _specularMap.BindTexture(TextureUnit.Texture2);  //Bind specular map

            liteReflectionShader.SetTexture(0);
            liteReflectionShader.SetNormalMap(1);
            liteReflectionShader.SetSpecularMap(2);
            liteReflectionShader.SetMaterial(_material);
            liteReflectionShader.SetTransformationMatrices(ref mirrorMatrix, ref modelMatrix, camera.getViewMatrix(), ref ProjectionMatrix);
            liteReflectionShader.SetDirectionalLight(Sun);
            liteReflectionShader.SetClipPlane(ref clipPlane);

            VAOManager.renderBuffers(_model.Buffer, PrimitiveType.Triangles);
            liteReflectionShader.stopProgram();
        }

        public void RenderWaterRefraction(DirectionalLight Sun, LiteCamera camera, ref Matrix4 ProjectionMatrix,
            Vector4 clipPlane = new Vector4())
        {
            if (_postConstructor)
                return;

            Matrix4 modelMatrix;
            modelMatrix = GetWorldMatrix();

            /*If clip plane is setted - enable clipping plane*/
            if (clipPlane.X == 0 && clipPlane.Y == 0 && clipPlane.Z == 0 && clipPlane.W == 0) { GL.Disable(EnableCap.ClipDistance0); }
            else { GL.Enable(EnableCap.ClipDistance0); }

            liteRefractionShader.startProgram();      //Бинд шейдера

            _texture.BindTexture(TextureUnit.Texture0); // diffusemap texture
            if (_normalMap != null)
                _normalMap.BindTexture(TextureUnit.Texture1);  // normalmap
            if (_specularMap != null)
                _specularMap.BindTexture(TextureUnit.Texture2);  //Bind specular map

            liteRefractionShader.SetTexture(0);
            liteRefractionShader.SetNormalMap(1);
            liteRefractionShader.SetSpecularMap(2);
            liteRefractionShader.SetMaterial(_material);
            liteRefractionShader.SetTransformationMatrices(ref modelMatrix, camera.getViewMatrix(), ref ProjectionMatrix);
            liteRefractionShader.SetDirectionalLight(Sun);
            liteRefractionShader.SetClipPlane(ref clipPlane);

            VAOManager.renderBuffers(_model.Buffer, PrimitiveType.Triangles);
            liteRefractionShader.stopProgram();
        }

        #endregion

        #region Renderer

        private void SafeBindGlowingMap(TextureUnit activeTexture)
        {
            if (!Object.Equals(_glowingMap, null))
                _glowingMap.BindTexture(activeTexture); // Bind glowing map
        }

        public override void renderObject(PrimitiveType mode, bool enableNormalMapping, DirectionalLight Sun,
           List<PointLight> lights, LiteCamera camera, ref Matrix4 ProjectionMatrix, Vector4 clipPlane = new Vector4())
        {
            postConstructor();

            Matrix4 modelMatrix;
            modelMatrix = GetWorldMatrix();

            /*If clip plane is setted - enable clipping plane*/
            if (clipPlane.X == 0 && clipPlane.Y == 0 && clipPlane.Z == 0 && clipPlane.W == 0) { GL.Disable(EnableCap.ClipDistance0); }
            else { GL.Enable(EnableCap.ClipDistance0); }

            _shader.startProgram();
            _texture.BindTexture(TextureUnit.Texture0);  //Bind texture

            SafeBindGlowingMap(TextureUnit.Texture3);

            if (enableNormalMapping && _normalMap != null)
                _normalMap.BindTexture(TextureUnit.Texture1);  //Bind normal map

            if (_specularMap != null)
                _specularMap.BindTexture(TextureUnit.Texture2);  //Bind specular map

            _shader.SetDiffuseMap(0);
            _shader.SetNormalMap(1, enableNormalMapping);
            _shader.SetSpecularMap(2);
            _shader.SetGlowingMap(3);
            _shader.SetMaterial(_material);
            _shader.SetTransformationMatrices(ref modelMatrix, camera.getViewMatrix(), ref ProjectionMatrix);
            _shader.SetPointLights(GetRelevantPointLights(lights));
            _shader.SetDirectionalLight(Sun);
            _shader.SetClippingPlane(ref clipPlane);
            _shader.SetMist(_mist);

            if (Sun != null)
            {
                ITexture shadowMap = Sun.GetShadow().GetShadowMapTexture();
                shadowMap.BindTexture(TextureUnit.Texture4); // shadowmap
                _shader.SetDirectionalLightShadowMatrix(Sun.GetShadow().GetShadowMatrix(ref modelMatrix, ref ProjectionMatrix));
            }
            _shader.SetDirectionalLightShadowMap(4);

            VAOManager.renderBuffers(_model.Buffer, mode);
            _shader.stopProgram();

            /*Show normal for every vertex*/
            if (mode == PrimitiveType.Lines)
            {
                _specialShader.startProgram();
                _specialShader.setUniformValues(ref modelMatrix, camera.getViewMatrix(),
                    ref ProjectionMatrix);
                VAOManager.renderBuffers(_model.Buffer, PrimitiveType.Triangles);
                _specialShader.stopProgram();
            }
        }

        #endregion

        #region Seter

        public void setGlowingMap(string glowingMapPath)
        {
            _glowingMap = ResourcePool.GetTexture(glowingMapPath);
        }

        #endregion

        #region Constructor

        private void postConstructor()
        {
            if (_postConstructor)
            {
               
                _postConstructor = !_postConstructor;
            }
        }

        public Building(string modelPath, string texturePath, string normalMapPath, string specularMapPath
            , Vector3 translation = new Vector3(), Vector3 rotation = new Vector3(), Vector3 scale = new Vector3())
            : base( modelPath,  texturePath,  normalMapPath,  specularMapPath, translation, rotation, scale)
        {
            _shader = (StaticEntityShader)ResourcePool.GetShaderProgram(ProjectFolders.ShadersPath + "buildingVShader.glsl",
                   ProjectFolders.ShadersPath + "buildingFShader.glsl", "", typeof(StaticEntityShader));
            _specialShader = (SpecialStaticEntityShader)ResourcePool.GetShaderProgram(ProjectFolders.ShadersPath + "buildingSpecialVShader.glsl",
                ProjectFolders.ShadersPath + "buildingSpecialFShader.glsl", ProjectFolders.ShadersPath + "buildingSpecialGShader.glsl",
                typeof(SpecialStaticEntityShader));

            liteReflectionShader = (WaterReflectionEntityShader)ResourcePool.GetShaderProgram(ProjectFolders.ShadersPath + "waterReflectionEntityVS.glsl",
                    ProjectFolders.ShadersPath + "waterReflectionEntityFS.glsl", "", typeof(WaterReflectionEntityShader));

            liteRefractionShader = (WaterRefractionEntityShader)ResourcePool.GetShaderProgram(ProjectFolders.ShadersPath + "waterRefractionEntityVS.glsl",
                  ProjectFolders.ShadersPath + "waterRefractionEntityFS.glsl", "", typeof(WaterRefractionEntityShader));

            _material = new Material(new Vector3(1, 1, 1), new Vector3(1, 1, 1), new Vector3(1, 1, 1), new Vector3(0, 0, 0),
                10.0f, 10.0f);
        }

        #endregion

        #region Cleaning

        public override void cleanUp()
        {
            ResourcePool.ReleaseShaderProgram(_shader);
            ResourcePool.ReleaseShaderProgram(_specialShader);
            ResourcePool.ReleaseShaderProgram(liteReflectionShader);
            ResourcePool.ReleaseShaderProgram(liteRefractionShader);
            _model.Dispose();
            if (_texture != null)
                _texture.CleanUp();
            if (_normalMap != null)
                _normalMap.CleanUp();
            if (_specularMap != null)
                _specularMap.CleanUp();
            if (_glowingMap != null)
                _glowingMap.CleanUp();
        }

        #endregion
    }
}
