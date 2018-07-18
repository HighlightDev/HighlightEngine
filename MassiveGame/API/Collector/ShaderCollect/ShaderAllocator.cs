using ShaderPattern;
using System;

namespace MassiveGame.API.Collector.ShaderCollect
{
    public static class ShaderAllocator
    {
        public static ShaderType LoadShaderFromFile<ShaderType>(string compositeKey) where ShaderType : new()
        {
            string[] shaderFiles = GetKeys(compositeKey);
            return (ShaderType)Activator.CreateInstance(typeof(ShaderType), shaderFiles);
        }

        private static string[] GetKeys(string compositeKey)
        {
            var result = compositeKey.Split(',');
            return result;
        }

        public static void ReleaseShader(Shader shader)
        {
            shader.cleanUp();
        }
    }
}
