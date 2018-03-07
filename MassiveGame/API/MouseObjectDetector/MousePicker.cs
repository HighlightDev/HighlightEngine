﻿using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MassiveGame
{
    public class MousePicker
    {
        #region Difinitions

        public Vector3 currentRay { private set; get; }

        private Matrix4 pojectionMatrix;
        private Matrix4 viewMatrix;
        private Camera camera;

        #endregion

        #region PickerUse

        public MousePicker(Matrix4 projectionMatrix, Camera camera)
        {
            this.camera = camera;
            this.pojectionMatrix = projectionMatrix;
            this.viewMatrix = camera.getViewMatrix();
        }

        public void update()
        {
            this.viewMatrix = camera.getViewMatrix();
            this.currentRay = calculateMouseRay();
        }

        #endregion

        #region Intermidiate_calculations

        private Vector3 calculateMouseRay()
        {
            float mouseX = OpenTK.Input.Mouse.GetCursorState().X;
            float mouseY = OpenTK.Input.Mouse.GetCursorState().Y;
            Vector2 normalizedCoords = getNormalizedDeviceCoords(mouseX, mouseY);
            Vector4 clipCoords = new Vector4(normalizedCoords.X, normalizedCoords.Y, -1f, 1f);
            Vector4 eyeCoords = toEyeCoords(clipCoords);
            Vector3 worldRay = toWorldCoords(eyeCoords);
            return worldRay;
        }

        private Vector3 toWorldCoords(Vector4 eyeCoords)
        {
            Matrix4 inverseViewMatrix;
            try
            {
                inverseViewMatrix = this.viewMatrix.Inverted();
            }
            catch (InvalidOperationException)
            {
                return new Vector3();
            }
            Vector4 worldCoords = Vector4.Transform(eyeCoords, inverseViewMatrix);
            Vector3 mouseRay = new Vector3(worldCoords.X, worldCoords.Y, worldCoords.Z);
            mouseRay.Normalize();
            return mouseRay;
        }

        private Vector4 toEyeCoords(Vector4 clipCoords)
        {
            Matrix4 inverseProjMatrix = this.pojectionMatrix.Inverted();
            Vector4 eyeCoords = Vector4.Transform(clipCoords, inverseProjMatrix);
            return new Vector4(eyeCoords.X, eyeCoords.Y, -1f, 0f);
        }

        private Vector2 getNormalizedDeviceCoords(float mouseX,float mouseY)
        {
            float x = (2f * mouseX) / DisplayDevice.Default.Width - 1;
            float y = (2f * mouseY) / DisplayDevice.Default.Height - 1;
            return new Vector2(x, -y);
        }

        #endregion
    }
}
