using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace MassiveGame.RenderCore.Lights
{
    public class PointLight : ShaderLight
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
