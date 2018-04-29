using System;
using System.Collections.Generic;
using PhysicsBox;
using MassiveGame.RenderCore.Lights;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using MassiveGame.RenderCore.Shadows;

namespace MassiveGame
{
    #region DirectionEnum

    public enum directions // Directions of object
    {
        LEFT,
        RIGHT,
        FORWARD,
        BACK,
        STAY
    }

    #endregion

    public abstract class MotionEntities : Entity
    {
        #region Definitions

        protected int _id;
        protected Vector3 Move { set; get; }
        protected float _speed;
        protected MovementStack objectStack;
        public event EventHandler ActionMove;

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

        public Vector3 GetMove()
        {
            return Move;
        }

        #endregion

        #region Collision

        public override void SetCollisionDetector(CollisionDetector collisionDetector)
        {
            base.SetCollisionDetector(collisionDetector);
            this.collisionDetection.addCollisionBox(this.Box);
        }

        #endregion

        #region Setter

        protected float setRotation(directions direction)
        {
            switch (direction)
            {
                case directions.FORWARD:
                    {
                        return MathHelper.DegreesToRadians(180.0f);
                    }
                case directions.BACK:
                    {
                        return MathHelper.DegreesToRadians(0.0f);
                    }
                case directions.LEFT:
                    {
                        return MathHelper.DegreesToRadians(90.0f);
                    }
                case directions.RIGHT:
                    {
                        return MathHelper.DegreesToRadians(270.0f);
                    }
                default :
                    return MathHelper.DegreesToRadians(0.0f);
            }
        }

        #endregion

        #region Renderer

        public virtual void renderObject(PrimitiveType mode, bool enableNormalMapping, DirectionalLight Sun,
            List<PointLight> lights, Terrain terrain, LiteCamera camera, ref Matrix4 ProjectionMatrix, Vector4 clipPlane)
        {

        }

        #endregion

        #region Movement

        public void SetActionMovedDelegateListener(EventHandler handler)
        {
            this.ActionMove += handler;
        }

        public virtual void objectMove(directions direction, Terrain terrain)       //Move object in 5 directions
        {
            float heightStep = terrain.getLandscapeHeight(Box.getCenter().X, Box.getCenter().Z);
            Rotation = new Vector3(rotation.X, setRotation(direction), rotation.Z);
            switch (direction)
            {
                case directions.LEFT:
                    {
                        this.Move = new Vector3(this.Move.X - _speed, heightStep - _bottomY, this.Move.Z);
                        break;
                    }
                case directions.RIGHT:
                    {
                        this.Move = new Vector3(this.Move.X + _speed, heightStep - _bottomY, this.Move.Z);
                        break;
                    }
                case directions.FORWARD:
                    {
                        this.Move = new Vector3(this.Move.X, heightStep - _bottomY, this.Move.Z - _speed);
                        break;
                    }
                case directions.BACK:
                    {
                        this.Move = new Vector3(this.Move.X, heightStep - _bottomY, this.Move.Z + _speed);
                        break;
                    }
            }

            this.Box.synchronizeCoordinates(this._leftX + this.Move.X, base._rightX + this.Move.X,
                    this._bottomY + this.Move.Y, this._topY + this.Move.Y, this._nearZ + this.Move.Z, this._farZ + this.Move.Z);

            // begin collision detection
            if (!Object.Equals(collisionDetection, null))
            {
                collisionDetection.isCollision(this.Box);
            }

            #region Current entity position calculation

            ObjectPosition = new Vector3(
                (objectStack.LeftX + objectStack.RightX) / 2 + Move.X,
                Move.Y,
                (objectStack.FarZ + objectStack.NearZ) / 2 + Move.Z
            );

            #endregion

            #region Movement of sound source
            ////need to add to center of a sound movement offset on X axis and Z axis
            //sourceStep.SetPosition(
            //    soundCenter.X + base.Move.X,
            //    base.Move.Y,
            //    soundCenter.Z + base.Move.Z
            //);
            //sourceCollide.SetPosition(
            //    soundCenter.X + base.Move.X,
            //    base.Move.Y,
            //    soundCenter.Z + base.Move.Z
            //);
            #endregion

            // need to add if statement in case of collision

            //if (!sourceStep.IsPlaying())
            //{
            //    sourceStep.SetBuffer(_SB_step[_SB_stepSwitcher++]);
            //    if (_SB_stepSwitcher == _SB_step.Length)
            //        _SB_stepSwitcher = 0;
            //    sourceStep.Play();
            //}
        }

        #endregion

        // TEMPORARY
        public void move(Terrain terrain, Camera camera)
        {
            if (ActionMove != null)
            {
                this.ActionMove(this, null);
            }

            var direction = camera.GetDirection();
            direction.Y = 0;

            Vector3 donePath = direction * Speed;
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

            // Update bound's transformation
            bTransformationDirty = true;
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
