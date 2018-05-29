using OpenTK;
using PhysicsBox;
using PhysicsBox.ComponentCore;
using PhysicsBox.MathTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassiveGame.Physics.OutputData;

namespace MassiveGame.Physics
{
    public class CollisionHeadUnit
    {
        List<CollisionUnit> CollisionUnits;

        List<CollisionOutputData> CollisionOutput;

        CollisionBehaviorManager BehaviorManager;

        public void AddCollisionObserver(Component rootComponent)
        {
            CollisionUnits.Add(new CollisionUnit(rootComponent));
        }

        // when component's transform is changed notify bound boxes that they have to be changed
        public void NotifyCollisionObserver(Component rootComponent)
        {
            CollisionUnit characterCollisionUnit = CollisionUnits.Find(unit => unit.RootComponent == rootComponent);
            characterCollisionUnit.bBoundingBoxesTransformDirty = true;
        }

        public CollisionHeadUnit()
        {
            CollisionUnits = new List<CollisionUnit>();
            BehaviorManager = new CollisionBehaviorManager();
            CollisionOutput = new List<CollisionOutputData>(2);
            CollisionOutput.Capacity = 2;
        }

/*
//         public Vector3 GetIntersectionPosition(FRay ray)
//         {
//             float shortestDistance = -1.0f;
// 
//             shortestDistance = TerrainRayIntersection.Intersection_TerrainRay(DOUEngine.terrain, ray);
// 
//             foreach (var unit in CollisionUnits)
//             {
//                 foreach (var bound in unit.GetBoundingBoxes())
//                 {
//                     float localDistance = -1;
//                     var boundType = bound.GetBoundType();
//                     if ((boundType & BoundBase.BoundType.AABB) == BoundBase.BoundType.AABB)
//                     {
//                         localDistance = (GeometricMath.Intersection_RayAABB(ray, bound as AABB));
//                         if (localDistance > -1.0)
//                         {
//                             shortestDistance = Math.Min(shortestDistance, localDistance);
//                         }
//                     }
//                     else if ((boundType & BoundBase.BoundType.OBB) == BoundBase.BoundType.OBB)
//                     {
//                         localDistance = (GeometricMath.Intersection_RayOBB(ray, bound as OBB));
//                         if (localDistance > -1.0)
//                         {
//                             shortestDistance = Math.Min(shortestDistance, localDistance);
//                         }
//                     }
//                 }
//             }
// 
//             return ray.GetPositionInTime(shortestDistance);
//         }
*/

        private bool BoundBaseCollision_Ext(BoundBase characterBound, BoundBase collidedRootBound)
        {
            bool bHasCollision = false;
            BoundBase.BoundType collisionType = characterBound.GetBoundType() | collidedRootBound.GetBoundType();
            
            if (collisionType == (BoundBase.BoundType.AABB | BoundBase.BoundType.OBB))
            {
                AABB aabb = characterBound.GetBoundType() == BoundBase.BoundType.AABB ? characterBound as AABB : collidedRootBound as AABB;
                OBB obb = collidedRootBound.GetBoundType() == BoundBase.BoundType.OBB ? collidedRootBound as OBB : characterBound as OBB;
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

        private void CheckFrameBoundingBoxCollision(ref bool bFrameBoundBoxCollision, ref Component characterRootComponent, ref CollisionUnit characterCollisionUnit,
            ref Component collidedRootComponent, ref List<BoundBase> collidedRootBounds)
        {
            foreach (var unit in CollisionUnits)
            {
                if (characterCollisionUnit.RootComponent == unit.RootComponent)
                    continue;

                AABB aabb1 = characterCollisionUnit.GetFramingBoundBox();
                AABB aabb2 = unit.GetFramingBoundBox();
                if (GeometricMath.AABBAABB(aabb1, aabb2))
                {
                    bFrameBoundBoxCollision = true;
                    collidedRootComponent = aabb2.ParentComponent.GetRootComponent();
                    collidedRootBounds.AddRange(unit.GetBoundingBoxes());
                }
            }

            if (bFrameBoundBoxCollision)
            {
                CollisionOutput.Add(new CollisionOutputFramingAABB(characterRootComponent, collidedRootComponent));
            }
            else
            {
                CollisionOutput.Add(new CollisionOutputNoCollided(characterRootComponent));
            }
        }

        private void CheckRegularBoundingBoxCollision(ref bool bRegularBoundingBoxCollision, ref Component characterRootComponent, ref Component collidedRootComponent,
            ref List<BoundBase> collidedRootBounds, ref BoundBase characterBound, ref List<BoundBase> collidedBounds)
        {
            foreach (var testingBound in collidedRootBounds)
            {
                if (BoundBaseCollision_Ext(characterBound, testingBound))
                {
                    collidedBounds.Add(testingBound);
                }
            }

            if (collidedBounds.Count > 0)
                bRegularBoundingBoxCollision = true;

            if (bRegularBoundingBoxCollision)
            {
                CollisionOutput.Add(new CollisionOutputRegularOBB(characterRootComponent, collidedRootComponent, collidedBounds));
            }
        }

        public void TryCollision(Component characterRootComponent)
        {
            CollisionUnit characterCollisionUnit = CollisionUnits.Find(unit => unit.RootComponent == characterRootComponent);
            Component collidedRootComponent = null;
            BoundBase characterBound = characterCollisionUnit.GetFirstBoundingBox();
            List<BoundBase> collidedRootBounds = new List<BoundBase>();
            bool bFrameBoundingBoxCollision = false, bRegularBoundingBoxCollision = false;

            // Check Collision. Step 1  - check axis aligned framing bounding boxes for collision -- (PRE COLLISION)
            CheckFrameBoundingBoxCollision(ref bFrameBoundingBoxCollision, ref characterRootComponent, ref characterCollisionUnit, ref collidedRootComponent, ref collidedRootBounds);

            // Check Collision. Step 2 - check all bounding boxes of concrete component for collision -- (PURE COLLISION DETECTION)
            List<BoundBase> collidedBounds = new List<BoundBase>();
            CheckRegularBoundingBoxCollision(ref bRegularBoundingBoxCollision, ref characterRootComponent, ref collidedRootComponent, ref collidedRootBounds, ref characterBound, ref collidedBounds);

            // Check Collision. Step 3 - Ray trace -- (BEHAVIOR OF COLLISION RESOLVING)
            BehaviorManager.ProcessCollision(CollisionOutput);

            // Clear all output after collision handling
            CollisionOutput.Clear();
        }
    }
}
