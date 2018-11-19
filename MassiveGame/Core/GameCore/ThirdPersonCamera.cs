using MassiveGame.Core.GameCore.Entities.MoveEntities;
using MassiveGame.Core.MathCore;
using OpenTK;
using System;
using System.Runtime.Serialization;

namespace MassiveGame.Core.GameCore
{
    [Serializable]
    public class ThirdPersonCamera : BaseCamera
    {
        private float m_distanceFromTargetToCamera;
        private MovableEntity m_thirdPersonTarget;
        private Vector3 m_actualTargetVector;
        private float m_lerpTimeElapsed = 0.0f;
        private float m_timeForInterpolation = 0.15f;

        private bool m_bThirdPersonTargetTransformationDirty = false;

        public float MaxDistanceFromTargetToCamera { set; get; }

        public ThirdPersonCamera() : base()
        {
            m_distanceFromTargetToCamera = MaxDistanceFromTargetToCamera = 20.0f;
            m_thirdPersonTarget = null;
        }

        public ThirdPersonCamera(Vector3 localSpaceForwardVector, float camDistanceToThirdPersonTarget)
            : this()
        {
            this.m_localSpaceForwardVector = m_eyeSpaceForwardVector = localSpaceForwardVector.Normalized();
            m_distanceFromTargetToCamera = MaxDistanceFromTargetToCamera = camDistanceToThirdPersonTarget;
        }

        #region Serialization

        protected ThirdPersonCamera(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {

        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

        }

        #endregion

        public override void CameraTick(float DeltaTime)
        {
            if (bTransformationDirty)
            {
                if (m_collisionHeadUnit != null)
                {
                    m_collisionHeadUnit.TryCameraCollision(this);
                }
                bTransformationDirty = false;
            }

            if (m_bThirdPersonTargetTransformationDirty)
            {
                m_lerpTimeElapsed = Math.Min(m_lerpTimeElapsed + DeltaTime, m_timeForInterpolation);

                Vector3 finalTargetVector = m_thirdPersonTarget.ComponentTranslation;
                m_actualTargetVector = LerpPosition(m_lerpTimeElapsed, 0.0f, m_timeForInterpolation, ref m_actualTargetVector, ref finalTargetVector);

                // If camera is at final position  
                if (GeometryMath.CMP(m_lerpTimeElapsed, m_timeForInterpolation) > 0)
                {
                    m_lerpTimeElapsed = 0.0f;
                    m_bThirdPersonTargetTransformationDirty = false;
                }
            }
        }

        public override Vector3 GetLocalSpaceUpVector()
        {
            return m_localSpaceUpVector;
        }

        public override Vector3 GetEyeVector()
        {
            if (m_thirdPersonTarget == null)
                return Vector3.Zero;

            return GetTargetVector() - (GetEyeSpaceForwardVector() * m_distanceFromTargetToCamera);
        }

        public override Vector3 GetTargetVector()
        {
            if (m_thirdPersonTarget == null)
                return Vector3.Zero;

            return m_actualTargetVector;
        }

        public void SetDistanceFromTargetToCamera(float distanceFromTargetToCamera)
        {
            this.m_distanceFromTargetToCamera = distanceFromTargetToCamera;
        }

        public float GetDistanceFromTargetToCamera()
        {
            return m_distanceFromTargetToCamera;
        }

        public MovableEntity GetThirdPersonTarget()
        {
            return m_thirdPersonTarget;
        }

        public void SetThirdPersonTarget(MovableEntity thirdPersonTarget)
        {
            this.m_thirdPersonTarget = thirdPersonTarget;
            m_actualTargetVector = thirdPersonTarget.ComponentTranslation;
        }

        public void SetThirdPersonTargetTransformationDirty()
        {
            m_lerpTimeElapsed = 0.0f;
            m_bThirdPersonTargetTransformationDirty = true;
        }
    }
}
