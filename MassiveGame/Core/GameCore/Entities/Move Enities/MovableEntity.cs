using System;
using System.Collections.Generic;
using PhysicsBox;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using PhysicsBox.MathTypes;
using MassiveGame.Core.PhysicsCore;
using MassiveGame.Core.RenderCore.Lights;

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
        protected BehaviorState actorState;
        protected ActorPositionMemento positionMemento;

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

        public abstract void renderObject(PrimitiveType mode, bool enableNormalMapping, DirectionalLight Sun,
            List<PointLight> lights, BaseCamera camera, ref Matrix4 ProjectionMatrix, Vector4 clipPlane);

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

        // maybe will be needed later
        public void MoveActorBack() { }
        public void MoveActorLeft() { }
        public void MoveActorRight() { }

        #region Position_stack_functions

        public virtual void pushPosition()
        {
            positionMemento.SetSavedOffset(ComponentTranslation);
        }

        public virtual void popPosition()
        {
            ComponentTranslation = positionMemento.GetSavedOffset();
            TransformationDirtyEvent?.Invoke(this, null);
        }

        #endregion

        #region Constructor

        public MovableEntity() { }

        public MovableEntity(string modelPath, string texturePath, string normalMapPath, string specularMapPath
            , float Speed, Vector3 translation, Vector3 rotation, Vector3 scale) :
            base(modelPath, texturePath, normalMapPath, specularMapPath, translation, rotation, scale)
        {
            ActorState = BehaviorState.FREE_FALLING;
            this.positionMemento = new ActorPositionMemento();
            pushPosition();
            this.Speed = Speed;
        }

        #endregion
    }
}
