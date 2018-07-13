using System;
using System.Collections.Generic;
using PhysicsBox;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using PhysicsBox.MathTypes;
using MassiveGame.Core.PhysicsCore;
using MassiveGame.Core.RenderCore.Lights;
using MassiveGame.Core.GameCore.Water;
using GpuGraphics;
using TextureLoader;
using MassiveGame.Settings;
using MassiveGame.API.Collector;

namespace MassiveGame.Core.GameCore.Entities.MoveEntities
{
    public enum BehaviorState
    {
        IDLE,
        MOVE,
        FREE_FALLING
    }

    public abstract class MovableEntity : Entity
    {
        #region Definitions

        protected BehaviorState actorState;
        protected ActorPositionMemento positionMemento;

        protected MovableEntityShader shader;
        protected Material material;

        private WaterReflectionEntityShader liteReflectionShader;
        private WaterRefractionEntityShader liteRefractionShader;

        #endregion

        #region Constructors

        public MovableEntity() { }

        public MovableEntity(string modelPath, string texturePath, string normalMapPath, string specularMapPath
            , float Speed, Vector3 translation, Vector3 rotation, Vector3 scale) :
            base(modelPath, texturePath, normalMapPath, specularMapPath, translation, rotation, scale)
        {
            material = new Material(new Vector3(1.0f, 1.0f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f),
               new Vector3(1.0f, 1.0f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f), 20.0f, 1.0f);

            ActorState = BehaviorState.FREE_FALLING;
            this.positionMemento = new ActorPositionMemento();
            pushPosition();
            this.Speed = Speed;
        }

        private void postConstructor()
        {
            if (bPostConstructor)
            {
                shader = (MovableEntityShader)ResourcePool.GetShaderProgram(ProjectFolders.ShadersPath + "movableEntityVS.glsl",
                    ProjectFolders.ShadersPath + "movableEntityFS.glsl", "", typeof(MovableEntityShader));

                liteReflectionShader = (WaterReflectionEntityShader)ResourcePool.GetShaderProgram(ProjectFolders.ShadersPath + "waterReflectionEntityVS.glsl",
                    ProjectFolders.ShadersPath + "waterReflectionEntityFS.glsl", "", typeof(WaterReflectionEntityShader));

                liteRefractionShader = (WaterRefractionEntityShader)ResourcePool.GetShaderProgram(ProjectFolders.ShadersPath + "waterRefractionEntityVS.glsl",
                    ProjectFolders.ShadersPath + "waterRefractionEntityFS.glsl", "", typeof(WaterRefractionEntityShader));

                this.bPostConstructor = !this.bPostConstructor;
            }
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

            _texture.BindTexture(TextureUnit.Texture0); // diffusemap texture
            if (_normalMap != null)
                _normalMap.BindTexture(TextureUnit.Texture1);  // normalmap

            liteRefractionShader.SetTexture(0);
            liteRefractionShader.SetNormalMap(1);
            liteRefractionShader.SetMaterial(material);
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
            liteReflectionShader.SetMaterial(material);
            liteReflectionShader.SetTransformationMatrices(ref mirrorMatrix, ref modelMatrix, camera.GetViewMatrix(), ref ProjectionMatrix);
            liteReflectionShader.SetDirectionalLight(Sun);
            liteReflectionShader.SetClipPlane(ref clipPlane);

            VAOManager.renderBuffers(_model.Buffer, PrimitiveType.Triangles);
            liteReflectionShader.stopProgram();
        }

