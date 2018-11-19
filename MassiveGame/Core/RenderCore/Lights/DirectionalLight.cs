using MassiveGame.API.ResourcePool;
using MassiveGame.API.ResourcePool.PoolHandling;
using MassiveGame.Core.GameCore;
using MassiveGame.Core.RenderCore.Shadows;
using OpenTK;
using System;
using System.Runtime.Serialization;
using TextureLoader;

namespace MassiveGame.Core.RenderCore.Lights
{
    [Serializable]
    public class DirectionalLight 
        : LightBase
    {
        #region Definitions

        public Vector3 Direction { set; get; }
        public Vector3 Position { set; get; }
        public Vector3 Destination { set; get; }

        #endregion

        #region Constructors

        public DirectionalLight(Vector3 direction, Vector4 ambient, Vector4 diffuse, Vector4 specular)
            : base(ambient, diffuse, specular)
        {
            this.Direction = direction;
        }

        #endregion

        #region Serialization

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("direction", Direction, typeof(Vector3));
            info.AddValue("position", Position, typeof(Vector3));
            info.AddValue("destination", Destination, typeof(Vector3));
        }

        protected DirectionalLight(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Direction = (Vector3)info.GetValue("direction", typeof(Vector3));
            Position = (Vector3)info.GetValue("position", typeof(Vector3));
            Destination = (Vector3)info.GetValue("destination", typeof(Vector3));
        }

        #endregion
    }
}
