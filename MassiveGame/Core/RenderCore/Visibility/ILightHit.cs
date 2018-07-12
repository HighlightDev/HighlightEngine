using MassiveGame.Core.RenderCore.Lights;
using System.Collections.Generic;

namespace MassiveGame.Core.RenderCore.Visibility
{
    public interface ILightHit
    {
        void IsLitByLightSource(List<PointLight> lightSources);
    }
}
