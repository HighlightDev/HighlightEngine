using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using GpuGraphics;
using MassiveGame.API.Collector;
using TextureLoader;
using MassiveGame.Core.GameCore.Water;
using MassiveGame.Core.RenderCore.Lights;
using MassiveGame.Settings;

namespace MassiveGame.Core.GameCore.Entities.MoveEntities
{
    public sealed class Player : MovableEntity
    {
        #region Definitions

        private MovableEntityShader _shader;
        private Material _material;

        private WaterReflectionEntityShader liteReflectionShader;
        private WaterRefractionEntityShader liteRefractionShader;

        #endregion

        #region Overrides
     
        public sealed override void popPosition()
        {
            base.popPosition();
        }

        #endregion

        #region Renderer

        public override Matrix4 GetWorldMatrix()
        {
            Matrix4 parentWorldMatrix = Matrix4.Identity;
            if (ParentComponent != null)
            {
                parentWorldMatrix *= ParentComponent.GetWorldMatrix();
            }

            Matrix4 modelMatrix = Matrix4.Identity;
            // Rotation
            modelMatrix *= Matrix4.CreateRotationX(MathHelper.DegreesToRadians(ComponentRotation.X));
            modelMatrix *= Matrix4.CreateRotationY(MathHelper.DegreesToRadians(ComponentRotation.Y));
            modelMatrix *= Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(ComponentRotation.Z));
            // Scale
            modelMatrix *= Matrix4.CreateScale(ComponentScale);
            // Translation
            modelMatrix *= Matrix4.CreateTranslation(ComponentTranslation);
            modelMatrix *= parentWorldMatrix;
            return modelMatrix;
        }

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

        public void RenderWaterRefraction( DirectionalLight Sun, BaseCamera camera, ref Matrix4 ProjectionMatrix,
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

            _texture.BindTexture(TextureUnit.Texture0); // diffusemap texture
            if (_normalMap != null)
                _normalMap.BindTexture(TextureUnit.Texture1);  // normalmap

            liteRefractionShader.SetTexture(0);
            liteRefractionShader.SetNormalMap(1);
            liteRefractionShader.SetMaterial(_material);
            liteRefractionShader.SetTransformationMatrices(ref modelMatrix, camera.GetViewMatrix(), ref ProjectionMatrix);
            liteRefractionShader.SetDirectionalLight(Sun);
            liteRefractionShader.SetClipPlane(ref clipPlane);

            VAOManager.renderBuffers(_model.Buffer, PrimitiveType.Triangles);
            liteRefractionShader.stopProgram();
        }

        public void RenderWaterReflection(WaterPlane water, DirectionalLight Sun, BaseCamera camera, ref Matrix4 ProjectionMatrix,
            Vector4 clipPlane = new Vector4())
        {
            if (bPostConstructor)
                return;

            Matrix4 modelMatrix, mirrorMatrix;
            mirrorMatrix = GetMirrorMatrix(water);
            modelMatrix = GetWorldMatrix();

            /*If clip plane is set - enable clipping plane*/
            if (clipPlane.X == 0 && clipPlane.Y == 0 && clipPlane.Z == 0 && clipPlane.W == 0) { GL.Disable(EnableCap.ClipDistance0); }
            else { GL.Enable(EnableCap.ClipDistance0); }

            liteReflectionShader.startProgram();

            _texture.BindTexture(TextureUnit.Texture0); // diffusemap
            if (_normalMap != null)
                _normalMap.BindTexture(TextureUnit.Texture1);  // normalmap

            liteReflectionShader.SetTexture(0);
            liteReflectionShader.SetNormalMap(1);
            liteReflectionShader.SetMaterial(_material);
            liteReflectionShader.SetTransformationMatrices(ref mirrorMatrix, ref modelMatrix, camera.GetViewMatrix(), ref ProjectionMatrix);
            liteReflectionShader.SetDirectionalLight(Sun);
            liteReflectionShader.SetClipPlane(ref clipPlane);

            VAOManager.renderBuffers(_model.Buffer, PrimitiveType.Triangles);
            liteReflectionShader.stopProgram();
        }

        // Mirroring

