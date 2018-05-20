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

        private bool BoundBaseCollision_Ext(BoundBase characterBound, BoundBase collidedRootBound)
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
                    collidedRootBounds = unit.GetBoundingBoxes();
                    break;
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
            List<BoundBase> collidedRootBounds = null;
            bool bFrameBoundingBoxCollision = false, bRegularBoundingBoxCollision = false;

            // Check Collision. Step 1  - check axis aligned framing bounding boxes for collision -- (PRE COLLISION)
            CheckFrameBoundingBoxCollision(ref bFrameBoundingBoxCollision, ref characterRootComponent, ref characterCollisionUnit, ref collidedRootComponent, ref collidedRootBounds);

            // Check Collision. Step 2 - check all bounding boxes of concrete component for collision -- (PURE COLLISION DETECTION)
            // character must have only one collision bound! take only first from list!
            List<BoundBase> collidedBounds = new List<BoundBase>();
            CheckRegularBoundingBoxCollision(ref bRegularBoundingBoxCollision, ref characterRootComponent, ref collidedRootComponent, ref collidedRootBounds, ref characterBound, ref collidedRootBounds);

            // !DO ALL RAY TRACINGS SOMEWHERE ELSE, THAT IS A SPECIFIC BEHAVIOR OF EACH CHARACTER! //

            // Check Collision. Step 3 - Ray trace to find out height of current step (character can be in air, so check it) -- (BEHAVIOR OF COLLISION RESOLVING)

            BehaviorManager.ProcessCollision(CollisionOutput);

            // Clear all output after collision handling
            CollisionOutput.Clear();
        }
    }
}
