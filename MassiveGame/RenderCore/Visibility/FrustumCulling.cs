using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using PhysicsBox;

namespace MassiveGame.Optimization
{
    public static class FrustumCulling
    {
        #region Find_intersection

        public static bool isBoxIntersection(CollisionSphereBox box, Matrix4 viewMatrix, ref Matrix4 projectionMatrix)
        {
            Matrix4 viewprojectionMatrix = Matrix4.Mult(viewMatrix, projectionMatrix);

            /*Transform vertices to clipped space*/

            /*Every vertex of box at bottom plane*/
            Vector4 LBN = Vector4.Transform(new Vector4(box.LBNCoordinates, 1.0f), viewprojectionMatrix);
            Vector4 RBN = Vector4.Transform(new Vector4(box.RTFCoordinates.X, box.LBNCoordinates.Y, box.LBNCoordinates.Z, 1.0f), viewprojectionMatrix);
            Vector4 RBF = Vector4.Transform(new Vector4(box.RTFCoordinates.X, box.LBNCoordinates.Y, box.RTFCoordinates.Z, 1.0f), viewprojectionMatrix);
            Vector4 LBF = Vector4.Transform(new Vector4(box.LBNCoordinates.X, box.LBNCoordinates.Y, box.RTFCoordinates.Z, 1.0f), viewprojectionMatrix);

            /*Every vertex of box at top plane*/
            Vector4 LTN = Vector4.Transform(new Vector4(box.LBNCoordinates.X, box.RTFCoordinates.Y, box.LBNCoordinates.Z, 1.0f), viewprojectionMatrix);
            Vector4 RTN = Vector4.Transform(new Vector4(box.RTFCoordinates.X, box.RTFCoordinates.Y, box.LBNCoordinates.Z, 1.0f), viewprojectionMatrix);
            Vector4 RTF = Vector4.Transform(new Vector4(box.RTFCoordinates, 1.0f), viewprojectionMatrix);
            Vector4 LTF = Vector4.Transform(new Vector4(box.LBNCoordinates.X, box.RTFCoordinates.Y, box.RTFCoordinates.Z, 1.0f), viewprojectionMatrix);

            /*Transform vertices to normalized device coordinates*/

            Vector4 ndcLBN = new Vector4(new Vector3(LBN) / LBN.W, LBN.W);
            Vector4 ndcRBN = new Vector4(new Vector3(RBN) / RBN.W, RBN.W);
            Vector4 ndcRBF = new Vector4(new Vector3(RBF) / RBF.W, RBF.W);
            Vector4 ndcLBF = new Vector4(new Vector3(LBF) / LBF.W, LBF.W);

            Vector4 ndcLTN = new Vector4(new Vector3(LTN) / LTN.W, LTN.W);
            Vector4 ndcRTN = new Vector4(new Vector3(RTN) / RTN.W, RTN.W);
            Vector4 ndcRTF = new Vector4(new Vector3(RTF) / RTF.W, RTF.W);
            Vector4 ndcLTF = new Vector4(new Vector3(LTF) / LTF.W, LTF.W);

            /*Check if box doesn't lie in clipped area*/

            /*Every vertex of box at bottom plane*/
            if ((ndcLBN.X > -1.0f && ndcLBN.X < 1.0f) && (ndcLBN.Y > -1.0f && ndcLBN.Y < 1.0f) && (ndcLBN.Z > -1.0f && ndcLBN.Z < 1.0f))
            {
                return true;
            }
            if ((ndcRBN.X > -1.0f && ndcRBN.X < 1.0f) && (ndcRBN.Y > -1.0f && ndcRBN.Y < 1.0f) && (ndcRBN.Z > -1.0f && ndcRBN.Z < 1.0f))
            {
                return true;
            }
            if ((ndcRBF.X > -1.0f && ndcRBF.X < 1.0f) && (ndcRBF.Y > -1.0f && ndcRBF.Y < 1.0f) && (ndcRBF.Z > -1.0f && ndcRBF.Z < 1.0f))
            {
                return true;
            }
            if ((ndcLBF.X > -1.0f && ndcLBF.X < 1.0f) && (ndcLBF.Y > -1.0f && ndcLBF.Y < 1.0f) && (ndcLBF.Z > -1.0f && ndcLBF.Z < 1.0f))
            {
                return true;
            }

            /*Every vertex of box at top plane*/
            if ((ndcLTN.X > -1.0f && ndcLTN.X < 1.0f) && (ndcLTN.Y > -1.0f && ndcLTN.Y < 1.0f) && (ndcLTN.Z > -1.0f && ndcLTN.Z < 1.0f))
            {
                return true;
            }
            if ((ndcRTN.X > -1.0f && ndcRTN.X < 1.0f) && (ndcRTN.Y > -1.0f && ndcRTN.Y < 1.0f) && (ndcRTN.Z > -1.0f && ndcRTN.Z < 1.0f))
            {
                return true;
            }
            if ((ndcRTF.X > -1.0f && ndcRTF.X < 1.0f) && (ndcRTF.Y > -1.0f && ndcRTF.Y < 1.0f) && (ndcRTF.Z > -1.0f && ndcRTF.Z < 1.0f))
            {
                return true;
            }
            if ((ndcLTF.X > -1.0f && ndcLTF.X < 1.0f) && (ndcLTF.Y > -1.0f && ndcLTF.Y < 1.0f) && (ndcLTF.Z > -1.0f && ndcLTF.Z < 1.0f))
            {
                return true;
            }
 
            // no intersections
            return false;
        }

