using MassiveGame.Core.RenderCore.Shadows;
using OpenTK;
using TextureLoader;

namespace MassiveGame.Core.RenderCore.Lights
{
    public class DirectionalLight : ShaderLight
    {
        #region Definitions

        public Vector3 Direction { set; get; }
        public Vector3 Position { set; get; }
        public Vector3 Destination { set; get; }

        public ShadowDirectionalLight GetShadow()
        {
            return (ShadowDirectionalLight)Shadow;
        }

        #endregion

        #region Constructors

        public DirectionalLight(TextureParameters rtParams, Vector3 direction, Vector4 ambient, Vector4 diffuse, Vector4 specular)
            : base(ambient, diffuse, specular)
        {
            Shadow = new ShadowDirectionalLight(EngineStatics.Camera, rtParams, this);
            this.Direction = direction;
        }

        #endregion
    }
}
