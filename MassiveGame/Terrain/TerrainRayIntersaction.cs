using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;

namespace MassiveGame
{
    public static class TerrainRayIntersaction
    {
        #region Definition

        private const float RAY_DISTANCE = 400;
        private const int RECURSION_COUNT = 200;

        #endregion

        #region Getter

        public static Vector3 getIntersactionPoint(Terrain Terrain, Vector3 Ray, Vector3 CameraPos) //Бинарный поиск
        {
            Vector3 endPoint = getEndPoint(Ray, CameraPos);
            Vector3 startPoint = CameraPos;
            Vector3 middlePoint = new Vector3(0);
            for (int i = 0; i < RECURSION_COUNT; i++)
            {
                middlePoint = endPoint - (endPoint - startPoint) / 2;   //Делим пополам
                if (isUnderTerrain(Terrain, middlePoint)) //Если ландшафт над middlePoint
                {
                    endPoint = middlePoint;
                }
                else { startPoint = middlePoint; }  //Если ландшафт под middlePoint
            }
            return middlePoint;
        }

        private static Vector3 getEndPoint(Vector3 Ray, Vector3 CameraPos)
        {
            return new Vector3(CameraPos += Ray * RAY_DISTANCE);
        }

        private static bool isUnderTerrain(Terrain Terrain, Vector3 MiddlePoint)
        {
            if (Terrain.getLandscapeHeight(MiddlePoint.X, MiddlePoint.Z) > MiddlePoint.Y) return true;
            else return false;
        }

        #endregion

    }
}
