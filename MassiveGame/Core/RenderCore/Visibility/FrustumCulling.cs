using System;
using OpenTK;
using MassiveGame.Core.MathCore.MathTypes;
using MassiveGame.Core.MathCore;

namespace MassiveGame.Core.RenderCore.Visibility
{
    public static class FrustumCulling
    {
        #region Find_intersection

        public static bool GetIsBoundInFrustum(BoundBase boundingBox, ref Matrix4 viewMatrix, ref Matrix4 projectionMatrix)
        {
            bool bInFrustum = false;

            Matrix4 viewprojectionMatrix = Matrix4.Mult(viewMatrix, projectionMatrix);
            Vector3[] bbVertices = boundingBox.GetWorldSpaceVertices();

            for (Int32 i = 0; i < bbVertices.Length; i++)
            {
                // Transform to clipped space
                Vector4 clippedSpace = Vector4.Transform(new Vector4(bbVertices[i], 1.0f), viewprojectionMatrix);
                // Transform to normalized device coordinates
                Vector3 ndc = clippedSpace.Xyz / clippedSpace.W;

                if (ndc.X >= -1 && ndc.X <= 1 &&
                    ndc.Y >= -1 && ndc.Y <= 1 &&
                    ndc.Z >= -1 && ndc.Z <= 1)
                {
                    bInFrustum = true;
                    break;
                }
            }

            return bInFrustum;
        }

        #endregion

        #region Quad_objects

        public static bool isQuadIntersection(CollisionQuad quad, Matrix4 viewMatrix, Matrix4 projectionMatrix)
        {
            Matrix4 viewprojectionMatrix;
            Matrix4.Mult(ref viewMatrix, ref projectionMatrix, out viewprojectionMatrix);

            /*Transform vertices to clipped space*/

            Vector4 LBZ = Vector4.Transform(new Vector4(quad.LBNCoordinates.X, quad.LBNCoordinates.Y, quad.LBNCoordinates.Z, 1.0f), viewprojectionMatrix);
            Vector4 RBZ = Vector4.Transform(new Vector4(quad.RTFCoordinates.X, quad.LBNCoordinates.Y, quad.LBNCoordinates.Z, 1.0f), viewprojectionMatrix);
            Vector4 RTZ = Vector4.Transform(new Vector4(quad.RTFCoordinates.X, quad.RTFCoordinates.Y, quad.LBNCoordinates.Z, 1.0f), viewprojectionMatrix);
            Vector4 LTZ = Vector4.Transform(new Vector4(quad.LBNCoordinates.X, quad.RTFCoordinates.Y, quad.LBNCoordinates.Z, 1.0f), viewprojectionMatrix);

            /*Transform vertices to normalized device coordinates*/

            Vector4 ndcLBZ = new Vector4(new Vector3(LBZ) / LBZ.W, LBZ.W);
            Vector4 ndcRBZ = new Vector4(new Vector3(RBZ) / RBZ.W, RBZ.W);
            Vector4 ndcRTZ = new Vector4(new Vector3(RTZ) / RTZ.W, RTZ.W);
            Vector4 ndcLTZ = new Vector4(new Vector3(LTZ) / LTZ.W, LTZ.W);

            /*Check if quad doesn't lie in clipped area*/

            if ((ndcLBZ.X > -1.0f && ndcLBZ.X < 1.0f) && (ndcLBZ.Y > -1.0f && ndcLBZ.Y < 1.0f) && (ndcLBZ.Z > -1.0f && ndcLBZ.Z < 1.0f))
            {
                return true;
            }
            if ((ndcRBZ.X > -1.0f && ndcRBZ.X < 1.0f) && (ndcRBZ.Y > -1.0f && ndcRBZ.Y < 1.0f) && (ndcRBZ.Z > -1.0f && ndcRBZ.Z < 1.0f))
            {
                return true;
            }
            if ((ndcRTZ.X > -1.0f && ndcRTZ.X < 1.0f) && (ndcRTZ.Y > -1.0f && ndcRTZ.Y < 1.0f) && (ndcRTZ.Z > -1.0f && ndcRTZ.Z < 1.0f))
            {
                return true;
            }
            if ((ndcLTZ.X > -1.0f && ndcLTZ.X < 1.0f) && (ndcLTZ.Y > -1.0f && ndcLTZ.Y < 1.0f) && (ndcLTZ.Z > -1.0f && ndcLTZ.Z < 1.0f))
            {
                return true;
            }
            return false;
        }

        #endregion

        #region Water_optimization

        public static Vector3[] divideWaterCollisionBox(CollisionQuad quad, Int32 divideCount)
        {
            // disable optimize
            if (divideCount == 0) return null;

            float stepX = 0.0f, stepZ = 0.0f;
            float stepValueX = (quad.RTFCoordinates.X - quad.LBNCoordinates.X) / divideCount;
            float stepValueZ = (quad.RTFCoordinates.Z - quad.LBNCoordinates.Z) / divideCount;
            Vector3[] checkPoints = new Vector3[(2 + divideCount) * (2 + divideCount)];
            for (Int32 i = 0; i < checkPoints.Length; i++, stepX += stepValueX)
            {
                /*To Do :
                 true - transition to next row
                 false - continue work with current row*/
                if ((i + 1) % (divideCount + 2) == 0)
                {
                    stepX = 0;
                    stepZ += stepValueZ;
                }

                checkPoints[i] = new Vector3(quad.LBNCoordinates.X + stepX, quad.LBNCoordinates.Y, quad.LBNCoordinates.Z + stepZ);
            }
            return checkPoints;
        }

        public static bool isWaterIntersection(Vector3[] boxCheckPoints, Matrix4 viewMatrix, ref Matrix4 projectionMatrix)
        {
            var intersection = false;
            Matrix4 viewprojectionMatrix = Matrix4.Mult(viewMatrix, projectionMatrix);
            /*Transform vertices to clipped space
             and then transform vertices to normalized device coordinates*/

            Vector4[] ndcCheckPoints = new Vector4[boxCheckPoints.Length];
            for (Int32 i = 0; i < ndcCheckPoints.Length; i++)
            {
                ndcCheckPoints[i] = Vector4.Transform(new Vector4(boxCheckPoints[i], 1.0f), viewprojectionMatrix);
                ndcCheckPoints[i] = new Vector4(new Vector3(ndcCheckPoints[i]) / ndcCheckPoints[i].W, ndcCheckPoints[i].W);

                /*Check if box doesn't lie in clipped area*/
                if ((ndcCheckPoints[i].X > -1.0f && ndcCheckPoints[i].X < 1.0f) && 
                    (ndcCheckPoints[i].Y > -1.0f && ndcCheckPoints[i].Y < 1.0f) && 
                    (ndcCheckPoints[i].Z > -1.0f && ndcCheckPoints[i].Z < 1.0f))
                {
                    intersection = true;
                    break;
                }
            }

            return intersection;
        } 

        #endregion
    }
}
