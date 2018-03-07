using System.Collections.Generic;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using MassiveGame.RenderCore.Lights;

using AudioEngine;
using GpuGraphics;
using PhysicsBox;
using MassiveGame.API.Collector;
using MassiveGame.RenderCore.Shadows;
using TextureLoader;
using MassiveGame.RenderCore;

namespace MassiveGame
{
    #region SoundPosition

    public struct SoundCenter
    {
        public void Init(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public float X { get; private set; }
        public float Y { get; private set; }
        public float Z { get; private set; }
    }

    #endregion

    public sealed class Player : MotionEntities
    {
        #region Definitions

        private SoundCenter soundCenter;
        private Source sourceStep;
        private Source sourceCollide;
        private int[] _SB_step;
        private int[] _SB_collide;
        private int _SB_stepSwitcher = 0;
        private int _SB_collideSwitcher = 0;

        private MotionEntityShader _shader;
        private Material _material;

        private WaterReflectionEntityShader liteReflectionShader;
        private WaterRefractionEntityShader liteRefractionShader;

        #endregion

        #region Overrides

        public override CollisionSphereBox Box
        {
            get
            {
                return base.Box;
            }
            protected set
            {
                base.Box = value;
            }
        }

        public sealed override bool IsInViewFrustum(ref Matrix4 projectionMatrix, Matrix4 viewMatrix)
        {
            if (_postConstructor)
            {
                return IsInCameraView = true;
            }
            return base.IsInViewFrustum(ref projectionMatrix, viewMatrix);
        }

        public sealed override void popPositionStack()
        {
            base.popPositionStack();

            //if (!sourceCollide.IsPlaying())
            //{
            //    sourceCollide.SetBuffer(_SB_collide[_SB_collideSwitcher++]);
            //    if (_SB_collideSwitcher == _SB_collide.Length)
            //        _SB_collideSwitcher = 0;
            //    sourceCollide.Play();
            //}
        }

        #endregion

        #region Seters

        public void setSoundAttachment(int[] SB_step = null, int[] SB_collide = null)
        {

            // add paramenter int[] SB_stepOffset to manage different groups of sounds
            if (SB_step != null) // if sound of step attached
            {
                _SB_step = SB_step;
                sourceStep = new Source(_SB_step[0], 1, 6, 65);
                sourceStep.SetVolume(1.5f);
                // to set correct position of a sound it must be set in accordance of translation and center of a sound
                sourceStep.SetPosition(
                    ObjectPosition.X + soundCenter.X,
                    ObjectPosition.Y + soundCenter.Y,
                    ObjectPosition.Z + soundCenter.Z
                );
            }

            if (SB_collide != null) // if sound of collide attached
            {
                _SB_collide = SB_collide;
                sourceCollide = new Source(_SB_collide[0], 1, 6, 65);
                sourceCollide.SetVolume(0.2f);
                // to set correct position of a sound it must be set in accordance of translation and center of a sound
                sourceCollide.SetPosition(
                    ObjectPosition.X + soundCenter.X,
                    ObjectPosition.Y + soundCenter.Y,
                    ObjectPosition.Z + soundCenter.Z
                );
            }

            if (SB_collide != null && SB_step != null)
            {
                soundCenter.Init(
                    (this._leftX + this._rightX) / 2,
                    (this._bottomY + this._topY) / 2,
                    (this._nearZ + this._farZ) / 2
                );
            }
        }

        #endregion

        #region Movement

        public override void objectMove(directions direction, Terrain terrain)       //Move object in 5 directions
        {
            base.objectMove(direction, terrain);
        }

        #endregion

        #region Renderer

        public override Matrix4 GetWorldMatrix()
        {
            Matrix4 modelMatrix = Matrix4.Identity;
            //Поворот
            modelMatrix *= Matrix4.CreateRotationX(MathHelper.DegreesToRadians(_rotation.X));
            modelMatrix *= Matrix4.CreateRotationY(MathHelper.DegreesToRadians(_rotation.Y));
            modelMatrix *= Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(_rotation.Z));
            //Масштабирование
            modelMatrix *= Matrix4.CreateScale(base._scale);
            //Перемещение
            modelMatrix *= Matrix4.CreateTranslation(base._translation + base.Move);
            return modelMatrix;
        }

        public Matrix4 GetMirrorMatrix(WaterEntity water)
        {
            Vector3 currentPosition = base._translation + base.Move;
            float translationPositionY = (2 * water.GetTranslation().Y) - currentPosition.Y;
            Matrix4 mirrorMatrix = Matrix4.Identity;
            mirrorMatrix *= Matrix4.CreateRotationX(MathHelper.DegreesToRadians(_rotation.X));
            mirrorMatrix *= Matrix4.CreateRotationY(MathHelper.DegreesToRadians(_rotation.Y));
            mirrorMatrix *= Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(_rotation.Z));
            mirrorMatrix *= Matrix4.CreateScale(_scale.X, -_scale.Y, _scale.Z);
            mirrorMatrix *= Matrix4.CreateTranslation(currentPosition.X, translationPositionY, currentPosition.Z);
            return mirrorMatrix;
        }

