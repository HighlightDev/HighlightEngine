﻿using OpenTK;

namespace MassiveGame.Core.RenderCore.Lights
{
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
