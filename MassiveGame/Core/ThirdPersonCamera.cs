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
        private float rotateSensetivity = 0.08f;

        private Vector3 localSpaceRightVector;
        private Vector3 localSpaceUpVector;
        private Vector3 localSpaceForwardVector;

        private Vector3 eyeSpaceRightVector;
        private Vector3 eyeSpaceForwardVector;

        private Matrix3 rotationMatrix;

        private float cameraDistanceToTarget;

        private MovableEntity thirdPersonTarget;

        public ThirdPersonCamera()
        {
            localSpaceForwardVector = new Vector3(0, 0, 1);
            localSpaceUpVector = new Vector3(0, 1, 0);
            localSpaceRightVector = new Vector3(1, 0, 0);
            cameraDistanceToTarget = 20.0f;
            rotationMatrix = Matrix3.Identity;
            thirdPersonTarget = null;
        }

        public ThirdPersonCamera(Vector3 localSpaceForwardVector, float camDistanceToThirdPersonTarget)
            : this()
        {
            this.localSpaceForwardVector = localSpaceForwardVector;
            cameraDistanceToTarget = camDistanceToThirdPersonTarget;
        }

        public void SetLocalSpaceUpVector(Vector3 upVector)
        {
            localSpaceUpVector = upVector;
        }

        public void SetLocalSpaceForwardVector(Vector3 forwardVector)
        {
            localSpaceForwardVector = forwardVector;
        }

        public void SetLocalSpaceRightVector(Vector3 rightVector)
        {
            localSpaceRightVector = rightVector;
        }

        public void SetCameraSensetivity(float rotateSensetivity)
        {
            this.rotateSensetivity = rotateSensetivity;
        }

        public Vector3 GetUpVector()
        {
            return localSpaceUpVector;
        }

        public Vector3 GetRightVector()
        {
            return localSpaceRightVector;
        }

        public Vector3 GetForwardVector()
        {
            return localSpaceForwardVector;
        }

        public Vector3 GetEyeSpaceForwardVector()
        {
            return VectorMath.multMatrix(rotationMatrix, localSpaceForwardVector);
        }

        public Vector3 GetEyeSpaceRightVector()
        {
            return eyeSpaceRightVector;
        }

        public Matrix3 GetRotationMatrix()
        {
            return rotationMatrix;
        }

        public Vector3 getEyeVector()
        {
            if (thirdPersonTarget == null)
                return Vector3.Zero;

            return GetTargetVector() - (GetEyeSpaceForwardVector() * cameraDistanceToTarget);
        }

        public Vector3 GetTargetVector()
        {
            if (thirdPersonTarget == null)
                return Vector3.Zero;

            return (thirdPersonTarget as Component).ComponentTranslation;
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
        }

        public void Rotate(Int32 x, Int32 y, Point screenRezolution)
        {
            Int32 middleX = screenRezolution.X >> 1;
            Int32 middleY = screenRezolution.Y >> 1; 

            Int32 captionHeight = ((DOUEngine.WINDOW_BORDER != WindowBorder.Hidden) && (DOUEngine.WINDOW_STATE != WindowState.Fullscreen)) ?
                SystemInformation.CaptionHeight : 0;

            Cursor.Position = new Point(DOUEngine.SCREEN_POSITION_X + middleX,
                DOUEngine.SCREEN_POSITION_Y + middleY + captionHeight);

            Int32 deltaX = middleX - x;
            Int32 deltaY = middleY - y;


            UpdateRotationMatrix(-deltaX, -deltaY);
        }

        private void UpdateRotationMatrix(Int32 deltaX, Int32 deltaY)
        {
            eyeSpaceForwardVector = VectorMath.multMatrix(rotationMatrix, localSpaceForwardVector).Normalized();
            eyeSpaceRightVector = Vector3.Cross(eyeSpaceForwardVector, localSpaceUpVector).Normalized();

            float anglePitch = deltaY * rotateSensetivity;
            float angleYaw = deltaX * rotateSensetivity;
     
            Matrix3 rotatePitch = Matrix3.CreateFromAxisAngle(eyeSpaceRightVector, MathHelper.DegreesToRadians(anglePitch));
            Matrix3 rotateYaw = Matrix3.CreateRotationY(MathHelper.DegreesToRadians(angleYaw));
           
            Matrix3 rotMat = Matrix3.Identity;
            rotMat *= rotateYaw;
            rotMat *= rotatePitch;

            rotationMatrix = rotMat * rotationMatrix;
        }

        public Matrix4 getViewMatrix()
        {
            return Matrix4.LookAt(getEyeVector(), GetTargetVector(), GetUpVector());
        }
    }
}
