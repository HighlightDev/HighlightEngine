using ShaderPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MassiveGame.API.Collector.ShaderCollect
{
    public static class ShaderAllocator
    {
        public static Shader LoadShaderFromFile(string compositeKey, ConstructorInfo ctor)
        {
            string[] shaderFiles = GetKeys(compositeKey);
            return (Shader)ctor.Invoke(shaderFiles);
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
