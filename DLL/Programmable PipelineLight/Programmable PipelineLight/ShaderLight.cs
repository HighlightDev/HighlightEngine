using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace Programmable_PipelineLight
{
    public abstract class ShaderLight
    {
        public Vector4 Ambient { set; get; }
        public Vector4 Diffuse { set; get; }
        public Vector4 Specular { set; get; }

        public ShaderLight(Vector4 ambient, Vector4 diffuse, Vector4 specular)
        {
            this.Ambient = ambient;
            this.Diffuse = diffuse;
            this.Specular = specular;
        }
    }
}
