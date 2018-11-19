using OpenTK;

namespace MassiveGame.Core.RenderCore.Lights
{
    public class SpotLight : LightBase
    {
        public Vector4 Position { set; get; }
        public Vector3 Direction { set; get; }
        public Vector3 Attenuation { private set; get; }
        public float SpotCutoff { private set; get; }
        public float SpotExponent { private set; get; }

        public SpotLight(Vector4 position, Vector3 direction, Vector4 ambient, Vector4 diffuse,
            Vector4 specular, Vector3 attenuation, float spotCutoff, float spotExponent)
            : base(ambient, diffuse, specular)
        {
            this.Position = position;
            this.Attenuation = attenuation;
            this.Direction = direction;
            this.SpotCutoff = spotCutoff;
            this.SpotExponent = spotExponent;
        }
    }
}