        public override void renderObject(PrimitiveType mode, bool bEnableNormalMapping,
            DirectionalLight Sun, List<PointLight> lights, BaseCamera camera, ref Matrix4 ProjectionMatrix,
            Vector4 clipPlane = new Vector4())
        {
            postConstructor();

            Matrix4 modelMatrix;
            modelMatrix = GetWorldMatrix();

            /*If clip plane is setted - enable clipping plane*/
            if (clipPlane.X == 0 && clipPlane.Y == 0 && clipPlane.Z == 0 && clipPlane.W == 0) { GL.Disable(EnableCap.ClipDistance0); }
            else { GL.Enable(EnableCap.ClipDistance0); }

            _shader.startProgram(); 

            // pass uniform varibles to shader
            if (Sun != null)
            {
                // Get shadow handler
                ITexture shadowMap = Sun.GetShadow().GetShadowMapTexture();
                shadowMap.BindTexture(TextureUnit.Texture1); // shadowmap
                _shader.SetDirectionalLightShadowMatrix(Sun.GetShadow().GetShadowMatrix(ref modelMatrix, ref ProjectionMatrix));
            }
            _texture.BindTexture(TextureUnit.Texture0); // diffusemap
            if (bEnableNormalMapping && _normalMap != null)
                _normalMap.BindTexture(TextureUnit.Texture2);  // normalmap

            _shader.SetDiffuseMap(0);
            _shader.SetNormalMap(2, bEnableNormalMapping);
            _shader.SetMaterial(_material);
            _shader.SetTransformationMatrices(ref modelMatrix, camera.GetViewMatrix(), ref ProjectionMatrix);
            _shader.SetPointLights(lights);
            _shader.SetDirectionalLight(Sun);
            _shader.SetClippingPlane(ref clipPlane);
            _shader.SetMist(_mist);
            _shader.SetDirectionalLightShadowMap(1);
            

            VAOManager.renderBuffers(_model.Buffer, mode);
            _shader.stopProgram();
        }

        #endregion

        #region Constructors

        public Player(string modelPath, string texturePath, string normalMapPath, string specularMapPath, float Speed,
            Vector3 translation = new Vector3(), Vector3 rotation = new Vector3(), Vector3 scale = new Vector3())
            : base(modelPath, texturePath, normalMapPath, specularMapPath, Speed, translation, rotation, scale)
        {
            _material = new Material(new Vector3(1.0f, 1.0f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f),
               new Vector3(1.0f, 1.0f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f), 20.0f, 1.0f);
        }

        private void postConstructor()
        {
            if (bPostConstructor)
            {
                _shader = (MovableEntityShader)ResourcePool.GetShaderProgram(ProjectFolders.ShadersPath + "playerVertexShader.glsl",
                    ProjectFolders.ShadersPath + "playerFragmentShader.glsl", "", typeof(MovableEntityShader));

                liteReflectionShader = (WaterReflectionEntityShader)ResourcePool.GetShaderProgram(ProjectFolders.ShadersPath + "waterReflectionEntityVS.glsl",
                    ProjectFolders.ShadersPath + "waterReflectionEntityFS.glsl", "", typeof(WaterReflectionEntityShader));

                liteRefractionShader = (WaterRefractionEntityShader)ResourcePool.GetShaderProgram(ProjectFolders.ShadersPath + "waterRefractionEntityVS.glsl",
                    ProjectFolders.ShadersPath + "waterRefractionEntityFS.glsl", "", typeof(WaterRefractionEntityShader));

                this.bPostConstructor = !this.bPostConstructor;
            }
        }

        #endregion

        #region Cleaning

        public override void cleanUp()
        {
            //source.Delete();
            ResourcePool.ReleaseShaderProgram(_shader);
            ResourcePool.ReleaseShaderProgram(liteReflectionShader);
            ResourcePool.ReleaseShaderProgram(liteRefractionShader);
            _model.Dispose();
            if (_texture != null)
                _texture.CleanUp();
            if (_normalMap != null)
                _normalMap.CleanUp();
            if (_specularMap != null)
                _specularMap.CleanUp();
        }

        #endregion
    }
}