        #endregion

        #region Quad_objects

        public static bool isQuadIntersection(CollisionQuad quad, Matrix4 viewMatrix, Matrix4 projectionMatrix)
        {
            Matrix4 viewprojectionMatrix;
            Matrix4.Mult(ref viewMatrix, ref projectionMatrix, out viewprojectionMatrix);

            /*Transform vertices to clipped space*/

            Vector4 LBZ = Vector4.Transform(new Vector4(quad.LBCoordinates.X, quad.LBCoordinates.Y, quad.ZCoordinate, 1.0f), viewprojectionMatrix);
            Vector4 RBZ = Vector4.Transform(new Vector4(quad.RTCoordinates.X, quad.LBCoordinates.Y, quad.ZCoordinate, 1.0f), viewprojectionMatrix);
            Vector4 RTZ = Vector4.Transform(new Vector4(quad.RTCoordinates.X, quad.RTCoordinates.Y, quad.ZCoordinate, 1.0f), viewprojectionMatrix);
            Vector4 LTZ = Vector4.Transform(new Vector4(quad.LBCoordinates.X, quad.RTCoordinates.Y, quad.ZCoordinate, 1.0f), viewprojectionMatrix);

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

        public static Vector3[] divideWaterCollisionBox(CollisionSphereBox box, int divideCount)
        {
            // disable optimize
            if (divideCount == 0) return null;

            float stepX = 0.0f, stepZ = 0.0f;
            float stepValueX = (box.RTFCoordinates.X - box.LBNCoordinates.X) / divideCount;
            float stepValueZ = (box.RTFCoordinates.Z - box.LBNCoordinates.Z) / divideCount;
            Vector3[] checkPoints = new Vector3[(2 + divideCount) * (2 + divideCount)];
            for (int i = 0; i < checkPoints.Length; i++, stepX += stepValueX)
            {
                /*To Do :
                 true - transition to next row
                 false - continue work with current row*/
                if ((i + 1) % (divideCount + 2) == 0)
                {
                    stepX = 0;
                    stepZ += stepValueZ;
                }
                
                checkPoints[i] = new Vector3(box.LBNCoordinates.X + stepX, box.LBNCoordinates.Y, box.LBNCoordinates.Z + stepZ);
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
            for (int i = 0; i < ndcCheckPoints.Length; i++)
            {
                ndcCheckPoints[i] = Vector4.Transform(new Vector4(boxCheckPoints[i], 1.0f), viewprojectionMatrix);
                ndcCheckPoints[i] = new Vector4(new Vector3(ndcCheckPoints[i]) / ndcCheckPoints[i].W, ndcCheckPoints[i].W);

                /*Check if box doesn't lie in clipped area*/
                if ((ndcCheckPoints[i].X > -1.0f && ndcCheckPoints[i].X < 1.0f) && 
                    (ndcCheckPoints[i].Y > -1.0f && ndcCheckPoints[i].Y < 1.0f) && 
                    (ndcCheckPoints[i].Z > -1.0f && ndcCheckPoints[i].Z < 1.0f))
                {
                    return true;
                }
            }

            return intersection;
        } 

        #endregion
    }
}
