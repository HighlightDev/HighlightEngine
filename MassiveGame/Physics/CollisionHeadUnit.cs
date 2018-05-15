using OpenTK;
using PhysicsBox;
using PhysicsBox.ComponentCore;
using PhysicsBox.MathTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassiveGame.Physics
{
    public class CollisionHeadUnit
    {
        public delegate void CollisionDelegate(Component character, Component collidedComponent);
        public event CollisionDelegate FrameAABB_Collision;
        public event CollisionDelegate MemberBB_Collision;

        List<CollisionUnit> collisionUnits;

        public void AddCollisionObserver(Component rootComponent)
        {
            collisionUnits.Add(new CollisionUnit(rootComponent));
        }

        public CollisionHeadUnit()
        {
            collisionUnits = new List<CollisionUnit>();
        }

        private bool CheckBoundsCollision(BoundBase characterBound, BoundBase collidedRootBound)
        {
            bool bHasCollision = false;
            BoundBase.BoundType collisionType = characterBound.GetBoundType() | collidedRootBound.GetBoundType();
            
            if (collisionType == (BoundBase.BoundType.AABB | BoundBase.BoundType.OBB))
            {
                AABB aabb = collisionType == BoundBase.BoundType.AABB ? characterBound as AABB : collidedRootBound as AABB;
                OBB obb = collisionType == BoundBase.BoundType.OBB ? characterBound as OBB : collidedRootBound as OBB;
                bHasCollision = GeometricMath.AABBOBB(aabb, obb);
            }
            else if (collisionType == (BoundBase.BoundType.AABB | BoundBase.BoundType.AABB))
            {
                bHasCollision = GeometricMath.AABBAABB(characterBound as AABB, collidedRootBound as AABB);
            }
            else
            {
                bHasCollision = GeometricMath.OBBOBB(characterBound as OBB, collidedRootBound as OBB);
            }
            return bHasCollision;
        }

        public void TryCollision(Component characterRootComponent)
        {
            CollisionUnit characterCollisionUnit = collisionUnits.Find(unit => unit.RootComponent == characterRootComponent);
            Component collidedRootComponent = null;
            BoundBase characterBound = characterCollisionUnit.GetBoundingBoxes().First();
            List<BoundBase> collidedRootBounds = null;

            bool bFrameBoundBoxCollision = false;
            
            // Check Collision. Step 1  - check axis aligned bounding boxes for collision
            foreach (var unit in collisionUnits)
            {
                if (characterCollisionUnit.RootComponent == unit.RootComponent)
                    continue;

                AABB aabb1 = characterCollisionUnit.GetFramingBoundBox();
                AABB aabb2 = unit.GetFramingBoundBox();
                if (GeometricMath.AABBAABB(aabb1, aabb2))
                {
                    bFrameBoundBoxCollision = true;
                    collidedRootComponent = aabb2.ParentComponent.GetRootComponent();
                    collidedRootBounds = unit.GetBoundingBoxes();
                    break;
                }
            }

            if (bFrameBoundBoxCollision)
            {
                FrameAABB_Collision(characterRootComponent, collidedRootComponent);
            }

            // TODO:
            // Check Collision. Step 2 - check all bounding boxes of concrete component for collision
            // character must have only one collision bound! take only first from list!
            List<BoundBase> collidedBounds = new List<BoundBase>();
            foreach (var collisionBox in collidedRootBounds)
            {
                if (CheckBoundsCollision(characterBound, collisionBox))
                    collidedBounds.Add(collisionBox);
            }

            // Check Collision. Step 3 - Ray trace to find out height of current step (character can be in air, so check it)

        }

        // when component's transform is changed notify bound boxes that they have to be changed
        public void NotifyObserver(Component rootComponent)
        {
            CollisionUnit characterCollisionUnit = collisionUnits.Find(unit => unit.RootComponent == rootComponent);
            characterCollisionUnit.bBoundingBoxesTransformDirty = true;
        }
    }
}
