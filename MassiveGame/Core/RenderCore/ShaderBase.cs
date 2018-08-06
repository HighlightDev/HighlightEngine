using ShaderPattern;
using System;

namespace MassiveGame.Core.RenderCore
{
    public class ShaderBase : Shader
    {
        public ShaderBase() : base("", "", "", "") { }

        public ShaderBase(string shaderName, string VertexShaderFile, string FragmentShaderFile, string GeometryShaderFile = "") : base(shaderName, VertexShaderFile, FragmentShaderFile, GeometryShaderFile)
        {
            if (base.m_shaderLoaded)
            {
                base.showCompileLogInfo(shaderName);
                base.showLinkLogInfo(shaderName);
                Debug.Log.AddToFileStreamLog(getCompileLogInfo(shaderName));
                Debug.Log.AddToFileStreamLog(getLinkLogInfo(shaderName));
            }
            else Debug.Log.AddToFileStreamLog(DateTime.Now.ToString() + "  " + shaderName + " : shader file(s) not found!");
        }

        protected override void SetShaderMacros()
        {
            // Implement in derived shaders
            throw new NotImplementedException();
        }
    }
}
