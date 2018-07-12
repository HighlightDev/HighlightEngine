using MassiveGame.Core.GameCore.Entities.MoveEntities;
using OpenTK;
using PhysicsBox;
using System;

namespace MassiveGame.Core.GameCore
{
    public class ThirdPersonCamera : BaseCamera
    {
        private float distanceFromTargetToCamera;
        private MovableEntity thirdPersonTarget;
        private bool bThirdPersonTargetTransformationDirty = false;
        private Vector3 actualTargetVector;
        private float lerpTimeElapsed = 0.0f;
        private float timeForInterpolation = 0.15f;

        public readonly float SeekDistanceFromTargetToCamera;

        public ThirdPersonCamera() : base()
        {
            distanceFromTargetToCamera = SeekDistanceFromTargetToCamera = 20.0f;
            thirdPersonTarget = null;
        }

        public ThirdPersonCamera(Vector3 localSpaceForwardVector, float camDistanceToThirdPersonTarget)
            : this()
        {
            this.localSpaceForwardVector = this.eyeSpaceForwardVector = localSpaceForwardVector.Normalized();
            distanceFromTargetToCamera = SeekDistanceFromTargetToCamera = camDistanceToThirdPersonTarget;
        }

        public override void CameraTick(float DeltaTime)
        {
            if (bTransformationDirty)
            {
                if (collisionHeadUnit != null)
                {
                    collisionHeadUnit.TryCameraCollision(this);
                }
                bTransformationDirty = false;
            }

            if (bThirdPersonTargetTransformationDirty)
            {
                lerpTimeElapsed = Math.Min(lerpTimeElapsed + DeltaTime, timeForInterpolation);

                Vector3 finalTargetVector = thirdPersonTarget.ComponentTranslation;
                actualTargetVector = LerpPosition(lerpTimeElapsed, 0.0f, timeForInterpolation, ref actualTargetVector, ref finalTargetVector);

                // If camera is at final position  
                if (GeometricMath.CMP(lerpTimeElapsed, timeForInterpolation) > 0)
                {
                    lerpTimeElapsed = 0.0f;
                    bThirdPersonTargetTransformationDirty = false;
                }
            }
        }

        public override Vector3 GetLocalSpaceUpVector()
        {
            return localSpaceUpVector;
        }

        public override Vector3 GetEyeVector()
        {
            if (thirdPersonTarget == null)
                return Vector3.Zero;

            return GetTargetVector() - (GetEyeSpaceForwardVector() * distanceFromTargetToCamera);
        }

        public override Vector3 GetTargetVector()
        {
            if (thirdPersonTarget == null)
                return Vector3.Zero;

            return actualTargetVector;
        }

        public void SetDistanceFromTargetToCamera(float distanceFromTargetToCamera)
        {
            this.distanceFromTargetToCamera = distanceFromTargetToCamera;
        }

        public float GetDistanceFromTargetToCamera()
        {
            return distanceFromTargetToCamera;
        }

        public MovableEntity GetThirdPersonTarget()
        {
            return thirdPersonTarget;
        }

        public void SetThirdPersonTarget(MovableEntity thirdPersonTarget)
        {
            this.thirdPersonTarget = thirdPersonTarget;
            actualTargetVector = thirdPersonTarget.ComponentTranslation;
            thirdPersonTarget.TransformationDirtyEvent += new EventHandler(ThirdPersonTargetTransformationDirty);
        }

        private void ThirdPersonTargetTransformationDirty(object sender, EventArgs e)
        {
            lerpTimeElapsed = 0.0f;
            bThirdPersonTargetTransformationDirty = true;
        }
    }
}
