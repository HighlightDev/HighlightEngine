using OpenTK;
using System;
using System.Runtime.Serialization;
using MassiveGame.Core.GameCore;

namespace MassiveGame.Core.RenderCore.Lights
{
    [Serializable]
    public abstract class LightBase : ISerializable, IObservable
    {
        public Vector4 Ambient { set; get; }
        public Vector4 Diffuse { set; get; }
        public Vector4 Specular { set; get; }

        public LightBase(Vector4 ambient, Vector4 diffuse, Vector4 specular)
        {
            this.Ambient = ambient;
            this.Diffuse = diffuse;
            this.Specular = specular;
        }

        public virtual bool GetHasShadow()
        {
            return false;
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ambient", Ambient, typeof(Vector4));
            info.AddValue("diffuse", Diffuse, typeof(Vector4));
            info.AddValue("specular", Specular, typeof(Vector4));
        }


        public void NotifyAdded() { }

        public void NotifyRemoved() { }

        protected LightBase(SerializationInfo info, StreamingContext context)
        {
            Ambient = (Vector4)info.GetValue("ambient", typeof(Vector4));
            Diffuse = (Vector4)info.GetValue("diffuse", typeof(Vector4));
            Specular = (Vector4)info.GetValue("specular", typeof(Vector4));
        }
    }
}
