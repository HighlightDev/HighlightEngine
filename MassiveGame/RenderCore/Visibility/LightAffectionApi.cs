using MassiveGame.RenderCore.Lights;
using System.Collections.Generic;

namespace MassiveGame.RenderCore.Visibility
{
    public static class LightAffectionApi
    {
        public static void IsLightAffects(List<ILightAffection> Primitives, List<PointLight> PointLights)
        {
            foreach (var item in Primitives)
            {
                if (item != null)
                {
                    item.IsLightAffecting(PointLights);
                }
            }
        }
    }
}
