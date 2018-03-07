using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace MassiveGame.Optimization
{
    public static class VisibilityApi
    {
        #region Optimize

        public static void IsInView(IList<IVisible> optimizeObjects, ref Matrix4 projectionMatrix, Matrix4 viewMatrix)
        {
            // check objects that realizes interface
            foreach (IVisible obj in optimizeObjects)
            {
                if (obj != null)
                {
                    // if object is equal to sun - delete translations from matrix
                    if (obj.GetType() == typeof(SunRenderer))
                    {
                        var matrix = viewMatrix;
                        matrix[3, 0] = 0.0f;
                        matrix[3, 1] = 0.0f;
                        matrix[3, 2] = 0.0f;
                        obj.IsInViewFrustum(ref projectionMatrix, matrix);
                    }
                    else obj.IsInViewFrustum(ref projectionMatrix, viewMatrix);
                }
            }
        }

        #endregion
    }
}
