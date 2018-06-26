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

namespace MassiveGame
{
    public class LiteCameraExt
    {
        float ROTATE_MEASURE = 0.08f;

        public Vector3 LocalSpaceCamDir { set; get; }
        public float DistanceToTarget { set; get; }
        public Entity Target { set; get; }
        public Matrix3 AccumulatedRotation { set; get; }

        public LiteCameraExt(Vector3 CamDir, float distanceToTarget, Entity target)
        {
            this.LocalSpaceCamDir = CamDir;
            DistanceToTarget = distanceToTarget;
            Target = target;
            AccumulatedRotation = Matrix3.Identity;
        }


        public void RotateDirByMouse(Int32 x, Int32 y, Int32 screenWidth, Int32 screenHeight)
        {
            Int32 middleX = screenWidth >> 1;
            Int32 middleY = screenHeight >> 1; 


            Int32 captionHeight = ((DOUEngine.WINDOW_BORDER != WindowBorder.Hidden) && (DOUEngine.WINDOW_STATE != WindowState.Fullscreen)) ?
                SystemInformation.CaptionHeight : 0;

            Cursor.Position = new Point(DOUEngine.SCREEN_POSITION_X + middleX,
                DOUEngine.SCREEN_POSITION_Y + middleY + captionHeight);

            Int32 deltaX = middleX - x;
            Int32 deltaY = middleY - y;


            RotateDirection(-deltaX, -deltaY);
        }

        private void RotateDirection(Int32 deltaX, Int32 deltaY)
        {
            float anglePitch = -deltaY * ROTATE_MEASURE;
            float angleYaw = -deltaX * ROTATE_MEASURE;

            Vector3 rightVector = Vector3.Cross(LocalSpaceCamDir, new Vector3(0, 1, 0)).Normalized();

            Matrix3 rotatePitch = Matrix3.CreateFromAxisAngle(rightVector, MathHelper.DegreesToRadians(anglePitch));
            Matrix3 rotateYaw = Matrix3.CreateRotationY(MathHelper.DegreesToRadians(angleYaw));
           
            Matrix3 rotationMatrix = Matrix3.Identity;
            rotationMatrix *= rotateYaw;
            rotationMatrix *= rotatePitch;

            AccumulatedRotation *= rotationMatrix;
        }

        public Matrix4 GetViewMatrix()
        {
            Vector3 WorldSpaceCameraDirection = VectorMath.multMatrix(AccumulatedRotation, LocalSpaceCamDir);

            Vector3 target = (Target as Component).ComponentTranslation;
            Vector3 eye = target + (-WorldSpaceCameraDirection * DistanceToTarget);
            Vector3 up = new Vector3(0, 1, 0);

            return Matrix4.LookAt(eye, target, up);
        }

    }

    public class LiteCamera
    {
        #region Definitions

        public Vector3 PreviousPosition { set; get; }

        public Vector3 PosVector { set; get; }
        public Vector3 LookVector { set; get; }

        protected Vector3 posVector;
        protected Vector3 lookVector;
        protected Vector3 upVector;

        public Matrix4 ModifierMatrix;

        #endregion

        #region Getters

        public MovableEntity Target { set; get; }

        public Vector3 GetNormalizedDirection()
        {
            return (LookVector - PosVector).Normalized();
        }

        public Vector3 GetDirection()
        {
            return LookVector - PosVector;
        }

        public Vector3 getPositionVector()
        {
            return PosVector;
        }
        public Vector3 getLookVector()
        {
            return LookVector;
        }
        public Vector3 getUpVector()
        {
            return upVector;
        }
        public Matrix4 getViewMatrix()
        {
            return (ModifierMatrix * Matrix4.LookAt(PosVector, LookVector, upVector));
        }

        #endregion

        #region Setters

        public void setPosition(Vector3 position)
        {
            PosVector = position;
        }

        public void SetModifier(Matrix4 modifierMatrix)
        {
            ModifierMatrix = modifierMatrix;
        }

        #endregion

        #region Transformations

        /*TODO : Inverts camera pitch*/
        public void invertPitch()
        {
            var direction = lookVector - PosVector;
            float length = direction.Length;

            float pitch = Convert.ToSingle(Math.Asin(direction.Y / length));
            float yaw;

            if (Math.Abs(direction.Z) < 0.00001f)
            {
                // Special case
                if (direction.X > 0)
                {
                    yaw = Convert.ToSingle(Math.PI / 2.0f);
                }
                else if (direction.X < 0)
                {
                    yaw = Convert.ToSingle(-Math.PI / 2.0f);
                }
                else
                {
                    yaw = 0.0f;
                }
            }
            else
            {
                yaw = Convert.ToSingle(Math.Atan2(direction.X, direction.Z));
            }
            yaw = -yaw; //Invert yaw 
            Vector3 dest = new Vector3();
            dest.X = Convert.ToSingle(-Math.Cos(pitch) * Math.Sin(yaw));
            dest.Y = Convert.ToSingle(-Math.Sin(pitch));
            dest.Z = Convert.ToSingle(Math.Cos(pitch) * Math.Cos(yaw));
            dest.Normalize();
            Vector3 vec = PosVector;
            vec += dest * 100;

            this.LookVector = new Vector3(vec);
        }

        #endregion

        #region Constructor

        public LiteCamera() {
            ModifierMatrix = Matrix4.Identity;
            PreviousPosition = new Vector3(0);
            upVector = new Vector3(0, 1, 0);
            PosVector = new Vector3();
            lookVector = new Vector3();
        }

        public LiteCamera(float eyeX, float eyeY, float eyeZ,
            float centerX, float centerY, float centerZ,
            float upX, float upY, float upZ) : this()
        {
            PreviousPosition = new Vector3(0);
            upVector = new Vector3(upX, upY, upZ);
            PosVector = new Vector3(eyeX, eyeY, eyeZ);
            lookVector = new Vector3(centerX, centerY, centerZ);
        }

        public LiteCamera(LiteCamera camera) : this()
        {
            LookVector = camera.getLookVector();
            PosVector = camera.getPositionVector();
            this.upVector = camera.getUpVector();
        }

        #endregion
    }
}
