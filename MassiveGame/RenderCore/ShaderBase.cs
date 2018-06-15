using ShaderPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassiveGame.RenderCore
{
    public class ShaderBase : Shader
    {
        public ShaderBase(string shaderName, string VertexShaderFile, string FragmentShaderFile, string GeometryShaderFile = "") : base(VertexShaderFile, FragmentShaderFile, GeometryShaderFile)
        {
            if (base.ShaderLoaded)
            {
                base.showCompileLogInfo(shaderName);
                base.showLinkLogInfo(shaderName);
                Debug.Log.addToLog(getCompileLogInfo(shaderName));
                Debug.Log.addToLog(getLinkLogInfo(shaderName));
            }
            else Debug.Log.addToLog(DateTime.Now.ToString() + "  " + shaderName + " : shader file(s) not found!");
        }

        protected override void SetShaderMacros()
        {
            // Implement in derived shaders
            throw new NotImplementedException();
        }
    }
}
