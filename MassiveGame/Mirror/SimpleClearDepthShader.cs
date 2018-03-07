using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ShaderPattern;

namespace MassiveGame
{
    public class SimpleClearDepthShader : Shader
    {
        private const string SHADER_NAME = "Simple Clear Depth Shader";

        public SimpleClearDepthShader(string vsPath, string fsPath)
            : base(vsPath, fsPath)
        {
            if (ShaderLoaded)
            {
                base.showCompileLogInfo(SHADER_NAME);
                base.showLinkLogInfo(SHADER_NAME);
            }
        }
    }
}
