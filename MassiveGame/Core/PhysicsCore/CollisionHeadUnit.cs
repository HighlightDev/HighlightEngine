using MassiveGame.Core.GameCore;
using MassiveGame.Core.PhysicsCore.OutputData;
using OpenTK;
using MassiveGame.Core.ComponentCore;
using System.Collections.Generic;
using MassiveGame.Core.MathCore;
using MassiveGame.Core.MathCore.MathTypes;

namespace MassiveGame.Core.PhysicsCore
{
    public class CollisionHeadUnit
    {
        // ENTITY

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
                AABB aabb = characterBound.GetBoundType() == BoundBase.BoundType.AABB ? characterBound as AABB : collidedRootBound as AABB;
                OBB obb = collidedRootBound.GetBoundType() == BoundBase.BoundType.OBB ? collidedRootBound as OBB : characterBound as OBB;
                bHasCollision = GeometryMath.AABBOBB(aabb, obb);
            }
            else if (collisionType == (BoundBase.BoundType.AABB | BoundBase.BoundType.AABB))
            {
                bHasCollision = GeometryMath.AABBAABB(characterBound as AABB, collidedRootBound as AABB);
            }
            else
            {
                bHasCollision = GeometryMath.OBBOBB(characterBound as OBB, collidedRootBound as OBB);
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

                AABB aabb1 = characterCollisionUnit.GetAndTryUpdateFramingBoundingBox();
                AABB aabb2 = unit.GetAndTryUpdateFramingBoundingBox();
                if (GeometryMath.AABBAABB(aabb1, aabb2))
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

        public void TryEntityCollision(Component characterRootComponent)
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

        // CAMERA 

        private List<BoundBase> GetBoundingBoxesForCameraCollisionTest(ref FSphere cameraCollisionSphere, Component characterRootComponent)
        {
            List<BoundBase> resultCollidedRootBounds = new List<BoundBase>();

            foreach (var unit in CollisionUnits)
            {
                if (unit.RootComponent == characterRootComponent)
                    continue;

                FSphere aabbCollisionSphere = (FSphere)(unit.GetAndTryUpdateFramingBoundingBox());
                if (GeometryMath.IsSphereVsSphereIntersection(ref cameraCollisionSphere, ref aabbCollisionSphere))
                {
                    resultCollidedRootBounds.AddRange(unit.GetBoundingBoxes());
                }
            }

            return resultCollidedRootBounds;
        }

        private void CheckRegularBoundingBoxAndCameraSphereCollision(ref bool bSphereAndFrameBoundingBoxCollision,
            ref List<BoundBase> collidedRootBounds, ref FSphere cameraCollisionSphere)
        {
            foreach (var testingBound in collidedRootBounds)
            {
                FSphere obbCollisionSphere = (FSphere)testingBound;

                if (GeometryMath.IsSphereVsSphereIntersection(ref cameraCollisionSphere, ref obbCollisionSphere))
                {
                    bSphereAndFrameBoundingBoxCollision = true;
                    break;
                }
            }
        }

        private bool IsCameraCollisionWithBoundingBoxes(ref FSphere cameraCollisionSphere, List<BoundBase> collidedRootBounds)
        {
            bool bSphereAndRegularBoundingBoxCollision = false;
           
            CheckRegularBoundingBoxAndCameraSphereCollision(ref bSphereAndRegularBoundingBoxCollision, ref collidedRootBounds, ref cameraCollisionSphere);

            return bSphereAndRegularBoundingBoxCollision;
        }
       
        public void TryCameraCollision(BaseCamera camera)
        {
            ThirdPersonCamera thirdPersonCamera = camera as ThirdPersonCamera;
            if (thirdPersonCamera == null)
                return;

            // Start moving from camera target to it's seek position
            float distanceFromTargetToCamera = thirdPersonCamera.SeekDistanceFromTargetToCamera;
            Vector3 cameraForwardVector = thirdPersonCamera.GetEyeSpaceForwardVector();
            Vector3 startPosition = thirdPersonCamera.GetTargetVector();

            float safeInterval = thirdPersonCamera.CameraCollisionSphereRadius;
           
            for (float interval = safeInterval; interval <= distanceFromTargetToCamera; interval += (thirdPersonCamera.CameraCollisionSphereRadius / 10.0f))
            {
                var intermediatePosition = startPosition - cameraForwardVector * interval;

                FSphere cameraCollisionSphere = new FSphere(intermediatePosition, thirdPersonCamera.CameraCollisionSphereRadius);
                var boundingBoxes = GetBoundingBoxesForCameraCollisionTest(ref cameraCollisionSphere, thirdPersonCamera.GetThirdPersonTarget().GetRootComponent());
                if (boundingBoxes.Count > 0)
                {
                    //if (IsCameraCollisionWithBoundingBoxes(ref cameraCollisionSphere, boundingBoxes))
                    //{
                    //    break;
                    //}
                }

                safeInterval = interval;
            }

            thirdPersonCamera.SetDistanceFromTargetToCamera(safeInterval);
        }
    }
}
