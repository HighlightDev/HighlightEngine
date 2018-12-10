using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using MassiveGame.Core.PhysicsCore;
using MassiveGame.Core.RenderCore.Lights;
using MassiveGame.Core.GameCore.Water;
using TextureLoader;
using MassiveGame.Settings;
using MassiveGame.API.ResourcePool.PoolHandling;
using MassiveGame.API.ResourcePool.Policies;
using MassiveGame.API.ResourcePool;
using MassiveGame.Core.MathCore;
using MassiveGame.Core.MathCore.MathTypes;
using System.Runtime.Serialization;
using MassiveGame.Core.RenderCore.Shadows;

namespace MassiveGame.Core.GameCore.Entities.MoveEntities
{
    [Serializable]
    public enum BehaviorState
    {
        IDLE,
        MOVE,
        FREE_FALLING
    }

    [Serializable]
    public abstract class MovableEntity : Entity
    {
        #region Definitions

        protected BehaviorState m_actorState;

        protected ActorPositionMemento m_positionMemento;

        private Vector3 m_velocity;

        private float m_speed;

        private WaterReflectionEntityShader m_liteReflectionShader;

        private WaterRefractionEntityShader m_liteRefractionShader;

        public BehaviorState ActorState
        {
            set
            {
                m_actorState = value;

                // Set velocity to zero 
                if (value == BehaviorState.IDLE)
                    Velocity = Vector3.Zero;
            }
            get
            {
                return m_actorState;
            }
        }

        public Vector3 Velocity { set { m_velocity = value; } get { return m_velocity; } }

        public float Speed { set { m_speed = value; } get { return m_speed; } }

        #endregion

        #region Constructors

        public MovableEntity() { }

        public MovableEntity(string modelPath, string texturePath, string normalMapPath, string specularMapPath, Vector3 translation, Vector3 rotation, Vector3 scale) :
            base(modelPath, texturePath, normalMapPath, specularMapPath, translation, rotation, scale)
        {
            ActorState = BehaviorState.FREE_FALLING;
            m_positionMemento = new ActorPositionMemento();
            pushPosition();
            this.Speed = 0.3f;
        }

        private void postConstructor()
        {
            if (bPostConstructor)
                bPostConstructor = false;
        }

        #endregion

        #region Serialization

        protected MovableEntity(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            m_actorState = (BehaviorState)info.GetValue("actorState", typeof(BehaviorState));
            m_positionMemento = (ActorPositionMemento)info.GetValue("positionMemento", typeof(ActorPositionMemento));
            m_velocity = (Vector3)info.GetValue("velocity", typeof(Vector3));
            m_speed = info.GetSingle("speed");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("actorState", m_actorState, typeof(BehaviorState));
            info.AddValue("positionMemento", m_positionMemento, typeof(ActorPositionMemento));
            info.AddValue("velocity", m_velocity, typeof(Vector3));
            info.AddValue("speed", m_speed);
        }

        #endregion

        private MovableEntityShader GetShader()
        {
            return m_shader as MovableEntityShader;
        }

        protected override void InitShader()
        {
            m_shader = PoolProxy.GetResource<ObtainShaderPool, ShaderAllocationPolicy<MovableEntityShader>, string, MovableEntityShader>(ProjectFolders.ShadersPath + "movableEntityVS.glsl" + "," + ProjectFolders.ShadersPath + "movableEntityFS.glsl");
            m_liteReflectionShader = PoolProxy.GetResource<ObtainShaderPool, ShaderAllocationPolicy<WaterReflectionEntityShader>, string, WaterReflectionEntityShader>(ProjectFolders.ShadersPath + "waterReflectionEntityVS.glsl" + "," + ProjectFolders.ShadersPath + "waterReflectionEntityFS.glsl");
            m_liteRefractionShader = PoolProxy.GetResource<ObtainShaderPool, ShaderAllocationPolicy<WaterRefractionEntityShader>, string, WaterRefractionEntityShader>(ProjectFolders.ShadersPath + "waterRefractionEntityVS.glsl" + "," + ProjectFolders.ShadersPath + "waterRefractionEntityFS.glsl");
        }

        protected override void FreeShader()
        {
            PoolProxy.FreeResourceMemory<ObtainShaderPool, ShaderAllocationPolicy<MovableEntityShader>, string, MovableEntityShader>(GetShader());
            PoolProxy.FreeResourceMemory<ObtainShaderPool, ShaderAllocationPolicy<WaterReflectionEntityShader>, string, WaterReflectionEntityShader>(m_liteReflectionShader);
            PoolProxy.FreeResourceMemory<ObtainShaderPool, ShaderAllocationPolicy<WaterRefractionEntityShader>, string, WaterRefractionEntityShader>(m_liteRefractionShader);
        }

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
            Vector4 clipPlane = default(Vector4))
        {
            if (bPostConstructor)
                return;

            Matrix4 modelMatrix;
            modelMatrix = GetWorldMatrix();

            /*If clip plane is set - enable clipping plane*/
            if (GeometryMath.CMP(clipPlane.LengthSquared, 0.0f) > 0)
                GL.Disable(EnableCap.ClipDistance0); 
            else
                GL.Enable(EnableCap.ClipDistance0); 

            m_liteRefractionShader.startProgram();   

            m_texture.BindTexture(TextureUnit.Texture0);
            m_normalMap?.BindTexture(TextureUnit.Texture1); 

            m_liteRefractionShader.SetAlbedoTexture(0);
            m_liteRefractionShader.SetNormalMap(1);
            m_liteRefractionShader.SetMaterial(m_material);
            m_liteRefractionShader.SetTransformationMatrices(ref modelMatrix, camera.GetViewMatrix(), ref ProjectionMatrix);
            m_liteRefractionShader.SetDirectionalLight(Sun);
            m_liteRefractionShader.SetClipPlane(ref clipPlane);

            m_skin.Buffer.RenderVAO(PrimitiveType.Triangles);
            m_liteRefractionShader.stopProgram();
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

            m_liteReflectionShader.startProgram();

            m_texture.BindTexture(TextureUnit.Texture0);
            m_normalMap?.BindTexture(TextureUnit.Texture1); 

            m_liteReflectionShader.SetTexture(0);
            m_liteReflectionShader.SetNormalMap(1);
            m_liteReflectionShader.SetMaterial(m_material);
            m_liteReflectionShader.SetTransformationMatrices(ref mirrorMatrix, ref modelMatrix, camera.GetViewMatrix(), ref ProjectionMatrix);
            m_liteReflectionShader.SetDirectionalLight(Sun);
            m_liteReflectionShader.SetClipPlane(ref clipPlane);

            m_skin.Buffer.RenderVAO(PrimitiveType.Triangles);
            m_liteReflectionShader.stopProgram();
        }

