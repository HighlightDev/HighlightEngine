﻿using System;
using System.Collections.Generic;
using PhysicsBox;
using MassiveGame.RenderCore.Lights;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using MassiveGame.RenderCore.Shadows;
using PhysicsBox.MathTypes;
using MassiveGame.Physics;

namespace MassiveGame
{
    public enum BEHAVIOR_STATE
    {
        IDLE,
        MOVE,
        FREE_FALLING
    }

    public abstract class MovableEntity : Entity
    {
        #region Definitions

        protected BEHAVIOR_STATE actorState;
        protected Vector3 velocity;

        public BEHAVIOR_STATE ActorState
        {
            set
            {
                // Set velocity to default 
                if (value == BEHAVIOR_STATE.IDLE)
                    Velocity = new Vector3(0);
                actorState = value;
            }
            get { return actorState; }
        }

        public Vector3 Velocity
        {
            set { velocity = value; }
            get { return velocity; }
        }

        public BoundBase GetCharacterCollisionBound()
        {
            return ChildrenComponents[0].Bound;
        }

        [Obsolete("DEPRECATED PROPERTY, MUST BE ELIMINATED")]
        protected int _id;

        protected float _speed;
        protected MovementStack objectStack;
        public event EventHandler ActionMove;

        [Obsolete("DEPRECATED PROPERTY, MUST BE ELIMINATED")]
        public override CollisionSphereBox Box
        {
            protected set { base._box = value; }
            get
            {
                return this._box;
            }
        }

        public virtual float Speed
        {
            protected set { this._speed = value; }
            get { return this._speed; }
        }

        #endregion

        public override void UpdateTransformation()
        {
            if (collisionHeadUnit != null)
                collisionHeadUnit.NotifyCollisionObserver(this);
            base.UpdateTransformation();
        }

        public override void Tick(ref Matrix4 projectionMatrix, ref Matrix4 viewMatrix)
        {
            base.Tick(ref projectionMatrix, ref viewMatrix);

            if (ActorState == BEHAVIOR_STATE.FREE_FALLING)
            {
                // Character is in free fall, must be calculated new height regarding to body free falling mechanics
                if (collisionHeadUnit != null)
                    collisionHeadUnit.TryCollision(this);
                Velocity = BodyMechanics.UpdateFreeFallVelocity(Velocity);
            }
            else if (ActorState == BEHAVIOR_STATE.MOVE)
            {
                if (collisionHeadUnit != null)
                    collisionHeadUnit.TryCollision(this);
            }
            else if (ActorState == BEHAVIOR_STATE.IDLE)
            {

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
            List<PointLight> lights, LiteCamera camera, ref Matrix4 ProjectionMatrix, Vector4 clipPlane);

        #endregion

        #region Movement

        public void SetActionMovedDelegateListener(EventHandler handler)
        {
            this.ActionMove += handler;
        }

        #endregion

        // TEMPORARY

        public void collisionOffset(Vector3 newPosition)
        {
            ComponentTranslation = newPosition + new Vector3(0, GetCharacterCollisionBound().GetExtent().Y, 0);
            if (ActionMove != null)
                ActionMove(this, null);
        }

        public void MoveActor()
        {
            // Actor shouldn't move while free fall
            if (ActorState == BEHAVIOR_STATE.FREE_FALLING)
                return;

            ActorState = BEHAVIOR_STATE.MOVE;
            var velocityVector = DOUEngine.Camera.GetNormalizedDirection();
            velocityVector.Y = 0.0f;
            Velocity = velocityVector;

            if (ActionMove != null)
                this.ActionMove(this, null);

            ComponentTranslation = ComponentTranslation + Velocity * Speed;
        }

        #region Position_stack_functions

        public virtual void pushPositionStack()
        {
            objectStack.resetPositionValues(ComponentTranslation.X, ComponentTranslation.Y, ComponentTranslation.Z);
        }

        public virtual void popPositionStack()
        {
            ComponentTranslation = objectStack.popPositionValues();
            if (ActionMove != null)
                ActionMove(this, null);
        }

        #endregion

        #region Constructor

        public MovableEntity() { }

        public MovableEntity(string modelPath, string texturePath, string normalMapPath, string specularMapPath
            , float Speed, int ID, Vector3 translation, Vector3 rotation, Vector3 scale) :
            base(modelPath, texturePath, normalMapPath, specularMapPath, translation, rotation, scale)
        {
            ActorState = BEHAVIOR_STATE.FREE_FALLING;
            this.objectStack = new MovementStack();
            pushPositionStack();
            this._speed = Speed;
            this._id = ID;
            this._box.ID = ID;
        }

        #endregion
    }
}
