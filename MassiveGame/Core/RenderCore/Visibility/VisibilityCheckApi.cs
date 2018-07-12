using System.Collections.Generic;
using OpenTK;
using MassiveGame.Core.GameCore.Sun;

namespace MassiveGame.Core.RenderCore.Visibility
{
    public static class VisibilityCheckApi
    {
        #region Optimize

        public static void CheckMeshIsVisible(IList<IVisible> meshes, ref Matrix4 projectionMatrix, Matrix4 viewMatrix)
        {
            // check objects that realizes interface
            foreach (IVisible mesh in meshes)
            {
                if (mesh != null)
                {
                    // if object is equal to sun - delete translations from matrix
                    if (mesh.GetType() == typeof(SunRenderer))
                    {
                        var matrix = viewMatrix;
                        matrix[3, 0] = 0.0f;
                        matrix[3, 1] = 0.0f;
                        matrix[3, 2] = 0.0f;
                        mesh.IsInViewFrustum(ref projectionMatrix, matrix);
                    }
                    else
                        mesh.IsInViewFrustum(ref projectionMatrix, viewMatrix);
                }
            }
        }

        #endregion
    }
}
