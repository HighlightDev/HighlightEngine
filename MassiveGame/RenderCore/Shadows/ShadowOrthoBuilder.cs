using OpenTK;
using PhysicsBox;
using PhysicsBox.MathTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MassiveGame.RenderCore.Shadows
{
    public class ShadowOrthoBuilder
    {
        private static Vector3 RightVector = new Vector3(1, 0, 0);
        private static Vector3 UpVector = new Vector3(0, 1, 0);
        private static Vector3 ForwardVector = new Vector3(0, 0, 1);

        public ShadowOrthoBuilder()
        {

        }

        /*      A
        *      /\
        *     /  \ 
        *    /    \
        *   C------B
        */

        private Vector3 GetLeftRightIntersectionPoint(Vector3 A, FPlane cbPlane, FPlane incidentPlane)
        {
            Vector3 result;
            Vector3 acNormal = new Vector3(incidentPlane.X, incidentPlane.Y, incidentPlane.Z);
            Vector3 acRayDirection = Vector3.Cross(acNormal, UpVector);

            Vector3 cbNormal = new Vector3(cbPlane.X, cbPlane.Y, cbPlane.Z);

            // if ac ray is opposite directed - invert it's direction
            acRayDirection *= Vector3.Dot(cbNormal, acRayDirection) > 0.0 ? 1 : -1;
            FRay acRay = new FRay(A, acRayDirection);

            result = GeometricMath.GetIntersectionRayPlane(cbPlane, acRay);
            return result;
        }

        private Vector3 GetTopBottomIntersectionPoint(Vector3 A, FPlane cbPlane, FPlane incidentPlane)
        {
            Vector3 result;
            Vector3 acNormal = new Vector3(incidentPlane.X, incidentPlane.Y, incidentPlane.Z);
            Vector3 acRayDirection = Vector3.Cross(acNormal, RightVector);

            Vector3 cbNormal = new Vector3(cbPlane.X, cbPlane.Y, cbPlane.Z);

            // if ac ray is opposite directed - invert it's direction
            acRayDirection *= Vector3.Dot(cbNormal, acRayDirection) > 0.0 ? 1 : -1;
            FRay acRay = new FRay(A, acRayDirection);

            result = GeometricMath.GetIntersectionRayPlane(cbPlane, acRay);
            return result;
        }

        private void GetEdgePoints(LiteCamera viewerCamera, ConvexVolume volume, out Vector3 LBN, out Vector3 RTF)
        {
            Vector3 ViewerPosition = viewerCamera.getPosition();

            // Find left and right edges
            Vector3 IntersectionPointC = GetLeftRightIntersectionPoint(ViewerPosition, volume.FarPlane, volume.LeftPlane);
            Vector3 IntersectionPointB = GetLeftRightIntersectionPoint(ViewerPosition, volume.FarPlane, volume.RightPlane);
            // Find top and bottom edges
            Vector3 IntersectionPointBottom = GetTopBottomIntersectionPoint(ViewerPosition, volume.FarPlane, volume.BottomPlane);
            Vector3 IntersectionPointTop = GetTopBottomIntersectionPoint(ViewerPosition, volume.FarPlane, volume.TopPlane);

            float left, right, top, bottom, near, far;
            // left 
            Dictionary<float, Vector3> projectedValues = new Dictionary<float, Vector3>();
            projectedValues.Add(GeometricMath.ProjectVectorOnNormalizedVector(IntersectionPointB, -RightVector), IntersectionPointB);
            projectedValues.Add(GeometricMath.ProjectVectorOnNormalizedVector(IntersectionPointC, -RightVector), IntersectionPointC);
            projectedValues.Add(GeometricMath.ProjectVectorOnNormalizedVector(ViewerPosition, -RightVector), ViewerPosition);
            left = projectedValues[projectedValues.Keys.Max()].X;

            // right
            projectedValues.Clear();
            projectedValues.Add(GeometricMath.ProjectVectorOnNormalizedVector(IntersectionPointB, RightVector), IntersectionPointB);
            projectedValues.Add(GeometricMath.ProjectVectorOnNormalizedVector(IntersectionPointC, RightVector), IntersectionPointC);
            projectedValues.Add(GeometricMath.ProjectVectorOnNormalizedVector(ViewerPosition, RightVector), ViewerPosition);
            right = projectedValues[projectedValues.Keys.Max()].X;

            // top 
            projectedValues.Clear();
            projectedValues.Add(GeometricMath.ProjectVectorOnNormalizedVector(IntersectionPointTop, UpVector), IntersectionPointTop);
            projectedValues.Add(GeometricMath.ProjectVectorOnNormalizedVector(IntersectionPointBottom, UpVector), IntersectionPointBottom);
            projectedValues.Add(GeometricMath.ProjectVectorOnNormalizedVector(ViewerPosition, UpVector), ViewerPosition);
            top = projectedValues[projectedValues.Keys.Max()].Y;

            // bottom
            projectedValues.Clear();
            projectedValues.Add(GeometricMath.ProjectVectorOnNormalizedVector(IntersectionPointTop, -UpVector), IntersectionPointTop);
            projectedValues.Add(GeometricMath.ProjectVectorOnNormalizedVector(IntersectionPointBottom, -UpVector), IntersectionPointBottom);
            projectedValues.Add(GeometricMath.ProjectVectorOnNormalizedVector(ViewerPosition, -UpVector), ViewerPosition);
            bottom = projectedValues[projectedValues.Keys.Max()].Y;

            // far 
            projectedValues.Clear();
            projectedValues.Add(GeometricMath.ProjectVectorOnNormalizedVector(IntersectionPointB, ForwardVector), IntersectionPointB);
            projectedValues.Add(GeometricMath.ProjectVectorOnNormalizedVector(IntersectionPointC, ForwardVector), IntersectionPointC);
            projectedValues.Add(GeometricMath.ProjectVectorOnNormalizedVector(ViewerPosition, ForwardVector), ViewerPosition);
            far = projectedValues[projectedValues.Keys.Max()].Z;

            // near
            projectedValues.Clear();
            projectedValues.Add(GeometricMath.ProjectVectorOnNormalizedVector(IntersectionPointB, -ForwardVector), IntersectionPointB);
            projectedValues.Add(GeometricMath.ProjectVectorOnNormalizedVector(IntersectionPointC, -ForwardVector), IntersectionPointC);
            projectedValues.Add(GeometricMath.ProjectVectorOnNormalizedVector(ViewerPosition, -ForwardVector), ViewerPosition);
            near = projectedValues[projectedValues.Keys.Max()].Z;

            LBN = new Vector3(left, bottom, near);
            RTF = new Vector3(right, top, far);
        }

        public Matrix4 CreateOrthographicProjection(LiteCamera viewerCamera, ref Matrix4 projectionMatrix)
        {
            Matrix4 result = Matrix4.Identity;
            Matrix4 ViewProjectionMatrix = Matrix4.Identity;
            ViewProjectionMatrix *= viewerCamera.getViewMatrix();
            ViewProjectionMatrix *= projectionMatrix;

            ConvexVolume cameraVolume = new ConvexVolume(ViewProjectionMatrix);
            Vector3 lbn, rtf;
            GetEdgePoints(viewerCamera, cameraVolume, out lbn, out rtf);

            result = Matrix4.CreateOrthographic(rtf.X - lbn.X, 100, 1, 400);
            return result;
        }
    }
}
