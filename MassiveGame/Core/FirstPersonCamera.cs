using OpenTK;

namespace MassiveGame.Core
{
    public enum CameraDirections
    {
        LEFT,
        RIGHT,
        FORWARD,
        BACK,
        STAY
    }
   
    public class FirstPersonCamera : BaseCamera
    {
        private float CameraMoveSpeed = 20.5f;
        private Vector3 FirstPersonCameraPosition;

        public FirstPersonCamera(Vector3 localSpaceForwardVector, Vector3 CameraPosition)
        : base()
        {
            this.localSpaceForwardVector = localSpaceForwardVector;
            FirstPersonCameraPosition = CameraPosition;
        }

        public FirstPersonCamera()
            : base()
        {
            FirstPersonCameraPosition = Vector3.Zero;
        }

        public override Vector3 GetEyeVector()
        {
            return FirstPersonCameraPosition;
        }

        public override Vector3 GetTargetVector()
        {
            return (FirstPersonCameraPosition + eyeSpaceForwardVector * 10);
        }

        public override Vector3 GetLocalSpaceUpVector()
        {
            return localSpaceUpVector;
        }

        public void moveCamera(CameraDirections direction)
        {
            switch (direction)
            {
                case CameraDirections.FORWARD: FirstPersonCameraPosition += GetEyeSpaceForwardVector() * CameraMoveSpeed; break;
                case CameraDirections.BACK: FirstPersonCameraPosition -= GetEyeSpaceForwardVector() * CameraMoveSpeed; break;
                case CameraDirections.LEFT: FirstPersonCameraPosition -= GetEyeSpaceRightVector() * CameraMoveSpeed; break;
                case CameraDirections.RIGHT: FirstPersonCameraPosition += GetEyeSpaceRightVector() * CameraMoveSpeed; break;
            }
        }
    }
}
