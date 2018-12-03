using OpenTK;
using System;
using System.Runtime.Serialization;

namespace MassiveGame.Core.GameCore
{
    public enum CameraDirections
    {
        LEFT,
        RIGHT,
        FORWARD,
        BACK,
        STAY
    }
   
    [Serializable]
    public class FirstPersonCamera : BaseCamera
    {
        private float m_cameraMoveSpeed;
        private Vector3 m_firstPersonCameraPosition;

        public FirstPersonCamera(Vector3 localSpaceForwardVector, Vector3 CameraPosition)
        : base()
        {
            this.m_localSpaceForwardVector = localSpaceForwardVector.Normalized();
            m_firstPersonCameraPosition = CameraPosition;
            m_cameraMoveSpeed = 1.5f;
        }

        public FirstPersonCamera()
            : base()
        {
            m_firstPersonCameraPosition = Vector3.Zero;
        }

        #region Serialization

        protected FirstPersonCamera(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            m_cameraMoveSpeed = info.GetSingle("m_cameraMoveSpeed");
            m_firstPersonCameraPosition = (Vector3)info.GetValue("m_firstPersonCameraPosition", typeof(Vector3));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("m_cameraMoveSpeed", m_cameraMoveSpeed);
            info.AddValue("m_firstPersonCameraPosition", m_firstPersonCameraPosition, typeof(Vector3));
        }

        #endregion

        public override void CameraTick(float DeltaTime)
        {
            // do some actions here

        }

        public float GetCameraMoveSpeed()
        {
            return m_cameraMoveSpeed;
        }

        public void SetCameraMoveSpeed(float speed)
        {
            m_cameraMoveSpeed = speed;
        }

        public override Vector3 GetEyeVector()
        {
            return m_firstPersonCameraPosition;
        }

        public override Vector3 GetTargetVector()
        {
            var result = m_firstPersonCameraPosition + m_eyeSpaceForwardVector * 10;
            return result;
        }

        public override Vector3 GetLocalSpaceUpVector()
        {
            return m_localSpaceUpVector;
        }

        public void moveCamera(CameraDirections direction)
        {
            switch (direction)
            {
                case CameraDirections.FORWARD: m_firstPersonCameraPosition += GetEyeSpaceForwardVector() * m_cameraMoveSpeed; break;
                case CameraDirections.BACK: m_firstPersonCameraPosition -= GetEyeSpaceForwardVector() * m_cameraMoveSpeed; break;
                case CameraDirections.LEFT: m_firstPersonCameraPosition -= GetEyeSpaceRightVector() * m_cameraMoveSpeed; break;
                case CameraDirections.RIGHT: m_firstPersonCameraPosition += GetEyeSpaceRightVector() * m_cameraMoveSpeed; break;
            }
        }
    }
}
