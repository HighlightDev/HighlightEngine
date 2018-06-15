using MassiveGame.RenderCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassiveGame.PostFX
{
    public class PostProcessShaderBase : ShaderBase
    {

        public PostProcessShaderBase(string shaderName, string VertexShaderFile, string FragmentShaderFile) 
            : base(shaderName, VertexShaderFile, FragmentShaderFile)
        {

        }

        protected override void SetShaderMacros()
        {
        }
    }
}
