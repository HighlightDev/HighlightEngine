using OpenTK;
using System;

namespace MassiveGame.Core.RenderCore.Lights
{
    [Serializable]
    public class Material : ShaderLight
    {
        public Vector3 Emission { private set; get; }
        public float ShineDamper { private set; get; }
        public float Reflectivity { private set; get; }

        public Material(Vector3 ambient, Vector3 diffuse, Vector3 specular, Vector3 emission,
            float shineDamper, float reflectivity)
            : base(new Vector4(ambient, 1.0f), new Vector4(diffuse, 1.0f), new Vector4(specular, 1.0f))
        {
            this.Emission = emission;
            this.ShineDamper = shineDamper;
            this.Reflectivity = reflectivity;
        }
    }
}
