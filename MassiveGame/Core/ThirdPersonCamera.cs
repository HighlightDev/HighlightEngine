using OpenTK;
using PhysicsBox;
using PhysicsBox.ComponentCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VMath;

namespace MassiveGame.Core
{
    public class ThirdPersonCamera : BaseCamera
    {
        private float cameraDistanceToTarget;
        private MovableEntity thirdPersonTarget;
        private bool bThirdPersonTargetTransformationDirty = false;
        private Vector3 actualTargetVector;
        private float lerpTimeElapsed = 0.0f;
        private float timeForInterpolation = 0.1f;

        public ThirdPersonCamera() : base()
        {
            cameraDistanceToTarget = 20.0f;
            thirdPersonTarget = null;
        }

        public ThirdPersonCamera(Vector3 localSpaceForwardVector, float camDistanceToThirdPersonTarget)
            : this()
        {
            this.localSpaceForwardVector = localSpaceForwardVector;
            cameraDistanceToTarget = camDistanceToThirdPersonTarget;
        }

        public override void CameraTick(float DeltaTime)
        {
            if (bThirdPersonTargetTransformationDirty)
            {
                Vector3 finalTargetVector = thirdPersonTarget.ComponentTranslation;
                lerpTimeElapsed = Math.Min(lerpTimeElapsed + DeltaTime, timeForInterpolation);

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

            return GetTargetVector() - (GetEyeSpaceForwardVector() * cameraDistanceToTarget);
        }

        public override Vector3 GetTargetVector()
        {
            if (thirdPersonTarget == null)
                return Vector3.Zero;

            return actualTargetVector;
        }

        public void SetCameraDistanceToTarget(float cameraDistanceToTarget)
        {
            this.cameraDistanceToTarget = cameraDistanceToTarget;
        }

        public float GetCameraDistanceToTarget()
        {
            return cameraDistanceToTarget;
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
