using MassiveGame.RenderCore.Lights;
using System.Collections.Generic;

namespace MassiveGame.RenderCore.Visibility
{
    public static class LightHitCheckApi
    {
        public static void CheckLightSourceHitsMesh(List<ILightHit> meshes, List<PointLight> lightSources)
        {
            foreach (var mesh in meshes)
            {
                if (mesh != null)
                {
                    mesh.IsLitByLightSource(lightSources);
                }
            }
        }
    }
}
