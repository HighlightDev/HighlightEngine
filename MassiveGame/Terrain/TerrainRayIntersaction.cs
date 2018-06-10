using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using PhysicsBox.MathTypes;
using PhysicsBox;

namespace MassiveGame
{
    public static class TerrainRayIntersection
    {
        #region Definition

        private const float RAY_DISTANCE = 400;
        private const int RECURSION_COUNT = 200;

        #endregion

        #region Getter

        public static Vector3 getIntersectionPoint(Terrain terrain, FRay ray) //Binary search
        {
            Vector3 endPoint = getEndPoint(ray);
            Vector3 startPoint = ray.StartPosition;
            Vector3 middlePoint = new Vector3(0);
            for (int i = 0; i < RECURSION_COUNT; i++)
            {
                middlePoint = endPoint - (endPoint - startPoint) / 2; // Divide by two
                if (isUnderTerrain(terrain, middlePoint)) //If height of landscape is beyond point
                    endPoint = middlePoint;
                else
                    startPoint = middlePoint; //If height of landscape is under middlePoint
            }
            return middlePoint;
        }

        public static float Intersection_TerrainRay(Terrain terrain, FRay ray)
        {
            float t = -1.0f;
            if (terrain != null)
            {
                Vector3 P = getIntersectionPoint(terrain, ray);

                if (GeometricMath.CMP(ray.Direction.X, 0.0f) <= 0.0f && GeometricMath.CMP(P.X, 0.0f) <= 0.0f)
                    t = (P.X - ray.StartPosition.X) / ray.Direction.X;
                else if (GeometricMath.CMP(ray.Direction.Y, 0.0f) <= 0.0f && GeometricMath.CMP(P.Y, 0.0f) <= 0.0f)
                    t = (P.Y - ray.StartPosition.Y) / ray.Direction.Y;
                else if (GeometricMath.CMP(ray.Direction.Z, 0.0f) <= 0.0f && GeometricMath.CMP(P.Z, 0.0f) <= 0.0f)
                    t = (P.Z - ray.StartPosition.Z) / ray.Direction.Z;
            }
            return t;
        }

        private static Vector3 getEndPoint(FRay ray)
        {
            return ray.GetPositionInTime(RAY_DISTANCE);
        }

        private static bool isUnderTerrain(Terrain Terrain, Vector3 MiddlePoint)
        {
            if (Terrain.getLandscapeHeight(MiddlePoint.X, MiddlePoint.Z) > MiddlePoint.Y)
                return true;
            else
                return false;
        }

        #endregion

    }
}
