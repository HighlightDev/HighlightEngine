using MassiveGame.Core.PhysicsCore;
using OpenTK;
using PhysicsBox.MathTypes;
using System;
using System.Drawing;
using System.Windows.Forms;
using VectorMath;

namespace MassiveGame.Core.GameCore
{
    public abstract class BaseCamera
    {
        // some unnecessary staff
        public bool SwitchCamera { set; get; }

        protected Vector3 localSpaceRightVector;
        protected Vector3 localSpaceUpVector;
        protected Vector3 localSpaceForwardVector;
        protected Vector3 eyeSpaceRightVector;
        protected Vector3 eyeSpaceForwardVector;
        protected Matrix3 rotationMatrix;
        protected CollisionHeadUnit collisionHeadUnit = null;

        protected bool bTransformationDirty = false;

        public float CameraCollisionSphereRadius { set; get; } = 8.0f;
        private float rotateSensetivity = 0.08f;

        public BaseCamera()
        {
            localSpaceForwardVector = eyeSpaceForwardVector = new Vector3(0, 0, 1);
            localSpaceUpVector = new Vector3(0, 1, 0);
            localSpaceRightVector = eyeSpaceRightVector = new Vector3(1, 0, 0);
            rotationMatrix = Matrix3.Identity;
            SwitchCamera = false;
        }

        public abstract void CameraTick(float DeltaTime);

        public void SetCollisionHeadUnit(CollisionHeadUnit collisionHeadUnit)
        {
            this.collisionHeadUnit = collisionHeadUnit;
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

        public abstract Vector3 GetEyeVector();
        public abstract Vector3 GetTargetVector();
        public abstract Vector3 GetLocalSpaceUpVector();

        public Vector3 GetLocalSpaceRightVector()
        {
            return localSpaceRightVector;
        }

        public Vector3 GetLocalSpaceForwardVector()
        {
            return localSpaceForwardVector;
        }

        public Vector3 GetEyeSpaceForwardVector()
        {
            return eyeSpaceForwardVector;
        }

        public Vector3 GetEyeSpaceRightVector()
        {
            return eyeSpaceRightVector;
        }

        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(GetEyeVector(), GetTargetVector(), GetLocalSpaceUpVector());
        }

        public Matrix3 GetRotationMatrix()
        {
            return rotationMatrix;
        }

        public FSphere GetCameraCollisionSphere()
        {
            return new FSphere(GetEyeVector(), CameraCollisionSphereRadius);
        }

        public void Rotate(Int32 x, Int32 y, Point screenRezolution)
        {
            Int32 middleX = screenRezolution.X >> 1;
            Int32 middleY = screenRezolution.Y >> 1;

            Int32 captionHeight = ((EngineStatics.WINDOW_BORDER != WindowBorder.Hidden) && (EngineStatics.WINDOW_STATE != WindowState.Fullscreen)) ?
                SystemInformation.CaptionHeight : 0;

            Cursor.Position = new Point(EngineStatics.SCREEN_POSITION_X + middleX,
                EngineStatics.SCREEN_POSITION_Y + middleY + captionHeight);

            Int32 deltaX = middleX - x;
            Int32 deltaY = middleY - y;

            UpdateRotationMatrix(-deltaX, -deltaY);
        }

        private void UpdateRotationMatrix(Int32 deltaX, Int32 deltaY)
        {
            eyeSpaceForwardVector = VectorMathOperations.multMatrix(rotationMatrix, localSpaceForwardVector).Normalized();
            eyeSpaceRightVector = Vector3.Cross(eyeSpaceForwardVector, localSpaceUpVector).Normalized();

            float anglePitch = deltaY * rotateSensetivity;
            float angleYaw = deltaX * rotateSensetivity;

            Matrix3 rotatePitch = Matrix3.CreateFromAxisAngle(eyeSpaceRightVector, MathHelper.DegreesToRadians(anglePitch));
            Matrix3 rotateYaw = Matrix3.CreateRotationY(MathHelper.DegreesToRadians(angleYaw));

            Matrix3 rotMat = Matrix3.Identity;
            rotMat *= rotateYaw;
            rotMat *= rotatePitch;

            rotationMatrix = rotMat * rotationMatrix;

            bTransformationDirty = true;

            //Console.Clear();
            //pitch = (float)Math.Atan2(-rotationMatrix[2, 0], Math.Sqrt(rotationMatrix[2, 1] * rotationMatrix[2, 1] + rotationMatrix[2, 2] * rotationMatrix[2, 2]));
            //Console.WriteLine(pitch);
        }

        public Vector3 LerpPosition(float t, float t1, float t2, ref Vector3 position1, ref Vector3 position2)
        {
            Vector3 resultPosition = Vector3.Zero;

            float x_delta = t2 - t1;
            float x_zero_offset = t - t1;

            resultPosition.X = ((position2.X - position1.X) / x_delta) * x_zero_offset + position1.X;
            resultPosition.Y = ((position2.Y - position1.Y) / x_delta) * x_zero_offset + position1.Y;
            resultPosition.Z = ((position2.Z - position1.Z) / x_delta) * x_zero_offset + position1.Z;

            return resultPosition;
        }
    }
}
