using System;
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

    public abstract class MotionEntities : Entity
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

        [Obsolete("DEPRECATED PROPERTY, MUST BE ELIMINATED")]
        protected int _id;

        private Vector3 move;

        [Obsolete("DEPRECATED PROPERTY, MUST BE ELIMINATED")]
        public Vector3 Move
        {
            set
            {
                move = value;
                bTransformationDirty = true;
            }
            get
            {
                return move;
            }
        }

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

        public Vector3 ObjectPosition { get; private set; }

        #endregion

        public override void Tick(ref Matrix4 projectionMatrix, ref Matrix4 viewMatrix)
        {
            /*
            // Character is in free fall, must be calculated new height regarding to body free falling mechanics
            if (ActorState == BEHAVIOR_STATE.FREE_FALLING)
            {
                throw new NotImplementedException("IMPLEMENT FREE FALLING");
                // DO BETTER!

                Velocity = BodyMechanics.UpdateFreeFallVelocity(Velocity, Speed);

                throw new NotImplementedException("IMPLEMENT FREE FALLING");
                // Check collision
                (new CollisionHeadUnit()).TryCollision(this);
            }
            else if (ActorState == BEHAVIOR_STATE.MOVE)
            {
                throw new NotImplementedException("IMPLEMENT COLLISIONS");
                // Check collision
                (new CollisionHeadUnit()).TryCollision(this);
            }
            else if (ActorState == BEHAVIOR_STATE.IDLE)
            {

            }*/
            
            base.Tick(ref projectionMatrix, ref viewMatrix);
        }

        #region Collision

        public override void SetCollisionDetector(CollisionDetector collisionDetector)
        {
            base.SetCollisionDetector(collisionDetector);
            this.collisionDetection.addCollisionBox(this.Box);
        }

        #endregion

        #region Renderer

        public abstract void renderObject(PrimitiveType mode, bool enableNormalMapping, DirectionalLight Sun,
            List<PointLight> lights, Terrain terrain, LiteCamera camera, ref Matrix4 ProjectionMatrix, Vector4 clipPlane);

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
            this.Move = new Vector3(Move.X, newPosition.Y - _bottomY, Move.Z);
            ObjectPosition = new Vector3(
               (objectStack.LeftX + objectStack.RightX) / 2 + Move.X,
               Move.Y,
               (objectStack.FarZ + objectStack.NearZ) / 2 + Move.Z
           );
        }

        public void MoveActor(Terrain terrain)
        {
            // Actor shouldn't move while free fall
            if (ActorState == BEHAVIOR_STATE.FREE_FALLING)
                return;

            ActorState = BEHAVIOR_STATE.MOVE;
            var velocityVector = DOUEngine.Camera.GetNormalizedDirection();
            velocityVector.Y = 0.0f;
            Velocity = velocityVector;

            if (ActionMove != null)
            {
                this.ActionMove(this, null);
            }

            Vector3 donePath = Velocity * Speed;
            Vector3 newPosition = Box.getCenter() + donePath;
            float heightStep = terrain.getLandscapeHeight(newPosition.X, newPosition.Z);
            this.Move = new Vector3(donePath.X + Move.X, heightStep - _bottomY, donePath.Z + Move.Z);

            this.Box.synchronizeCoordinates(
                this._leftX + this.Move.X,
                base._rightX + this.Move.X,
                this._bottomY + this.Move.Y,
                this._topY + this.Move.Y,
                this._nearZ + this.Move.Z,
                this._farZ + this.Move.Z);

            // begin collision detection
            if (!Object.Equals(collisionDetection, null))
            {
                collisionDetection.isCollision(this.Box);
            }

            ObjectPosition = new Vector3(
                (objectStack.LeftX + objectStack.RightX) / 2 + Move.X,
                Move.Y,
                (objectStack.FarZ + objectStack.NearZ) / 2 + Move.Z
            );
        }

        #region Position_stack_functions

        public virtual void pushPositionStack()
        {
            objectStack.resetPositionValues(Move.X, Move.Y, Move.Z);
        }

        public virtual void popPositionStack()
        {
            Move = objectStack.popPositionValues();
            this._box.synchronizeCoordinates(_leftX + Move.X, _rightX + Move.X,
                    _bottomY + Move.Y, _topY + Move.Y, _nearZ + Move.Z, _farZ + Move.Z);
        }

        #endregion

        #region Constructor

        public MotionEntities() { }

        public MotionEntities(string modelPath, string texturePath, string normalMapPath, string specularMapPath
            , float Speed, int ID, Vector3 translation, Vector3 rotation, Vector3 scale) :
            base(modelPath, texturePath, normalMapPath, specularMapPath, translation, rotation, scale)
        {
            ActorState = BEHAVIOR_STATE.IDLE;
            _verticesVectors = null;
            this.Move = new Vector3(0.0f);
            this._speed = Speed;
            this._id = ID;
            this._box.ID = ID;
            this.objectStack = new MovementStack(base._leftX, base._rightX, base._bottomY, base._topY,
               base._nearZ, base._farZ);
            this.ObjectPosition = translation;
        }

        #endregion
    }
}
