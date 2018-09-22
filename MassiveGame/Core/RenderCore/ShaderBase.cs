using ShaderPattern;
using System;

namespace MassiveGame.Core.RenderCore
{
    public class ShaderBase : Shader
    {
        public ShaderBase() : base("", "", "", "") { }

        protected override void getAllUniformLocations()
        {
            if (base.m_shaderLoaded)
            {
                base.showCompileLogInfo(m_shaderName);
                base.showLinkLogInfo(m_shaderName);
                Debug.Log.AddToFileStreamLog(getCompileLogInfo(m_shaderName));
                Debug.Log.AddToFileStreamLog(getLinkLogInfo(m_shaderName));
            }
            else Debug.Log.AddToFileStreamLog(DateTime.Now.ToString() + "  " + m_shaderName + " : shader file(s) not found!");
        }

        public ShaderBase(string shaderName, string VertexShaderFile, string FragmentShaderFile, string GeometryShaderFile = "") : base(shaderName, VertexShaderFile, FragmentShaderFile, GeometryShaderFile)
        {
        }

        protected override void SetShaderMacros()
        {
            // Implement in derived shaders
            throw new NotImplementedException();
        }
    }
}
