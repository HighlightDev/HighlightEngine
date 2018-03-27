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

        public void FindCollision(Component characterRootComponent)
        {
            CollisionUnit characterCollisionUnit = collisionUnits.Find(unit => unit.RootComponent == characterRootComponent);
            Component collidedRootComponent = null;

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
                    break;
                }
            }

            if (bFrameBoundBoxCollision)
            {
                FrameAABB_Collision(characterRootComponent, collidedRootComponent);
            }

            // TODO:
            // Check Collision. Step 2 - check all bounding boxes of concrete component for collision
            // Check Collision. Step 3 - Ray trace to find out height of current step (character can be in air, so check it)

        }

        // when component's transform is changed notify bound boxes that they have to be changed
        public void NotifyObservers()
        {

        }
    }
}
