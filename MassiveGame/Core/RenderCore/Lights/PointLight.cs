using MassiveGame.Core.GameCore;
using OpenTK;
using System;

namespace MassiveGame.Core.RenderCore.Lights
{
    [Serializable]
    public class PointLight : LightBase
    {
        public Vector4 Position { set; get; }
        public Vector3 Attenuation { private set; get; }

        /*Distance at which light affects to objects*/
        public float AttenuationRadius { set; get; }

        public PointLight(Vector4 position, Vector4 ambient, Vector4 diffuse, Vector4 specular,
            Vector3 attenuation)
            : base(ambient, diffuse, specular)
        {
            this.Position = position;
            this.Attenuation = attenuation;
            AttenuationRadius = 1000;
        }
    }
}
