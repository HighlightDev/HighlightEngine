using MassiveGame.RenderCore.Lights;
using System.Collections.Generic;

namespace MassiveGame.RenderCore.Visibility
{
    public interface ILightHit
    {
        void IsLitByLightSource(List<PointLight> lightSources);
    }
}
