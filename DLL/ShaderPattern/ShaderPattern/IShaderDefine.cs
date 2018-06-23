using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK.Graphics.OpenGL;
using static ShaderPattern.Shader;

namespace ShaderPattern
{
    public interface IShaderDefine
    {
        void SetDefine<T>(ShaderTypeFlag shaderType, string name, T value) where T : struct;
    }
}
