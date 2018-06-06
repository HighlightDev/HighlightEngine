using OpenTK;
using PhysicsBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassiveGame
{
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
