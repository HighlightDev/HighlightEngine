﻿using OpenTK;
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
    public abstract class BaseCamera
    {
        // some unnecessary staff
        public bool SwitchCamera { set; get; }

        protected float rotateSensetivity = 0.08f;

        protected Vector3 localSpaceRightVector;
        protected Vector3 localSpaceUpVector;
        protected Vector3 localSpaceForwardVector;

        protected Vector3 eyeSpaceRightVector;
        protected Vector3 eyeSpaceForwardVector;

        protected Matrix3 rotationMatrix;

        public BaseCamera()
        {
            localSpaceForwardVector = eyeSpaceForwardVector = new Vector3(0, 0, 1);
            localSpaceUpVector = new Vector3(0, 1, 0);
            localSpaceRightVector = eyeSpaceRightVector = new Vector3(1, 0, 0);
            rotationMatrix = Matrix3.Identity;
            SwitchCamera = false;
        }

        public abstract void CameraTick(float DeltaTime);

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

        public Vector3 LerpPosition(float t, float t1, float t2, Vector3 position1, Vector3 position2)
        {
            Vector3 resultPosition = Vector3.Zero;

            float x_delta = t2 - t1;
            float x_zero_offset = t2 - t;

            resultPosition.X = ((position2.X - position1.X) / x_delta) * x_zero_offset + position1.X;
            resultPosition.Y = ((position2.Y - position1.Y) / x_delta) * x_zero_offset + position1.Y;
            resultPosition.Z = ((position2.Z - position1.Z) / x_delta) * x_zero_offset + position1.Z;

            return resultPosition;
        }
    }
}
