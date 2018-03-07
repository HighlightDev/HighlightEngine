using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK.Graphics.OpenGL;

namespace ShaderPattern
{
    public interface IShaderDefine
    {
        void SetDefine(ShaderType shaderType, string name, string formatValue);
    }
}
