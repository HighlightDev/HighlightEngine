using MassiveGame.RenderCore.Lights;
using System.Collections.Generic;

namespace MassiveGame.RenderCore.Visibility
{
    public interface ILightAffection
    {
        void IsLightAffecting(List<PointLight> LightList);
    }
}
