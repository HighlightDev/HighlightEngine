using MassiveGame.Core.ComponentCore;
using MassiveGame.Core.MathCore.MathTypes;
using System.Collections.Generic;

namespace MassiveGame.Core.PhysicsCore.OutputData
{
    public abstract class CollisionOutputData
    {
        public enum OutDataType
        {
            FramingAABB,
            RegularOBB,
            NoCollided
        }

        public abstract OutDataType GetDataType();

        protected object CollisionSender;
        protected object CollisionReceiver;
        protected object CollidedComponentBounds;
    }

    public class CollisionOutputNoCollided : CollisionOutputData
    {
        public Component GetCharacterRootComponent()
        {
            return CollisionSender as Component;
        }

        public override OutDataType GetDataType()
        {
            return OutDataType.NoCollided;
        }

        public CollisionOutputNoCollided(Component collisionSender)
        {
            CollisionSender = collisionSender;
        }
    }

    public class CollisionOutputFramingAABB : CollisionOutputNoCollided
    {
        public Component GetCollidedRootComponent()
        {
            return CollisionReceiver as Component;
        }

        public override OutDataType GetDataType()
        {
            return OutDataType.FramingAABB;
        }

        public CollisionOutputFramingAABB(Component collisionSender, Component collisionReceiver) : base(collisionSender)
        {
            CollisionReceiver = collisionReceiver;
        }
    }

    public class CollisionOutputRegularOBB : CollisionOutputFramingAABB
    {
        public List<BoundBase> GetCollidedBoundingBoxes()
        {
            return CollidedComponentBounds as List<BoundBase>;
        }

        public override OutDataType GetDataType()
        {
            return OutDataType.RegularOBB;
        }

        public CollisionOutputRegularOBB(Component collisionSender, Component collisionReceiver, List<BoundBase> collidedComponentBounds) :
            base(collisionSender, collisionReceiver)
        {
            CollidedComponentBounds = collidedComponentBounds;
        }
    }
}