        public virtual void RenderEntity(PrimitiveType mode, bool bEnableNormalMapping,
            DirectionalLight Sun, List<PointLight> lights, BaseCamera camera, ref Matrix4 ProjectionMatrix,
            Vector4 clipPlane = new Vector4())
        {
            postConstructor();

            Matrix4 modelMatrix;
            modelMatrix = GetWorldMatrix();

            /*If clip plane is setted - enable clipping plane*/
            if (clipPlane.X == 0 && clipPlane.Y == 0 && clipPlane.Z == 0 && clipPlane.W == 0) { GL.Disable(EnableCap.ClipDistance0); }
            else { GL.Enable(EnableCap.ClipDistance0); }

            shader.startProgram();

            // pass uniform varibles to shader
            if (Sun != null)
            {
                // Get shadow handler
                ITexture shadowMap = Sun.GetShadow().GetShadowMapTexture();
                shadowMap.BindTexture(TextureUnit.Texture1); // shadowmap
                shader.SetDirectionalLightShadowMatrix(Sun.GetShadow().GetShadowMatrix(ref modelMatrix, ref ProjectionMatrix));
            }
            _texture.BindTexture(TextureUnit.Texture0); // diffusemap
            if (bEnableNormalMapping && _normalMap != null)
                _normalMap.BindTexture(TextureUnit.Texture2);  // normalmap

            shader.SetDiffuseMap(0);
            shader.SetNormalMap(2, bEnableNormalMapping);
            shader.SetMaterial(material);
            shader.SetTransformationMatrices(ref modelMatrix, camera.GetViewMatrix(), ref ProjectionMatrix);
            shader.SetPointLights(lights);
            shader.SetDirectionalLight(Sun);
            shader.SetClippingPlane(ref clipPlane);
            shader.SetMist(_mist);
            shader.SetDirectionalLightShadowMap(1);


            VAOManager.renderBuffers(_model.Buffer, mode);
            shader.stopProgram();
        }

        #endregion

        #region Cleaning

        public override void cleanUp()
        {
            //source.Delete();
            ResourcePool.ReleaseShaderProgram(shader);
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

        public BehaviorState ActorState
        {
            set
            {
                // Set velocity to default 
                if (value == BehaviorState.IDLE)
                    Velocity = new Vector3(0);

                actorState = value;
            }
            get { return actorState; }
        }
        public Vector3 Velocity { set; get; }
        public float Speed { protected set; get; }
        public event EventHandler TransformationDirtyEvent;

        public BoundBase GetCharacterCollisionBound()
        {
            return ChildrenComponents[0].Bound;
        }

        public override void UpdateTransformation()
        {
            if (collisionHeadUnit != null)
                collisionHeadUnit.NotifyCollisionObserver(this);
            base.UpdateTransformation();
        }

        public override void Tick(ref Matrix4 projectionMatrix, ref Matrix4 viewMatrix)
        {
            base.Tick(ref projectionMatrix, ref viewMatrix);

            if (ActorState == BehaviorState.FREE_FALLING)
            {
                // Character is in free fall, must be calculated new height regarding to body free falling mechanics
                if (collisionHeadUnit != null)
                    collisionHeadUnit.TryEntityCollision(this);
                Velocity = BodyMechanics.UpdateFreeFallVelocity(Velocity);
            }
            else if (ActorState == BehaviorState.MOVE)
            {
                if (collisionHeadUnit != null)
                    collisionHeadUnit.TryEntityCollision(this);
            }
            else if (ActorState == BehaviorState.IDLE)
            {
                // no implementation here
            }
        }

        public override void SetCollisionHeadUnit(CollisionHeadUnit collisionHeadUnit)
        {
            base.SetCollisionHeadUnit(collisionHeadUnit);
            collisionHeadUnit.AddCollisionObserver(this);
        }

        public void SetActionMovedDelegateListener(EventHandler handler)
        {
            this.TransformationDirtyEvent += handler;
        }

        public void SetPosition(Vector3 newPosition)
        {
            ComponentTranslation = newPosition;
            TransformationDirtyEvent?.Invoke(this, null);
        }

        public void MoveActorForward()
        {
            // Actor shouldn't move while is in free fall state
            if (ActorState == BehaviorState.FREE_FALLING)
                return;

            ActorState = BehaviorState.MOVE;
            Velocity = EngineStatics.Camera.GetEyeSpaceForwardVector() * new Vector3(1, 0, 1);

            var newPosition = ComponentTranslation + Velocity * Speed;
            SetPosition(newPosition);
        }

        public virtual void pushPosition()
        {
            positionMemento.SetSavedOffset(ComponentTranslation);
        }

        public virtual void popPosition()
        {
            ComponentTranslation = positionMemento.GetSavedOffset();
            TransformationDirtyEvent?.Invoke(this, null);
        }

        // maybe will be needed later
        public void MoveActorBack() { }
        public void MoveActorLeft() { }
        public void MoveActorRight() { }

      
    }
}
