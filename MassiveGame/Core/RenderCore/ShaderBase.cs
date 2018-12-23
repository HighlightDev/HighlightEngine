using MassiveGame.Core.DebugCore;
using ShaderPattern;
using System;

namespace MassiveGame.Core.RenderCore
{
    public class ShaderBase : Shader
    {
        public ShaderBase() : base("", "", "", "") { }

        protected override void getAllUniformLocations()
        {
            if (m_shaderLoaded)
            {
                showCompileLogInfo(m_shaderName);
                showLinkLogInfo(m_shaderName);
                Log.AddToFileStreamLog(getCompileLogInfo(m_shaderName));
                Log.AddToFileStreamLog(getLinkLogInfo(m_shaderName));
            }
            else Log.AddToFileStreamLog(DateTime.Now.ToString() + "  " + m_shaderName + " : shader file(s) not found!");
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