        public void RenderWaterRefraction( DirectionalLight Sun, LiteCamera camera, ref Matrix4 ProjectionMatrix,
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

            liteRefractionShader.SetTexture(0);
            liteRefractionShader.SetNormalMap(1);
            liteRefractionShader.SetMaterial(_material);
            liteRefractionShader.SetTransformationMatrices(ref modelMatrix, camera.getViewMatrix(), ref ProjectionMatrix);
            liteRefractionShader.SetDirectionalLight(Sun);
            liteRefractionShader.SetClipPlane(ref clipPlane);

            VAOManager.renderBuffers(_model.Buffer, PrimitiveType.Triangles);
            liteRefractionShader.stopProgram();
        }

        public void RenderWaterReflection(WaterEntity water, DirectionalLight Sun, LiteCamera camera, ref Matrix4 ProjectionMatrix,
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

            liteReflectionShader.SetTexture(0);
            liteReflectionShader.SetNormalMap(1);
            liteReflectionShader.SetMaterial(_material);
            liteReflectionShader.SetTransformationMatrices(ref mirrorMatrix, ref modelMatrix, camera.getViewMatrix(), ref ProjectionMatrix);
            liteReflectionShader.SetDirectionalLight(Sun);
            liteReflectionShader.SetClipPlane(ref clipPlane);

            VAOManager.renderBuffers(_model.Buffer, PrimitiveType.Triangles);
            liteReflectionShader.stopProgram();
        }

        // Mirroring

        public override void renderObject(PrimitiveType mode, bool bEnableNormalMapping,
            DirectionalLight Sun, List<PointLight> lights, Terrain terrain, LiteCamera camera, ref Matrix4 ProjectionMatrix,
            Vector4 clipPlane = new Vector4())
        {
            postConstructor(terrain);

            Matrix4 modelMatrix;
            modelMatrix = GetWorldMatrix();

            /*If clip plane is setted - enable clipping plane*/
            if (clipPlane.X == 0 && clipPlane.Y == 0 && clipPlane.Z == 0 && clipPlane.W == 0) { GL.Disable(EnableCap.ClipDistance0); }
            else { GL.Enable(EnableCap.ClipDistance0); }

            _shader.startProgram();      //Бинд шейдера

            // pass uniform varibles to shader
            if (Sun != null)
            {
                // Get shadow handler
                ITexture shadowMap = Sun.GetShadowHandler().GetTextureHandler();
                shadowMap.BindTexture(TextureUnit.Texture1); // shadowmap
                _shader.SetDirectionalLightShadowMatrix(Sun.GetShadowHandler().GetShadowMatrix(ref modelMatrix, ref ProjectionMatrix));
            }
            _texture.BindTexture(TextureUnit.Texture0); // diffusemap texture
            if (bEnableNormalMapping && _normalMap != null)
                _normalMap.BindTexture(TextureUnit.Texture2);  // normalmap

            _shader.SetDiffuseMap(0);
            _shader.SetNormalMap(2, bEnableNormalMapping);
            _shader.SetMaterial(_material);
            _shader.SetTransformationMatrices(ref modelMatrix, camera.getViewMatrix(), ref ProjectionMatrix);
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

        public Player(string modelPath, string texturePath, string normalMapPath, string specularMapPath, float Speed, int ID,
            Vector3 translation = new Vector3(), Vector3 rotation = new Vector3(), Vector3 scale = new Vector3())
            : base(modelPath, texturePath, normalMapPath, specularMapPath, Speed, ID, translation, rotation, scale)
        {
            _material = new Material(new Vector3(1.0f, 1.0f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f),
               new Vector3(1.0f, 1.0f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f), 20.0f, 1.0f);
        }

        private void postConstructor(Terrain terrain)
        {
            if (_postConstructor)
            {
                base.Move = new Vector3(base.Move.X, terrain.getLandscapeHeight(Box.getCenter().X,
                    Box.getCenter().Z) - _bottomY, base.Move.Z);//Инициализация высоты
                // update collsion box - lift it on terrain height
                this._box.synchronizeCoordinates(this._leftX + this.Move.X, base._rightX + this.Move.X,
                   this._bottomY + this.Move.Y, this._topY + this.Move.Y, this._nearZ + this.Move.Z, this._farZ + this.Move.Z);

                _shader = (MotionEntityShader)ResourcePool.GetShaderProgram(ProjectFolders.ShadersPath + "playerVertexShader.glsl",
                    ProjectFolders.ShadersPath + "playerFragmentShader.glsl", "", typeof(MotionEntityShader));

                liteReflectionShader = (WaterReflectionEntityShader)ResourcePool.GetShaderProgram(ProjectFolders.ShadersPath + "waterReflectionEntityVS.glsl",
                    ProjectFolders.ShadersPath + "waterReflectionEntityFS.glsl", "", typeof(WaterReflectionEntityShader));

                liteRefractionShader = (WaterRefractionEntityShader)ResourcePool.GetShaderProgram(ProjectFolders.ShadersPath + "waterRefractionEntityVS.glsl",
                    ProjectFolders.ShadersPath + "waterRefractionEntityFS.glsl", "", typeof(WaterRefractionEntityShader));

                this._postConstructor = !this._postConstructor;
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
