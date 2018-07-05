using System;
using System.Collections.Generic;
using PhysicsBox;
using MassiveGame.RenderCore.Lights;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using MassiveGame.RenderCore.Shadows;
using PhysicsBox.MathTypes;
using MassiveGame.Physics;
using MassiveGame.Core;

namespace MassiveGame
{
    public enum BehaviorState
    {
        IDLE,
        MOVE,
        FREE_FALLING
    }

    public abstract class MovableEntity : Entity
    {
        protected BehaviorState actorState;
        protected MovementStack objectStack;

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

        #region Collision

        public override void SetCollisionHeadUnit(CollisionHeadUnit collisionHeadUnit)
        {
            base.SetCollisionHeadUnit(collisionHeadUnit);
            collisionHeadUnit.AddCollisionObserver(this);
        }

        #endregion

        #region Renderer

        public abstract void renderObject(PrimitiveType mode, bool enableNormalMapping, DirectionalLight Sun,
            List<PointLight> lights, BaseCamera camera, ref Matrix4 ProjectionMatrix, Vector4 clipPlane);

        #endregion

        #region Movement

        public void SetActionMovedDelegateListener(EventHandler handler)
        {
            this.TransformationDirtyEvent += handler;
        }

        #endregion

        // TEMPORARY

        public void collisionOffset(Vector3 newPosition)
        {
            ComponentTranslation = newPosition;
            if (TransformationDirtyEvent != null)
                TransformationDirtyEvent(this, null);
        }

        public void MoveActor()
        {
            // Actor shouldn't move while free fall
            if (ActorState == BehaviorState.FREE_FALLING)
                return;

            ActorState = BehaviorState.MOVE;
            var velocityVector = EngineStatics.Camera.GetEyeSpaceForwardVector();
            velocityVector.Y = 0.0f;
            Velocity = velocityVector;

            ComponentTranslation = ComponentTranslation + Velocity * Speed;

            if (TransformationDirtyEvent != null)
                this.TransformationDirtyEvent(this, null);
        }

        #region Position_stack_functions

        public virtual void pushPositionStack()
        {
            objectStack.resetPositionValues(ComponentTranslation.X, ComponentTranslation.Y, ComponentTranslation.Z);
        }

        public virtual void popPositionStack()
        {
            ComponentTranslation = objectStack.popPositionValues();
            if (TransformationDirtyEvent != null)
                TransformationDirtyEvent(this, null);
        }

        public Vector3 GetStackPosition()
        {
            return objectStack.popPositionValues();
        }

        #endregion

        #region Constructor

        public MovableEntity() { }

        public MovableEntity(string modelPath, string texturePath, string normalMapPath, string specularMapPath
            , float Speed, Vector3 translation, Vector3 rotation, Vector3 scale) :
            base(modelPath, texturePath, normalMapPath, specularMapPath, translation, rotation, scale)
        {
            ActorState = BehaviorState.FREE_FALLING;
            this.objectStack = new MovementStack();
            pushPositionStack();
            this.Speed = Speed;
        }

        #endregion
    }
}
