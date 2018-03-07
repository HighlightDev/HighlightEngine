using OpenTK;
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

        protected Vector3 posVector;
        protected Vector3 lookVector;
        protected Vector3 upVector;

        public Matrix4 ModifierMatrix;

        #endregion

        #region Getters

        public Vector3 GetDirection()
        {
            return Vector3.Normalize(lookVector - posVector);
        }

        public Vector3 getPosition()
        {
            return new Vector3(posVector.X, posVector.Y, posVector.Z);
        }
        public Vector3 getLook()
        {
            return lookVector;
        }
        public Vector3 getUp()
        {
            return upVector;
        }
        public Matrix4 getViewMatrix()
        {
            return (ModifierMatrix * Matrix4.LookAt(posVector, lookVector, upVector));
        }

        #endregion

        #region Setters

        public void setPosition(Vector3 position)
        {
            this.posVector = position;
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
            var direction = lookVector - posVector;
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
            Vector3 vec = this.posVector;
            vec += dest * 100;

            this.lookVector = new Vector3(vec);
        }

        #endregion

        #region Constructor

        public LiteCamera() {
            ModifierMatrix = Matrix4.Identity;
        }

        public LiteCamera(float eyeX, float eyeY, float eyeZ,
            float centerX, float centerY, float centerZ,
            float upX, float upY, float upZ) : this()
        {
            posVector = new Vector3();
            lookVector = new Vector3();
            upVector = new Vector3();
            posVector.X = eyeX;
            posVector.Y = eyeY;
            posVector.Z = eyeZ;
            lookVector.X = centerX;
            lookVector.Y = centerY;
            lookVector.Z = centerZ;
            upVector.X = upX;
            upVector.Y = upY;
            upVector.Z = upZ;
        }

        public LiteCamera(LiteCamera camera) : this()
        {
            this.lookVector = camera.getLook();
            this.posVector = camera.getPosition();
            this.upVector = camera.getUp();
        }

        #endregion
    }
}
