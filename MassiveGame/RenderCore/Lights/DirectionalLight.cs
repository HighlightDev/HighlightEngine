using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using MassiveGame.RenderCore.Shadows;
using FramebufferAPI;
using TextureLoader;

namespace MassiveGame.RenderCore.Lights
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
