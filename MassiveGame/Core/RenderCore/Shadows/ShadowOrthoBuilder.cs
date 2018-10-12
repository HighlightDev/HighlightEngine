using MassiveGame.Core.GameCore;
using MassiveGame.Core.MathCore;
using MassiveGame.Core.MathCore.MathTypes;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MassiveGame.Core.RenderCore.Shadows
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

            result = GeometryMath.GetIntersectionRayPlane(cbPlane, acRay);
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

            result = GeometryMath.GetIntersectionRayPlane(cbPlane, acRay);
            return result;
        }

        private void GetEdgePoints(BaseCamera viewerCamera, ConvexVolume volume, out Vector3 LBN, out Vector3 RTF)
        {
            Vector3 ViewerPosition = viewerCamera.GetEyeVector();

            // Find left and right edges
            Vector3 IntersectionPointC = GetLeftRightIntersectionPoint(ViewerPosition, volume.FarPlane, volume.LeftPlane);
            Vector3 IntersectionPointB = GetLeftRightIntersectionPoint(ViewerPosition, volume.FarPlane, volume.RightPlane);
            // Find top and bottom edges
            Vector3 IntersectionPointBottom = GetTopBottomIntersectionPoint(ViewerPosition, volume.FarPlane, volume.BottomPlane);
            Vector3 IntersectionPointTop = GetTopBottomIntersectionPoint(ViewerPosition, volume.FarPlane, volume.TopPlane);

            float left, right, top, bottom, near, far;
            // left 
            Dictionary<float, Vector3> projectedValues = new Dictionary<float, Vector3>();
            projectedValues.Add(GeometryMath.ProjectVectorOnNormalizedVector(IntersectionPointB, -RightVector), IntersectionPointB);
            projectedValues.Add(GeometryMath.ProjectVectorOnNormalizedVector(IntersectionPointC, -RightVector), IntersectionPointC);
            projectedValues.Add(GeometryMath.ProjectVectorOnNormalizedVector(ViewerPosition, -RightVector), ViewerPosition);
            left = projectedValues[projectedValues.Keys.Max()].X;

            // right
            projectedValues.Clear();
            projectedValues.Add(GeometryMath.ProjectVectorOnNormalizedVector(IntersectionPointB, RightVector), IntersectionPointB);
            projectedValues.Add(GeometryMath.ProjectVectorOnNormalizedVector(IntersectionPointC, RightVector), IntersectionPointC);
            projectedValues.Add(GeometryMath.ProjectVectorOnNormalizedVector(ViewerPosition, RightVector), ViewerPosition);
            right = projectedValues[projectedValues.Keys.Max()].X;

            // top 
            projectedValues.Clear();
            projectedValues.Add(GeometryMath.ProjectVectorOnNormalizedVector(IntersectionPointTop, UpVector), IntersectionPointTop);
            projectedValues.Add(GeometryMath.ProjectVectorOnNormalizedVector(IntersectionPointBottom, UpVector), IntersectionPointBottom);
            projectedValues.Add(GeometryMath.ProjectVectorOnNormalizedVector(ViewerPosition, UpVector), ViewerPosition);
            top = projectedValues[projectedValues.Keys.Max()].Y;

            // bottom
            projectedValues.Clear();
            projectedValues.Add(GeometryMath.ProjectVectorOnNormalizedVector(IntersectionPointTop, -UpVector), IntersectionPointTop);
            projectedValues.Add(GeometryMath.ProjectVectorOnNormalizedVector(IntersectionPointBottom, -UpVector), IntersectionPointBottom);
            projectedValues.Add(GeometryMath.ProjectVectorOnNormalizedVector(ViewerPosition, -UpVector), ViewerPosition);
            bottom = projectedValues[projectedValues.Keys.Max()].Y;

            // far 
            projectedValues.Clear();
            projectedValues.Add(GeometryMath.ProjectVectorOnNormalizedVector(IntersectionPointB, ForwardVector), IntersectionPointB);
            projectedValues.Add(GeometryMath.ProjectVectorOnNormalizedVector(IntersectionPointC, ForwardVector), IntersectionPointC);
            projectedValues.Add(GeometryMath.ProjectVectorOnNormalizedVector(ViewerPosition, ForwardVector), ViewerPosition);
            far = projectedValues[projectedValues.Keys.Max()].Z;

            // near
            projectedValues.Clear();
            projectedValues.Add(GeometryMath.ProjectVectorOnNormalizedVector(IntersectionPointB, -ForwardVector), IntersectionPointB);
            projectedValues.Add(GeometryMath.ProjectVectorOnNormalizedVector(IntersectionPointC, -ForwardVector), IntersectionPointC);
            projectedValues.Add(GeometryMath.ProjectVectorOnNormalizedVector(ViewerPosition, -ForwardVector), ViewerPosition);
            near = projectedValues[projectedValues.Keys.Max()].Z;

            LBN = new Vector3(left, bottom, near);
            RTF = new Vector3(right, top, far);
        }

        public Matrix4 CreateOrthographicProjection(BaseCamera viewerCamera, ref Matrix4 projectionMatrix)
        {
            Matrix4 result = Matrix4.Identity;
            Matrix4 ViewProjectionMatrix = Matrix4.Identity;
            ViewProjectionMatrix *= viewerCamera.GetViewMatrix();
            ViewProjectionMatrix *= projectionMatrix;

            ConvexVolume cameraVolume = new ConvexVolume(ViewProjectionMatrix);
            Vector3 lbn, rtf;
            GetEdgePoints(viewerCamera, cameraVolume, out lbn, out rtf);

            result = Matrix4.CreateOrthographic(rtf.X - lbn.X, 100, 1, 400);
            return result;
        }
    }

    public class Test1
    {
        private BaseCamera m_camera;

        public Test1(BaseCamera camera) { m_camera = camera; }
        /*________*/
        /*|   A  |*/
        /*|  /\  |*/
        /*| /  \ |*/
        /*|C----B|*/
        /*|______|*/

        private void someFoo()
        {
            float ShadowFarPlane = 100.0f;
            float ShadowNearPlane = 30.0f;
            float FoV = EngineStatics.FoV;
            float halfFoV = FoV * 0.5f;
            float AspectRatio = EngineStatics.SCREEN_ASPECT_RATIO;

            Vector3 camWorldPosition = m_camera.GetEyeVector();
            Vector3 camForward = m_camera.GetEyeSpaceForwardVector();

            float rotByHalfFov_x1 = (float)Math.Cos(MathHelper.DegreesToRadians(halfFoV));
            float rotByHalfFov_z1 = (float)Math.Sin(MathHelper.DegreesToRadians(halfFoV));

            float rotByHalfFov_x2 = (float)Math.Cos(MathHelper.DegreesToRadians(-halfFoV));
            float rotByHalfFov_z2 = (float)Math.Sin(MathHelper.DegreesToRadians(-halfFoV));

            float edge_x1 = camForward.X * rotByHalfFov_x1;
            float edge_z1 = camForward.Z * rotByHalfFov_z1;

            float edge_x2 = camForward.X * rotByHalfFov_x2;
            float edge_z2 = camForward.Z * rotByHalfFov_z2;

            Vector3 camEdgePlaneDirection1 = (new Vector3(edge_x1, 0.0f, edge_z1)).Normalized();
            Vector3 camEdgePlaneDirection2 = (new Vector3(edge_x2, 0.0f, edge_z2)).Normalized();

            Vector3 camFarPlaneWorldPosition = camWorldPosition + camForward * ShadowFarPlane;

            float edge1ProjValue = Vector3.Dot(camEdgePlaneDirection1, camFarPlaneWorldPosition);
            float edge2ProjValue = Vector3.Dot(camEdgePlaneDirection2, camFarPlaneWorldPosition);

            Vector3 edgePosition1 = camWorldPosition + camEdgePlaneDirection1 * edge1ProjValue;
            Vector3 edgePosition2 = camWorldPosition + camEdgePlaneDirection2 * edge2ProjValue;
            Vector3 edgePosition3 = camWorldPosition - camForward * ShadowNearPlane;

            // TODO -> Find maxX, minX, maxZ, minZ

            float maxX = Math.Max(Math.Max(edgePosition1.X, edgePosition2.X), edgePosition3.X);
            //float maxY = Math.Max(Math.Max(edgePosition1.Y, edgePosition2.Y), edgePosition3.Y);
            float maxZ = Math.Max(Math.Max(edgePosition1.Z, edgePosition2.Z), edgePosition3.Z);

            float minX = Math.Min(Math.Min(edgePosition1.X, edgePosition2.X), edgePosition3.X);
            //float minY = Math.Min(Math.Min(edgePosition1.Y, edgePosition2.Y), edgePosition3.Y);
            float minZ = Math.Min(Math.Min(edgePosition1.Z, edgePosition2.Z), edgePosition3.Z);
            
        }      
    }
}
