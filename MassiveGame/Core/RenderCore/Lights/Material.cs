using OpenTK;
using System;
using System.Runtime.Serialization;

namespace MassiveGame.Core.RenderCore.Lights
{
    [Serializable]
    public class Material : LightBase
    {
        public Vector3 Emission { private set; get; }
        public float ShineDamper { private set; get; }
        public float Reflectivity { private set; get; }

        #region Serialization

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("emission", Emission, typeof(Vector3));
            info.AddValue("shineDamper", ShineDamper);
            info.AddValue("reflectivity", Reflectivity);
        }

        protected Material(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.Emission = (Vector3)info.GetValue("emission", typeof(Vector3));
            this.ShineDamper = info.GetSingle("shineDamper");
            this.Reflectivity = info.GetSingle("reflectivity");
        }

        #endregion

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
