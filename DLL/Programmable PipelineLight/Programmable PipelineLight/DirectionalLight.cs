using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace Programmable_PipelineLight
{
    public class DirectionalLight : ShaderLight
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
    }
}
