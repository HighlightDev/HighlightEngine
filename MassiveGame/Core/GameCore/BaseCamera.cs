using MassiveGame.Core.MathCore.MathTypes;
using MassiveGame.Core.PhysicsCore;
using OpenTK;
using System;
using System.Drawing;
using System.Runtime.Serialization;
using System.Windows.Forms;
using VectorMath;

namespace MassiveGame.Core.GameCore
{
    [Serializable]
    public abstract class BaseCamera : ISerializable
    {
        // some unnecessary staff
        public bool SwitchCamera { set; get; }

        protected Vector3 m_localSpaceRightVector;
        protected Vector3 m_localSpaceUpVector;
        protected Vector3 m_localSpaceForwardVector;
        protected Vector3 m_eyeSpaceRightVector;
        protected Vector3 m_eyeSpaceForwardVector;
        protected Matrix3 m_rotationMatrix;

        [NonSerialized]
        protected CollisionHeadUnit m_collisionHeadUnit = null;

        protected bool bTransformationDirty = false;

        public float CameraCollisionSphereRadius { set; get; } = 8.0f;
        private float rotateSensetivity = 0.08f;

        public BaseCamera()
        {
            m_localSpaceForwardVector = m_eyeSpaceForwardVector = new Vector3(0, 0, 1);
            m_localSpaceUpVector = new Vector3(0, 1, 0);
            m_localSpaceRightVector = m_eyeSpaceRightVector = new Vector3(1, 0, 0);
            m_rotationMatrix = Matrix3.Identity;
            SwitchCamera = false;
        }

        #region Serialization

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            SwitchCamera = false;
            bTransformationDirty = false;
            info.AddValue("m_localSpaceRightVector", m_localSpaceRightVector, typeof(Vector3));
            info.AddValue("m_localSpaceUpVector", m_localSpaceUpVector, typeof(Vector3));
            info.AddValue("m_localSpaceForwardVector", m_localSpaceForwardVector, typeof(Vector3));
            info.AddValue("m_eyeSpaceRightVector", m_eyeSpaceRightVector, typeof(Vector3));
            info.AddValue("m_eyeSpaceForwardVector", m_eyeSpaceForwardVector, typeof(Vector3));
            info.AddValue("m_rotationMatrix", m_rotationMatrix, typeof(Matrix3));
            info.AddValue("CameraCollisionSphereRadius", CameraCollisionSphereRadius);
            info.AddValue("rotateSensetivity", rotateSensetivity);
        }

        protected BaseCamera(SerializationInfo info, StreamingContext context)
        {
            m_localSpaceRightVector = (Vector3)info.GetValue("m_localSpaceRightVector", typeof(Vector3));
            m_localSpaceUpVector = (Vector3)info.GetValue("m_localSpaceUpVector", typeof(Vector3));
            m_localSpaceForwardVector = (Vector3)info.GetValue("m_localSpaceForwardVector", typeof(Vector3));
            m_eyeSpaceRightVector = (Vector3)info.GetValue("m_eyeSpaceRightVector", typeof(Vector3));
            m_eyeSpaceForwardVector = (Vector3)info.GetValue("m_eyeSpaceForwardVector", typeof(Vector3));
            m_rotationMatrix = (Matrix3)info.GetValue("m_rotationMatrix", typeof(Matrix3));
            CameraCollisionSphereRadius = info.GetSingle("CameraCollisionSphereRadius");
            rotateSensetivity = info.GetSingle("rotateSensetivity");
        }

        #endregion
       
        public abstract void CameraTick(float DeltaTime);

        public void SetCollisionHeadUnit(CollisionHeadUnit collisionHeadUnit)
        {
            this.m_collisionHeadUnit = collisionHeadUnit;
        }

        public void SetLocalSpaceUpVector(Vector3 upVector)
        {
            m_localSpaceUpVector = upVector;
        }

        public void SetLocalSpaceForwardVector(Vector3 forwardVector)
        {
            m_localSpaceForwardVector = forwardVector;
        }

        public void SetLocalSpaceRightVector(Vector3 rightVector)
        {
            m_localSpaceRightVector = rightVector;
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
            return m_localSpaceRightVector;
        }

        public Vector3 GetLocalSpaceForwardVector()
        {
            return m_localSpaceForwardVector;
        }

        public Vector3 GetEyeSpaceForwardVector()
        {
            return m_eyeSpaceForwardVector;
        }

        public Vector3 GetEyeSpaceRightVector()
        {
            return m_eyeSpaceRightVector;
        }

        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(GetEyeVector(), GetTargetVector(), GetLocalSpaceUpVector());
        }

        public Matrix3 GetRotationMatrix()
        {
            return m_rotationMatrix;
        }

        public FSphere GetCameraCollisionSphere()
        {
            return new FSphere(GetEyeVector(), CameraCollisionSphereRadius);
        }

        private void Rotate(Int32 x, Int32 y, ref Point screenRezolution)
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

        public void RotateFacade(Int32 x, Int32 y, Point screenRezolution)
        {
            Rotate(x, y, ref screenRezolution);
        }

        public void RotateFacade(Point mousePosition, Point screenRezolution)
        {
            Rotate(mousePosition.X, mousePosition.Y, ref screenRezolution);
        }

        private void UpdateRotationMatrix(Int32 deltaX, Int32 deltaY)
        {
            m_eyeSpaceForwardVector = VectorMathOperations.multMatrix(m_rotationMatrix, m_localSpaceForwardVector).Normalized();
            m_eyeSpaceRightVector = Vector3.Cross(m_eyeSpaceForwardVector, m_localSpaceUpVector).Normalized();

            float anglePitch = deltaY * rotateSensetivity;
            float angleYaw = deltaX * rotateSensetivity;

            Matrix3 rotatePitch = Matrix3.CreateFromAxisAngle(m_eyeSpaceRightVector, MathHelper.DegreesToRadians(anglePitch));
            Matrix3 rotateYaw = Matrix3.CreateRotationY(MathHelper.DegreesToRadians(angleYaw));

            Matrix3 tempRotationMatrix = Matrix3.Identity;
            tempRotationMatrix *= rotateYaw;
            tempRotationMatrix *= rotatePitch;

            m_rotationMatrix = tempRotationMatrix * m_rotationMatrix;

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