        public virtual void RenderEntity(PrimitiveType mode, DirectionalLight directionalLight,
            List<PointLight> lights, BaseCamera camera, ref Matrix4 ProjectionMatrix, Vector4 clipPlane = new Vector4())
        {
            postConstructor();

            Matrix4 modelMatrix;
            modelMatrix = GetWorldMatrix();

            /*If clip plane is set - enable clipping plane*/
            if (clipPlane.X == 0 && clipPlane.Y == 0 && clipPlane.Z == 0 && clipPlane.W == 0) { GL.Disable(EnableCap.ClipDistance0); }
            else { GL.Enable(EnableCap.ClipDistance0); }

            GetShader().startProgram();

            // pass uniform variables to shader
            if (directionalLight != null && directionalLight.GetHasShadow())
            {
                DirectionalLightWithShadow lightWithShadow = directionalLight as DirectionalLightWithShadow;
                // Get shadow handler
                ITexture shadowMap = lightWithShadow.GetShadowMapTexture();
                shadowMap.BindTexture(TextureUnit.Texture1);
                GetShader().SetDirectionalLightShadowMatrix(lightWithShadow.GetShadowMatrix(ref modelMatrix, ref ProjectionMatrix));
            }
            m_texture.BindTexture(TextureUnit.Texture0);
            bool bEnableNormalMap = m_normalMap != null;
            if (bEnableNormalMap)
                m_normalMap.BindTexture(TextureUnit.Texture2);

            GetShader().SetDiffuseMap(0);
            GetShader().SetNormalMap(2, bEnableNormalMap);
            GetShader().SetMaterial(m_material);
            GetShader().SetTransformationMatrices(ref modelMatrix, camera.GetViewMatrix(), ref ProjectionMatrix);
            GetShader().SetPointLights(GetRelevantPointLights(lights));
            GetShader().SetDirectionalLight(directionalLight);
            GetShader().SetClippingPlane(ref clipPlane);
            GetShader().SetMist(m_mist);
            GetShader().SetDirectionalLightShadowMap(1);

            m_skin.Buffer.RenderVAO(mode);
            GetShader().stopProgram();
        }

        #endregion

        #region Cleaning

        public override void CleanUp()
        {
            base.CleanUp();
        }

        #endregion

        public BoundBase GetCharacterCollisionBound()
        {
            return ChildrenComponents[0].Bound;
        }

        public override void UpdateTransformation()
        {
            if (m_collisionHeadUnit != null)
                m_collisionHeadUnit.NotifyCollisionObserver(this);
            base.UpdateTransformation();
        }

        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);

            if (ActorState == BehaviorState.FREE_FALLING)
            {
                // Character is in free fall, must be calculated new height regarding to body free falling mechanics
                if (m_collisionHeadUnit != null)
                    m_collisionHeadUnit.TryEntityCollision(this);
                Velocity = BodyMechanics.UpdateFreeFallVelocity(Velocity);
            }
            else if (ActorState == BehaviorState.MOVE)
            {
                if (m_collisionHeadUnit != null)
                    m_collisionHeadUnit.TryEntityCollision(this);
            }
            else if (ActorState == BehaviorState.IDLE)
            {
                // no implementation here
            }
        }

        public void SetPosition(Vector3 newPosition)
        {
            ComponentTranslation = newPosition;
            ProceedTransformationDirty();
        }

        private void ProceedTransformationDirty()
        {
            BaseCamera worldCamera = GameWorld.GetWorldInstance().GetLevel().Camera;

            if (worldCamera != null && worldCamera is ThirdPersonCamera)
            {
                (worldCamera as ThirdPersonCamera).SetThirdPersonTargetTransformationDirty(); // Update position of third person camera
            }
        }

        public void MoveActorForward()
        {
            // Actor shouldn't move while is in free fall state
            if (ActorState == BehaviorState.FREE_FALLING)
                return;

            ActorState = BehaviorState.MOVE;
            Velocity = GameWorld.GetWorldInstance().GetLevel().Camera.GetEyeSpaceForwardVector() * new Vector3(1, 0, 1)/*truncate y-velocity*/;

            var newPosition = ComponentTranslation + Velocity * Speed;
            SetPosition(newPosition);
        }

        public virtual void pushPosition()
        {
            m_positionMemento.SetSavedOffset(ComponentTranslation);
        }

        public virtual void popPosition()
        {
            ComponentTranslation = m_positionMemento.GetSavedOffset();
            ProceedTransformationDirty();
        }

        // maybe will be needed later
        public void MoveActorBack() { }
        public void MoveActorLeft() { }
        public void MoveActorRight() { }

    }
}
